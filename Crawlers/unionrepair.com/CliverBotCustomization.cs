//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************

using System;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
//using mshtml;
using Cliver;
using System.Configuration;
using System.Windows.Forms;
//using MySql.Data.MySqlClient;
using Cliver.Bot;
using Cliver.BotGui;

namespace Cliver.BotCustomization
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            //Cliver.CrawlerHost.Linker.ResolveAssembly();
            main();
        }

        static void main()
        {
            //Cliver.Bot.Program.Run();//It is the entry when the app runs as a console app.
            Cliver.BotGui.Program.Run();//It is the entry when the app uses the default GUI.
        }   
    }

    public class CustomBotGui : Cliver.BotGui.BotGui
    {
        override public string[] GetConfigControlNames()
        {
            return new string[] { "General", "Input", "Output", "Web", /*"Spider", "Browser",*/ "Proxy", "Log" };
        }

        override public Cliver.BaseForm GetToolsForm()
        {
            return null;
        }

        override public Type GetBotThreadControlType()
        {
            //return typeof(IeRoutineBotThreadControl);
            return typeof(WebRoutineBotThreadControl);
        }
    }

    public class CustomBot : Cliver.Bot.Bot
    {
        new static public string GetAbout()
        {
            return @"WEB CRAWLER
Created: " + Cliver.Bot.Program.GetCustomizationCompiledTime().ToString() + @"
Developed by: www.cliversoft.com";
        }

        new static public void SessionCreating()
        {
            //InternetDateTime.CHECK_TEST_PERIOD_VALIDITY(2014, 12, 30);

            Cliver.BotGui.Program.BindProgressBar2InputItemQueue(typeof(ListItem));
            //listing_id21 = Session.GetSingleValueWorkItemDictionary<ListingId, int>();

            FhrApi.CrawlerHost.CrawlerApi.Initialize();
        }

        HttpRoutine HR = new HttpRoutine();

        public class HomeItem : InputItem
        {
            readonly public string Url;

            public HomeItem(string url)
            {
                Url = url;
            }

            override public void PROCESSOR(BotCycle bc)
            {
                CustomBot cb = (CustomBot)bc.Bot;
                if (!cb.HR.Get(Url))
                    throw new ProcessorException(ProcessorExceptionType.RESTORE_AS_NEW, "Could not get: " + Url);

                DataSifter.Capture gc = cb.category.Parse(cb.HR.HtmlResult);
                string[] urls = Spider.GetAbsoluteUrls(gc.ValuesOf("CategoryUrl"), cb.HR.ResponseUrl, cb.HR.HtmlResult);
                foreach (string url in urls)
                    cb.BotCycle.Add(new ListItem(url));
            }
        }

        public class ListItem : InputItem
        {
            readonly public string Url;

            public ListItem(string url)
            {
                Url = url;
            }

            override public void PROCESSOR(BotCycle bc)
            {
                CustomBot cb = (CustomBot)bc.Bot;
                if (!cb.HR.Get(Url))
                    throw new ProcessorException(ProcessorExceptionType.RESTORE_AS_NEW, "Could not get: " + Url);

                DataSifter.Capture gc = cb.list.Parse(cb.HR.HtmlResult);
                
                {
                    string url = gc.ValueOf("NextPageUrl");
                    if (url != null)
                        cb.BotCycle.Add(new ListItem(Spider.GetAbsoluteUrl(url, cb.HR.ResponseUrl)));
                }
                
                string[] urls = Spider.GetAbsoluteUrls(gc.ValuesOf("ProductUrl"), cb.HR.ResponseUrl, cb.HR.HtmlResult);
                foreach (string url in urls)
                {
                    cb.BotCycle.Add(new ProductItem(url));
                }
            }
        }

        public class ProductItem : InputItem
        {
            readonly public string Url;

            public ProductItem(string url)
            {
                Url = url;
            }

            override public void PROCESSOR(BotCycle bc)
            {
                CustomBot cb = (CustomBot)bc.Bot;
                if (!cb.HR.Get(Url))
                    throw new ProcessorException(ProcessorExceptionType.RESTORE_AS_NEW, "Could not get: " + Url);

                DataSifter.Capture gc = cb.product.Parse(cb.HR.HtmlResult);

                FhrApi.CrawlerHost.Product product = new FhrApi.CrawlerHost.Product(
                    id: gc.ValueOf("Id"),
                    url: Url,
                    name: gc.ValueOf("Name"),
                    sku: gc.ValueOf("Sku"),
                    price: gc.ValueOf("Price"),
                    category_branch: gc.ValuesOf("Category"),
                    image_urls: Spider.GetAbsoluteUrls(gc.ValuesOf("ImageUrl"), Url, cb.HR.HtmlResult),
                    stock: gc.ValueOf("Stock") != null ? (decimal)FhrApi.CrawlerHost.ProductStock.IN_STOCK : (decimal)FhrApi.CrawlerHost.ProductStock.NOT_IN_STOCK,
                    description: gc.ValueOf("Description")
                    );
                if (!Cliver.FhrApi.CrawlerHost.CrawlerApi.SaveProductAsJson(product))
                    throw new ProcessorException(ProcessorExceptionType.ERROR, "Product was not saved.");
            }
        }

        DataSifter.Parser category = new DataSifter.Parser("Category.fltr");
        DataSifter.Parser list = new DataSifter.Parser("List.fltr");
        DataSifter.Parser product = new DataSifter.Parser("Product.fltr");
    }
}