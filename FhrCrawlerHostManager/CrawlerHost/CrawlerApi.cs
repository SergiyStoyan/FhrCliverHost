using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;
using Cliver.CrawlerHost;
using Cliver.Bot;

namespace Cliver.FhrCrawlerHost
{
    public class Crawler : Cliver.CrawlerHost.Crawler
    {
        new public enum ProductState : int
        {
            NEW = 1,
            //    //PARSED = 2,
            //    //INVALID = 3,
            DELETED = 4,
            REPLICATED = 5,
            INVALID = 6,
        }
    }

    public class CrawlerApi : Cliver.CrawlerHost.CrawlerApi
    {

    }
}

