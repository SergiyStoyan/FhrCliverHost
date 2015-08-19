using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Cliver.ProductIdentifier
{
    static public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //Application.Run(MainForm.This);
                Engine engine = new Engine();
                List<ProductLink> ls = engine.CreateProductLinkList(new int[] { 52658 }, 3);
                List<Product> ps = ls[0].Product1s.ToList();
                ps.AddRange(ls[1].Product2s.ToList());
                engine.SaveLink(ps.Select(x => x.DbProduct.Id).ToArray());
                engine.Configuration.Save();
            }
            catch (Exception e)
            {

            }
        }
    }
}
