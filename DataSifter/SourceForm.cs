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
using System.Linq;

namespace Cliver.DataSifter
{
    /// <summary>
    /// Form that hosts parsed text and filter tree editor control
    /// </summary>
    internal partial class SourceForm : BaseForm
    {        
        #region initializing

        string title = Program.Title;

        internal static readonly SourceForm This;

        static SourceForm()
        {
            This = new SourceForm();
            This.splitContainer1.Panel2.Controls.Add(FilterTreeForm.This);
            FilterTreeForm.This.Dock = DockStyle.Fill;

            This.splitContainer1.BackColor = SplitterColor;
            This.splitContainer1.Panel1.BackColor = SystemColors.Control;
            This.splitContainer1.Panel2.BackColor = SystemColors.Control;
        }
        
        static internal readonly Color SplitterColor = Color.Gray;

        SourceForm()
        {
            InitializeComponent();
            set_tool_tip();

            Document.DocumentUpdated += new Document.DocumentUpdatedEventHandler(Document_DocumentUpdated);
            Document.SetTestText();
            //TreeForm_Click(null, null);

            if (System.IO.File.Exists(Settings.Default.LastSourceFile))
                Document.LoadFromFile(Settings.Default.LastSourceFile);
            if (System.IO.File.Exists(Settings.Default.LastFilterTreeFile))
                FilterTreeForm.This.LoadFilterTree(Settings.Default.LastFilterTreeFile);
        }
        
        void  Document_DocumentUpdated()
        {
            ignore_selection_changed = true;
            //TextBox is used to display parse results
            TextBox.Rtf = null;
            TextBox.Text = Document.Text;
            count_positions_ignored_by_rich_text_box();

            Text = Document.Title + " - " + title;

            CaptureLabels.Clear();
            fill_NavigateBy(true);

            ////set rtf header   
            //StringBuilder rtf = new StringBuilder(TextBox.Rtf);
            ////get the needed font declared in the head
            //TextBox.Text = "12";
            //TextBox.Select(1, 1);
            //TextBox.SelectionFont = Settings.Default.CaptureLabelFont;
            //string rtf2 = TextBox.Rtf;
            //string head = rtf2.Substring(0, rtf2.IndexOf("\r\n"));
            ////create color table
            //Color html_tags_color = Settings.Default.HtmlTagsColor;
            //Color html_comment_color = Settings.Default.HtmlCommentColor;
            //string colortb = string.Format(@"{{\colortbl;\red{0}\green{1}\blue{2};\red{3}\green{4}\blue{5};}}",
            //    html_comment_color.R, html_comment_color.B, html_comment_color.G,
            //    html_tags_color.R, html_tags_color.B, html_tags_color.G);
            //head += colortb;
            //rtf.Remove(0, RtfEditor.IndexOf(rtf, "\r\n"));
            //rtf.Insert(0, head);
            if (Settings.Default.HighlightHtmlTags)
            {  //set rtf header   
                StringBuilder rtf = new StringBuilder(TextBox.Rtf);
                Color html_tags_color = Settings.Default.HtmlTagsColor;
                Color html_comment_color = Settings.Default.HtmlCommentColor;
                Color html_javascript_color = Settings.Default.HtmlJavascriptColor;
                string colortb = string.Format(@"{{\colortbl;\red{0}\green{1}\blue{2};\red{3}\green{4}\blue{5};\red{6}\green{7}\blue{8};}}",
                    html_comment_color.R, html_comment_color.G, html_comment_color.B,
                    html_tags_color.R, html_tags_color.G, html_tags_color.B,
                    html_javascript_color.R, html_javascript_color.G, html_javascript_color.B);
                rtf.Insert(RtfEditor.IndexOf(rtf, "\r\n"), colortb);

                int rtf_length = rtf.Length;
                for (int i = 0; i < rtf_length; i++)
                {
                    if (rtf[i] != '<')
                        continue;

                    if (RtfEditor.IsEqualIgnoringCase(rtf, i + 1, "!--"))
                    {
                        rtf.Insert(i, @"\cf1 ");
                        i += 5;
                        rtf_length += 5;
                        int tag_end = RtfEditor.IndexOf(rtf, "-->", i);
                        if (tag_end >= 0)
                        {
                            rtf.Insert(tag_end + 1, @"\cf0 ");
                            i = tag_end + 5;
                            rtf_length += 5;
                        }
                    }
                    else if (RtfEditor.IsEqualIgnoringCase(rtf, i + 1, "script"))
                    {
                        rtf.Insert(i, @"\cf2 ");
                        i += 5;
                        rtf_length += 5;
                        int tag_end = RtfEditor.IndexOf(rtf, ">", i);
                        if (tag_end >= 0)
                        {
                            rtf.Insert(tag_end + 1, @"\cf3 ");
                            i = tag_end + 5;
                            rtf_length += 5;
                        }
                    }
                    else
                    {
                        rtf.Insert(i, @"\cf2 ");
                        i += 5;
                        rtf_length += 5;
                        int tag_end = RtfEditor.IndexOf(rtf, ">", i);
                        if (tag_end >= 0)
                        {
                            rtf.Insert(tag_end + 1, @"\cf0 ");
                            i = tag_end + 5;
                            rtf_length += 5;
                        }
                    }
                }
                TextBox.Rtf = rtf.ToString();
            }

            ignore_selection_changed = false;
        }
        
