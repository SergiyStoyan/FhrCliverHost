namespace Cliver.DataSifter
{
    partial class OutputForm
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
            this.Copy = new System.Windows.Forms.Button();
            this.bClose = new System.Windows.Forms.Button();
            this.radioGroupNames = new System.Windows.Forms.RadioButton();
            this.Save = new System.Windows.Forms.Button();
            this.radioCaptures = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.GroupNames = new System.Windows.Forms.ListBox();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.CaptureSeparator = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.bShow = new System.Windows.Forms.Button();
            this.bSaveAsDefault = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.statusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Copy
            // 
            this.Copy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Copy.Location = new System.Drawing.Point(98, 313);
            this.Copy.Name = "Copy";
            this.Copy.Size = new System.Drawing.Size(75, 23);
            this.Copy.TabIndex = 1;
            this.Copy.Text = "Copy";
            this.Copy.UseVisualStyleBackColor = true;
            this.Copy.Click += new System.EventHandler(this.Copy_Click);
            // 
            // bClose
            // 
            this.bClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bClose.Location = new System.Drawing.Point(98, 409);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(75, 23);
            this.bClose.TabIndex = 3;
            this.bClose.Text = "Close";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.Close_Click);
            // 
            // radioGroupNames
            // 
            this.radioGroupNames.AutoSize = true;
            this.radioGroupNames.Checked = true;
            this.radioGroupNames.Location = new System.Drawing.Point(6, 19);
            this.radioGroupNames.Name = "radioGroupNames";
            this.radioGroupNames.Size = new System.Drawing.Size(88, 17);
            this.radioGroupNames.TabIndex = 0;
            this.radioGroupNames.TabStop = true;
            this.radioGroupNames.Text = "Group names";
            this.radioGroupNames.UseVisualStyleBackColor = true;
            this.radioGroupNames.CheckedChanged += new System.EventHandler(this.radioGroupNames_CheckedChanged);
            // 
            // Save
            // 
            this.Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Save.Location = new System.Drawing.Point(98, 341);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 2;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // radioCaptures
            // 
            this.radioCaptures.AutoSize = true;
            this.radioCaptures.Location = new System.Drawing.Point(6, 42);
            this.radioCaptures.Name = "radioCaptures";
            this.radioCaptures.Size = new System.Drawing.Size(67, 17);
            this.radioCaptures.TabIndex = 1;
            this.radioCaptures.Text = "Captures";
            this.radioCaptures.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioGroupNames);
            this.groupBox1.Controls.Add(this.radioCaptures);
            this.groupBox1.Location = new System.Drawing.Point(-1, 178);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(174, 73);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Copy";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Output group names:";
            // 
            // GroupNames
            // 
            this.GroupNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupNames.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.GroupNames.FormattingEnabled = true;
            this.GroupNames.IntegralHeight = false;
            this.GroupNames.Location = new System.Drawing.Point(0, 13);
            this.GroupNames.Margin = new System.Windows.Forms.Padding(0);
            this.GroupNames.Name = "GroupNames";
            this.GroupNames.Size = new System.Drawing.Size(305, 422);
            this.GroupNames.TabIndex = 0;
            this.GroupNames.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.GroupNames_DrawItem);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusBar.Location = new System.Drawing.Point(0, 435);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(482, 20);
            this.statusBar.TabIndex = 8;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(118, 15);
            this.statusLabel.Text = "toolStripStatusLabel1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Capture separator:";
            // 
            // CaptureSeparator
            // 
            this.CaptureSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CaptureSeparator.Location = new System.Drawing.Point(-1, 13);
            this.CaptureSeparator.Name = "CaptureSeparator";
            this.CaptureSeparator.Size = new System.Drawing.Size(174, 159);
            this.CaptureSeparator.TabIndex = 11;
            this.CaptureSeparator.Text = "";
            this.CaptureSeparator.WordWrap = false;
            this.CaptureSeparator.Enter += new System.EventHandler(this.CaptureSeparator_Enter);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.GroupNames);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.bShow);
            this.splitContainer1.Panel2.Controls.Add(this.bSaveAsDefault);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.CaptureSeparator);
            this.splitContainer1.Panel2.Controls.Add(this.Save);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.bClose);
            this.splitContainer1.Panel2.Controls.Add(this.Copy);
            this.splitContainer1.Size = new System.Drawing.Size(482, 435);
            this.splitContainer1.SplitterDistance = 305;
            this.splitContainer1.TabIndex = 12;
            // 
            // bShow
            // 
            this.bShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bShow.Location = new System.Drawing.Point(98, 370);
            this.bShow.Name = "bShow";
            this.bShow.Size = new System.Drawing.Size(75, 23);
            this.bShow.TabIndex = 13;
            this.bShow.Text = "Show";
            this.bShow.UseVisualStyleBackColor = true;
            this.bShow.Click += new System.EventHandler(this.bShow_Click);
            // 
            // bSaveAsDefault
            // 
            this.bSaveAsDefault.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bSaveAsDefault.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSaveAsDefault.Location = new System.Drawing.Point(0, 259);
            this.bSaveAsDefault.Margin = new System.Windows.Forms.Padding(0);
            this.bSaveAsDefault.Name = "bSaveAsDefault";
            this.bSaveAsDefault.Size = new System.Drawing.Size(173, 21);
            this.bSaveAsDefault.TabIndex = 12;
            this.bSaveAsDefault.Text = "Save As Default";
            this.bSaveAsDefault.UseVisualStyleBackColor = true;
            this.bSaveAsDefault.Click += new System.EventHandler(this.bSaveAsDefault_Click);
            // 
            // OutputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 455);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusBar);
            this.Name = "OutputForm";
            this.Text = "Output Data";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        void set_tool_tip()
        {
            ToolTip.AutoPopDelay = 100000;

            ToolTip.SetToolTip(this.radioCaptures, "Copy captures of the selected named groups");
            ToolTip.SetToolTip(this.radioGroupNames, "Copy the group names");
            ToolTip.SetToolTip(this.Copy, "Copy the data to the clipboard");
            ToolTip.SetToolTip(this.Save, "Save the data to a file");
            ToolTip.SetToolTip(this.CaptureSeparator, "Text that will separate the output captures");
            //ToolTip.SetToolTip(this.bClose, "Capture separator will be saved as default, else click button X in the header");
        }
        System.Windows.Forms.ToolTip ToolTip = new System.Windows.Forms.ToolTip();

        private System.Windows.Forms.Button Copy;
        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.RadioButton radioGroupNames;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.RadioButton radioCaptures;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox GroupNames;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox CaptureSeparator;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button bSaveAsDefault;
        private System.Windows.Forms.Button bShow;
    }
}