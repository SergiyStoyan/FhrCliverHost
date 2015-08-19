using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace Cliver.DataSifter
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
        }

        public static void Sleep(int mss)
        {
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, mss);
            while (dt > DateTime.Now)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
        }

        public static bool WaitForCondition(Func<bool> check_condition, int timeout_in_mss)
        {
            DateTime dt = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout_in_mss);
            do
            {
                if (check_condition())
                    return true;
                Thread.Sleep(10);
                Application.DoEvents();
            }
            while (dt > DateTime.Now);
            return false;
        }

        protected void Invoke(MethodInvoker code)
        {
            InvokeInControlThread(this, code);
        }

        protected void BeginInvoke(MethodInvoker code)
        {
            BeginInvokeInControlThread(this, code);
        }

        public static void InvokeInControlThread(Control c, MethodInvoker code)
        {
            if (c.InvokeRequired)
            {
                c.Invoke(code);
                return;
            }
            code.Invoke();
        }

        public static void BeginInvokeInControlThread(Control c, MethodInvoker code)
        {
            //c.BeginInvoke(code);
            if (c.InvokeRequired)
                c.BeginInvoke(code);
            else
                c.Invoke(code);
        }
    }
}
