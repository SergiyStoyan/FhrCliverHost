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
using System.Reflection;

namespace Cliver.DataSifter
{
    /// <summary>
    /// Control (or Form) that provides creating/editing filter tree.
    /// This part provides the main functionality that includes inserting/deleting filter nodes.
    /// </summary>
    internal partial class FilterTreeForm : BaseControl //BaseForm
    {
        #region initializing

        static FilterTreeForm()
        {
            This = new FilterTreeForm();

            Document.DocumentUpdated += Document_DocumentUpdated;

            This.splitContainer1.BackColor = SourceForm.SplitterColor;
            This.splitContainer1.Panel1.BackColor = SystemColors.Control;
            This.splitContainer1.Panel2.BackColor = SystemColors.Control;
            This.splitContainer2.BackColor = SourceForm.SplitterColor;
            This.splitContainer2.Panel1.BackColor = SystemColors.Control;
            This.splitContainer2.Panel2.BackColor = SystemColors.Control;
        }

        static void Document_DocumentUpdated()
        {
            if (string.IsNullOrWhiteSpace(This.FilterTreeName.Text))
            {
                This.FilterTreeName.Text = Path.GetFileNameWithoutExtension(Document.Title);
                This.FilterTreeChanged = true;
            }
        }

        static internal readonly FilterTreeForm This;

        static readonly string prepared_filter_tree_dir = Program.AppDir + "\\PreparedFilterTrees";
        static readonly string deleted_prepared_filter_tree_dir = prepared_filter_tree_dir + "\\deleted";
        static readonly string title = "- FILTER TREE - " + Program.Title;

        private FilterTreeForm()
        {
            InitializeComponent();

            set_tool_tip();

            this.Text = title;
            load_prepared_filter_trees();

            foreach (Type ft in FilterApi.GetFilterTypes())
                FilterTypes.Items.Add(new FilterTypesItem(FilterApi.GetFilterReadableTypeName(ft), ft));
            if (FilterTypes.Items.Count > 0)
                FilterTypes.SelectedIndex = 0;
            else
            {
                Add.Enabled = false;
                Insert.Enabled = false;
            }
        }

        //(!)it is set to public because of a bug in VS2005 removing its constructor from designer
       /* public FilterTreeForm()
        {
            InitializeComponent();

            set_tool_tip();

            this.Text = title;
            load_prepared_filter_trees();

            foreach (Type ft in FilterApi.GetFilterTypes())
                FilterTypes.Items.Add(new FilterTypesItem(FilterApi.GetFilterName(ft), ft));
            if (FilterTypes.Items.Count > 0)
                FilterTypes.SelectedIndex = 0;
            else
            {
                Add.Enabled = false;
                Insert.Enabled = false;
            }
        }*/

        public class FilterTypesItem
        {
            public readonly string Text;
            public readonly Type Value;

            public override string ToString()
            {
                return Text;
            }

            public FilterTypesItem(string text, Type value)
            {
                Text = text;
                Value = value;
            }
        }

        /// <summary>
        /// Load filter tree from xml file
        /// </summary>
        /// <param name="filter_tree_xml_file"></param>
        internal void LoadFilterTree(string filter_tree_xml_file)
        {
            try
            {
                FilterTree.Nodes.Clear();
                CurrentFilterControl = null;
               // Filter_flowLayoutPanel.BackColor = Color.Empty;
                pLevelHighlight.BackColor = Color.Empty;
                InputGroupName.Items.Clear();
                FilterComment.Text = "";

                if (filter_tree_xml_file == null)
                    return;

                this.Text = filter_tree_xml_file.Substring(filter_tree_xml_file.LastIndexOf("\\") + 1) + " " + title;
                
                //create filter tree
                Parser p = new Parser(filter_tree_xml_file);
                foreach (Filter lr in p.RootFilters)
                    FilterTree.Nodes.Add(create_tree(lr, 0));
                foreach (TreeNode tn in FilterTree.Nodes)
                    tn.ExpandAll();

                //find default filter chain and check it
                if (FilterTree.Nodes.Count > 0)
                {
                    TreeNode tn = FilterTree.Nodes[0];
                    for (; tn.FirstNode != null; tn = tn.FirstNode) ;
                    tn.Checked = true;
                }

                string tree_name = filter_tree_xml_file;
                tree_name = tree_name.Substring(tree_name.LastIndexOf("\\") + 1);
                if (tree_name.EndsWith("." + Program.FilterTreeFileExtension, StringComparison.InvariantCultureIgnoreCase))
                    tree_name = tree_name.Substring(0, tree_name.LastIndexOf("."));
                FilterTreeName.Text = tree_name;
                FilterTreeFileDir.Text = Path.GetDirectoryName(Settings.Default.LastFilterTreeFile) + @"\";
                FilterTreeFileDir.SelectionStart = FilterTreeFileDir.Text.Length;
                FilterTreeFileDir.ScrollToCaret();
            }
            catch (Exception e)
            {
                Message.Error(e);
            }
        }

