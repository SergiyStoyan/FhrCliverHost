using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;

namespace Cliver.ProductIdentifier
{
    public partial class Company
    {
        public double WordWeight(Field field, string word)
        {
            return GetWordWeight(word);
        }

        internal int WordCount(Field field, string word)
        {
            Dictionary<string, int> ws2c;
            if (!fields2words2count.TryGetValue(field, out ws2c))
            {
                ws2c = new Dictionary<string, int>();
                fields2words2count[field] = ws2c;
            }
            int c;
            if (!ws2c.TryGetValue(word, out c))
            {
                HashSet<int> product_ids;
                if (!Words2ProductIds(field).TryGetValue(word, out product_ids))
                    c = 0;
                else
                    c = (from x in product_ids select GetWordCountInProduct(field, x, word)).Sum();
                ws2c[word] = c;
            }
            return c;
        }
        Dictionary<Field, Dictionary<string, int>> fields2words2count = new Dictionary<Field, Dictionary<string, int>>();

        internal int GetWordCountInProduct(Field field, int product_id, string word)
        {
            return engine.Products.Get(product_id).Words2Count(field)[word];
        }

        internal double WordDensity(Field field, string word)
        {
            Dictionary<string, double> ws2wd;
            if (!fields2words2word_density.TryGetValue(field, out ws2wd))
            {
                ws2wd = new Dictionary<string, double>();
                fields2words2word_density[field] = ws2wd;
            }
            double wd;
            if (!ws2wd.TryGetValue(word, out wd))
            {
                wd = (double)WordCount(field, word) / WordNumber(field);
                ws2wd[word] = wd;
            }
            return wd;
        }
        Dictionary<Field, Dictionary<string, double>> fields2words2word_density = new Dictionary<Field, Dictionary<string, double>>();

        internal double WordProductFrequency(Field field, string word)
        {
            Dictionary<string, double> ws2pf;
            if (!fields2words2product_frequency.TryGetValue(field, out ws2pf))
            {
                ws2pf = new Dictionary<string, double>();
                fields2words2product_frequency[field] = ws2pf;
            }
            double pf;
            if (!ws2pf.TryGetValue(word, out pf))
            {
                HashSet<int> company_product_ids_for_word;
                if (!Words2ProductIds(field).TryGetValue(word, out company_product_ids_for_word))
                    pf = 0;
                else
                    pf = (double)company_product_ids_for_word.Count / DbCompany.Products.Count;
                ws2pf[word] = pf;
            }
            return pf;
        }
        Dictionary<Field, Dictionary<string, double>> fields2words2product_frequency = new Dictionary<Field, Dictionary<string, double>>();
    }
}

