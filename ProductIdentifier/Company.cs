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
        internal readonly Fhr.ProductOffice.Models.Company DbCompany;
        
        internal string NormalizedCategory(string category)
        {
            string t;
            if (!categorys2normalized_category.TryGetValue(category, out t))
            {
                t = Regex.Replace(category == null ? "" : category, Regex.Escape(Fhr.ProductOffice.DataApi.Product.CATEGORY_SEPARATOR), " ", RegexOptions.Compiled | RegexOptions.Singleline);
                t = Cliver.PrepareField.Html.GetDbField(t);
                t = t.ToLower();
                t = ReplaceWithSynonyms(t);
                t = StripOfIgnoredWords(t);
                t = Regex.Replace(t, @"\s\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                t = t.Trim();
                categorys2normalized_category[category] = t;
            }
            return t;
        }
        Dictionary<string, string> categorys2normalized_category = new Dictionary<string, string>();

        internal Dictionary<string, int> CategoryWords2Count(string normalized_category)
        {
            Dictionary<string, int> cw2c;
            if (!categories2words2count.TryGetValue(normalized_category, out cw2c))
            {
                cw2c = GetWords2Count(normalized_category);
                categories2words2count[normalized_category] = cw2c;
            }
            return cw2c;
        }
        readonly Dictionary<string, Dictionary<string, int>> categories2words2count = new Dictionary<string,Dictionary<string,int>>();
    
        internal Dictionary<string, int> GetWords2Count(string normalized_text)
        {
            Dictionary<string, int> w2c = new Dictionary<string, int>();
            foreach (Match m in Regex.Matches(normalized_text, @"\w+|\d+[\:\.]\d+", RegexOptions.Singleline))
            {
                string word = m.Value.Trim().ToLower();
                //if(w != w.ToUpper())
                //string word = Char.ToLowerInvariant(w[0]) + w.Substring(1);
                word = Regex.Replace(word, "-", "", RegexOptions.Singleline);
                if (!w2c.ContainsKey(word))
                    w2c[word] = 0;
                w2c[word]++;
            }
            return w2c;
        }
        
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
        
        List<Product> GetProductsByCategory(string category)
        {
            return Products.Where(p => p.DbProduct.Category != null && Regex.IsMatch(p.DbProduct.Category, @"^\s*" + Regex.Escape(category), RegexOptions.IgnoreCase | RegexOptions.Multiline)).ToList();
        }

        internal HashSet<string> Categories
        {
            get
            {
                return categories;
            }
        }
        HashSet<string> categories = new HashSet<string>();

        Dictionary<string, dynamic> get_company_category_tree(int company_id)
        {
            List<string> categories = engine.Db.Products.Where(p => p.CompanyId == company_id).GroupBy(p => p.Category).Select(c => c.Key).ToList();
            Dictionary<string, dynamic> tree = build_tree_from_paths(categories);
            return tree;
        }

        Dictionary<string, dynamic> build_tree_from_paths(List<string> paths)
        {
            Dictionary<string, dynamic> tree = new Dictionary<string, dynamic>();
            foreach (string path in paths)
                if (path != null)
                    add_path(path, tree);
            return tree;
        }

        void add_path(string path, Dictionary<string, dynamic> tree)
        {
            Match m = Regex.Match(path, "^(.*?)" + Regex.Escape(Fhr.ProductOffice.DataApi.Product.CATEGORY_SEPARATOR) + "+(.+)", RegexOptions.Compiled | RegexOptions.Singleline);
            if (m.Success)
            {
                string name = m.Groups[1].Value;
                dynamic child_tree;
                if (!tree.TryGetValue(name, out child_tree))
                {
                    child_tree = new Dictionary<string, object>();
                    tree[name] = child_tree;
                }
                add_path(m.Groups[2].Value, child_tree);
            }
            else
                tree[path] = new Dictionary<string, object>();
        }

        internal Company(Engine engine, Fhr.ProductOffice.Models.Company company)
        {
            this.engine = engine;
            DbCompany = company;

            List<string> categories = engine.Db.Products.Where(p => p.CompanyId == company.Id).GroupBy(p => p.Category).Select(c => c.Key).ToList();
            this.categories = new HashSet<string>(categories);

            initialize_settings();
        }
        readonly Engine engine;
    }
}

