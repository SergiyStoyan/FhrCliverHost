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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Cliver.DataSifter
{
    /// <summary>
    /// lists group names used in the current filter tree
    /// </summary>
    internal partial class OutputForm : BaseForm
    {
        internal OutputForm(List<OutputGroup> all_ogs, List<OutputGroup> captured_ogs)
        {
            InitializeComponent();
            set_tool_tip();

            this.all_ogs = all_ogs;
            this.captured_ogs = captured_ogs;
                        
            CaptureSeparator.Text = Settings.Default.OutputDataSeparator;
            
            if (captured_ogs == null)
            {
                radioCaptures.Enabled = false;
                radioGroupNames.Checked = true;
            }
            else
            {
                radioGroupNames.Checked = Settings.Default.OutputGroupNames;
                radioCaptures.Checked = !Settings.Default.OutputGroupNames;
            }

            radioGroupNames_CheckedChanged(null, null);
            
            bShow.Enabled = false;
        }

        List<OutputGroup> all_ogs;
        List<OutputGroup> captured_ogs;
        List<OutputGroup> ogs_;

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            string output = get_data();
            if (string.IsNullOrEmpty(output))
                output = " ";
            Clipboard.SetText(output, TextDataFormat.Text);
        }

        /// <summary>
        /// constructs the output string
        /// </summary>
        /// <returns></returns>
        string get_data()
        {
            if (radioGroupNames.Checked)
            {
                string gns = "";
                foreach (OutputGroup og in all_ogs)
                    gns += get_indent(og.Level) + og.Name + "\n";
                return gns;
            }

            if (radioCaptures.Checked)
            {
               // if (SourceForm.This.GroupCapture0 == null || Document.DocumentLoadedTime > SourceForm.This.FilterTreeParsedTime)
                {
                    Message.Exclaim("You have to perform parsing before getting results.");
                    return null;
                }
               // if (SourceForm.This.FilterTreeChangedTime > SourceForm.This.FilterTreeParsedTime)
                {
                    Message.Exclaim("Filter tree was changed after the last parsing. You have to perform parsing before getting results.");
                    return null;
                }
                if (GroupNames.SelectedItem == null)
                {
                    Message.Exclaim("No group name is selected.");
                    return null;
                }
                return get_captures_for_selected_group(captured_ogs[GroupNames.SelectedIndex].GetChain(), Regex.Replace(CaptureSeparator.Text, @"(?<!\r)\n", "\r\n", RegexOptions.Compiled| RegexOptions.Singleline | RegexOptions.IgnoreCase));
            }
            return "";
        }

        string get_indent(int level)
        {
            string indent = "";
            for (int i = 0; i < level; i++)
                indent += "\t";
            return indent;
        }

        /// <summary>
        /// output the parse resutls
        /// </summary>
        /// <returns></returns>
        private string get_captures_for_selected_group(OutputGroup[] chain, string capture_separator)
        {
            StringBuilder cs = new StringBuilder("");

            List<Capture> gcs = new List<Capture>();
           // gcs.Add(SourceForm.This.GroupCapture0);
            for (int i = 0; i < chain.Length; i++)
            {
                OutputGroup og = chain[i];
                List<Capture> child_gcs = new List<Capture>();
                foreach (Capture gc in gcs)
                    foreach (Capture child_gc in gc[og.Name])
                        child_gcs.Add(child_gc);
                gcs = child_gcs;
            }
            {//getting values from the last level gcs
                foreach (Capture gc in gcs)
                {
                    cs.Append(gc.Value);
                    cs.Append(capture_separator);
                }
            }

            return cs.ToString();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                string data = get_data();
                if(string.IsNullOrEmpty(data))
                    return;
                SaveFileDialog d = new SaveFileDialog();
                d.DefaultExt = "txt";
                d.CheckPathExists = true;
                string file_name = "";
                if (radioGroupNames.Checked)
                {
                    file_name = "filter_names";
                    d.Title = "Save output group names to a file";
                }
                else if (radioCaptures.Checked)
                {
                    if (GroupNames.SelectedItem == null)
                    {
                        Message.Exclaim("No group name is selected.");
                        return;
                    }
                    file_name = captured_ogs[GroupNames.SelectedIndex].Name;
                    d.Title = "Save parse results to a file";
                }
                d.FileName = file_name + ".txt";
                d.AddExtension = true;
                if (d.ShowDialog(this) != DialogResult.OK)
                    return;
                System.IO.File.WriteAllText(d.FileName, data);
                bShow.Enabled = true;
                this.file_name = d.FileName;
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
        }
        string file_name = null;

        private void radioGroupNames_CheckedChanged(object sender, EventArgs e)
        {
            if (radioGroupNames.Checked)
            {
                CaptureSeparator.Enabled = false;
                ogs_ = this.all_ogs;
                statusLabel.Text = "Number of named groups: " + all_ogs.Count;
            }
            else
            {
                CaptureSeparator.Enabled = true;
                ogs_ = this.captured_ogs;
                statusLabel.Text = "Number of captured groups: " + captured_ogs.Count;
            }

            GroupNames.Items.Clear();
            foreach (OutputGroup og in ogs_)
                GroupNames.Items.Add(og.GetPath(" / "));

            if (GroupNames.Items.Count == 1)
                GroupNames.SelectedIndex = 0;
            else
                GroupNames.SelectedItem = null;
        }

        private void CaptureSeparator_Enter(object sender, EventArgs e)
        {
            CaptureSeparator.Select(CaptureSeparator.Text.Length, CaptureSeparator.Text.Length);
        }

        private void bSaveAsDefault_Click(object sender, EventArgs e)
        {
            Settings.Default.OutputGroupNames = radioGroupNames.Checked;
            Settings.Default.OutputDataSeparator = CaptureSeparator.Text;

            Settings.Default.Save();
        }

        private void GroupNames_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            if ((e.State & DrawItemState.Selected) != DrawItemState.Selected)
            {
                Brush brush = new SolidBrush(Settings.Default.GetFilterBackColor(ogs_[e.Index].Level));
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            else
                e.DrawBackground();
            {
                Brush brush = new SolidBrush(e.ForeColor);
                e.Graphics.DrawString((string)((ListBox)sender).Items[e.Index], e.Font, brush, e.Bounds);
            }
        }

        private void bShow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.file_name))
            {
                bShow.Enabled = false;
                return;
            }
            System.Diagnostics.Process.Start(this.file_name);
        }
    }

    internal class OutputGroup
    {
        internal string Name = null;
        internal int Level = 0;
        internal OutputGroup Parent = null;

        internal OutputGroup(string name, OutputGroup parent)
        {
            Name = name;
            Parent = parent;
            for (OutputGroup og = this.Parent; og != null; og = og.Parent)
                Level++;
        }

        /// <summary>
        /// return standard path used for comparison
        /// </summary>
        /// <returns></returns>
        internal string GetPath(string separator = "/")
        {
            List<string> path = new List<string>();
            for (OutputGroup og = this; og != null; og = og.Parent)
                path.Insert(0, og.Name);
            return string.Join(separator, path.ToArray());
        }

        internal OutputGroup[] GetChain()
        {
            List<OutputGroup> chain = new List<OutputGroup>();
            for (OutputGroup og = this; og != null; og = og.Parent)
                chain.Insert(0, og);
            return chain.ToArray();
        }
    }
}