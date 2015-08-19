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
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

namespace Cliver.DataSifter
{
    /// <summary>
    /// fanctionality of FilterTreeForm concerning to visibilty and external methods
    /// </summary>
    partial class FilterTreeForm
    {
        #region prepared filters routines

        void load_prepared_filter_trees()
        {
            PreparedFilterTrees.Items.Clear();
            prepared_filter_tree_files.Clear();

            DirectoryInfo di = new DirectoryInfo(prepared_filter_tree_dir);
            if (!di.Exists)
                return;

            foreach (FileInfo fi in di.GetFiles("*." + Program.FilterTreeFileExtension))
            {
                string name = fi.Name.Substring(0, fi.Name.LastIndexOf("."));
                prepared_filter_tree_files[name] = fi.FullName;
                PreparedFilterTrees.Items.Add(name);
            }
        }
        Dictionary<string,string> prepared_filter_tree_files = new Dictionary<string,string>();

        /// <summary>
        /// saves the current filter tree as a prepared filter tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsPrepared_Click(object sender, EventArgs e)
        {
            Parser p = GetFilterTreeParser();
            if (p == null)
            {
                Message.Exclaim("Filter tree is blank.");
                return;
            }
            if (p.RootFilters == null || p.RootFilters.Length < 1)
            {
                Message.Exclaim("No filter tree is active.");
                return;
            }

            NameForm nf = new NameForm((string)PreparedFilterTrees.SelectedItem);
            do
            {
                nf.StartPosition = FormStartPosition.CenterParent;
                if (nf.ShowDialog(this) != DialogResult.OK)
                    return;
                if (string.IsNullOrEmpty(nf.FileName))
                    return;
                if (!PreparedFilterTrees.Items.Contains(nf.FileName))
                    break;
            }
            while (!Message.YesNo(nf.FileName + " already exists. Do you want to overwrite it?"));

            string prepared_filter_tree_name = nf.FileName;
            string prepared_filter_tree_file = prepared_filter_tree_dir + "\\" + prepared_filter_tree_name + "." + Program.FilterTreeFileExtension;

            if (!Directory.Exists(prepared_filter_tree_dir))
                Directory.CreateDirectory(prepared_filter_tree_dir);

            FileInfo fi = new FileInfo(prepared_filter_tree_file);
            if (fi.Exists)
            {
                Directory.CreateDirectory(deleted_prepared_filter_tree_dir);
                string new_old_file = deleted_prepared_filter_tree_dir + "\\" + prepared_filter_tree_name + "_" + DateTime.Now.ToString("yyMMddHHmmss") + "." + Program.FilterTreeFileExtension;
                fi.MoveTo(new_old_file);
                Message.Inform("The old prepared filter tree was moved to the folder:\n" + deleted_prepared_filter_tree_dir);
            }

            p.WriteFilterTreeToXmlFile(prepared_filter_tree_file);
            load_prepared_filter_trees();

            FilterTreeChanged = false;
        }

        /// <summary>
        /// deletes the selected prepared filter tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePrepared_Click(object sender, EventArgs e)
        {
            if (PreparedFilterTrees.SelectedIndex < 0)
            {
                Message.Exclaim("No prepared filter tree is selected.");
                return;
            }

            string prepared_filter_name = (string)PreparedFilterTrees.SelectedItem;

            if (!Message.YesNo("Are you sure deleting " + prepared_filter_name + " from prepared filter trees?"))
                return;

            FileInfo fi = new FileInfo(prepared_filter_tree_files[prepared_filter_name]);
            if (fi.Exists)
            {
                Directory.CreateDirectory(deleted_prepared_filter_tree_dir);
                string new_old_file = deleted_prepared_filter_tree_dir + "\\" + prepared_filter_name + "_" + DateTime.Now.ToString("yyMMddHHmmss") + "." + Program.FilterTreeFileExtension;
                fi.MoveTo(new_old_file);
                Message.Inform("The prepared filter tree was moved to the folder:\n" + deleted_prepared_filter_tree_dir);
            }

            load_prepared_filter_trees();
        }

