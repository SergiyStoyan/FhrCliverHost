using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.FhrCrawlerHost;
using Cliver.Bot;

using System.Text.RegularExpressions;

namespace Cliver.FhrDbReplicator
{
    class Parser
    {
        //static Cliver.DataSifter.Parser currency = new DataSifter.Parser("Currency.fltr");
 
        static public bool ParsePrice(string text, out Db2Api.Currency currency_id, out decimal price)
        {
            string t = Regex.Replace(text, @"\s+", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = Regex.Match(t, @"(?'Left'[^\.\,\;\d]*)(?'Price'\d+\.?\d*|\.\d+)(?'Right'[^\.\,\;\d]*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                string p = m.Groups["Price"].Value;
                if (decimal.TryParse(p, out price))
                {
                    string c = m.Groups["Left"].Value + "|" + m.Groups["Right"].Value;
                    if (Regex.IsMatch(c, @"\$|usd", RegexOptions.IgnoreCase | RegexOptions.Singleline))
                        currency_id = Db2Api.Currency.USD;
                    else
                    {
                        if (Regex.IsMatch(c, @"€|eur", RegexOptions.IgnoreCase | RegexOptions.Singleline))
                            currency_id = Db2Api.Currency.EUR;
                        else
                        {
                            if (Regex.IsMatch(c, @"£|gbp", RegexOptions.IgnoreCase | RegexOptions.Singleline))
                                currency_id = Db2Api.Currency.GBP;
                            else
                            {
                                LogMessage.Error("Could not detect price in " + text);
                                currency_id = Db2Api.Currency.USD;
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }
            LogMessage.Error("Could not detect price in " + text);
            price = -1;
            currency_id = Db2Api.Currency.USD;
            return false;
        }
    }
}