using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
//using System.Data.Odbc;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;
using Cliver.Bot;

namespace Cliver.Fhr.ProductOffice.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    public partial class ProductOfficeEntities : DbContext
    {
        public ProductOfficeEntities(string cs)
            : base(cs)
        {
        }
    }

    public class DbApi : ProductOfficeEntities
    {
        static public DbApi Create()
        {        
            return new DbApi(Cliver.CrawlerHost.Api.GetConnectionString(DATABASE_CONNECTION_STRING_NAME));
        }

        DbApi(string connection_string)
            : base(connection_string)
        {
            try
            {
                if (!ProgramRoutines.IsWebContext)
                    Log.Main.Write("DbApi ConnectionString: " + connection_string);
            }
            catch(Exception e)
            {
                string m = "The app could not connect the database with string:\r\n" + connection_string + "\r\nBe sure " + Cliver.CrawlerHost.Api.CrawlerHost_CONGIG_FILE_NAME + " file exists and is correct.\r\n\r\n" + e.Message;
                if (!ProgramRoutines.IsWebContext)
                    LogMessage.Exit(m);
                else
                    throw new Exception(m);
            }
        }

        public const string DATABASE_CONNECTION_STRING_NAME = "ProductOfficeEntities";

        public static string GetProviderConnectionString()
        {
            string connection_string = Cliver.CrawlerHost.Api.GetConnectionString(DATABASE_CONNECTION_STRING_NAME);
            Match m = Regex.Match(connection_string, @"provider\s+connection\s+string\s*=\s*([""'])(.*?)\1", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
                connection_string = m.Groups[2].Value;
            return connection_string;
        }

        //public static string GetConnectionString()
        //{
        //    return Cliver.CrawlerHost.Api.GetConnectionString(DATABASE_CONNECTION_STRING_NAME);
        //}

        public readonly DbConnection Connection;

        public static void RenewContext(ref DbApi context)
        {
            if (context != null)
                context.Dispose();
            context = new DbApi(Cliver.CrawlerHost.Api.GetConnectionString(DATABASE_CONNECTION_STRING_NAME));
        }
    }
}

