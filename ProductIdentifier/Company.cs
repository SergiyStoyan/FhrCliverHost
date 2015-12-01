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
    internal class Company
    {
        internal readonly Fhr.ProductOffice.Models.Company DbCompany;

        internal int WordNumber(Field field)
        {
            int wn;
            if (!field2word_number.TryGetValue(field, out wn))
            {
                wn = (from p in Products select p.Words2Count(field).Values.Sum()).Sum();
                field2word_number[field] = wn;
            }
            return wn;
        }
        readonly Dictionary<Field, int> field2word_number = new Dictionary<Field, int>();

        internal List<Product> Products
        {
            get
            {
                return (from p in engine.Products.All where p.DbProduct.CompanyId == DbCompany.Id select p).ToList();
            }
        }

        internal Dictionary<string, HashSet<int>> Words2ProductIds(Field field)
        {
            Dictionary<string, HashSet<int>> w2pis;
            if (!field2word2product_ids.TryGetValue(field, out w2pis))
            {
                w2pis = new Dictionary<string, HashSet<int>>();
                foreach (Product p in engine.Products.All)
                {
                    foreach (string word in p.Words(field))
                    {
                        HashSet<int> product_ids = null;
                        if (!w2pis.TryGetValue(word, out product_ids))
                        {
                            product_ids = new HashSet<int>();
                            w2pis.Add(word, product_ids);
                        }
                        w2pis[word].Add(p.DbProduct.Id);
                    }
                }
                field2word2product_ids[field] = w2pis;
            }
            return w2pis;
        }
        Dictionary<Field, Dictionary<string, HashSet<int>>> field2word2product_ids = new Dictionary<Field, Dictionary<string, HashSet<int>>>();

        internal Company(Engine engine, Fhr.ProductOffice.Models.Company company)
        {
            this.engine = engine;
            DbCompany = company;
        }
        readonly Engine engine;
    }

    internal class Companies
    {
        internal Companies(Engine engine)
        {
            this.engine = engine;
        }
        readonly Engine engine;

        internal Company Get(int company_id)
        {
            Company c = null;
            if (!company_ids2Company.TryGetValue(company_id, out c))
            {
                Fhr.ProductOffice.Models.Company dc = engine.Db.Companies.Where(x => x.Id == company_id).FirstOrDefault();
                if (dc == null)
                    throw new Exception("No Company for Id=" + company_id);
                c = new Company(engine, dc);
                company_ids2Company[company_id] = c;
                engine.Products.Ininitalize(dc.Products);
            }
            return c;
        }
        Dictionary<int, Company> company_ids2Company = new Dictionary<int, Company>();
    }
}