        FilterControl CurrentFilterControl
        {
            get
            {
                if (FilterBox.Controls.Count < 1)
                    return null;
                return (FilterControl)FilterBox.Controls[0];
            } 
            set
            {
                FilterBox.Controls.Clear();
                if (value != null)
                {
                    FilterBox.Controls.Add(value);
                    value.Dock = DockStyle.Fill;
                }
            }
        }

        #endregion

        #region FilterTree initiating

        private TreeNode create_tree(Filter filter, int level)
        {
            if (filter == null)
                return null;
            TreeNode tn = create_tree_node(filter, level);
            foreach (Filter r in filter.Next)
                tn.Nodes.Add(create_tree(r, level + 1));

            return tn;
        }

        TreeNode create_tree_node(Filter f, int level)
        {
            TreeNode tn = new TreeNode();
            set_tree_node(tn, f);
            tn.BackColor = Settings.Default.GetFilterBackColor(level);
            return tn;
        }

        void set_tree_node(TreeNode tn, Filter f)
        {
            tn.Tag = f;
            tn.Text = string.Join(", ", f.GetGroupRawNames()) + NODE_SEPARATOR + f.GetVisibleDefinition();
        }

        //(!) must not be configurable by user
        const string NODE_SEPARATOR = " :: ";

        /// <summary>
        /// fill the group drop down menu after selected filter tree node was changed
        /// </summary>
        void fill_InputGroupName()
        {
            try
            {
                InputGroupName.Items.Clear();
                InputGroupName.Text = "";

                TreeNode stn = (TreeNode)FilterTree.SelectedNode;
                if (stn == null)
                    return;

                TreeNode ptn = null;

                if (!is_tree_node_within_checked_path(stn))
                    ptn = stn.Parent;
                else
                {//looking for nearest checked parent
                    for (TreeNode tn = stn.Parent; tn != null; tn = tn.Parent)
                        if (tn.Checked)
                        {
                            ptn = tn;
                            break;
                        }
                }
                if (ptn == null)
                {
                    InputGroupName.Enabled = false;
                    return;
                }

                InputGroupName.Enabled = true;

                string[] gns = ((Filter)ptn.Tag).GetGroupRawNames();
                string ign = ((Filter)stn.Tag).InputGroupName;
                bool group_found = false;
                foreach (string gn in gns)
                {
                    InputGroupName.Items.Add(gn);
                    if (ign == gn)
                    {
                        InputGroupName.SelectedIndex = InputGroupName.Items.Count - 1;
                        group_found = true;
                    }
                }
                if (group_found)
                    InputGroupName.BackColor = Color.Empty;
                else
                    InputGroupName.BackColor = Color.Coral;
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
        }

        /// <summary>
        /// used when inserting/removing node
        /// </summary>
        /// <param name="tn"></param>
        void set_tree_node_color(TreeNode tn)
        {
            tn.BackColor = Settings.Default.GetFilterBackColor(tn.Level);
            foreach (TreeNode n in tn.Nodes)
                set_tree_node_color(n);
        }

        private void FilterTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                empty_selection_in_FilterBox();
        }

        void empty_selection_in_FilterBox()
        {
            //clear FilterBox from selection to give a possibility adding filter to 0 level
            FilterTree.SelectedNode = null;
            CurrentFilterControl = null;
            //Filter_flowLayoutPanel.BackColor = Color.Empty;
            pLevelHighlight.BackColor = Color.Empty;
            InputGroupName.Items.Clear();
            FilterComment.Text = "";
        }

        #endregion

        #region edit FilterTree node routines

        /// <summary>
        /// on new filter tree node was selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TreeNode stn = (TreeNode)FilterTree.SelectedNode;
                if (stn == null)
                    return;

                fill_InputGroupName();

