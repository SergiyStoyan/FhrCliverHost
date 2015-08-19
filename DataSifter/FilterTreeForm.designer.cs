namespace Cliver.DataSifter
{
    partial class FilterTreeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterTreeForm));
            this.FilterTree = new System.Windows.Forms.TreeView();
            this.FilterTreeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteBranchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.upToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.NewFilterTree = new System.Windows.Forms.Button();
            this.OpenFilterTree = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.buttonParse = new System.Windows.Forms.CheckBox();
            this.buttonOutputData = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.FilterTypes = new System.Windows.Forms.ComboBox();
            this.Add = new System.Windows.Forms.Button();
            this.Insert = new System.Windows.Forms.Button();
            this.Remove = new System.Windows.Forms.Button();
            this.bRemoveBranch = new System.Windows.Forms.Button();
            this.bUp = new System.Windows.Forms.Button();
            this.bDown = new System.Windows.Forms.Button();
            this.bSaveAsPatternFilter = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.pFilterTreeNameChangedHighlight = new System.Windows.Forms.Panel();
            this.FilterTreeName = new System.Windows.Forms.TextBox();
            this.FilterComment = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.PreparedFilterTrees = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.AddPrepared = new System.Windows.Forms.Button();
            this.SavePrepared = new System.Windows.Forms.Button();
            this.DeletePrepared = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.FilterBox = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.FilterTreeFileDir = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Filter_flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.pLevelHighlight = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.FilterType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.InputGroupName = new System.Windows.Forms.ComboBox();
            this.FilterBoxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.undoFilterBoxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoFilterBoxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cutFilterBoxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFilterBoxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteFilterBoxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFilterBoxMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FilterTreeMenu.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.Filter_flowLayoutPanel.SuspendLayout();
            this.FilterBoxMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // FilterTree
            // 
            this.FilterTree.CheckBoxes = true;
            this.FilterTree.ContextMenuStrip = this.FilterTreeMenu;
            this.FilterTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilterTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.FilterTree.HideSelection = false;
            this.FilterTree.Location = new System.Drawing.Point(0, 0);
            this.FilterTree.Name = "FilterTree";
            this.FilterTree.Size = new System.Drawing.Size(952, 120);
            this.FilterTree.TabIndex = 0;
            this.FilterTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.FilterTree_AfterCheck);
            this.FilterTree.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.FilterTree_DrawNode);
            this.FilterTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.FilterTree_AfterSelect);
            this.FilterTree.Enter += new System.EventHandler(this.FilterTree_Enter);
            this.FilterTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterTree_KeyDown);
            this.FilterTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FilterTree_MouseDown);
            // 
            // FilterTreeMenu
            // 
            this.FilterTreeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.insertToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.deleteBranchToolStripMenuItem,
            this.toolStripSeparator1,
            this.upToolStripMenuItem,
            this.downToolStripMenuItem});
            this.FilterTreeMenu.Name = "contextMenuStrip1";
            this.FilterTreeMenu.Size = new System.Drawing.Size(158, 142);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.insertToolStripMenuItem.Text = "Insert";
            this.insertToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.deleteToolStripMenuItem.Text = "Remove";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // deleteBranchToolStripMenuItem
            // 
            this.deleteBranchToolStripMenuItem.Name = "deleteBranchToolStripMenuItem";
            this.deleteBranchToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.deleteBranchToolStripMenuItem.Text = "Remove branch";
            this.deleteBranchToolStripMenuItem.Click += new System.EventHandler(this.deleteBranchToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(154, 6);
            // 
            // upToolStripMenuItem
            // 
            this.upToolStripMenuItem.Name = "upToolStripMenuItem";
            this.upToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.upToolStripMenuItem.Text = "Up";
            this.upToolStripMenuItem.Click += new System.EventHandler(this.upToolStripMenuItem_Click);
            // 
            // downToolStripMenuItem
            // 
            this.downToolStripMenuItem.Name = "downToolStripMenuItem";
            this.downToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.downToolStripMenuItem.Text = "Down";
            this.downToolStripMenuItem.Click += new System.EventHandler(this.downToolStripMenuItem_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.NewFilterTree);
            this.flowLayoutPanel1.Controls.Add(this.OpenFilterTree);
            this.flowLayoutPanel1.Controls.Add(this.Save);
            this.flowLayoutPanel1.Controls.Add(this.buttonParse);
            this.flowLayoutPanel1.Controls.Add(this.buttonOutputData);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.FilterTypes);
            this.flowLayoutPanel1.Controls.Add(this.Add);
            this.flowLayoutPanel1.Controls.Add(this.Insert);
            this.flowLayoutPanel1.Controls.Add(this.Remove);
            this.flowLayoutPanel1.Controls.Add(this.bRemoveBranch);
            this.flowLayoutPanel1.Controls.Add(this.bUp);
            this.flowLayoutPanel1.Controls.Add(this.bDown);
            this.flowLayoutPanel1.Controls.Add(this.bSaveAsPatternFilter);
            this.flowLayoutPanel1.Controls.Add(this.label8);
            this.flowLayoutPanel1.Controls.Add(this.pFilterTreeNameChangedHighlight);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(712, 29);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // NewFilterTree
            // 
            this.NewFilterTree.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.NewFilterTree.Image = ((System.Drawing.Image)(resources.GetObject("NewFilterTree.Image")));
            this.NewFilterTree.Location = new System.Drawing.Point(0, 3);
            this.NewFilterTree.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.NewFilterTree.Name = "NewFilterTree";
            this.NewFilterTree.Size = new System.Drawing.Size(43, 23);
            this.NewFilterTree.TabIndex = 0;
            this.NewFilterTree.UseVisualStyleBackColor = true;
            this.NewFilterTree.Click += new System.EventHandler(this.NewFilterTree_Click);
            // 
            // OpenFilterTree
            // 
            this.OpenFilterTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenFilterTree.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.OpenFilterTree.Image = ((System.Drawing.Image)(resources.GetObject("OpenFilterTree.Image")));
            this.OpenFilterTree.Location = new System.Drawing.Point(44, 3);
            this.OpenFilterTree.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.OpenFilterTree.Name = "OpenFilterTree";
            this.OpenFilterTree.Size = new System.Drawing.Size(43, 23);
            this.OpenFilterTree.TabIndex = 1;
            this.OpenFilterTree.UseVisualStyleBackColor = true;
            this.OpenFilterTree.Click += new System.EventHandler(this.OpenFilterTree_Click);
            // 
            // Save
            // 
            this.Save.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Save.Image = ((System.Drawing.Image)(resources.GetObject("Save.Image")));
            this.Save.Location = new System.Drawing.Point(88, 3);
            this.Save.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(43, 23);
            this.Save.TabIndex = 2;
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // buttonParse
            // 
            this.buttonParse.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonParse.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonParse.Image = ((System.Drawing.Image)(resources.GetObject("buttonParse.Image")));
            this.buttonParse.Location = new System.Drawing.Point(132, 3);
            this.buttonParse.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.buttonParse.Name = "buttonParse";
            this.buttonParse.Size = new System.Drawing.Size(43, 23);
            this.buttonParse.TabIndex = 3;
            this.buttonParse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonParse.UseVisualStyleBackColor = true;
            this.buttonParse.CheckedChanged += new System.EventHandler(this.buttonParse_CheckedChanged);
            // 
            // buttonOutputData
            // 
            this.buttonOutputData.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonOutputData.Image = ((System.Drawing.Image)(resources.GetObject("buttonOutputData.Image")));
            this.buttonOutputData.Location = new System.Drawing.Point(176, 3);
            this.buttonOutputData.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.buttonOutputData.Name = "buttonOutputData";
            this.buttonOutputData.Size = new System.Drawing.Size(43, 23);
            this.buttonOutputData.TabIndex = 4;
            this.buttonOutputData.UseVisualStyleBackColor = true;
            this.buttonOutputData.Click += new System.EventHandler(this.OutputData_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(223, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 24);
            this.label4.TabIndex = 20;
            this.label4.Text = "Filter:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FilterTypes
            // 
            this.FilterTypes.FormattingEnabled = true;
            this.FilterTypes.Location = new System.Drawing.Point(266, 3);
            this.FilterTypes.Name = "FilterTypes";
            this.FilterTypes.Size = new System.Drawing.Size(96, 21);
            this.FilterTypes.TabIndex = 21;
            // 
            // Add
            // 
            this.Add.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Add.Image = ((System.Drawing.Image)(resources.GetObject("Add.Image")));
            this.Add.Location = new System.Drawing.Point(365, 3);
            this.Add.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(43, 23);
            this.Add.TabIndex = 5;
            this.Add.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Add.UseVisualStyleBackColor = true;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // Insert
            // 
            this.Insert.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Insert.Image = ((System.Drawing.Image)(resources.GetObject("Insert.Image")));
            this.Insert.Location = new System.Drawing.Point(409, 3);
            this.Insert.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.Insert.Name = "Insert";
            this.Insert.Size = new System.Drawing.Size(43, 23);
            this.Insert.TabIndex = 6;
            this.Insert.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Insert.UseVisualStyleBackColor = true;
            this.Insert.Click += new System.EventHandler(this.Insert_Click);
            // 
            // Remove
            // 
            this.Remove.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Remove.Image = ((System.Drawing.Image)(resources.GetObject("Remove.Image")));
            this.Remove.Location = new System.Drawing.Point(453, 3);
            this.Remove.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.Remove.Name = "Remove";
            this.Remove.Size = new System.Drawing.Size(43, 23);
            this.Remove.TabIndex = 7;
            this.Remove.UseVisualStyleBackColor = true;
            this.Remove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // bRemoveBranch
            // 
            this.bRemoveBranch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bRemoveBranch.Image = ((System.Drawing.Image)(resources.GetObject("bRemoveBranch.Image")));
            this.bRemoveBranch.Location = new System.Drawing.Point(497, 3);
            this.bRemoveBranch.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.bRemoveBranch.Name = "bRemoveBranch";
            this.bRemoveBranch.Size = new System.Drawing.Size(43, 23);
            this.bRemoveBranch.TabIndex = 10;
            this.bRemoveBranch.UseVisualStyleBackColor = true;
            this.bRemoveBranch.Click += new System.EventHandler(this.bRemoveBranch_Click);
            // 
            // bUp
            // 
            this.bUp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bUp.Image = ((System.Drawing.Image)(resources.GetObject("bUp.Image")));
            this.bUp.Location = new System.Drawing.Point(541, 3);
            this.bUp.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.bUp.Name = "bUp";
            this.bUp.Size = new System.Drawing.Size(43, 23);
            this.bUp.TabIndex = 8;
            this.bUp.UseVisualStyleBackColor = true;
            this.bUp.Click += new System.EventHandler(this.bUp_Click);
            // 
            // bDown
            // 
            this.bDown.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bDown.Image = ((System.Drawing.Image)(resources.GetObject("bDown.Image")));
            this.bDown.Location = new System.Drawing.Point(585, 3);
            this.bDown.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.bDown.Name = "bDown";
            this.bDown.Size = new System.Drawing.Size(43, 23);
            this.bDown.TabIndex = 9;
            this.bDown.UseVisualStyleBackColor = true;
            this.bDown.Click += new System.EventHandler(this.bDown_Click);
            // 
            // bSaveAsPatternFilter
            // 
            this.bSaveAsPatternFilter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSaveAsPatternFilter.Image = ((System.Drawing.Image)(resources.GetObject("bSaveAsPatternFilter.Image")));
            this.bSaveAsPatternFilter.Location = new System.Drawing.Point(629, 3);
            this.bSaveAsPatternFilter.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.bSaveAsPatternFilter.Name = "bSaveAsPatternFilter";
            this.bSaveAsPatternFilter.Size = new System.Drawing.Size(43, 23);
            this.bSaveAsPatternFilter.TabIndex = 11;
            this.bSaveAsPatternFilter.UseVisualStyleBackColor = true;
            this.bSaveAsPatternFilter.Click += new System.EventHandler(this.bSaveAsPatternFilter_Click);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(673, 0);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 24);
            this.label8.TabIndex = 18;
            this.label8.Text = "File:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pFilterTreeNameChangedHighlight
            // 
            this.pFilterTreeNameChangedHighlight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pFilterTreeNameChangedHighlight.Location = new System.Drawing.Point(707, 0);
            this.pFilterTreeNameChangedHighlight.Margin = new System.Windows.Forms.Padding(0);
            this.pFilterTreeNameChangedHighlight.Name = "pFilterTreeNameChangedHighlight";
            this.pFilterTreeNameChangedHighlight.Size = new System.Drawing.Size(5, 5);
            this.pFilterTreeNameChangedHighlight.TabIndex = 19;
            // 
            // FilterTreeName
            // 
            this.FilterTreeName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterTreeName.Location = new System.Drawing.Point(996, 4);
            this.FilterTreeName.Margin = new System.Windows.Forms.Padding(0);
            this.FilterTreeName.Name = "FilterTreeName";
            this.FilterTreeName.Size = new System.Drawing.Size(66, 20);
            this.FilterTreeName.TabIndex = 1;
            this.FilterTreeName.TextChanged += new System.EventHandler(this.FilterTreeName_TextChanged);
            this.FilterTreeName.Enter += new System.EventHandler(this.FilterTreeName_Enter);
            this.FilterTreeName.Leave += new System.EventHandler(this.FilterTreeName_Leave);
            // 
            // FilterComment
            // 
            this.FilterComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterComment.Location = new System.Drawing.Point(519, 6);
            this.FilterComment.Name = "FilterComment";
            this.FilterComment.Size = new System.Drawing.Size(542, 20);
            this.FilterComment.TabIndex = 0;
            this.FilterComment.Leave += new System.EventHandler(this.FilterComment_Leave);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(456, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 24);
            this.label7.TabIndex = 16;
            this.label7.Text = "Comment:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PreparedFilterTrees
            // 
            this.PreparedFilterTrees.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PreparedFilterTrees.FormattingEnabled = true;
            this.PreparedFilterTrees.IntegralHeight = false;
            this.PreparedFilterTrees.Location = new System.Drawing.Point(30, 13);
            this.PreparedFilterTrees.Margin = new System.Windows.Forms.Padding(0);
            this.PreparedFilterTrees.Name = "PreparedFilterTrees";
            this.PreparedFilterTrees.Size = new System.Drawing.Size(78, 107);
            this.PreparedFilterTrees.TabIndex = 0;
            this.PreparedFilterTrees.DoubleClick += new System.EventHandler(this.PreparedFilters_DoubleClick);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Prepared Filter Trees:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.PreparedFilterTrees);
            this.panel4.Controls.Add(this.flowLayoutPanel2);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(108, 120);
            this.panel4.TabIndex = 18;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.AddPrepared);
            this.flowLayoutPanel2.Controls.Add(this.SavePrepared);
            this.flowLayoutPanel2.Controls.Add(this.DeletePrepared);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 13);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(30, 107);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // AddPrepared
            // 
            this.AddPrepared.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.AddPrepared.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddPrepared.ForeColor = System.Drawing.Color.Green;
            this.AddPrepared.Location = new System.Drawing.Point(3, 0);
            this.AddPrepared.Margin = new System.Windows.Forms.Padding(3, 0, 3, 2);
            this.AddPrepared.Name = "AddPrepared";
            this.AddPrepared.Size = new System.Drawing.Size(23, 23);
            this.AddPrepared.TabIndex = 0;
            this.AddPrepared.Text = "<";
            this.AddPrepared.UseVisualStyleBackColor = true;
            this.AddPrepared.Click += new System.EventHandler(this.AddPrepared_Click);
            // 
            // SavePrepared
            // 
            this.SavePrepared.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SavePrepared.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SavePrepared.ForeColor = System.Drawing.Color.Blue;
            this.SavePrepared.Location = new System.Drawing.Point(3, 25);
            this.SavePrepared.Margin = new System.Windows.Forms.Padding(3, 0, 3, 2);
            this.SavePrepared.Name = "SavePrepared";
            this.SavePrepared.Size = new System.Drawing.Size(23, 23);
            this.SavePrepared.TabIndex = 1;
            this.SavePrepared.Text = ">";
            this.SavePrepared.UseVisualStyleBackColor = true;
            this.SavePrepared.Click += new System.EventHandler(this.SaveAsPrepared_Click);
            // 
            // DeletePrepared
            // 
            this.DeletePrepared.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DeletePrepared.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeletePrepared.ForeColor = System.Drawing.Color.DimGray;
            this.DeletePrepared.Location = new System.Drawing.Point(3, 50);
            this.DeletePrepared.Margin = new System.Windows.Forms.Padding(3, 0, 3, 2);
            this.DeletePrepared.Name = "DeletePrepared";
            this.DeletePrepared.Size = new System.Drawing.Size(23, 23);
            this.DeletePrepared.TabIndex = 2;
            this.DeletePrepared.Text = "X";
            this.DeletePrepared.UseVisualStyleBackColor = true;
            this.DeletePrepared.Click += new System.EventHandler(this.DeletePrepared_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 30);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.FilterBox);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(1062, 221);
            this.splitContainer1.SplitterDistance = 120;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 19;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.FilterTree);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel4);
            this.splitContainer2.Size = new System.Drawing.Size(1062, 120);
            this.splitContainer2.SplitterDistance = 952;
            this.splitContainer2.SplitterWidth = 2;
            this.splitContainer2.TabIndex = 0;
            // 
            // FilterBox
            // 
            this.FilterBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilterBox.Location = new System.Drawing.Point(0, 13);
            this.FilterBox.Name = "FilterBox";
            this.FilterBox.Size = new System.Drawing.Size(1062, 86);
            this.FilterBox.TabIndex = 9;
            this.FilterBox.Leave += new System.EventHandler(this.FilterBox_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(563, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Filter to be applied to the initial text if it is a root filter or to the capture" +
    "s of the group of the parent filter if it is a child filter:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.FilterTreeFileDir);
            this.panel2.Controls.Add(this.FilterTreeName);
            this.panel2.Controls.Add(this.flowLayoutPanel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1062, 30);
            this.panel2.TabIndex = 1;
            // 
            // FilterTreeFileDir
            // 
            this.FilterTreeFileDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterTreeFileDir.Location = new System.Drawing.Point(712, 4);
            this.FilterTreeFileDir.Margin = new System.Windows.Forms.Padding(0);
            this.FilterTreeFileDir.Name = "FilterTreeFileDir";
            this.FilterTreeFileDir.ReadOnly = true;
            this.FilterTreeFileDir.Size = new System.Drawing.Size(284, 20);
            this.FilterTreeFileDir.TabIndex = 2;
            this.FilterTreeFileDir.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.FilterComment);
            this.panel1.Controls.Add(this.Filter_flowLayoutPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 251);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1062, 34);
            this.panel1.TabIndex = 0;
            // 
            // Filter_flowLayoutPanel
            // 
            this.Filter_flowLayoutPanel.Controls.Add(this.label6);
            this.Filter_flowLayoutPanel.Controls.Add(this.pLevelHighlight);
            this.Filter_flowLayoutPanel.Controls.Add(this.label5);
            this.Filter_flowLayoutPanel.Controls.Add(this.FilterType);
            this.Filter_flowLayoutPanel.Controls.Add(this.label2);
            this.Filter_flowLayoutPanel.Controls.Add(this.InputGroupName);
            this.Filter_flowLayoutPanel.Controls.Add(this.label7);
            this.Filter_flowLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.Filter_flowLayoutPanel.Name = "Filter_flowLayoutPanel";
            this.Filter_flowLayoutPanel.Size = new System.Drawing.Size(518, 27);
            this.Filter_flowLayoutPanel.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 24);
            this.label6.TabIndex = 35;
            this.label6.Text = "Mark Color:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pLevelHighlight
            // 
            this.pLevelHighlight.BackColor = System.Drawing.SystemColors.Control;
            this.pLevelHighlight.Location = new System.Drawing.Point(64, 3);
            this.pLevelHighlight.Name = "pLevelHighlight";
            this.pLevelHighlight.ReadOnly = true;
            this.pLevelHighlight.Size = new System.Drawing.Size(76, 20);
            this.pLevelHighlight.TabIndex = 36;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(143, 0);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 24);
            this.label5.TabIndex = 33;
            this.label5.Text = "Type:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FilterType
            // 
            this.FilterType.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.FilterType.Location = new System.Drawing.Point(180, 3);
            this.FilterType.Name = "FilterType";
            this.FilterType.ReadOnly = true;
            this.FilterType.Size = new System.Drawing.Size(77, 20);
            this.FilterType.TabIndex = 34;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(260, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 24);
            this.label2.TabIndex = 32;
            this.label2.Text = "Input Group:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InputGroupName
            // 
            this.InputGroupName.FormattingEnabled = true;
            this.InputGroupName.Location = new System.Drawing.Point(329, 3);
            this.InputGroupName.Name = "InputGroupName";
            this.InputGroupName.Size = new System.Drawing.Size(121, 21);
            this.InputGroupName.TabIndex = 22;
            this.InputGroupName.Leave += new System.EventHandler(this.InputGroupName_Leave);
            // 
            // FilterBoxMenu
            // 
            this.FilterBoxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoFilterBoxMenuItem,
            this.redoFilterBoxMenuItem,
            this.toolStripSeparator2,
            this.cutFilterBoxMenuItem,
            this.copyFilterBoxMenuItem,
            this.pasteFilterBoxMenuItem,
            this.deleteFilterBoxMenuItem});
            this.FilterBoxMenu.Name = "FilterBoxMenu";
            this.FilterBoxMenu.Size = new System.Drawing.Size(108, 142);
            this.FilterBoxMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FilterBoxMenu_Opening);
            // 
            // undoFilterBoxMenuItem
            // 
            this.undoFilterBoxMenuItem.Name = "undoFilterBoxMenuItem";
            this.undoFilterBoxMenuItem.Size = new System.Drawing.Size(107, 22);
            this.undoFilterBoxMenuItem.Text = "Undo";
            this.undoFilterBoxMenuItem.Click += new System.EventHandler(this.undoFilterBoxMenuItem_Click);
            // 
            // redoFilterBoxMenuItem
            // 
            this.redoFilterBoxMenuItem.Name = "redoFilterBoxMenuItem";
            this.redoFilterBoxMenuItem.Size = new System.Drawing.Size(107, 22);
            this.redoFilterBoxMenuItem.Text = "Redo";
            this.redoFilterBoxMenuItem.Click += new System.EventHandler(this.redoFilterBoxMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(104, 6);
            // 
            // cutFilterBoxMenuItem
            // 
            this.cutFilterBoxMenuItem.Name = "cutFilterBoxMenuItem";
            this.cutFilterBoxMenuItem.Size = new System.Drawing.Size(107, 22);
            this.cutFilterBoxMenuItem.Text = "Cut";
            this.cutFilterBoxMenuItem.Click += new System.EventHandler(this.cutFilterBoxMenuItem_Click);
            // 
            // copyFilterBoxMenuItem
            // 
            this.copyFilterBoxMenuItem.Name = "copyFilterBoxMenuItem";
            this.copyFilterBoxMenuItem.Size = new System.Drawing.Size(107, 22);
            this.copyFilterBoxMenuItem.Text = "Copy";
            this.copyFilterBoxMenuItem.Click += new System.EventHandler(this.copyFilterBoxMenuItem_Click);
            // 
            // pasteFilterBoxMenuItem
            // 
            this.pasteFilterBoxMenuItem.Name = "pasteFilterBoxMenuItem";
            this.pasteFilterBoxMenuItem.Size = new System.Drawing.Size(107, 22);
            this.pasteFilterBoxMenuItem.Text = "Paste";
            this.pasteFilterBoxMenuItem.Click += new System.EventHandler(this.pasteFilterBoxMenuItem_Click);
            // 
            // deleteFilterBoxMenuItem
            // 
            this.deleteFilterBoxMenuItem.Name = "deleteFilterBoxMenuItem";
            this.deleteFilterBoxMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteFilterBoxMenuItem.Text = "Delete";
            this.deleteFilterBoxMenuItem.Click += new System.EventHandler(this.deleteFilterBoxMenuItem_Click);
            // 
            // FilterTreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "FilterTreeForm";
            this.Size = new System.Drawing.Size(1062, 285);
            this.FilterTreeMenu.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.Filter_flowLayoutPanel.ResumeLayout(false);
            this.Filter_flowLayoutPanel.PerformLayout();
            this.FilterBoxMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView FilterTree;
        private System.Windows.Forms.Button Remove;
        private System.Windows.Forms.Button Insert;
        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.ListBox PreparedFilterTrees;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox FilterComment;
        private System.Windows.Forms.Button AddPrepared;
        private System.Windows.Forms.Button DeletePrepared;
        private System.Windows.Forms.Button SavePrepared;
        private System.Windows.Forms.Button Save;
        internal System.Windows.Forms.CheckBox buttonParse;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox FilterTreeName;
        private System.Windows.Forms.Button OpenFilterTree;
        
        System.Windows.Forms.ToolTip ToolTip = new System.Windows.Forms.ToolTip();

        void set_tool_tip()
        {
            ToolTip.AutoPopDelay = 100000;

            ToolTip.SetToolTip(this.OpenFilterTree, "Open filter tree file");
            ToolTip.SetToolTip(this.buttonOutputData, "Output data");
            ToolTip.SetToolTip(this.NewFilterTree, "Clear filter tree");
            //ToolTip.SetToolTip(this.TreeName, "Name of " + Program.FilterTreeFileExtension + " file");
            //ToolTip.SetToolTip(this.RegexName, "Name of " + Program.FilterTreeFileExtension + " file");
            ToolTip.SetToolTip(this.buttonParse, "Parse");
            ToolTip.SetToolTip(this.Save, "Save filter tree");
            ToolTip.SetToolTip(this.SavePrepared, "Save as prepared filter tree");
            ToolTip.SetToolTip(this.AddPrepared, "Add prepared filter tree");
            ToolTip.SetToolTip(this.DeletePrepared, "Delete prepared filter tree");
            ToolTip.SetToolTip(this.Add, "Add filter");
            ToolTip.SetToolTip(this.Insert, "Insert filter");
            ToolTip.SetToolTip(this.Remove, "Remove filter");
            ToolTip.SetToolTip(this.bUp, "Move filter up");
            ToolTip.SetToolTip(this.bDown, "Move filter down");
            ToolTip.SetToolTip(this.bRemoveBranch, "Remove filter branch");
            ToolTip.SetToolTip(this.pFilterTreeNameChangedHighlight, "Filter tree was modified");
           // ToolTip.SetToolTip(this.pFilterBoxHighlight, "Highlighting color of regex being edited");
            ToolTip.SetToolTip(this.bSaveAsPatternFilter, "Make selected filter the new filter pattern");
            //ToolTip.SetToolTip(this.InputGroupName, "Input group");
            //ToolTip.SetToolTip(this.FilterBox, "Regex");
            //ToolTip.SetToolTip(this.FilterTree, "Filter tree");
        }

        private System.Windows.Forms.Button NewFilterTree;
        private System.Windows.Forms.Button buttonOutputData;
        private System.Windows.Forms.ContextMenuStrip FilterTreeMenu;
        private System.Windows.Forms.ToolStripMenuItem upToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button bUp;
        private System.Windows.Forms.Button bDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bRemoveBranch;
        private System.Windows.Forms.ToolStripMenuItem deleteBranchToolStripMenuItem;
        private System.Windows.Forms.Panel pFilterTreeNameChangedHighlight;
        private System.Windows.Forms.Button bSaveAsPatternFilter;
        private System.Windows.Forms.ContextMenuStrip FilterBoxMenu;
        private System.Windows.Forms.ToolStripMenuItem copyFilterBoxMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteFilterBoxMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem undoFilterBoxMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoFilterBoxMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutFilterBoxMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteFilterBoxMenuItem;
        private System.Windows.Forms.FlowLayoutPanel Filter_flowLayoutPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox FilterTypes;
        private System.Windows.Forms.Panel FilterBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox FilterType;
        private System.Windows.Forms.ComboBox InputGroupName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox pLevelHighlight;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox FilterTreeFileDir;
    }
}