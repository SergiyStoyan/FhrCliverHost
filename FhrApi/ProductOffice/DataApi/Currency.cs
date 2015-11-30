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

namespace Cliver.FhrApi.ProductOffice.DataApi
{
    public enum Currency
    {
        USD = 1,
        GBP = 2,
        SEK = 3,
        EUR = 4
    }
}

