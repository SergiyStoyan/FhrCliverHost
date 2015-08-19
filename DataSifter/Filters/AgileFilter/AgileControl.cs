using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cliver.DataSifter
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AgileControl : FilterControl
    {
        /// <summary>
        /// needed for VS designer
        /// </summary>
        public AgileControl()
        {
            InitializeComponent();
        }

        internal AgileControl(AgileFilter filter)
            : base(filter)
        {
            InitializeComponent();

            XpathBox.Text = filter.Xpath;
            GroupName.Text = filter.GroupName;
        }

        override internal string GetUpdatedFilterDefinition()
        {
            //validate
            HtmlAgilityPack.HtmlDocument d = new HtmlAgilityPack.HtmlDocument();
            d.LoadHtml("<html></html>");
            HtmlAgilityPack.HtmlNodeCollection nc = d.DocumentNode.SelectNodes(XpathBox.Text);
            
            return XpathBox.Text + "\n" + GroupName.Text;
        }
    }
}