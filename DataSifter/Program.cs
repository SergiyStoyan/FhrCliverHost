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
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Cliver.DataSifter
{
    static class Program
    {
        //TEST
        static int[] A = new int[] { -1, 3, -4, 5, 1, -6, 2, 1 };

        static  int g = solution(A);

        public static int solution(int[] A)
        {
            if (A.Length < 1)
                return -1;
            int s = A.Sum();            
            int s1 = 0;
            int s2 = s - A[0];
            for (int i = 0; i < A.Length - 1; i++)
            {
                if (s1 == s2)
                    return i;
                s1 += A[i];
                s2 -= A[i + 1];
            }
            if (s1 == 0)
                return A.Length - 1;
            return -1;
        }
        //END OF TEST

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                if (Settings.Default.ApplicationVersion != Application.ProductVersion)
                {
                    //string HelpFileUri = Settings.Default.HelpFileUri;
                    Settings.Default.Upgrade();
                    //Settings.Default.HelpFileUri = HelpFileUri;
                    Settings.Default.ApplicationVersion = Application.ProductVersion;
                    Settings.Default.Save();
                }

                Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                
                Application.Run(SourceForm.This);

                Settings.Default.Save();
            }
            catch (Exception e)
            {
                Message.Error(e);
            }
        }

        //static readonly internal System.Windows.Forms.ToolTip ToolTip = new ToolTip();
        
        static readonly internal string AppTitle = Application.ProductName;

        static readonly internal string Title = AppTitle;

        static Program()
        {
            try
            {
                AssemblyName ean = Assembly.GetEntryAssembly().GetName();
                AppTitle = ean.Name;
                if (ean.Version.Major > 0 || ean.Version.Minor > 0)
                    AppTitle += ean.Version.Major + "." + ean.Version.Minor;
            }
            catch(Exception e)
            {
                Message.Error(e);
            }
        }

        static internal void Help()
        {
            try
            {
                if (File.Exists(AppDir + Settings.Default.HelpFile))
                    Process.Start(Settings.Default.HelpFile);
                else
                    Process.Start(Settings.Default.HelpFileUri);
            }
            catch (Exception ex)
            {
                Message.Error(ex);
            }
        }

        static internal string AppDir = Application.StartupPath + "\\";

        internal const string FilterTreeFileExtension = "fltr";
    }
}