        /// <summary>
        /// RichTextBox takes '\r\n' as a single char so it counts them to restore a correct position
        /// </summary>
        void count_positions_ignored_by_rich_text_box()
        {
            positions_ignored_by_rich_text_box.Clear();
            MatchCollection mc = Regex.Matches(Document.Text, @"\r{1,2}\n", RegexOptions.Compiled | RegexOptions.Singleline);
            foreach (Match m in mc)
                for (int i = m.Length - 1; i > 0; i--)
                    positions_ignored_by_rich_text_box.Add(m.Index + 1);
        }
        List<int> positions_ignored_by_rich_text_box = new List<int>();
        
        private void SourceForm_Load(object sender, EventArgs e)
        {
            try
            {
                NavigateBy.DisplayMember = "name";
                NavigateBy.ValueMember = "value";

                FilterTreeForm.This.Show();
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
        }

#endregion

        #region parsing

        /// <summary>
        /// start parsing by the current debug regex chain
        /// </summary>
        /// <returns></returns>
        internal bool RunParse()
        {
            ignore_selection_changed = true;
            PrevMark.Enabled = true;
            NextMark.Enabled = true;
            if (current_capture_label_index >= 0 && current_capture_label_index < CaptureLabels.Count)
                unmark_capture_branch(CaptureLabels[current_capture_label_index]);
            current_capture_label_index = -1;
            current_path_capture_count = 0;

            fill_NavigateBy(true);

            CaptureLabels.Clear();
            TextBox.SelectAll();
            TextBox.SelectionBackColor = Color.White;
            TextBox.DeselectAll();
            //RefreshDocument();

            parser = FilterTreeForm.This.GetFilterChainParser();
            if (parser.RootFilters == null || parser.RootFilters.Length < 1)
                return false;
            //page = TextBox.Text;
            ThreadStart ts = new ThreadStart(parse);
            parse_thread = new Thread(ts);
            parse_thread.Start();
            return true;
        }

        /// <summary>
        /// breaks parsing thread and stops parsing
        /// </summary>
        internal void StopParse()
        {
            if (parse_thread != null)
            {
                if (parse_thread.IsAlive)
                    parse_thread.Abort();
                parse_thread = null;
                System.GC.Collect();
            }
            FilterTreeForm.This.buttonParse.Checked = false;
            ignore_selection_changed = false;
        }

        Thread parse_thread;
        Parser parser;
        //used by OutputForm
        internal DateTime FilterTreeParsedTime;
        //used by OutputForm
        internal Capture GroupCapture0 = null;
        //used by OutputForm
        internal DateTime FilterTreeChangedTime;
        //used by OutputForm
        internal Parser ParserForCurrentChain
        {
            get
            {
                return parser;
            }
        }

        void parse()
        {
            try
            {
                GroupCapture0 = parser.Parse(Document.Text);
                FilterTreeParsedTime = DateTime.Now;
                Invoke(() =>
                {
                    TextBox.DeselectAll();
                    FilterTreeForm.This.buttonParse.Checked = false;
                    fill_NavigateBy(false);
                });
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
            ignore_selection_changed = false;
        }

#endregion

        #region handlers

        private void File_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog d = new OpenFileDialog();
                d.Title = "Pick a file to be parsed";
                d.InitialDirectory = get_corresponding_source_folder(Settings.Default.LastFilterTreeFile);
                if (string.IsNullOrWhiteSpace(d.InitialDirectory) || !Directory.Exists(d.InitialDirectory))
                    d.InitialDirectory = null;
                if (d.ShowDialog(this) != DialogResult.OK || d.FileName == "")
                    return;
                Settings.Default.LastSourceFile = d.FileName;
                Settings.Default.Save();
                Document.LoadFromFile(d.FileName);
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
        }

        string get_corresponding_source_folder(string filter_tree_file)
        {
            string ft_folder = Path.GetDirectoryName(filter_tree_file);
            if (Settings.Default.FilterTreeFolder2SourceFolder.Contains(ft_folder))
                return (string)Settings.Default.FilterTreeFolder2SourceFolder[ft_folder];
            return Path.GetDirectoryName(Settings.Default.LastSourceFile);
        }

        private void About_Click(object sender, EventArgs e)
        {
            AboutForm a = new AboutForm();
            a.ShowDialog();
        }

        private void Help_Click(object sender, EventArgs e)
        {
            Program.Help();
        }

        private void TreeForm_Click(object sender, EventArgs e)
        {
            //this.Location = new Point(0, 0);
            //this.Height = Screen.PrimaryScreen.WorkingArea.Height - FilterTreeForm.This.Height;
            //this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            //this.WindowState = FormWindowState.Normal;

            //FilterTreeForm.This.Location = new Point(0, Screen.PrimaryScreen.WorkingArea.Height - FilterTreeForm.This.Height);
            //FilterTreeForm.This.Width = Screen.PrimaryScreen.WorkingArea.Width;
            //FilterTreeForm.This.WindowState = FormWindowState.Normal;
            //FilterTreeForm.This.Visible = true;
            //FilterTreeForm.This.Activate();
            ////FilterTreeForm.This.Focus();
        }

        private void cHideFilters_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = cHideFilters.Checked;
            cHideFilters.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
        }