                Filter f = (Filter)stn.Tag;
                CurrentFilterControl = f.CreateControl();
                FilterType.Text = f.ReadableTypeName;
                //Filter_flowLayoutPanel.BackColor = stn.BackColor;
                pLevelHighlight.BackColor = stn.BackColor;
                FilterComment.Text = f.Comment;
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
        }

        /// <summary>
        /// on remove filter tree node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_Click(object sender, EventArgs e)
        {
            TreeNode stn = (TreeNode)FilterTree.SelectedNode;
            if (stn == null)
                return;

            if (highest_checked_node == stn)
            {
                TreeNode[] ctns = GetCheckedNodesChain();
                if (ctns.Length < 2)
                    highest_checked_node = null;
                else
                    highest_checked_node = ctns[ctns.Length - 2];
            }

            TreeNode ptn = stn.Parent;
            TreeNodeCollection tns;
            if (ptn != null)
                tns = ptn.Nodes;
            else
                tns = FilterTree.Nodes;

            tns.Remove(stn);
            foreach (TreeNode tn in stn.Nodes)
            {
                tns.Add(tn);
                set_tree_node_color(tn);
            }

            if (FilterTree.Nodes.Count < 1)
                FilterTreeName.Text = "";

            FilterTreeChanged = true;
        }

        /// <summary>
        /// on insert new filter tree node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Insert_Click(object sender, EventArgs e)
        {
            TreeNode stn = (TreeNode)FilterTree.SelectedNode;
            if (stn == null)
                return;
            Filter f = FilterApi.CreateDefaultFilter(((FilterTypesItem)FilterTypes.SelectedItem).Value);
            TreeNode ptn = stn.Parent;
            TreeNodeCollection tns;
            int level;
            if (ptn == null)
            {
                level = 0;
                tns = FilterTree.Nodes;
            }
            else
            {
                tns = ptn.Nodes;
                level = ptn.Level + 1;
                string[] gns = ((Filter)ptn.Tag).GetGroupRawNames();
                if (gns.Length > 1)
                    f.InputGroupName = gns[1];
                else
                    f.InputGroupName = gns[0];
            }
            TreeNode tn = create_tree_node(f, level);
            tns.Insert(stn.Index, tn);
            tns.Remove(stn);
            tn.Nodes.Add(stn);

            set_tree_node_color(stn);
            {
                string[] gns = f.GetGroupRawNames();
                if (gns.Length > 1)
                    ((Filter)stn.Tag).InputGroupName = gns[1];
                else
                    ((Filter)stn.Tag).InputGroupName = gns[0];
            }

            tn.ExpandAll();
            FilterTree.SelectedNode = tn;
            tn.Checked = true;

            FilterTreeChanged = true;
        }

        /// <summary>
        /// on add new filter tree node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, EventArgs e)
        {
            Type ft = ((FilterTypesItem)FilterTypes.SelectedItem).Value;
            Filter f = FilterApi.CreateFilter(ft, FilterApi.GetFilterVersion(ft), null, null, null);
            TreeNode stn = FilterTree.SelectedNode;
            TreeNode tn;
            if (stn != null)
            {
                string[] gns = ((Filter)stn.Tag).GetGroupRawNames();
                if (gns.Length > 1)
                    f.InputGroupName = gns[1];
                else
                    f.InputGroupName = gns[0];
                tn = create_tree_node(f, stn.Level + 1);
                stn.Nodes.Add(tn);
                stn.ExpandAll();
            }
            else
            {
                tn = create_tree_node(f, 0);
                FilterTree.Nodes.Add(tn);
            }
            FilterTree.SelectedNode = tn;
            tn.Checked = true;

            FilterTreeChanged = true;
        }

        /// <summary>
        /// Moves filter node up among siblings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bUp_Click(object sender, EventArgs e)
        {
            if (FilterTree.SelectedNode == null)
            {
                Message.Error("Select a node");
                return;
            }

            TreeNode tn = FilterTree.SelectedNode;
            TreeNodeCollection tnc = tn.Parent == null ? tn.TreeView.Nodes : tn.Parent.Nodes;
            if (tn.Index > 0)
            {
                int i = tn.Index - 1;
                tn.Remove();
                tnc.Insert(i, tn);
                FilterTree.SelectedNode = tn;
            }
        }

