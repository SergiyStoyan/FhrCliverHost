using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.FhrCrawlerHost;
using Cliver.Bot;
using System.Text.RegularExpressions;

namespace Cliver.FhrDbCleaner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CrawlerHost.Service.Run();
        }
    }
}