namespace Cliver.DataSifter
{
    partial class AgileControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.XpathBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.GroupName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // XpathBox
            // 
            this.XpathBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.XpathBox.DetectUrls = false;
            this.XpathBox.EnableAutoDragDrop = true;
            this.XpathBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.XpathBox.Location = new System.Drawing.Point(0, 0);
            this.XpathBox.Name = "XpathBox";
            this.XpathBox.Size = new System.Drawing.Size(480, 37);
            this.XpathBox.TabIndex = 38;
            this.XpathBox.Text = "";
            this.XpathBox.WordWrap = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(486, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 39;
            this.label1.Text = "Output Name:";
            // 
            // GroupName
            // 
            this.GroupName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupName.Location = new System.Drawing.Point(489, 16);
            this.GroupName.Name = "GroupName";
            this.GroupName.Size = new System.Drawing.Size(115, 20);
            this.GroupName.TabIndex = 40;
            // 
            // AgileControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GroupName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.XpathBox);
            this.Name = "AgileControl";
            this.Size = new System.Drawing.Size(607, 37);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox XpathBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox GroupName;
    }
}