        /// <summary>
        /// Moves filter node down among siblings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bDown_Click(object sender, EventArgs e)
        {
            if (FilterTree.SelectedNode == null)
            {
                Message.Error("Select a node");
                return;
            }

            TreeNode tn = FilterTree.SelectedNode;
            TreeNodeCollection tnc = tn.Parent == null ? tn.TreeView.Nodes : tn.Parent.Nodes;
            if (tn.Index < tnc.Count - 1)
            {
                int i = tn.Index + 1;
                tn.Remove();
                tnc.Insert(i, tn);
                FilterTree.SelectedNode = tn;
            }
        }

        /// <summary>
        /// remove complete branch from the selected node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bRemoveBranch_Click(object sender, EventArgs e)
        {
            TreeNode stn = (TreeNode)FilterTree.SelectedNode;
            if (stn == null)
                return;

            if (highest_checked_node != null)
                for (TreeNode tn = highest_checked_node; tn != null; tn = tn.Parent)
                {
                    if (tn == stn)
                    {
                        highest_checked_node = stn.Parent != null ? stn.Parent : null;
                        break;
                    }
                }
            stn.Remove();

            FilterTreeChanged = true;
        }

        #endregion

        #region checking if filter node was edited

        private void FilterTreeName_TextChanged(object sender, EventArgs e)
        {
            //if (!string.IsNullOrWhiteSpace(FilterTreeName.Text))
            //    FilterTreeChanged = true;
        }

        /// <summary>
        /// sets the filter name to the selected filter tree node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterComment_Leave(object sender, EventArgs e)
        {
            set_tree_node_if_filter_node_was_changed();
        }

        /// <summary>
        /// on filter edit box lost focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterBox_Leave(object sender, EventArgs e)
        {
            set_tree_node_if_filter_node_was_changed();
        }

        void set_tree_node_if_filter_node_was_changed()
        {
            TreeNode stn = (TreeNode)FilterTree.SelectedNode;
            if (stn == null)
                return;

            if (CurrentFilterControl == null)
                return;
            
            Filter f1 = (Filter)stn.Tag;

            string d2 = null;
            try
            {
                d2 = CurrentFilterControl.GetUpdatedFilterDefinition();
            }
            catch (Exception e)
            {
                Message.Error(e);
                return;
            }
            if (d2 == null)
                return;

            if (f1.ReadableTypeName == FilterType.Text && f1.InputGroupName == InputGroupName.Text && f1.GetDefinition() == d2 && f1.Comment == FilterComment.Text)
                return;
            
            FilterTreeChanged = true;
            try
            {
                Filter f2 = FilterApi.CreateFilter(f1.GetType(), f1.Version, d2, InputGroupName.Text, FilterComment.Text);
                set_tree_node(stn, f2);
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
            return;
        }

        private void InputGroupName_Leave(object sender, EventArgs e)
        {
            set_tree_node_if_filter_node_was_changed();
            InputGroupName.BackColor = Color.Empty;
        }

        #endregion

        #region check if filter tree file name was edited
        private void FilterTreeName_Enter(object sender, EventArgs e)
        {
            filter_tree_name = FilterTreeName.Text;
        }

        private void FilterTreeName_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(This.FilterTreeName.Text))
            {
                This.FilterTreeName.Text = Path.GetFileNameWithoutExtension(Document.Title);
                FilterTreeChanged = true;
            }

