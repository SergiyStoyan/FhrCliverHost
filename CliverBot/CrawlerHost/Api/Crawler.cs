using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
//using System.Data.Odbc;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;
using Cliver.CrawlerHost;
using Cliver.Bot;

namespace Cliver.CrawlerHost
{
    public class Crawler
    {
        public enum State : int { ENABLED = 1, DISABLED = 2, DEBUG = 3 }

        public enum Command : int { EMPTY = 0, STOP = 1, RESTART = 2, FORCE = 3, RESTART_WITH_CLEAR_SESSION = 4 }

        public enum SessionState : int { STARTED = 1, _COMPLETED = 25, COMPLETED = 2, _ERROR = 35, ERROR = 3, BROKEN = 4, KILLED = 5 }

        public enum ProductState : int { NEW = 1, DELETED = 4 }
    }
}

