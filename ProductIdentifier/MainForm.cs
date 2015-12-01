using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

namespace Cliver.ProductIdentifier
{
    public partial class MainForm : BaseForm
    {
        public static MainForm This
        {
            get
            {
                if (mf == null)
                    mf = new MainForm();
                return mf;
            }
        }
        static MainForm mf;

        MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //AddMessage("Loading company list...");
            //foreach (Fhr.ProductOffice.Company c in Fhr.Db.Companies)
            //    Company2.Items.Add(new Item(c.Name, c.Id));
            //ReplaceMessage("Site list loaded.");
        }

        Thread t;

        class Item
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public Item(string name, int value)
            {
                Name = name;
                Value = value;
            }
        }

        Engine engine = new Engine();

        private void Compare_Click(object sender, EventArgs e)
        {
            int[] prodict1_ids = (from x in Product1Ids.Text.Split('\n') where !string.IsNullOrWhiteSpace(x) select int.Parse(x)).ToArray();
            
            if (Company2.SelectedItem == null)
            {
                MessageBox.Show("Company2 is not chosen.");
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            if (t != null && t.IsAlive)
                t.Abort();
            t = new Thread(() =>
            {
                try
                {
                    List<ProductLink>pls = engine.CreateProductLinkList(prodict1_ids,((Item)Company2.SelectedItem).Value);
                }
                catch (Exception ex)
                {
                    Message.Error(ex);
                }
                ControlRoutines.BeginInvoke(This, () =>
                {
                    this.Cursor = Cursors.Default;
                });
            });
            t.Start();
        }

        private void help_Click(object sender, EventArgs e)
        {
            if (hf != null)
                hf.Close();
            hf = new HelpForm();
            hf.Show();
        }
        HelpForm hf;

        public static void AddMessage(string m)
        {
            ControlRoutines.BeginInvoke(This, () => { This.console.Text += m + "\r\n"; });
        }

        public static void ReplaceMessage(string m, int line_number2remove = 1)
        {
            ControlRoutines.BeginInvoke(This, () =>
            {
                This.console.Text = Regex.Replace(This.console.Text, @"^((?:.*\n)*?)(?:.*\n){0," + line_number2remove + "}$", @"$1", RegexOptions.RightToLeft);
                This.console.Text += m + "\r\n";
            });
        }
    }
}
