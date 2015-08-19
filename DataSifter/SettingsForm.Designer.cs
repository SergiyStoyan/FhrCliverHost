namespace Cliver.DataSifter
{
    partial class SettingsForm
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
            this.Cancel = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.Reset = new System.Windows.Forms.Button();
            this.About = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.flagCopySelectionToClipboard = new System.Windows.Forms.CheckBox();
            this.flagStripParsedTextInStatusBarFromHtmlTags = new System.Windows.Forms.CheckBox();
            this.flagHighlightHtmlTags = new System.Windows.Forms.CheckBox();
            this.flagPrintParseLabels = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.bHtmlJavascriptColor = new System.Windows.Forms.Button();
            this.bHtmlCommentColor = new System.Windows.Forms.Button();
            this.bHtmlTagsColor = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.bEditColor = new System.Windows.Forms.Button();
            this.bRemoveColor = new System.Windows.Forms.Button();
            this.bAddColor = new System.Windows.Forms.Button();
            this.listBoxLabelColors = new System.Windows.Forms.ListBox();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(372, 272);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 1;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(372, 243);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 0;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Reset
            // 
            this.Reset.Location = new System.Drawing.Point(372, 196);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(75, 23);
            this.Reset.TabIndex = 5;
            this.Reset.Text = "Reset";
            this.Reset.UseVisualStyleBackColor = true;
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // About
            // 
            this.About.Location = new System.Drawing.Point(372, 31);
            this.About.Name = "About";
            this.About.Size = new System.Drawing.Size(75, 23);
            this.About.TabIndex = 4;
            this.About.Text = "About";
            this.About.UseVisualStyleBackColor = true;
            this.About.Click += new System.EventHandler(this.About_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(363, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Some settings being changed require restarting the application to get effect.";
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Location = new System.Drawing.Point(12, 31);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(354, 268);
            this.tabControl2.TabIndex = 6;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.flagCopySelectionToClipboard);
            this.tabPage3.Controls.Add(this.flagStripParsedTextInStatusBarFromHtmlTags);
            this.tabPage3.Controls.Add(this.flagHighlightHtmlTags);
            this.tabPage3.Controls.Add(this.flagPrintParseLabels);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(346, 242);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Preferences";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // flagCopySelectionToClipboard
            // 
            this.flagCopySelectionToClipboard.AutoSize = true;
            this.flagCopySelectionToClipboard.Location = new System.Drawing.Point(6, 92);
            this.flagCopySelectionToClipboard.Name = "flagCopySelectionToClipboard";
            this.flagCopySelectionToClipboard.Size = new System.Drawing.Size(253, 17);
            this.flagCopySelectionToClipboard.TabIndex = 4;
            this.flagCopySelectionToClipboard.Text = "Automatically copy selected text to the clipboard";
            this.flagCopySelectionToClipboard.UseVisualStyleBackColor = true;
            // 
            // flagStripParsedTextInStatusBarFromHtmlTags
            // 
            this.flagStripParsedTextInStatusBarFromHtmlTags.AutoSize = true;
            this.flagStripParsedTextInStatusBarFromHtmlTags.Location = new System.Drawing.Point(6, 68);
            this.flagStripParsedTextInStatusBarFromHtmlTags.Name = "flagStripParsedTextInStatusBarFromHtmlTags";
            this.flagStripParsedTextInStatusBarFromHtmlTags.Size = new System.Drawing.Size(305, 17);
            this.flagStripParsedTextInStatusBarFromHtmlTags.TabIndex = 3;
            this.flagStripParsedTextInStatusBarFromHtmlTags.Text = "Strip capture text showed in the status bar from html entities";
            this.flagStripParsedTextInStatusBarFromHtmlTags.UseVisualStyleBackColor = true;
            // 
            // flagHighlightHtmlTags
            // 
            this.flagHighlightHtmlTags.AutoSize = true;
            this.flagHighlightHtmlTags.Location = new System.Drawing.Point(6, 45);
            this.flagHighlightHtmlTags.Name = "flagHighlightHtmlTags";
            this.flagHighlightHtmlTags.Size = new System.Drawing.Size(99, 17);
            this.flagHighlightHtmlTags.TabIndex = 2;
            this.flagHighlightHtmlTags.Text = "Tint html syntax";
            this.flagHighlightHtmlTags.UseVisualStyleBackColor = true;
            // 
            // flagPrintParseLabels
            // 
            this.flagPrintParseLabels.AutoSize = true;
            this.flagPrintParseLabels.Location = new System.Drawing.Point(6, 22);
            this.flagPrintParseLabels.Name = "flagPrintParseLabels";
            this.flagPrintParseLabels.Size = new System.Drawing.Size(116, 17);
            this.flagPrintParseLabels.TabIndex = 1;
            this.flagPrintParseLabels.Text = "Print capture labels";
            this.flagPrintParseLabels.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.bHtmlJavascriptColor);
            this.tabPage4.Controls.Add(this.bHtmlCommentColor);
            this.tabPage4.Controls.Add(this.bHtmlTagsColor);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.bEditColor);
            this.tabPage4.Controls.Add(this.bRemoveColor);
            this.tabPage4.Controls.Add(this.bAddColor);
            this.tabPage4.Controls.Add(this.listBoxLabelColors);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(346, 242);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Highlight Colors";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // bHtmlJavascriptColor
            // 
            this.bHtmlJavascriptColor.BackColor = System.Drawing.SystemColors.Window;
            this.bHtmlJavascriptColor.Location = new System.Drawing.Point(199, 92);
            this.bHtmlJavascriptColor.Name = "bHtmlJavascriptColor";
            this.bHtmlJavascriptColor.Size = new System.Drawing.Size(118, 23);
            this.bHtmlJavascriptColor.TabIndex = 9;
            this.bHtmlJavascriptColor.Text = "Html Javascript Color";
            this.bHtmlJavascriptColor.UseVisualStyleBackColor = false;
            // 
            // bHtmlCommentColor
            // 
            this.bHtmlCommentColor.BackColor = System.Drawing.SystemColors.Window;
            this.bHtmlCommentColor.Location = new System.Drawing.Point(199, 63);
            this.bHtmlCommentColor.Name = "bHtmlCommentColor";
            this.bHtmlCommentColor.Size = new System.Drawing.Size(118, 23);
            this.bHtmlCommentColor.TabIndex = 6;
            this.bHtmlCommentColor.Text = "Html Comment Color";
            this.bHtmlCommentColor.UseVisualStyleBackColor = false;
            // 
            // bHtmlTagsColor
            // 
            this.bHtmlTagsColor.BackColor = System.Drawing.SystemColors.Window;
            this.bHtmlTagsColor.Location = new System.Drawing.Point(199, 34);
            this.bHtmlTagsColor.Name = "bHtmlTagsColor";
            this.bHtmlTagsColor.Size = new System.Drawing.Size(118, 23);
            this.bHtmlTagsColor.TabIndex = 5;
            this.bHtmlTagsColor.Text = "Html Tag Color";
            this.bHtmlTagsColor.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Back colors of parsed text:";
            // 
            // bEditColor
            // 
            this.bEditColor.Location = new System.Drawing.Point(90, 202);
            this.bEditColor.Name = "bEditColor";
            this.bEditColor.Size = new System.Drawing.Size(36, 23);
            this.bEditColor.TabIndex = 3;
            this.bEditColor.Text = "Edit";
            this.bEditColor.UseVisualStyleBackColor = true;
            // 
            // bRemoveColor
            // 
            this.bRemoveColor.Location = new System.Drawing.Point(48, 202);
            this.bRemoveColor.Name = "bRemoveColor";
            this.bRemoveColor.Size = new System.Drawing.Size(36, 23);
            this.bRemoveColor.TabIndex = 2;
            this.bRemoveColor.Text = "-";
            this.bRemoveColor.UseVisualStyleBackColor = true;
            // 
            // bAddColor
            // 
            this.bAddColor.Location = new System.Drawing.Point(6, 202);
            this.bAddColor.Name = "bAddColor";
            this.bAddColor.Size = new System.Drawing.Size(36, 23);
            this.bAddColor.TabIndex = 1;
            this.bAddColor.Text = "+";
            this.bAddColor.UseVisualStyleBackColor = true;
            // 
            // listBoxLabelColors
            // 
            this.listBoxLabelColors.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxLabelColors.FormattingEnabled = true;
            this.listBoxLabelColors.Location = new System.Drawing.Point(6, 34);
            this.listBoxLabelColors.Name = "listBoxLabelColors";
            this.listBoxLabelColors.Size = new System.Drawing.Size(120, 160);
            this.listBoxLabelColors.TabIndex = 0;
            this.listBoxLabelColors.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxLabelColors_DrawItem);
            this.listBoxLabelColors.DoubleClick += new System.EventHandler(this.listBoxLabelColors_DoubleClick);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 311);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.About);
            this.Controls.Add(this.Reset);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "DataSifter Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Reset;
        private System.Windows.Forms.Button About;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox flagCopySelectionToClipboard;
        private System.Windows.Forms.CheckBox flagStripParsedTextInStatusBarFromHtmlTags;
        private System.Windows.Forms.CheckBox flagHighlightHtmlTags;
        private System.Windows.Forms.CheckBox flagPrintParseLabels;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button bHtmlJavascriptColor;
        private System.Windows.Forms.Button bHtmlCommentColor;
        private System.Windows.Forms.Button bHtmlTagsColor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bEditColor;
        private System.Windows.Forms.Button bRemoveColor;
        private System.Windows.Forms.Button bAddColor;
        private System.Windows.Forms.ListBox listBoxLabelColors;
    }
}