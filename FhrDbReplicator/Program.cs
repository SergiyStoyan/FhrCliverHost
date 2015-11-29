using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.FhrApi;
using Cliver.Bot;
using System.Text.RegularExpressions;

namespace Cliver.FhrDbReplicator
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