        private void SourceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FilterTreeForm.This.ToSavePreviousFilterTree())
                return;

            if (!Message.YesNo("Exiting " + Application.ProductName + ". Proceed?"))
            {
                e.Cancel = true;
                return;
            }

            This.Dispose();
        }

        internal void ReadFilterTreeXmlFile(string filter_tree_xml_file)
        {
            FilterTreeForm.This.LoadFilterTree(filter_tree_xml_file);
        }

        private void Abort_Click(object sender, EventArgs e)
        {
            if (parse_thread == null)
                return;
            parse_thread.Abort();
            //RefreshDocument();
        }

        private void SourceForm_Activated(object sender, EventArgs e)
        {
        }

        private void TextBox_SelectionChanged(object sender, EventArgs e)
        {
            if (ignore_selection_changed)
                return;

            set_status_by_position(TextBox.SelectionStart, TextBox.SelectionLength);

            if (!Settings.Default.CopySelectionToClipboard)
                return;

            TextBox.Copy();
        }
        bool ignore_selection_changed = true;

        private void bSettings_Click(object sender, EventArgs e)
        {
            SettingsForm sf = new SettingsForm();
            sf.ShowDialog(this);
        }

        internal void SetStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                Status.Text = " ";
            else
                Status.Text = status;
        }

        /// <summary>
        /// selects the complete captured group within the parsed text box by double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (CaptureLabels == null)
                return;
            
            if (current_capture_label_index < 0 || current_capture_label_index >= CaptureLabels.Count)
            {
                SetStatus("");
                return;
            }

            CaptureLabel owner_cl = null;
            int start = TextBox.SelectionStart;
            for (CaptureLabel c = CaptureLabels[current_capture_label_index]; c != null; c = c.parent)
            {
                if (!c.visible)
                    continue;
                if (start >= c.start3 && (owner_cl == null || c.level > owner_cl.level))
                    owner_cl = c;
            }
            if (owner_cl == null)
                return;

            TextBox.SelectionStart = owner_cl.start3;
            TextBox.SelectionLength = owner_cl.end3 - owner_cl.start3;           
        }

        static ThreadRunner tw;

        private void NavigateBy_DropDown(object sender, EventArgs e)
        {
            int width = 0;
            Graphics g = NavigateBy.CreateGraphics();
            Font font = NavigateBy.Font;
            foreach (string s in NavigateBy.Items)
            {
                int w = (int)g.MeasureString(s, font).Width;
                if (w > width)
                    width = w;
            }
            int vert_scroll_bar_width = (NavigateBy.Items.Count > NavigateBy.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;
            NavigateBy.DropDownWidth = width + vert_scroll_bar_width + 10;
        }

        private void NavigateBy_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            Brush brush;
            OutputGroup og = (OutputGroup)output_groups[e.Index];
            brush = new SolidBrush(Settings.Default.GetFilterBackColor(og.Level));
            e.Graphics.FillRectangle(brush, e.Bounds);
            brush = new SolidBrush(e.ForeColor);
            e.Graphics.DrawString((string)((ComboBox)sender).Items[e.Index], e.Font, brush, e.Bounds);
            e.DrawFocusRectangle();
        }

        #endregion
    }
}