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

namespace Cliver.ProductIdentifier
{
    internal class Word
    {
        readonly internal string Value;

        internal class Company
        {
            public double Weight
            {
                get
                {
                    return word.engine.Configuration.Get(company.DbCompany.Id).GetWordWeight(word.Value);
                }
            }

            internal int Count(Field field)
            {
                int c;
                if (!field2count.TryGetValue(field, out c))
                {
                    c = get_count(field);
                    field2count[field] = c;
                }
                return c;
            }
            Dictionary<Field, int> field2count = new Dictionary<Field, int>();

            int get_count(Field field)
            {
                HashSet<int> product_ids;
                if (!company.Words2ProductIds(field).TryGetValue(word.Value, out product_ids))
                    return 0;

                return (from x in product_ids select GetCountInProduct(field, x)).Sum();
            }

            internal int GetCountInProduct(Field field, int product_id)
            {
                return word.engine.Products.Get(product_id).Words2Count(field)[word.Value];
            }

            internal double WordDensity(Field field)
            {
                double wd;
                if (!word_density.TryGetValue(field, out wd))
                {
                    wd = get_word_density(company, field);
                    word_density[field] = wd;
                }
                return wd;
            }
            Dictionary<Field, double> word_density = new Dictionary<Field, double>();

            double get_word_density(Cliver.ProductIdentifier.Company company, Field field)
            {
                return (double)Count(field) / company.WordNumber(field);
            }

            internal double ProductFrequency(Field field)
            {
                double pf;
                if (!product_frequency.TryGetValue(field, out pf))
                {
                    pf = get_product_frequency(company, field);
                    product_frequency[field] = pf;
                }
                return pf;
            }
            Dictionary<Field, double> product_frequency = new Dictionary<Field, double>();

            double get_product_frequency(Cliver.ProductIdentifier.Company company, Field field)
            {
                HashSet<int> company_product_ids_for_word;
                if (!company.Words2ProductIds(field).TryGetValue(word.Value, out company_product_ids_for_word))
                    return 0;

                return (double)company_product_ids_for_word.Count / company.DbCompany.Products.Count;
            }

            Word word;
            Cliver.ProductIdentifier.Company company;

            public Company(Cliver.ProductIdentifier.Company company, Word word)
            {
                this.word = word;
                this.company = company;
            }
        }

        //public double Weight
        //{
        //    get
        //    {
        //        return Configuration.Get().GetWordWeight(Value);
        //    }
        //}

        //internal bool IsInDictionary
        //{
        //    get
        //    {
        //        if (!is_in_dictionary_set)
        //        {
        //            is_in_dictionary = engine.Words.Hunspell.Spell(Value);
        //            is_in_dictionary_set = true;
        //        }
        //        return is_in_dictionary;
        //    }
        //}
        //bool is_in_dictionary_set = false;
        //bool is_in_dictionary;

        Dictionary<int, Company> company_ids2Company = new Dictionary<int, Company>();

        //public double GetDensityInProduct(Field field, int product_sku)
        //{
        //    return (double)GetCountInProduct(field, product_sku) / (from x in Product.Get(product_sku).Word2Count(field) select x.Value).Sum();
        //}

        internal Word(Engine engine, string word)
        {
            this.engine = engine;
            Value = word;
        }
        readonly Engine engine;

        internal Company Get(int company_id)
        {
            Company c;
            if (!company_ids2Company.TryGetValue(company_id, out c))
            {
                c = new Company(engine.Companies.Get(company_id), this);
                company_ids2Company[company_id] = c;
            }
            return c;
        }
    }

    internal class Words
    {
        internal Words(Engine engine)
        {
            this.engine = engine;

            //HashSet<string> essential_words = new HashSet<string>(raw_essential_words);
            //UriBuilder u = new UriBuilder(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase));
            //string p = Uri.UnescapeDataString(u.Path);
            //Hunspell = new NHunspell.Hunspell(p + @"\en_us.aff", p + @"\en_us.dic");
            //foreach (string word in essential_words)
            //    Hunspell.Remove(word);
        }
        //readonly internal NHunspell.Hunspell Hunspell = null;

        readonly Engine engine;

        //readonly string[] raw_essential_words = new string[] { 
        //    "apple",
        //};

        internal Word Get(string word)
        {
            Word ws;
            if (!words2WordStatistics.TryGetValue(word, out ws))
            {
                ws = new Word(engine, word);
                words2WordStatistics[word] = ws;
            }
            return ws;
        }
        Dictionary<string, Word> words2WordStatistics = new Dictionary<string, Word>();
    }
}