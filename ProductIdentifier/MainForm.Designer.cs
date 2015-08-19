namespace Cliver.ProductIdentifier
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.Compare = new System.Windows.Forms.Button();
            this.Company2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.help = new System.Windows.Forms.Button();
            this.console = new System.Windows.Forms.RichTextBox();
            this.Product1Ids = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Product1 Ids:";
            // 
            // Compare
            // 
            this.Compare.Location = new System.Drawing.Point(410, 46);
            this.Compare.Name = "Compare";
            this.Compare.Size = new System.Drawing.Size(75, 23);
            this.Compare.TabIndex = 2;
            this.Compare.Text = "Compare";
            this.Compare.UseVisualStyleBackColor = true;
            this.Compare.Click += new System.EventHandler(this.Compare_Click);
            // 
            // Company2
            // 
            this.Company2.FormattingEnabled = true;
            this.Company2.Location = new System.Drawing.Point(267, 6);
            this.Company2.Name = "Company2";
            this.Company2.Size = new System.Drawing.Size(106, 21);
            this.Company2.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(201, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Company2:";
            // 
            // help
            // 
            this.help.Location = new System.Drawing.Point(410, 6);
            this.help.Name = "help";
            this.help.Size = new System.Drawing.Size(75, 23);
            this.help.TabIndex = 6;
            this.help.Text = "Help";
            this.help.UseVisualStyleBackColor = true;
            this.help.Click += new System.EventHandler(this.help_Click);
            // 
            // console
            // 
            this.console.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.console.Location = new System.Drawing.Point(0, 85);
            this.console.Name = "console";
            this.console.ReadOnly = true;
            this.console.Size = new System.Drawing.Size(499, 95);
            this.console.TabIndex = 7;
            this.console.Text = "";
            // 
            // Product1Ids
            // 
            this.Product1Ids.Location = new System.Drawing.Point(88, 9);
            this.Product1Ids.Multiline = true;
            this.Product1Ids.Name = "Product1Ids";
            this.Product1Ids.Size = new System.Drawing.Size(100, 60);
            this.Product1Ids.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 180);
            this.Controls.Add(this.Product1Ids);
            this.Controls.Add(this.console);
            this.Controls.Add(this.help);
            this.Controls.Add(this.Company2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Compare);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "Product Linker";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Compare;
        private System.Windows.Forms.ComboBox Company2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button help;
        private System.Windows.Forms.RichTextBox console;
        private System.Windows.Forms.TextBox Product1Ids;
    }
}