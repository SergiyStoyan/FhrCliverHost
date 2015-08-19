//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        17 February 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Configuration;
using System.Web;

namespace Cliver.DataSifter
{
    /// <summary>
    /// This part contains navigation routines
    /// </summary>
    partial class SourceForm
    {
        #region find box search

        private void FindPrev_Click(object sender, EventArgs e)
        {
            try
            {
                RichTextBox TextBox = null;
                TextBox = this.TextBox;

                int length = 0;
                if (TextBox.SelectionStart >= 0)
                    length = TextBox.SelectionStart;

                string text = TextBox.Text.Substring(0, length);
                Match m = Regex.Match(text, FindBox.Text, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.RightToLeft);
                if (m.Success)
                {
                  System.Text.RegularExpressions.Capture c = m.Groups[0].Captures[0];
                    TextBox.Select(c.Index, c.Length);
                }
                else
                    FindPrev.Enabled = false;

                FindNext.Enabled = true;
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
        }

        void find_next()
        {
            try
            {
                RichTextBox TextBox = null;
                TextBox = this.TextBox;

                int start = 0;
                if (TextBox.SelectionStart >= 0)
                    start = TextBox.SelectionStart + TextBox.SelectionLength;

                string text = TextBox.Text.Substring(start);
                Match m = Regex.Match(text, FindBox.Text, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (m.Success)
                {
                    System.Text.RegularExpressions.Capture c = m.Groups[0].Captures[0];
                    TextBox.Select(start + c.Index, c.Length);
                }
                else
                    FindNext.Enabled = false;

                FindPrev.Enabled = true;
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
        }

        private void FindNext_Click(object sender, EventArgs e)
        {
            find_next();
        }

        private void FindBox_TextChanged(object sender, EventArgs e)
        {
            RichTextBox TextBox = null;

            FindPrev.Enabled = true;
            FindNext.Enabled = true;

            TextBox = this.TextBox;

            TextBox.DeselectAll();
            find_next();
        }

        #endregion

        #region navigation

        void fill_NavigateBy(bool empty)
        {
            PrevMark.Enabled = true;
            NextMark.Enabled = true;

            NavigateBy.Enabled = true;
            NavigateBy.Items.Clear();
            NavigateBy.Text = "";
            output_groups.Clear();
            NavigateBy.BackColor = Color.Empty;

            if (empty)
                return;

            if (parser == null)
                return;
            FilterTreeForm.This.GetOutputGroupNames(parser.RootFilters, null, null, output_groups, false);
            foreach (OutputGroup og in output_groups)
                NavigateBy.Items.Add(og.GetPath("/"));

            if (NavigateBy.Items.Count >= 0)
                NavigateBy.SelectedIndex = NavigateBy.Items.Count - 1;
        }        

        List<OutputGroup> output_groups = new List<OutputGroup>();         

        private void NavigateBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NavigateBy.SelectedIndex < 0)
            {
                NavigateBy.BackColor = Color.Empty;
                return;
            }
            OutputGroup og = (OutputGroup)output_groups[NavigateBy.SelectedIndex];
            NavigateBy.BackColor = Settings.Default.GetFilterBackColor(og.Level);
            
            if (current_capture_label_index >= 0 && current_capture_label_index < CaptureLabels.Count)
                unmark_capture_branch(CaptureLabels[current_capture_label_index]);
            current_capture_label_index = -1;

            set_number_of_captures(og);

            NextUserMark_Click(null, null); 
        }
        int current_path_capture_count = -1;

        void set_number_of_captures(OutputGroup og)
        {
            int count = 0;
            string path = og.GetPath();
            foreach (CaptureLabel cl in CaptureLabels)
            {
                if (cl.level != og.Level)
                    continue;
                string cl_path = cl.get_path();
                if (cl_path != path)
                    continue;
                count++;
            }
            SetStatus("Number of [" + path + "] captures: " + count);
            current_path_capture_count = count;
        }

        void set_label_status()
        {
            if (current_capture_label_index >= 0 && current_capture_label_index < CaptureLabels.Count)
                SetStatus(CaptureLabels[current_capture_label_index].index_in_path + 1 + " of " + current_path_capture_count);
            else
                SetStatus("");
        }
        
        private void PrevUserMark_Click(object sender, EventArgs e)
        {
            try
            {
                int i = NavigateBy.SelectedIndex;
                if (i < 0)
                    return;
                ignore_selection_changed = true;
                prev_parse_mark((OutputGroup)output_groups[i]);
                set_label_status();
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
            ignore_selection_changed = false;
        }
        
        private void NextUserMark_Click(object sender, EventArgs e)
        {
            try
            {
                int i = NavigateBy.SelectedIndex;
                if (i < 0)
                    return;
                ignore_selection_changed = true;
                next_parse_mark((OutputGroup)output_groups[i]);
                set_label_status();
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
            ignore_selection_changed = false;
        }
        int current_capture_label_index = 0;

        private void prev_parse_mark(OutputGroup og)
        {
            string path = og.GetPath();
            for (int i = current_capture_label_index - 1; i >= 0; i--)
            {
                CaptureLabel cl = CaptureLabels[i];
                if (cl.level == og.Level
                    && cl.get_path() == path
                    )
                {
                    if (current_capture_label_index >= 0 && current_capture_label_index < CaptureLabels.Count)
                        unmark_capture_branch(CaptureLabels[current_capture_label_index]);
                    mark_capture_branch(cl, og, path);
                    current_capture_label_index = i;
                    break;
                }
            }

            if (current_capture_label_index >= 0 && current_capture_label_index < CaptureLabels.Count)
            {
                NextMark.Enabled = CaptureLabels[current_capture_label_index].index_in_path < current_path_capture_count - 1;
                PrevMark.Enabled = CaptureLabels[current_capture_label_index].index_in_path > 0;
            }
        }

        private void next_parse_mark(OutputGroup og)
        {
            string path = og.GetPath();
            for (int i = current_capture_label_index + 1; i < CaptureLabels.Count; i++)
            {
                CaptureLabel cl = CaptureLabels[i];
                if (cl.level == og.Level
                    && cl.get_path() == path
                    )
                {
                    if (current_capture_label_index >= 0 && current_capture_label_index < CaptureLabels.Count)
                        unmark_capture_branch(CaptureLabels[current_capture_label_index]);
                    mark_capture_branch(cl, og, path);
                    current_capture_label_index = i;
                    break;
                }
            }

            if (current_capture_label_index >= 0 && current_capture_label_index < CaptureLabels.Count)
            {
                NextMark.Enabled = CaptureLabels[current_capture_label_index].index_in_path < current_path_capture_count - 1;
                PrevMark.Enabled = CaptureLabels[current_capture_label_index].index_in_path > 0;
            }
        }

        void mark_capture_branch(CaptureLabel cl, OutputGroup og, string path)
        {
            List<CaptureLabel> cls = new List<CaptureLabel>();
            for (CaptureLabel c = cl; c != null; c = c.parent)
                cls.Add(c);
            int total_label_length = 0;
            for (int i = cls.Count - 1; i >= 0; i--)
            {
                CaptureLabel c = cls[i];
                if (!c.visible)
                    continue;
                c.start3 = c.start2 + total_label_length;
                c.end3 = c.end2 + total_label_length + c.label.Length;
                if (Settings.Default.PrintCaptureLabels)
                {
                    print_capture_label(c);
                    total_label_length += c.label.Length;
                }
                highlight_capture(c);
            }
            ignore_selection_changed = false;
            TextBox.Select(cl.start3, 0);
        }

        void unmark_capture_branch(CaptureLabel cl)
        {
            CaptureLabel scl = cl;    
            for (CaptureLabel c = cl; c != null; c = c.parent) 
            {
                TextBox.Select(c.start3, c.label.Length);
                TextBox.ReadOnly = false;//it is to avoid a bug in richtextbox. Morons from microsoft know about it since 2005 and still have not fixed it!
                TextBox.SelectedText = "";
                TextBox.ReadOnly = true;
                scl = c;
                c.start3 = -1;
                c.end3 = -1;
            }
            //TextBox.SelectAll();
            //TextBox.SelectionBackColor = Color.White;
            TextBox.Select(scl.start2, scl.end2 - scl.start2);
            TextBox.SelectionBackColor = Color.White;
            TextBox.Select(scl.start2, 0);
        }

        #endregion
    }
}