using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Entity;
using ProductOffice.Models;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Specialized;
using System.Xml;

namespace ProductOffice
{
    class CurrencyRoutines
    {
        public static List<KeyValuePair<string, decimal>> GetCurrencyListFromWeb(out DateTime currencyDate)
        {
            List<KeyValuePair<string, decimal>> returnList = new List<KeyValuePair<string, decimal>>();
            string date = string.Empty;
            using (XmlReader xmlr = XmlReader.Create(@"http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"))
            {
                xmlr.ReadToFollowing("Cube");
                while (xmlr.Read())
                {
                    if (xmlr.NodeType != XmlNodeType.Element) continue;
                    if (xmlr.GetAttribute("time") != null)
                    {
                        date = xmlr.GetAttribute("time");
                    }
                    else returnList.Add(new KeyValuePair<string, decimal>(xmlr.GetAttribute("currency"), decimal.Parse(xmlr.GetAttribute("rate"), System.Globalization.CultureInfo.InvariantCulture)));
                }
                currencyDate = DateTime.Parse(date);
            }
            returnList.Add(new KeyValuePair<string, decimal>("EUR", 1));
            return returnList;
        }
    }
}