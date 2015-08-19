//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        03 January 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace Cliver.DataSifter
{
    /// <summary>
    /// Show MessageForm with predefined features
    /// </summary>
    public static class Message
    { 
        /// <summary>
        /// Whether the message box must be displayed in the Windows taskbar.
        /// </summary>
        public static bool ShowInTaskbar = true;
      
        public static void Inform(string message)
        {
            show(Application.ProductName, SystemIcons.Information, message, new string[1] { "OK" });
        }

        public static void Exclaim(string message)
        {
            show(Application.ProductName, SystemIcons.Exclamation, message, new string[1] { "OK" });
        }        

        public static void Error(Exception e)
        {
            string cs = e.StackTrace;
            List<string> ms = new List<string>();
            for (; e != null; e = e.InnerException)
                ms.Add(e.Message);
            ms.Add("\r\n" + cs);

            show(Application.ProductName, SystemIcons.Error, string.Join("\r\n", ms), new string[1] { "OK" });
        }

        public static void Error(string message)
        {
            show(Application.ProductName, SystemIcons.Error, message, new string[1] { "OK" });
        }

        public static bool YesNo(string question)
        {
            return show(Application.ProductName, SystemIcons.Question, question, new string[2] { "Yes", "No" }) == 0;
        }
        
        static int show(string caption, Icon icon, string message, string[] buttons)
        {
                MessageForm mf = new MessageForm(caption, message, buttons, icon);
                
                mf.ShowInTaskbar = ShowInTaskbar;
                int i = mf.Display();
                return i;
        }
    }
}