            if (filter_tree_name != FilterTreeName.Text)
                FilterTreeChanged = true;
        }
        string filter_tree_name; 
        #endregion

        #region filter chain routines

        /// <summary>
        /// defines the current debug filter chain after checking filter node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (ignore_checked)
                return;

            if (!e.Node.Checked)
            {
                e.Node.BackColor = Color.Empty;
                if (FilterTree.SelectedNode == e.Node)
                    FilterTree_AfterSelect(null, null);
                if (highest_checked_node == e.Node)
                {
                    TreeNode[] tns = GetCheckedNodesChain();
                    if (tns.Length < 1)
                        highest_checked_node = null;
                    else
                        highest_checked_node = tns[tns.Length - 1];
                }
                return;
            }

            if (is_tree_node_within_checked_path(e.Node))
            {
                e.Node.BackColor = Settings.Default.GetFilterBackColor(e.Node.Level);
                if (FilterTree.SelectedNode == e.Node)
                    FilterTree_AfterSelect(null, null);
                return;
            }

            ignore_checked = true;
            uncheck_nodes(FilterTree.Nodes);

            for (TreeNode tn = e.Node; tn != null; tn = tn.Parent)
            {
                tn.Checked = true;
                tn.BackColor = Settings.Default.GetFilterBackColor(tn.Level);
            }
            highest_checked_node = e.Node;

            FilterTree_AfterSelect(null, null);
            ignore_checked = false;
        }
        bool ignore_checked = false;

        /// <summary>
        /// Create checked filter chain by highest_checked_node
        /// </summary>
        /// <returns></returns>
        internal TreeNode[] GetCheckedNodesChain()
        {
            if (highest_checked_node == null)
                return new TreeNode[0];
            List<TreeNode> checked_nodes = new List<TreeNode>();
            for (TreeNode tn = highest_checked_node; tn != null; tn = tn.Parent)
            {
                if (tn.Checked)
                    checked_nodes.Insert(0, tn);
            }
            return checked_nodes.ToArray();
        }
        TreeNode highest_checked_node;

        /// <summary>
        /// Create a chain of checked filteres to test it within DataSifter
        /// </summary>
        /// <returns>Parser, or null if error</returns>
        internal Parser GetFilterChainParser()
        {
            try
            {
                Filter[] prns = new Filter[0];
                TreeNode[] tns = GetCheckedNodesChain();
                for (int i = tns.Length - 1; i >= 0; i--)
                {
                    Filter fn = (Filter)tns[i].Tag;
                    fn.Level = tns[i].Level;
                    fn.Next = prns;
                    prns = new Filter[1] { fn };
                }

                //check for errors
                return new Parser(prns);
            }
            catch (Exception e)
            {
                Message.Error(e);
            }
            return null;
        }

        private void uncheck_nodes(TreeNodeCollection tns)
        {
            if (tns == null)
                return;
            foreach (TreeNode tn in tns)
            {
                tn.Checked = false;
                tn.BackColor = Color.Empty;
                uncheck_nodes(tn.Nodes);
            }
        }

        /// <summary>
        /// used while building the current debug filter chain 
        /// </summary>
        /// <param name="tree_node"></param>
        /// <returns></returns>
        bool is_tree_node_within_checked_path(TreeNode tree_node)
        {
            if (highest_checked_node == null || tree_node.Level >= highest_checked_node.Level)
                return false;
            for (TreeNode tn = highest_checked_node; tn != null; tn = tn.Parent)
                if (tn == tree_node)
                    return true;
            return false;
        }

        #endregion

        #region painting

        /// <summary>
        /// draws filter tree item in a customized manner with several colors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterTree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            string text = e.Node.Text;
            StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
            Font font = FilterTree.Font;
            PointF p = new PointF((float)e.Bounds.X, (float)e.Bounds.Y + ((float)(e.Bounds.Height - font.Height)) / 2f);//(float)e.Bounds.Y);//
            SizeF s = new SizeF();
            Brush def_brush;
            Brush rc_brush;
            Brush rgn_brush;
            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                Brush back_brush = new SolidBrush(SystemColors.ActiveCaption);
                e.Graphics.FillRectangle(back_brush, e.Bounds);
                def_brush = new SolidBrush(SystemColors.ActiveCaptionText);
                rc_brush = def_brush;
                rgn_brush = new SolidBrush(Color.Yellow);
            }
            else
            {
                Brush back_brush = new SolidBrush(e.Node.BackColor);
                e.Graphics.FillRectangle(back_brush, e.Bounds);
                def_brush = new SolidBrush(FilterTree.ForeColor);
                rc_brush = new SolidBrush(Settings.Default.FilterCommentColor);
                rgn_brush = new SolidBrush(Settings.Default.FilterGroupNameColor);
            }
            int p1 = text.IndexOf(NODE_SEPARATOR);
            if (p1 < 0)
                return;
            // string filter_comment = text.Substring(0, p1);
           // int p2 = text.IndexOf(NODE_SEPARATOR, p1 + NODE_SEPARATOR.Length) + NODE_SEPARATOR.Length;
           // if (p2 < 0)
           //     return;
            string group_names = text.Substring(0, p1);
            string filter = text.Substring(p1);
            //if (!string.IsNullOrEmpty(filter_comment))
            //{
            //    e.Graphics.DrawString(filter_comment, font, rc_brush, p, sf);
            //    s = e.Graphics.MeasureString(filter_comment, font, p, sf);
            //    p.X += s.Width;
            //}
            e.Graphics.DrawString(group_names, font, rgn_brush, p, sf);
            s = e.Graphics.MeasureString(group_names, font, p, sf);
            p.X += s.Width;
            e.Graphics.DrawString(filter, font, def_brush, p, sf);
        }

        #endregion

        #region FilterTree mouse right-click menu commands
        private void upToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bUp_Click(null, null);
        }

        private void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bDown_Click(null, null);
        }

        private void FilterTree_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode tn = FilterTree.GetNodeAt(e.X, e.Y);
            if (tn == null)
                empty_selection_in_FilterBox();
            if (e.Button == MouseButtons.Left)
                FilterTree.SelectedNode = tn;
            else
            {
                if (tn != null)
                {
                    FilterTreeMenu.Items["upToolStripMenuItem"].Enabled = true;
                    FilterTreeMenu.Items["downToolStripMenuItem"].Enabled = true;
                    FilterTreeMenu.Items["insertToolStripMenuItem"].Enabled = true;
                    FilterTreeMenu.Items["deleteToolStripMenuItem"].Enabled = true;
                    FilterTreeMenu.Items["deleteBranchToolStripMenuItem"].Enabled = true;
                    FilterTree.SelectedNode = tn;
                }
                else
                {
                    FilterTreeMenu.Items["upToolStripMenuItem"].Enabled = false;
                    FilterTreeMenu.Items["downToolStripMenuItem"].Enabled = false;
                    FilterTreeMenu.Items["insertToolStripMenuItem"].Enabled = false;
                    FilterTreeMenu.Items["deleteToolStripMenuItem"].Enabled = false;
                    FilterTreeMenu.Items["deleteBranchToolStripMenuItem"].Enabled = false;
                }
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add_Click(null, null);
        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Insert_Click(null, null);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Remove_Click(null, null);
        }

        private void deleteBranchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bRemoveBranch_Click(null, null);
        }
        #endregion

        #region tint filter syntax in FilterBox

        /// <summary>
        /// tints filter syntax in FilterBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterBox_TextChanged(object sender, EventArgs e)
        {
        }

        #endregion

        #region FilterBox key handlers and mouse right-click menu commands 

        private void FilterBoxMenu_Opening(object sender, CancelEventArgs e)
        {
            if (CurrentFilterControl == null)
                return;

            if (CurrentFilterControl.CanUndo)
                FilterBoxMenu.Items["undoFilterBoxMenuItem"].Enabled = true;
            else
                FilterBoxMenu.Items["undoFilterBoxMenuItem"].Enabled = false;

            if (CurrentFilterControl.CanRedo)
                FilterBoxMenu.Items["redoFilterBoxMenuItem"].Enabled = true;
            else
                FilterBoxMenu.Items["redoFilterBoxMenuItem"].Enabled = false;

            if (Clipboard.ContainsText())
                FilterBoxMenu.Items["pasteFilterBoxMenuItem"].Enabled = true;
            else
                FilterBoxMenu.Items["pasteFilterBoxMenuItem"].Enabled = false;

            if (string.IsNullOrEmpty(CurrentFilterControl.SelectedText))
            {
                FilterBoxMenu.Items["copyFilterBoxMenuItem"].Enabled = false;
                FilterBoxMenu.Items["cutFilterBoxMenuItem"].Enabled = false;
                FilterBoxMenu.Items["deleteFilterBoxMenuItem"].Enabled = false;
            }
            else
            {
                FilterBoxMenu.Items["copyFilterBoxMenuItem"].Enabled = true;
                FilterBoxMenu.Items["cutFilterBoxMenuItem"].Enabled = true;
                FilterBoxMenu.Items["deleteFilterBoxMenuItem"].Enabled = true;
            }
        }
        
        private void copyFilterBoxMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentFilterControl == null)
                return;
            CurrentFilterControl.Copy();
        }

        private void pasteFilterBoxMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentFilterControl == null)
                return;
            string s = Clipboard.GetText(System.Windows.Forms.TextDataFormat.Text);
            CurrentFilterControl.SelectedText = s;
        }

        private void cutFilterBoxMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentFilterControl == null)
                return;
            CurrentFilterControl.Cut();
        }

        private void deleteFilterBoxMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentFilterControl == null)
                return;
            CurrentFilterControl.SelectedText = "";
        }

        private void undoFilterBoxMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentFilterControl == null)
                return;
            CurrentFilterControl.Undo();
        }

        private void redoFilterBoxMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentFilterControl == null)
                return;
            CurrentFilterControl.Redo();
        }

        private void FilterBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.V))
                Clipboard.SetData(DataFormats.Text, Clipboard.GetText(System.Windows.Forms.TextDataFormat.Text));
        }
        #endregion
    }
}