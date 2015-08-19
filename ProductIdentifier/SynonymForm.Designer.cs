namespace Cliver.ProductIdentifier
{
    partial class SynonymForm
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
            this.synonyms = new System.Windows.Forms.ListBox();
            this.synonym_group = new System.Windows.Forms.RichTextBox();
            this.save = new System.Windows.Forms.Button();
            this.remove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // synonyms
            // 
            this.synonyms.Dock = System.Windows.Forms.DockStyle.Left;
            this.synonyms.FormattingEnabled = true;
            this.synonyms.HorizontalScrollbar = true;
            this.synonyms.Location = new System.Drawing.Point(0, 0);
            this.synonyms.Name = "synonyms";
            this.synonyms.Size = new System.Drawing.Size(318, 365);
            this.synonyms.TabIndex = 2;
            this.synonyms.SelectedIndexChanged += new System.EventHandler(this.synonyms_SelectedIndexChanged);
            // 
            // synonym_group
            // 
            this.synonym_group.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.synonym_group.Location = new System.Drawing.Point(324, 0);
            this.synonym_group.Name = "synonym_group";
            this.synonym_group.Size = new System.Drawing.Size(156, 173);
            this.synonym_group.TabIndex = 3;
            this.synonym_group.Text = "";
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.save.Location = new System.Drawing.Point(405, 179);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 4;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // remove
            // 
            this.remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.remove.Location = new System.Drawing.Point(324, 179);
            this.remove.Name = "remove";
            this.remove.Size = new System.Drawing.Size(75, 23);
            this.remove.TabIndex = 5;
            this.remove.Text = "Remove";
            this.remove.UseVisualStyleBackColor = true;
            this.remove.Click += new System.EventHandler(this.remove_Click);
            // 
            // SynonymForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 365);
            this.Controls.Add(this.remove);
            this.Controls.Add(this.save);
            this.Controls.Add(this.synonym_group);
            this.Controls.Add(this.synonyms);
            this.Name = "SynonymForm";
            this.Text = "SynonymForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox synonyms;
        private System.Windows.Forms.RichTextBox synonym_group;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button remove;

    }
}