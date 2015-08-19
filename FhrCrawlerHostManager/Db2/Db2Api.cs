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

namespace Cliver.FhrCrawlerHost
{
    public class Db2Api
    {
        public enum Currency
        {
            USD = 1, 
            GBP = 2, 
            SEK = 3,
            EUR = 4
        }
        
        public Db2Api()
        {
            AGAIN:
            try
            {
                if (!ProgramRoutines.IsWebContext)
                    Log.Main.Write("Db2Api ConnectionString: " + ConnectionString);
                Connection = DbConnection.Create(Db2Api.ConnectionString);
                RenewContext();
                Db2.Company c = Context.Companies.FirstOrDefault();//to check connection
            }
            catch(Exception e)
            {
                if (!LogMessage.DisableStumblingDialogs)
                {
                    string connection_string = Db2Api.ConnectionString;
                    if (string.IsNullOrWhiteSpace(connection_string))
                        connection_string = Properties.Settings.Default.ProductOfficeConnectionString;
                    string message = e.Message + "\r\n\r\nThe app could not connect the database. Please save the correct connection string in settings.";                    
                    Cliver.CrawlerHost.DbConnectionSettingsForm f = new CrawlerHost.DbConnectionSettingsForm("ProductOffice database", message, connection_string);
                    f.ShowDialog();
                    if (f.ConnectionString == null)
                    {
                        Log.Error(e);
                        LogMessage.Exit("The app cannot work and will exit.");
                    }
                    Db2Api.ConnectionString = f.ConnectionString;
                    goto AGAIN;
                }
                if (ProgramRoutines.IsWebContext)
                    throw e;
                LogMessage.Exit(e);
            }
        }
        
        public static string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            internal set
            {
                Cliver.Bot.AppRegistry.SetValue(Db2ConnectionString_registry_name, value);
                _ConnectionString = value;
                if (!ProgramRoutines.IsWebContext)
                    Log.Main.Write("Db2Api ConnectionString: " + ConnectionString);
            }
        }
        static string _ConnectionString = AppRegistry.GetString(Db2ConnectionString_registry_name, false);
        const string Db2ConnectionString_registry_name = @"ProductOfficeDbConnectionString";

        public readonly DbConnection Connection;
        
        public Db2.ProductOfficeDataContext Context
        {
            get
            {
                return Context_;
            }
        }
        Db2.ProductOfficeDataContext Context_ = null;

        public Db2.ProductOfficeDataContext RenewContext()
        {
            if (Context_ != null) 
                Context_.Dispose();
            Context_ = new Db2.ProductOfficeDataContext(Db2Api.ConnectionString);
            return Context_;
        }
    }
}

