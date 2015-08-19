//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        17 February 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Web;

namespace Cliver.DataSifter
{
    /// <summary>
    /// CaptureLabel routines
    /// </summary>
    internal partial class SourceForm
    {
        internal void print_capture_label(CaptureLabel cl)
        {
            Invoke(() =>
            {
                TextBox.Select(cl.start3, 0);
                TextBox.SelectedText = cl.label;
                TextBox.Select(cl.start3, cl.label.Length);
                TextBox.SelectionBackColor = Settings.Default.GetFilterBackColor(cl.level);
                TextBox.SelectionColor = cl.fgcolor;
                TextBox.SelectionFont = Settings.Default.CaptureLabelFont;
            });
        }

        /// <summary>
        /// Defines label for each parse capture
        /// </summary>
        internal class CaptureLabel
        {
            /// <summary>
            /// start position of capture as it is in Document.Source
            /// </summary>
            readonly internal int start;
            /// <summary>
            /// end position of capture as it is in Document.Source 
            /// </summary>
            readonly internal int end;
            /// <summary>
            /// start position of capture shifted on carriage_returns
            /// </summary>
            readonly internal int start2;
            /// <summary>
            /// end position of capture shifted on carriage_returns
            /// </summary>
            readonly internal int end2;
            /// <summary>
            /// start position of capture shifted on carriage_returns + if set, label's length
            /// </summary>
            internal int start3 = -1;
            /// <summary>
            /// end position of capture shifted on carriage_returns + if set, label's length
            /// </summary>
            internal int end3 = -1;
            readonly internal int level;
            readonly internal int match_id;
            readonly internal int group_id;
            readonly internal Color fgcolor = Color.White;
            readonly internal string group_name;
            readonly internal string label;
            readonly internal CaptureLabel parent = null;
            readonly internal int index_in_path = -1;
            /// <summary>
            /// is this capture highlighted
            /// </summary>
            readonly internal bool visible = false;

            internal CaptureLabel(int start, int length, int start2, int length2, int level, int match_id, int group_id, string group_name, CaptureLabel parent, bool visible, int index_in_path)
            {
                this.start = start;
                this.end = start + length;
                this.start2 = start2;
                this.end2 = start2 + length2;
                //this.start3 = this.start2;
                //this.end3 = this.end2;

                this.level = level;
                this.match_id = match_id;
                this.group_id = group_id;
                this.group_name = group_name;
                this.parent = parent;
                this.visible = visible;
                this.index_in_path = index_in_path;

                label = "[" + match_id.ToString() + "$" + group_id.ToString() + "]";
            }

            /// <summary>
            /// return standard path used for comparison
            /// </summary>
            /// <returns></returns>
            internal string get_path()
            {
                List<string> path = new List<string>();
                for (CaptureLabel cl = this; cl != null; cl = cl.parent)
                    path.Insert(0, cl.group_name);
                return string.Join("/", path.ToArray());
            }
        }

        /// <summary>
        /// List of parse labels to be inserted in text
        /// </summary>
        internal List<CaptureLabel> CaptureLabels = new List<CaptureLabel>();

        void highlight_capture(CaptureLabel cl)
        {
            Invoke(() =>
            {
                TextBox.Select(cl.start3, cl.end3 - cl.start3);
                TextBox.SelectionBackColor = Settings.Default.GetFilterBackColor(cl.level);
            });
        }

        /// <summary>
        /// shift position on positions ignored by RichTextBox
        /// </summary>
        /// <param name="position">position in source</param>
        /// <param name="length"></param>
        /// <returns>position in RichTextBox</returns>
        void shift_on_positions_ignored_by_rich_text_box(ref int position, ref int length)
        {
            int shift1 = 0;
            int shift2 = 0;
            int end = position + length;
            foreach (int crp in positions_ignored_by_rich_text_box)
            {
                if (crp <= position)
                    shift1++;
                else if (crp <= end)
                    shift2++;
                else
                    break;
            }
            position -= shift1;
            length -= shift2;
        }

