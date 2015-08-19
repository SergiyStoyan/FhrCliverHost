using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cliver.ProductIdentifier
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();

            browser.DocumentText = get_html();
        }

        string get_html()
        {
            return @"
<html>
<head>
<style>
.field_name {color:blue; font-weight:bold;}
table {border-collapse: collapse;}
td {border: 1px solid black; vertical-align: top; text-align: left;}
</style>
</head>
<body>
<div align='right'>Developed by <a href='http://cliversoft.com'>CliverSoft.com</a>, 2015</div>
<p>
<b>Usage:</b>
<p>
<p>
Select 2 sites whose products you want to compare and the comparison window will be showed. Buttons next1 and prev1 move among site1 products; next2 and prev2 - among site2 products.
<p>Site2 products are ordered by comparison score descending i.e. most close products are going in the beginning.
<p>If you click 'Approve' the current product pair is recorded into the db. You can 'Unlink' it anytime and make comparison again.
<p>
<p>
IMPORTANT: because the app needs to download all the products for each chosen site, it may consume 1-10 minutes depending on internet speed before the app starts showing site to site comparison. 

</body>
</html>
";
        }
    }
}
