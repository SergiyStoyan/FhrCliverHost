namespace Cliver.DataSifter
{
    partial class SourceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourceForm));
            this.TextBox = new System.Windows.Forms.RichTextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.FindPrev = new System.Windows.Forms.Button();
            this.FindNext = new System.Windows.Forms.Button();
            this.cHideFilters = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.NavigateBy = new System.Windows.Forms.ComboBox();
            this.PrevMark = new System.Windows.Forms.Button();
            this.NextMark = new System.Windows.Forms.Button();
            this.LabelStatus = new System.Windows.Forms.TextBox();
            this.bSettings = new System.Windows.Forms.Button();
            this.About = new System.Windows.Forms.Button();
            this.Help = new System.Windows.Forms.Button();
            this.File = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.FindBox = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // TextBox
            // 
            this.TextBox.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TextBox.HideSelection = false;
            this.TextBox.Location = new System.Drawing.Point(0, 0);
            this.TextBox.Name = "TextBox";
            this.TextBox.ReadOnly = true;
            this.TextBox.Size = new System.Drawing.Size(1028, 295);
            this.TextBox.TabIndex = 14;
            this.TextBox.TabStop = false;
            this.TextBox.Text = "";
            this.TextBox.SelectionChanged += new System.EventHandler(this.TextBox_SelectionChanged);
            this.TextBox.DoubleClick += new System.EventHandler(this.TextBox_DoubleClick);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.FindPrev);
            this.flowLayoutPanel1.Controls.Add(this.FindNext);
            this.flowLayoutPanel1.Controls.Add(this.cHideFilters);
            this.flowLayoutPanel1.Controls.Add(this.label7);
            this.flowLayoutPanel1.Controls.Add(this.NavigateBy);
            this.flowLayoutPanel1.Controls.Add(this.PrevMark);
            this.flowLayoutPanel1.Controls.Add(this.NextMark);
            this.flowLayoutPanel1.Controls.Add(this.LabelStatus);
            this.flowLayoutPanel1.Controls.Add(this.bSettings);
            this.flowLayoutPanel1.Controls.Add(this.About);
            this.flowLayoutPanel1.Controls.Add(this.Help);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(277, 3);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(756, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // FindPrev
            // 
            this.FindPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FindPrev.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.FindPrev.Image = ((System.Drawing.Image)(resources.GetObject("FindPrev.Image")));
            this.FindPrev.Location = new System.Drawing.Point(0, 3);
            this.FindPrev.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.FindPrev.Name = "FindPrev";
            this.FindPrev.Size = new System.Drawing.Size(35, 23);
            this.FindPrev.TabIndex = 0;
            this.FindPrev.UseVisualStyleBackColor = true;
            this.FindPrev.Click += new System.EventHandler(this.FindPrev_Click);
            // 
            // FindNext
            // 
            this.FindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FindNext.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.FindNext.Image = ((System.Drawing.Image)(resources.GetObject("FindNext.Image")));
            this.FindNext.Location = new System.Drawing.Point(36, 3);
            this.FindNext.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.FindNext.Name = "FindNext";
            this.FindNext.Size = new System.Drawing.Size(35, 23);
            this.FindNext.TabIndex = 1;
            this.FindNext.UseVisualStyleBackColor = true;
            this.FindNext.Click += new System.EventHandler(this.FindNext_Click);
            // 
            // cHideFilters
            // 
            this.cHideFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cHideFilters.Appearance = System.Windows.Forms.Appearance.Button;
            this.cHideFilters.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cHideFilters.Image = ((System.Drawing.Image)(resources.GetObject("cHideFilters.Image")));
            this.cHideFilters.Location = new System.Drawing.Point(72, 3);
            this.cHideFilters.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.cHideFilters.Name = "cHideFilters";
            this.cHideFilters.Size = new System.Drawing.Size(35, 23);
            this.cHideFilters.TabIndex = 18;
            this.cHideFilters.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cHideFilters.UseVisualStyleBackColor = true;
            this.cHideFilters.CheckedChanged += new System.EventHandler(this.cHideFilters_CheckedChanged);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.Location = new System.Drawing.Point(108, 3);
            this.label7.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 23);
            this.label7.TabIndex = 8;
            this.label7.Text = "Navigate by:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NavigateBy
            // 
            this.NavigateBy.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.NavigateBy.DropDownWidth = 120;
            this.NavigateBy.FormattingEnabled = true;
            this.NavigateBy.Location = new System.Drawing.Point(176, 3);
            this.NavigateBy.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.NavigateBy.MaxDropDownItems = 12;
            this.NavigateBy.Name = "NavigateBy";
            this.NavigateBy.Size = new System.Drawing.Size(224, 21);
            this.NavigateBy.TabIndex = 9;
            this.NavigateBy.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.NavigateBy_DrawItem);
            this.NavigateBy.DropDown += new System.EventHandler(this.NavigateBy_DropDown);
            this.NavigateBy.SelectedIndexChanged += new System.EventHandler(this.NavigateBy_SelectedIndexChanged);
            // 
            // PrevMark
            // 
            this.PrevMark.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PrevMark.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.PrevMark.Image = ((System.Drawing.Image)(resources.GetObject("PrevMark.Image")));
            this.PrevMark.Location = new System.Drawing.Point(401, 3);
            this.PrevMark.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.PrevMark.Name = "PrevMark";
            this.PrevMark.Size = new System.Drawing.Size(35, 23);
            this.PrevMark.TabIndex = 10;
            this.PrevMark.UseVisualStyleBackColor = true;
            this.PrevMark.Click += new System.EventHandler(this.PrevUserMark_Click);
            // 
            // NextMark
            // 
            this.NextMark.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NextMark.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.NextMark.Image = ((System.Drawing.Image)(resources.GetObject("NextMark.Image")));
            this.NextMark.Location = new System.Drawing.Point(437, 3);
            this.NextMark.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.NextMark.Name = "NextMark";
            this.NextMark.Size = new System.Drawing.Size(35, 23);
            this.NextMark.TabIndex = 11;
            this.NextMark.UseVisualStyleBackColor = true;
            this.NextMark.Click += new System.EventHandler(this.NextUserMark_Click);
            // 
            // LabelStatus
            // 
            this.LabelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelStatus.Location = new System.Drawing.Point(473, 3);
            this.LabelStatus.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.LabelStatus.Name = "LabelStatus";
            this.LabelStatus.ReadOnly = true;
            this.LabelStatus.Size = new System.Drawing.Size(133, 22);
            this.LabelStatus.TabIndex = 19;
            // 
            // bSettings
            // 
            this.bSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bSettings.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSettings.Image = ((System.Drawing.Image)(resources.GetObject("bSettings.Image")));
            this.bSettings.Location = new System.Drawing.Point(607, 3);
            this.bSettings.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.bSettings.Name = "bSettings";
            this.bSettings.Size = new System.Drawing.Size(35, 23);
            this.bSettings.TabIndex = 15;
            this.bSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSettings.UseVisualStyleBackColor = true;
            this.bSettings.Click += new System.EventHandler(this.bSettings_Click);
            // 
            // About
            // 
            this.About.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.About.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.About.Image = ((System.Drawing.Image)(resources.GetObject("About.Image")));
            this.About.Location = new System.Drawing.Point(643, 3);
            this.About.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.About.Name = "About";
            this.About.Size = new System.Drawing.Size(35, 23);
            this.About.TabIndex = 13;
            this.About.UseVisualStyleBackColor = true;
            this.About.Click += new System.EventHandler(this.About_Click);
            // 
            // Help
            // 
            this.Help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Help.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Help.Image = ((System.Drawing.Image)(resources.GetObject("Help.Image")));
            this.Help.Location = new System.Drawing.Point(679, 3);
            this.Help.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.Help.Name = "Help";
            this.Help.Size = new System.Drawing.Size(35, 23);
            this.Help.TabIndex = 12;
            this.Help.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Help.UseVisualStyleBackColor = true;
            this.Help.Click += new System.EventHandler(this.Help_Click);
            // 
            // File
            // 
            this.File.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.File.Image = ((System.Drawing.Image)(resources.GetObject("File.Image")));
            this.File.Location = new System.Drawing.Point(0, 3);
            this.File.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.File.Name = "File";
            this.File.Size = new System.Drawing.Size(35, 23);
            this.File.TabIndex = 0;
            this.File.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.File.UseVisualStyleBackColor = true;
            this.File.Click += new System.EventHandler(this.File_Click);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(36, 3);
            this.label8.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 23);
            this.label8.TabIndex = 1;
            this.label8.Text = "Find:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FindBox
            // 
            this.FindBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FindBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FindBox.Location = new System.Drawing.Point(71, 4);
            this.FindBox.Margin = new System.Windows.Forms.Padding(0, 3, 1, 3);
            this.FindBox.Name = "FindBox";
            this.FindBox.Size = new System.Drawing.Size(205, 22);
            this.FindBox.TabIndex = 2;
            this.FindBox.TextChanged += new System.EventHandler(this.FindBox_TextChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Status});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 562);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1028, 20);
            this.statusStrip1.TabIndex = 17;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Status
            // 
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(10, 15);
            this.Status.Text = " ";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.OrangeRed;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.TextBox);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Size = new System.Drawing.Size(1028, 562);
            this.splitContainer1.SplitterDistance = 326;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 18;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flowLayoutPanel2);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.FindBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 295);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1028, 31);
            this.panel1.TabIndex = 1;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.File);
            this.flowLayoutPanel2.Controls.Add(this.label8);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(68, 28);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // SourceForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 582);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.MinimumSize = new System.Drawing.Size(500, 200);
            this.Name = "SourceForm";
            this.Text = "Filter Treeer";
            this.Activated += new System.EventHandler(this.SourceForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SourceForm_FormClosing);
            this.Load += new System.EventHandler(this.SourceForm_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox TextBox;

        System.Windows.Forms.ToolTip ToolTip = new System.Windows.Forms.ToolTip();

        void set_tool_tip()
        {
            ToolTip.AutoPopDelay = 100000;

            ToolTip.SetToolTip(this.File, "Open file");
            //ToolTip.SetToolTip(this.ShowTree, "Toggle filter tree");
            ToolTip.SetToolTip(this.NextMark, "Next mark");
            ToolTip.SetToolTip(this.PrevMark, "Previous mark");
            ToolTip.SetToolTip(this.NavigateBy, "Choose navigation item");
            //ToolTip.SetToolTip(this.UserMarkView, "Toggle mark view");
            //ToolTip.SetToolTip(this.Unmark, "Clear mark");
            //ToolTip.SetToolTip(this.AddUserMark, "Mark selection");
            //ToolTip.SetToolTip(this.bTreeForm, "Switch to Browser window");
            ToolTip.SetToolTip(this.FindNext, "Find next");
            //ToolTip.SetToolTip(this.FindBox, "Regex");
            ToolTip.SetToolTip(this.FindPrev, "Find previous");
            ToolTip.SetToolTip(this.Help, "Help");
            ToolTip.SetToolTip(this.About, "About");
            ToolTip.SetToolTip(this.bSettings, "Settings");
            //ToolTip.SetToolTip(this.bFindAndAddXpathRegexTree, "Generate XPath regex chain for selection");
            //ToolTip.SetToolTip(this.ShowZeroGroup, "Highlight zero group captures");
        }

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button File;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox FindBox;
        private System.Windows.Forms.Button FindPrev;
        private System.Windows.Forms.Button FindNext;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox NavigateBy;
        private System.Windows.Forms.Button PrevMark;
        private System.Windows.Forms.Button NextMark;
        private System.Windows.Forms.Button Help;
        private System.Windows.Forms.Button About;
        private System.Windows.Forms.Button bSettings;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.CheckBox cHideFilters;
        private System.Windows.Forms.TextBox LabelStatus;
    }
}