        internal CaptureLabel AddCaptureLabel(
            int start,
            int length,
            int level,
            int group_id,
            string group_name,
            int group_count
            )
        {
            int start2 = start;
            int length2 = length;
            SourceForm.This.shift_on_positions_ignored_by_rich_text_box(ref start2, ref length2);

            //set parent
            CaptureLabel parent = null;
            if (level > 0)
            {
                int end2 = start2 + length2;
                for (int i = CaptureLabels.Count - 1; i >= 0; i--)
                {
                    CaptureLabel c = CaptureLabels[i];
                    if (c.start2 <= start2 && c.end2 >= end2 && c.level == level - 1)
                    {
                        parent = c;
                        break;
                    }
                }
            }

            //set match_id
            int match_id = -1;
            if (CaptureLabels.Count > 0)
            {
                if (level - 1 == CaptureLabels[CaptureLabels.Count - 1].level)
                    match_id = 0;
                else if (level == CaptureLabels[CaptureLabels.Count - 1].level
                    && group_id - 1 == CaptureLabels[CaptureLabels.Count - 1].group_id
                    )
                    match_id = CaptureLabels[CaptureLabels.Count - 1].match_id;
                else
                {
                    for (int i = CaptureLabels.Count - 1; i >= 0; i--)
                    {
                        CaptureLabel c = CaptureLabels[i];
                        if (level == c.level && group_id == c.group_id)
                        {
                            match_id = c.match_id + 1;
                            break;
                        }
                    }
                }
            }
            else
                match_id = 0;

            //index_in_path
            int index_in_path = 0;
            for (int i = CaptureLabels.Count - 1; i >= 0; i--)
            {
                CaptureLabel c = CaptureLabels[i];
                if (c.level == level && c.group_id == group_id)
                {
                    index_in_path = c.index_in_path + 1;
                    break;
                }
            }

            CaptureLabel cl = new CaptureLabel(start, length, start2, length2, level, match_id, group_id, group_name, parent, true, index_in_path);
            SourceForm.This.CaptureLabels.Add(cl);
            return cl;
        }

        /// <summary>
        /// Sets status bar text along the current cursor position in the parsed text box.
        /// Finds the selected capture, determines its filter tree path and constructs from them status text.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        void set_status_by_position(int start, int length)
        {
            if (CaptureLabels == null)
                return;

            if (current_capture_label_index < 0 || current_capture_label_index >= CaptureLabels.Count)
            {
                SetStatus("");
                return;
            }

            int end = start + length;
            CaptureLabel owner_cl = null;
            for (CaptureLabel c = CaptureLabels[current_capture_label_index]; c != null; c = c.parent)
            {
                if (!c.visible)
                    continue;
                if (start >= c.start3 && end <= c.end && (owner_cl == null || c.level > owner_cl.level))
                    owner_cl = c;
            }
            if (owner_cl == null)
            {
                SetStatus("");
                return;
            }

            string path = null;
            for (CaptureLabel l = owner_cl; l != null; l = l.parent)
            {
                string name;
                int r;
                if (int.TryParse(l.group_name, out r))
                    name = "$" + l.group_name;
                else
                    name = l.group_name;
                path = "/" + name + "[" + l.match_id.ToString() + "]" + path;
            }

            string text = Document.Text.Substring(owner_cl.start, owner_cl.end - owner_cl.start);

            if (tr != null)
                tr.Abort();
            tr = new ThreadRunner(10000, this, "Stripping text of the selected capture is not completed still. Break it?");
            text = (string)tr.Run(this, "set_status_by_position_2", text);

            SetStatus(path.Substring(1) + " - \"" + text + "\"");
        }
        ThreadRunner tr = null;

        public string set_status_by_position_2(string text)
        {
            if (Settings.Default.StripParsedTextInStatusBarFromHtmlTags)
            {
                text = Regex.Replace(text, "<!--.*?-->|[\n\r]", "", RegexOptions.Compiled | RegexOptions.Singleline);
                text = Regex.Replace(text, "</?(td|p|tr|br|body|option|div).*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                text = Regex.Replace(text, "<.*?>", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                text = HttpUtility.HtmlDecode(text);
            }
            else
                text = Regex.Replace(text, "[\n\r]+", "", RegexOptions.Compiled | RegexOptions.Singleline);
            text = Regex.Replace(text, @"\t+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
            text = Regex.Replace(text, @"\s{2,}", " ", RegexOptions.Compiled | RegexOptions.Singleline);//strip from more than 1 spaces	
            text = text.Trim();
            return text;
        }
    }
}