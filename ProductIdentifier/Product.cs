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
    public enum Field
    {
        Name,
        Description,
        Category
    }

    public class Product
    {
        public readonly FhrApi.ProductOffice.Models.Product DbProduct;

        public string NormalizedText(Field field)
        {
            string t;
            if (!field2text.TryGetValue(field, out t))
            {
                switch (field)
                {
                    case Field.Name:
                        t = DbProduct.Name;
                        break;
                    case Field.Description:
                        t = DbProduct.Description;
                        break;
                    case Field.Category:
                        t = Regex.Replace(DbProduct.Category == null ? "" : DbProduct.Category, Regex.Escape(FhrApi.ProductOffice.DataApi.Product.CATEGORY_SEPARATOR), " ", RegexOptions.Compiled | RegexOptions.Singleline);
                        break;
                    default:
                        throw new Exception("untreated case");
                }
                t = Cliver.PrepareField.Html.GetDbField(t);
                t = t.ToLower();
                t = engine.Configuration.Get(DbProduct.CompanyId).ReplaceWithSynonyms(t);
                t = engine.Configuration.Get(DbProduct.CompanyId).StripOfIgnoredWords(t);
                t = Regex.Replace(t, @"\s\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
                field2text[field] = t.Trim();
            }
            return t;
        }
        readonly Dictionary<Field, string> field2text = new Dictionary<Field, string>();

        public Dictionary<string, int> Words2Count(Field field)
        {
            Dictionary<string, int> w2c;
            if (!field2word2count.TryGetValue(field, out w2c))
            {
                w2c = new Dictionary<string, int>();
                foreach (Match m in Regex.Matches(NormalizedText(field), @"\w+|\d+[\:\.]\d+", RegexOptions.Singleline))
                {
                    string word = m.Value.Trim().ToLower();
                    //if(w != w.ToUpper())
                    //string word = Char.ToLowerInvariant(w[0]) + w.Substring(1);
                    word = Regex.Replace(word, "-", "", RegexOptions.Singleline);
                    if (!w2c.ContainsKey(word))
                        w2c[word] = 0;
                    w2c[word]++;
                }
                field2word2count[field] = w2c;
            }
            return w2c;
        }
        readonly Dictionary<Field, Dictionary<string, int>> field2word2count = new Dictionary<Field, Dictionary<string, int>>();

        public Dictionary<string, int>.KeyCollection Words(Field field)
        {
            return Words2Count(field).Keys;
        }

        internal Product(Engine engine, FhrApi.ProductOffice.Models.Product product)
        {
            this.engine = engine;
            if (product == null)
                throw new Exception("product is null");
            DbProduct = product;
        }
        readonly Engine engine;
    }

    internal class Products
    {
        internal Products(Engine engine)
        {
            this.engine = engine;
        }
        readonly Engine engine;

        internal Product Get(int product_id)
        {
            Product p = null;
            if (!product_ids2Product.TryGetValue(product_id, out p))
            {
                FhrApi.ProductOffice.Models.Product product = engine.Db.Products.Where(x => x.Id == product_id).FirstOrDefault();
                if (product == null)
                    throw new Exception("No Product with Id=" + product_id);
                p = new Product(engine, product);
                product_ids2Product[product.Id] = p;
            }
            return p;
        }

        internal Product[] GetLinked(int link_id)
        {
            List<Product> ps = new List<Product>();
            foreach (FhrApi.ProductOffice.Models.Product product in engine.Db.Products.Where(p => p.LinkId == link_id))
            {
                Product p = null;
                if (!product_ids2Product.TryGetValue(product.Id, out p))
                {
                    p = new Product(engine, product);
                    product_ids2Product[product.Id] = p;
                }
                ps.Add(p);
            }
            return ps.ToArray();
        }

        //public static Product Get(int product_id)
        //{
        //    Product p = null;
        //    if (!product_ids2Product.TryGetValue(product_id, out p))
        //    {
        //        FhrApi.ProductOffice.Product dp = FhrApi.Db.Products.Where(x => x.Id == product_id).FirstOrDefault();
        //        p = new Product(dp);
        //        product_ids2Product[product_id] = p;
        //    }
        //    return p;
        //}

        //public static Product Get(FhrApi.ProductOffice.Product product)
        //{
        //    return Get(product.Id);
        //}
        Dictionary<int, Product> product_ids2Product = new Dictionary<int, Product>();

        internal void Ininitalize(ICollection<FhrApi.ProductOffice.Models.Product> products)
        {
            foreach (FhrApi.ProductOffice.Models.Product product in products)
            {
                if (product == null)
                    throw new Exception("product is null");
                Product p = null;
                if (!product_ids2Product.TryGetValue(product.Id, out p))
                {
                    p = new Product(engine, product);
                    product_ids2Product[product.Id] = p;
                }
            }
        }

        internal List<Product> All
        {
            get
            {
                return product_ids2Product.Values.ToList();
            }
            set
            {
                foreach (Product p in value)
                    product_ids2Product[p.DbProduct.Id] = p;
            }
        }
    }
}
