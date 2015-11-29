//********************************************************************************************
//Author: Sergey Stoyan
//        stoyan@cliversoft.com        
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using Cliver.Bot;
using Cliver.CrawlerHost;

namespace Cliver.FhrManager
{
    public static class Program
    {
        [STAThreadAttribute]
        static void Main()
        {
            try
            {
                Cliver.CrawlerHost.Program.SetTitle(Assembly.GetExecutingAssembly());
                Cliver.CrawlerHost.Program.Main();
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }
    }
}
