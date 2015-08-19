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
using System.Text;
using System.Text.RegularExpressions;
//using System.Collections;
using System.Web;
using System.IO;
using System.Net;
using System.Windows.Forms;
//using System.Threading;

namespace Cliver.DataSifter
{
    static class Document
    {
        static string title = "";
        static internal string Title
        {
            get
            {
                return title;
            }
        }

        /// <summary>
        /// uri of the document
        /// </summary>
        static string uri = "";
        static internal string Uri
        {
            get
            {
                return uri;
            }
        }

        /// <summary>
        /// initial text string that is to be parsed in DataSifter
        /// </summary>
        static internal string Text
        {
            get
            {
                return text;
            }
        }
        static string text = "";

        internal static void SetTestText()
        {
            string test = null;
            try
            {
                test = System.IO.File.ReadAllText(Program.AppDir + @"\welcome.html");
            }
            catch { }
            if (string.IsNullOrEmpty(test))
                test = "<table width='100%'><tr><td bgcolor=\"#cccc99\"><h2><span style=\"color: #cc0033\">Welcome to DataSifter</span></h2><a href=\"http://www.cliversoft.com\">www.cliversoft.com</a> <p>  <span style=\"color: #000000;\"><strong>DataSifter's test  page</strong></span></td></tr> </table> <br />\r\r\r\n<meta mEtA><script>\rfunction</script><title>QWERTY</title><table>test&amp;<tr><td><p><a href='http://www.google.com'> TEST </a><p>String</td></tr></table>\r\n 1 2 2 3 43 qwerty\r\n\r\n\n  5fgdr4 &nbsp;&nbsp;gfd 6bbrt fDdg 5 \n6  t <b>yhhg </b>j jhg34RttQwE 654fjk SSSfhk <h class='test'> dj  hk  jfkfk 56 hh657";
            set(test, "test", "");
        }

        static void set(string text, string title, string uri)
        {
            Document.text = text;
            Document.title = title;
            Document.uri = uri;

            DocumentLoadedTime = DateTime.Now;

            if (DocumentUpdated != null)
                DocumentUpdated.Invoke();
        }
        //used by OutputForm
        internal static DateTime DocumentLoadedTime;

        internal delegate void DocumentUpdatedEventHandler();
        static internal event DocumentUpdatedEventHandler DocumentUpdated = null;

        static internal void LoadFromFile(string file)
        {
            Document.file = file;
            if (file == null || file == string.Empty)
                return;

            if (!System.IO.File.Exists(file))
            {
                Message.Error(uri + " does not exist");
                return;
            }

            Settings.Default.LastSourceFile = file;
            Settings.Default.Save();

            try
            {
                set(System.IO.File.ReadAllText(file, Encoding.UTF8), file, file);
            }
            catch (Exception e)
            {
                Message.Error(e);
            }
        }

        static string file = null;
        static internal string File
        {
            get
            {
                return file;
            }
        }
    }
}