        /// <summary>
        /// adds the selected prepared filter to the selected filter tree node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPrepared_Click(object sender, EventArgs e)
        {
            try
            {
                if (PreparedFilterTrees.SelectedIndex < 0)
                {
                    Message.Exclaim("No prepared filter tree is selected.");
                    return;
                }

                Parser p = new Parser(prepared_filter_tree_files[(string)PreparedFilterTrees.SelectedItem]);
                AddFilterTreeToSelectedTreeNode(p.RootFilters);
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
        }

        private void PreparedFilters_DoubleClick(object sender, EventArgs e)
        {
            AddPrepared_Click(null, null);
        }

        internal void AddFilterTreeToSelectedTreeNode(Filter[] rns)
        {
            if (rns == null)
                return;
            
            TreeNodeCollection tns;
            TreeNode stn = (TreeNode)FilterTree.SelectedNode;
            if (stn == null)
                tns = FilterTree.Nodes;
            else
                tns = stn.Nodes;
            TreeNode tn2select = null;
            add_unique_filter_nodes2filter_tree_branch(tns, rns, ref tn2select);
            if (stn == null)
                foreach (TreeNode tn in tns)
                    tn.ExpandAll();
            else
                stn.ExpandAll();
            //check new filter chain
            if (is_tree_node_within_checked_path(tn2select))
                uncheck_nodes(tn2select.Nodes);
            tn2select.Checked = true;
            tn2select.TreeView.SelectedNode = tn2select;
        }

        void add_unique_filter_nodes2filter_tree_branch(TreeNodeCollection tns, Filter[] fs, ref TreeNode tn2select)
        {
            foreach (Filter f in fs)
            {
                bool duplicated = false;
                foreach (TreeNode tn in tns)
                {
                    Filter tf = (Filter)tn.Tag;
                    if (f == tf)
                    {
                        tn2select = tn;
                        add_unique_filter_nodes2filter_tree_branch(tn.Nodes, f.Next, ref tn2select);
                        duplicated = true;
                        break;
                    }
                }
                if (duplicated)
                    continue;
                f.InputGroupName = "0";
                int level = 0;
                if (tns.Count > 0)
                    level = ((Filter)tns[0].Tag).Level;
                {                    
                    TreeNode tn = create_tree(f, level);
                    tns.Add(tn);

                    //find default filter chain and check it
                    for (; tn.FirstNode != null; tn = tn.FirstNode) ;
                    tn2select = tn;
                }
            }
        }
        #endregion

        #region creating filter tree from filter tree view

        /// <summary>
        /// Create a tree of Filter's currently edited in the form
        /// </summary>
        /// <returns>filter tree Parser, if error then NULL</returns>
        internal Parser GetFilterTreeParser()
        {
            try
            {
                Filter[] root_filters = new Filter[FilterTree.Nodes.Count];
                int i = 0;
                foreach (TreeNode tn in FilterTree.Nodes)
                {
                    Filter f = (Filter)tn.Tag;
                    root_filters[i++] = f;
                    add_filter_nodes(f, tn.Nodes);
                }                
                //check for errors
                return new Parser(root_filters);
            }
            catch (Exception e)
            {
                Message.Error(e);
            }
            return null;
        }

        void add_filter_nodes(Filter parent_f, TreeNodeCollection tree_nodes)
        {
            parent_f.Next = new Filter[tree_nodes.Count];
            int i = 0;
            foreach (TreeNode tn in tree_nodes)
            {
                Filter f = (Filter)tn.Tag;
                parent_f.Next[i++] = f;
                add_filter_nodes(f, tn.Nodes);
            }
        }

#endregion

        #region filter tree file routines

        /// <summary>
        /// saves current filter collection into file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                string file = null;
                string file_name = FilterTreeName.Text;
                if (file_name != null)
                    file_name.Trim();
                if (!string.IsNullOrEmpty(FilterTreeFileDir.Text) && !string.IsNullOrEmpty(file_name))
                {
                    file = FilterTreeFileDir.Text + "\\" + file_name;
                    if (!file_name.EndsWith("." + Program.FilterTreeFileExtension, StringComparison.InvariantCultureIgnoreCase))
                        file += "." + Program.FilterTreeFileExtension;
                }
                else
                {
                    if (string.IsNullOrEmpty(file_name))
                        file_name = "_" + DateTime.Now.ToString("yyMMddHHmmss") + "." + Program.FilterTreeFileExtension;

                    SaveFileDialog d = new SaveFileDialog();
                    d.OverwritePrompt = false;
                    d.FileName = file_name;
                    d.Title = "Save the current filter tree to a file";
                    d.AddExtension = true;
                    d.DefaultExt = Program.FilterTreeFileExtension;
                    d.Filter = "Filter tree files (*." + Program.FilterTreeFileExtension + ")|*." + Program.FilterTreeFileExtension + "|All files (*.*)|*.*";
                    if (!string.IsNullOrEmpty(Settings.Default.LastFilterTreeFile))
                        d.InitialDirectory = Path.GetDirectoryName(Settings.Default.LastFilterTreeFile);
                    if (d.ShowDialog(this) != DialogResult.OK)
                        return;
                    Settings.Default.LastFilterTreeFile = d.FileName;
                    Settings.Default.Save();
                    file = d.FileName;
                }

                Parser p = GetFilterTreeParser();
                if (p == null)
                    return;
                if (p.RootFilters == null || p.RootFilters.Length < 1)
                    return;

                FileInfo fi = new FileInfo(file);
                if (fi.Exists)
                {
                    string old_dir = fi.Directory + "\\_rewritten_filters";
                    Directory.CreateDirectory(old_dir);
                    string old_file = old_dir + "\\" + fi.Name.Insert(fi.Name.LastIndexOf("."), "_" + DateTime.Now.ToString("yyMMddHHmmss"));
                    fi.MoveTo(old_file);
                }

                p.WriteFilterTreeToXmlFile(file);

                int p1 = file.LastIndexOf(@"\") + 1;
                int p2 = file.LastIndexOf(".");
                FilterTreeName.Text = file.Substring(p1, p2 - p1);

                FilterTreeChanged = false;

                if (!string.IsNullOrWhiteSpace(Document.File))
                {
                    string ft_folder = Path.GetDirectoryName(file);
                    if (Settings.Default.FilterTreeFolder2SourceFolder.Contains(ft_folder))
                        Settings.Default.FilterTreeFolder2SourceFolder.Remove(ft_folder);
                    Settings.Default.FilterTreeFolder2SourceFolder.Add(ft_folder, Path.GetDirectoryName(Document.File));
                    if (Settings.Default.FilterTreeFolder2SourceFolder.Count > 30)
                        Settings.Default.FilterTreeFolder2SourceFolder.RemoveAt(0);
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
        }

        /// <summary>
        /// on opening filter tree from file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OpenFilterTree_Click(object sender, EventArgs e)
        {
            try
            {
                if (ToSavePreviousFilterTree())
                    return;

                OpenFileDialog d = new OpenFileDialog();
                d.Title = "Pick a filter tree file to open within DataSifter";
                d.Filter = "Filter tree files (*." + Program.FilterTreeFileExtension + ")|*." + Program.FilterTreeFileExtension + "|All files (*.*)|*.*";
                d.InitialDirectory = get_corresponding_filter_tree_folder(Settings.Default.LastSourceFile);
                if (string.IsNullOrWhiteSpace(d.InitialDirectory) || !Directory.Exists(d.InitialDirectory))
                    d.InitialDirectory = null;
                if (d.ShowDialog(this) != DialogResult.OK || d.FileName == "")
                    return;
                Settings.Default.LastFilterTreeFile = d.FileName;
                Settings.Default.Save();
                LoadFilterTree(d.FileName);
                Focus();
            }
            catch(Exception ex)
            {
                Message.Error(ex);
            }
        }

        string get_corresponding_filter_tree_folder(string source_file)
        {
            string s_folder = Path.GetDirectoryName(source_file);
            foreach (string ft_folder in Settings.Default.FilterTreeFolder2SourceFolder.Keys)
            {
                string sf = (string)Settings.Default.FilterTreeFolder2SourceFolder[ft_folder];
                if (sf == s_folder)
                    return ft_folder;
            }
            return Path.GetDirectoryName(Settings.Default.LastFilterTreeFile);
        }

        /// <summary>
        /// shows if the current filter tree was modified after saving
        /// </summary>
        internal bool FilterTreeChanged
        {
            get 
            {
                return pFilterTreeNameChangedHighlight.BackColor == CHANGED_FILTER_TREE_COLOR; 
            }
            set
            {
                if (value)
                {
                    pFilterTreeNameChangedHighlight.BackColor = CHANGED_FILTER_TREE_COLOR;
                    //SourceForm.This.FilterTreeChangedTime = DateTime.Now;
                }
                else
                    pFilterTreeNameChangedHighlight.BackColor = Color.Empty;
            }
        }
        readonly Color CHANGED_FILTER_TREE_COLOR = Color.Red;

        /// <summary>
        /// ask if previous filter tree must be saved
        /// </summary>
        /// <returns></returns>
        internal bool ToSavePreviousFilterTree()
        {
            if (FilterTreeChanged && FilterTree.Nodes.Count > 0 && Message.YesNo("The current filter tree was not saved so its modification will be lost. Do you want to save it?"))
                return true;
            FilterTreeChanged = false;
            return false;
        }

        /// <summary>
        /// clear tegex tree before starting editing a new
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFilterTree_Click(object sender, EventArgs e)
        {
            if (ToSavePreviousFilterTree())
                return;

            FilterTree.Nodes.Clear();
            CurrentFilterControl = null;
            //Filter_flowLayoutPanel.BackColor = Color.Empty;
            pLevelHighlight.BackColor = Color.Empty;
            FilterTreeName.Text = "";
            InputGroupName.Items.Clear();
            FilterComment.Text = "";

            //if (string.IsNullOrWhiteSpace(FilterTreeName.Text))
            //{
            //    FilterTreeName.Text = Path.GetFileNameWithoutExtension(Document.Title);
            //    FilterTreeChanged = true;
            //}
        }

        #endregion

        #region OutputData routines

        /// <summary>
        /// outputs parse data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputData_Click(object sender, EventArgs e)
        {
            Parser p = GetFilterTreeParser();
            if (p == null)
                return;
            List<OutputGroup> all_ogs = new List<OutputGroup>();
            GetOutputGroupNames(p.RootFilters, null, null, all_ogs, true);
            List<OutputGroup> captured_ogs = null;
            //if (SourceForm.This.ParserForCurrentChain != null)
            //{
            //    captured_ogs = new List<OutputGroup>();
            //    GetOutputGroupNames(SourceForm.This.ParserForCurrentChain.RootFilterNodes, null, null, captured_ogs, true);
            //}
            OutputForm f = new OutputForm(all_ogs, captured_ogs);
            f.ShowDialog();
        }
        
        /// <summary>
        /// Return output group names as an array
        /// </summary>
        /// <param name="rns">root filter nodes</param>
        /// <param name="parent_og"></param>
        /// <param name="parent_group_name"></param>
        /// <param name="ogs">List</param>
        /// <param name="named_only"></param>
        internal void GetOutputGroupNames(Filter[] fs, OutputGroup parent_og, string parent_group_name, List<OutputGroup> ogs, bool named_only)
        {
            foreach (Filter f in fs)
            {
                if (parent_group_name != null && parent_group_name != f.InputGroupName)
                    continue;
                foreach (string gn in f.GetGroupRawNames())
                {
                    int t;
                    OutputGroup og = parent_og;
                    if (!named_only || !int.TryParse(gn, out t))
                    {//named group
                        og = new OutputGroup(gn, parent_og);
                        ogs.Add(og);
                    }
                    GetOutputGroupNames(f.Next, og, gn, ogs, named_only);
                }
            }
        }

        #endregion

        #region miscellaneous

        private void bSaveAsPatternFilter_Click(object sender, EventArgs e)
        {
            TreeNode stn = (TreeNode)FilterTree.SelectedNode;
            if (stn == null)
            {
                Message.Error("No filter selected!");
                return;
            }

            try
            { 
                Filter f = (Filter)stn.Tag;
                Settings.Default.FilterTypeName2NewFilter[f.GetType().Name] = f.GetDefinition();
                Settings.Default.Save();
            }
            catch(Exception ex)
            {
                Message.Error(ex);
                return;
            }
        }

        /// <summary>
        /// start/break parsing thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonParse_CheckedChanged(object sender, EventArgs e)
        {
            if (this.buttonParse.Checked && SourceForm.This.RunParse())
                //this.buttonParse.BackColor = Color.FromArgb(0, 96, 88);
                this.buttonParse.BackColor = Color.FromArgb(240, 20, 60);
            else
            {
                SourceForm.This.StopParse();
                this.buttonParse.BackColor = this.BackColor;
            }
        }

        /// <summary>
        /// shows number of named groups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterTree_Enter(object sender, EventArgs e)
        {
            Parser p = GetFilterTreeParser();
            if (p == null)
                return;
            List<OutputGroup> ogs = new List<OutputGroup>();
            GetOutputGroupNames(p.RootFilters, null, null, ogs, true);
            SourceForm.This.SetStatus("Number of named groups: " + ogs.Count);
        }

        private void FilterTreeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            This.Visible = false;
        }

        #endregion
    }
}