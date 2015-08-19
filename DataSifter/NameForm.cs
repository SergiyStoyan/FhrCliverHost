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

namespace Cliver.DataSifter
{
    /// <summary>
    /// asks for name for the newly saved prepared filter tree
    /// </summary>
    internal partial class NameForm : BaseForm
    {
        internal NameForm(string default_file_name)
        {
            InitializeComponent();

            this.Text = Program.Title;
            boxName.Text = default_file_name;
        }

        internal string FileName = "";

        private void OK_Click(object sender, EventArgs e)
        {            
            FileName = boxName.Text;
            if (string.IsNullOrEmpty(FileName))
                return;
            this.Close();
        }
    }
}