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
    public class CompanyPairs
    {
        internal CompanyPairs(Engine engine)
        {
            this.engine = engine;
        }
        readonly Engine engine;

        #region API for general self-training

        internal void PrepareForSelfTraining()
        {
            company1_id_company2_id_to_category1s_to_mapped_category2s.Clear();
        }

        internal void SaveAfterSelfTraining()
        {
            foreach (string company1_id_company2_id in company1_id_company2_id_to_category1s_to_mapped_category2s.Keys)
                save_mapped_categories(company1_id_company2_id);
        }

        string get_company_pair_key(ref Product product1, ref Product product2)
        {
            if (product1.DbProduct.CompanyId > product2.DbProduct.CompanyId)
            {
                Product p = product1;
                product1 = product2;
                product2 = p;
            }
            return product1.DbProduct.CompanyId + "," + product2.DbProduct.CompanyId;
        }

        void save_mapped_categories(string company1_id_company2_id)
        {
            Dictionary<string, HashSet<string>> category1s_to_mapped_category2s;
            if (!company1_id_company2_id_to_category1s_to_mapped_category2s.TryGetValue(company1_id_company2_id, out category1s_to_mapped_category2s))
                return;
            Cliver.Bot.DbSettings.Save(engine.Dbc, SettingsKey.SCOPE, SettingsKey.CATEGORY_MAP + company1_id_company2_id, category1s_to_mapped_category2s);
        }

        internal void MapCategories(Product product1, Product product2)
        {
            Dictionary<string, HashSet<string>> category1s_to_mapped_category2s;
            string company1_id_company2_id = get_company_pair_key(ref product1, ref product2);
            if (!company1_id_company2_id_to_category1s_to_mapped_category2s.TryGetValue(company1_id_company2_id, out category1s_to_mapped_category2s))
            {
                category1s_to_mapped_category2s = new Dictionary<string, HashSet<string>>();
                company1_id_company2_id_to_category1s_to_mapped_category2s[company1_id_company2_id] = category1s_to_mapped_category2s;
            }
            HashSet<string> mapped_category2s;
            if (!category1s_to_mapped_category2s.TryGetValue(product1.Normalized(Field.Category), out mapped_category2s))
            {
                mapped_category2s = new HashSet<string>();
                category1s_to_mapped_category2s[product1.Normalized(Field.Category)] = mapped_category2s;
            }
            mapped_category2s.Add(product2.Normalized(Field.Category));
        }
        #endregion

        #region API for self-training during linking
        internal void UnmapCategoriesAndSave(Product product1, Product product2)
        {
            Dictionary<string, HashSet<string>> category1s_to_mapped_category2s;
            string company1_id_company2_id = get_company_pair_key(ref product1, ref product2);
            if (!company1_id_company2_id_to_category1s_to_mapped_category2s.TryGetValue(company1_id_company2_id, out category1s_to_mapped_category2s))
                return;
            HashSet<string> category2s;
            if (!category1s_to_mapped_category2s.TryGetValue(product1.Normalized(Field.Category), out category2s))
                return;
            category2s.Remove(product2.Normalized(Field.Category));
            if (category2s.Count < 1)
                category1s_to_mapped_category2s.Remove(product1.Normalized(Field.Category));
            save_mapped_categories(company1_id_company2_id);
        }

        internal void MapCategoriesAndSave(Product product1, Product product2)
        {
            MapCategories(product1, product2);
            string company1_id_company2_id = get_company_pair_key(ref product1, ref product2);
            save_mapped_categories(company1_id_company2_id);
        }
        #endregion

        Dictionary<string, Dictionary<string, HashSet<string>>> company1_id_company2_id_to_category1s_to_mapped_category2s = new Dictionary<string, Dictionary<string, HashSet<string>>>();

        bool do_categories_belong2mapped_ones(Product product1, Product product2)
        {
            Dictionary<string, HashSet<string>> category1s_to_mapped_category2s;
            string company1_id_company2_id = get_company_pair_key(ref product1, ref product2);
            if (!company1_id_company2_id_to_category1s_to_mapped_category2s.TryGetValue(company1_id_company2_id, out category1s_to_mapped_category2s))
            {
                var category1s_to_mapped_category2s_ = Cliver.Bot.DbSettings.Get<Dictionary<string, List<string>>>(engine.Dbc, SettingsKey.SCOPE, SettingsKey.CATEGORY_MAP + company1_id_company2_id);
                if (category1s_to_mapped_category2s_ != null)
                {
                    category1s_to_mapped_category2s = new Dictionary<string, HashSet<string>>();
                    foreach (string c1 in category1s_to_mapped_category2s_.Keys)
                        category1s_to_mapped_category2s[c1] = new HashSet<string>(category1s_to_mapped_category2s_[c1]);
                }
                else
                    category1s_to_mapped_category2s = null;
                company1_id_company2_id_to_category1s_to_mapped_category2s[company1_id_company2_id] = category1s_to_mapped_category2s;
            }
            if (category1s_to_mapped_category2s == null)
                return false;
            HashSet<string> mapped_category2s;
            foreach(string c1 in category1s_to_mapped_category2s.Keys)
                if (is_child_of_category(product1.Normalized(Field.Category), c1))
                  if (category1s_to_mapped_category2s.TryGetValue(c1, out mapped_category2s))
                      foreach(string c2 in mapped_category2s)
                          if (is_child_of_category(product2.Normalized(Field.Category), c2))
                              return true;
            return false;
        }

        bool is_child_of_category(string child_category, string category)
        {
            return Regex.IsMatch(child_category, @"^\s*" + Regex.Escape(category), RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        internal double GetCategoryComparisonScore(Product product1, Product product2)
        {
            Dictionary<string, double> category1_category2_to_score;
            string company1_id_company2_id = get_company_pair_key(ref product1, ref product2);
            if (!company1_id_company2_id_to_category1_category2_to_score.TryGetValue(company1_id_company2_id, out category1_category2_to_score))
            {
                category1_category2_to_score = new Dictionary<string, double>();
                company1_id_company2_id_to_category1_category2_to_score[company1_id_company2_id] = category1_category2_to_score;
            }
            double score;
            string category1_category2 = product1.Normalized(Field.Category) + "~" + product2.Normalized(Field.Category);
            if (!category1_category2_to_score.TryGetValue(category1_category2, out score))
            {
                score = get_category_comparison_score(product1, product2);
                category1_category2_to_score[category1_category2] = score;
            }
            return score;
        }
        Dictionary<string, Dictionary<string, double>> company1_id_company2_id_to_category1_category2_to_score = new Dictionary<string, Dictionary<string, double>>();

        double get_category_comparison_score(Product product1, Product product2)
        {
            Dictionary<string, double> word2category_score = new Dictionary<string, double>();
            foreach (string word in product1.Words2Count(Field.Category).Keys)
            {
                if (product2.Words2Count(Field.Category).ContainsKey(word))
                    word2category_score[word] = engine.Companies.Get(product1.DbProduct.CompanyId).WordWeight(Field.Category, word) * engine.Companies.Get(product2.DbProduct.CompanyId).WordWeight(Field.Category, word);
            }

            double score = 0;
            if (word2category_score.Count > 0)
            {
                score = ((double)word2category_score.Values.Sum() / word2category_score.Count)
                    * ((double)word2category_score.Count / product1.Words(Field.Category).Count)
                    * ((double)word2category_score.Count / product2.Words(Field.Category).Count)
                    * (1 - 0.3 * word2category_score.Count);
            }
            score = (do_categories_belong2mapped_ones(product1, product2) ? 1 : 0.5) * score;
            return score;
        }

        #region API for editing configuration
        
        string get_company_pair_key(ref int company1_id, ref int company2_id)
        {
            if (company1_id > company2_id)
            {
                int ci = company1_id;
                company1_id = company2_id;
                company2_id = ci;
            }
            return company1_id.ToString() + "," + company2_id;
        }

        public void GetMappedCategoriesAsString(int company1_id, int company2_id, out string mapped_categories)
        {
            string company1_id_company2_id = get_company_pair_key(ref company1_id, ref company2_id);
            //Dictionary<string, HashSet<string>> category1s_to_mapped_category2s;
            //if (!company1_id_company2_id_to_category1s_to_mapped_category2s.TryGetValue(company1_id_company2_id, out category1s_to_mapped_category2s))
            //    mapped_categories = null;
            //else
            //    mapped_categories = Cliver.Bot.SerializationRoutines.Json.Get(category1s_to_mapped_category2s);
            Dictionary<string, List<string>> category1s_to_mapped_category2s = Cliver.Bot.DbSettings.Get<Dictionary<string, List<string>>>(engine.Dbc, SettingsKey.SCOPE, SettingsKey.CATEGORY_MAP + company1_id_company2_id);
            mapped_categories = Bot.SerializationRoutines.Json.Get(category1s_to_mapped_category2s);
            mapped_categories = Regex.Replace(mapped_categories, @",", "$0\r\n", RegexOptions.Singleline);
            mapped_categories = Regex.Replace(mapped_categories, @"\:|\]\s*,", "$0\r\n", RegexOptions.Singleline);
        }

        public void SaveMappedCategoriesFromString(int company1_id, int company2_id, string mapped_categories)
        {
            Dictionary<string, HashSet<string>> category1s_to_mapped_category2s = Cliver.Bot.SerializationRoutines.Json.Get<Dictionary<string, HashSet<string>>>(mapped_categories);
            string company1_id_company2_id = get_company_pair_key(ref company1_id, ref company2_id); 
            company1_id_company2_id_to_category1s_to_mapped_category2s[company1_id_company2_id] = category1s_to_mapped_category2s;
            save_mapped_categories(company1_id_company2_id);
        }
        #endregion
    }
}