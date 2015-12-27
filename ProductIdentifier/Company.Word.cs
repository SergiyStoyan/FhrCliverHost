//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Forms;
//using System.Web;
//using System.IO;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Data;
//using System.Data.SqlClient;
//using System.Configuration;

//namespace Cliver.ProductIdentifier
//{
//    internal partial class Company
//    {
//        internal class Word
//        {
//            readonly internal string Value;

//            public double Weight
//            {
//                get
//                {
//                    return company.engine.Configuration.Get(company.DbCompany.Id).GetWordWeight(Value);
//                }
//            }

//            internal int Count(Field field)
//            {
//                int c;
//                if (!field2count.TryGetValue(field, out c))
//                {
//                    c = get_count(field);
//                    field2count[field] = c;
//                }
//                return c;
//            }
//            Dictionary<Field, int> field2count = new Dictionary<Field, int>();

//            int get_count(Field field)
//            {
//                HashSet<int> product_ids;
//                if (!company.Words2ProductIds(field).TryGetValue(Value, out product_ids))
//                    return 0;

//                return (from x in product_ids select GetCountInProduct(field, x)).Sum();
//            }

//            internal int GetCountInProduct(Field field, int product_id)
//            {
//                return company.engine.Products.Get(product_id).Words2Count(field)[Value];
//            }

//            internal double WordDensity(Field field)
//            {
//                double wd;
//                if (!word_density.TryGetValue(field, out wd))
//                {
//                    wd = get_word_density(company, field);
//                    word_density[field] = wd;
//                }
//                return wd;
//            }
//            Dictionary<Field, double> word_density = new Dictionary<Field, double>();

//            double get_word_density(Cliver.ProductIdentifier.Company company, Field field)
//            {
//                return (double)Count(field) / company.WordNumber(field);
//            }

//            internal double ProductFrequency(Field field)
//            {
//                double pf;
//                if (!product_frequency.TryGetValue(field, out pf))
//                {
//                    pf = get_product_frequency(company, field);
//                    product_frequency[field] = pf;
//                }
//                return pf;
//            }
//            Dictionary<Field, double> product_frequency = new Dictionary<Field, double>();

//            double get_product_frequency(Cliver.ProductIdentifier.Company company, Field field)
//            {
//                HashSet<int> company_product_ids_for_word;
//                if (!company.Words2ProductIds(field).TryGetValue(Value, out company_product_ids_for_word))
//                    return 0;

//                return (double)company_product_ids_for_word.Count / company.DbCompany.Products.Count;
//            }

//            readonly Cliver.ProductIdentifier.Company company;

//            public Word(Cliver.ProductIdentifier.Company company, string word)
//            {
//                this.Value = word;
//                this.company = company;
//            }

//            //public double Weight
//            //{
//            //    get
//            //    {
//            //        return Configuration.Get().GetWordWeight(Value);
//            //    }
//            //}

//            //internal bool IsInDictionary
//            //{
//            //    get
//            //    {
//            //        if (!is_in_dictionary_set)
//            //        {
//            //            is_in_dictionary = engine.Words.Hunspell.Spell(Value);
//            //            is_in_dictionary_set = true;
//            //        }
//            //        return is_in_dictionary;
//            //    }
//            //}
//            //bool is_in_dictionary_set = false;
//            //bool is_in_dictionary;

//            //public double GetDensityInProduct(Field field, int product_sku)
//            //{
//            //    return (double)GetCountInProduct(field, product_sku) / (from x in Product.Get(product_sku).Word2Count(field) select x.Value).Sum();
//            //}
//        }

//        internal Word GetWord(string word)
//        {
//            Word w;
//            if (!words2Word.TryGetValue(word, out w))
//            {
//                w = new Word(this, word);
//                words2Word[word] = w;
//            }
//            return w;
//        }
//        Dictionary<string, Word> words2Word = new Dictionary<string, Word>();
//    }
//}