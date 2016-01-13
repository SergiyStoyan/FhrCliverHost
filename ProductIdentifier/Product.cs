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
        public readonly Fhr.ProductOffice.Models.Product DbProduct;

        public string Normalized(Field field)
        {
            string t;
            if (!field2text.TryGetValue(field, out t))
            {
                switch (field)
                {
                    case Field.Name:
                        t = get_normalized(DbProduct.Name);
                        break;
                    case Field.Description:
                        t = get_normalized(DbProduct.Description);
                        field2text[field] = t;
                        break;
                    case Field.Category:
                        return engine.Companies.Get(DbProduct.CompanyId).NormalizedCategory(DbProduct.Category);
                    default:
                        throw new Exception("untreated case");
                }
            }
            field2text[field] = t;
            return t;
        }
        readonly Dictionary<Field, string> field2text = new Dictionary<Field, string>();

        internal string get_normalized(string text)
        {
            string t = Cliver.PrepareField.Html.GetDbField(text);
            t = t.ToLower();
            t = engine.Companies.Get(DbProduct.CompanyId).ReplaceWithSynonyms(t);
            t = engine.Companies.Get(DbProduct.CompanyId).StripOfIgnoredWords(t);
            t = Regex.Replace(t, @"\s\s+", " ", RegexOptions.Compiled | RegexOptions.Singleline);
            return t.Trim();
        }

        public Dictionary<string, int> Words2Count(Field field)
        {
            if (field == Field.Category)
                return engine.Companies.Get(DbProduct.CompanyId).CategoryWords2Count(Normalized(Field.Category));

            Dictionary<string, int> w2c;
            if (!field2word2count.TryGetValue(field, out w2c))
            {
                w2c = engine.Companies.Get(DbProduct.CompanyId).GetWords2Count(Normalized(field));
                field2word2count[field] = w2c;
            }
            return w2c;
        }
        readonly Dictionary<Field, Dictionary<string, int>> field2word2count = new Dictionary<Field, Dictionary<string, int>>();
        
        public Dictionary<string, int>.KeyCollection Words(Field field)
        {
            return Words2Count(field).Keys;
        }

        internal Product(Engine engine, Fhr.ProductOffice.Models.Product product)
        {
            engine.sw10.Start();
            this.engine = engine;
            if (product == null)
                throw new Exception("product is null");
            DbProduct = product;
            engine.sw10.Stop();
        }
        readonly Engine engine;
    }
}
