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
    public class SettingsKey : Cliver.Enum<string>
    {
        public const string SCOPE = "PRODUCT_IDENTIFIER";
        public const string COMPANY = "COMPANY/";
        public const string TRAINING_TIME = "TrainingTime";
        public const string ANALYSIS_TIME = "/AnalysisTime";
        public const string WORD_WEIGHTS = "/WordWeights";
        public const string SYNONYMS = "/Synonyms";
        public const string CATEGORY_MAP = "CATEGORYMAP/";

        public SettingsKey(string value) : base(value) { }
    }

    public partial class Configuration
    {
        public const int NO_COMPANY_DEPENDENT = -1;

        public Configuration(Engine engine)
        {
            this.engine = engine;
            
            //default_word_weights = default_word_weights.ToDictionary(x => x.Key.Trim().ToLower(), x => x.Value);

            List<string> iws = (from x in default_ignored_words select x.Trim()).ToList();
            default_ignored_words = new HashSet<string>(iws);

            default_synonyms = default_synonyms.ToDictionary(x => x.Key.Trim().ToLower(), x => x.Value.Trim().ToLower());            
        }
        readonly Engine engine;

        public Company Get(Product product)
        {
            return Get(product.DbProduct.CompanyId);
        }

        public Company Get(int company_id = NO_COMPANY_DEPENDENT)
        {
            Company c;
            if (!company_ids2Company.TryGetValue(company_id, out c))
            {
                c = new Company(this, company_id);
                company_ids2Company[company_id] = c;
            }
            return c;
        }
        Dictionary<int, Company> company_ids2Company = new Dictionary<int, Company>();

        #region API for self-training
        public void ClearBeforeSelfTraining()
        {
            company1_id_company2_id_to_category1s_to_mapped_category2s.Clear();
        }

        public void SaveAfterSelfTraining(bool update_traning_time = true)
        {
            Cliver.Bot.DbSettings.Delete(engine.Dbc, SettingsKey.SCOPE, "%");
            company_ids2Company.Values.ToList().ForEach(x => { x.SaveWordWeights(); x.SaveSynonyms(); });
            SaveMappedCategories();
            if (update_traning_time)
                Cliver.Bot.DbSettings.Save(engine.Dbc, SettingsKey.SCOPE, SettingsKey.TRAINING_TIME, DateTime.Now);
        }

        public void MapCategories(Product product1, Product product2)
        {
            if (product2.DbProduct.CompanyId < product1.DbProduct.CompanyId)
            {
                Product p = product1;
                product1 = product2;
                product2 = p;
            }
            Dictionary<string, HashSet<string>> category1s_to_mapped_category2s;
            string company1_id_company2_id = product1.DbProduct.CompanyId.ToString() + "," + product2.DbProduct.CompanyId;
            if (!company1_id_company2_id_to_category1s_to_mapped_category2s.TryGetValue(company1_id_company2_id, out category1s_to_mapped_category2s))
            {
                category1s_to_mapped_category2s = new Dictionary<string, HashSet<string>>();
                company1_id_company2_id_to_category1s_to_mapped_category2s[company1_id_company2_id] = category1s_to_mapped_category2s;
            }
            HashSet<string> mapped_category2s;
            if (!category1s_to_mapped_category2s.TryGetValue(product1.DbProduct.Category, out mapped_category2s))
            {
                mapped_category2s = new HashSet<string>();
                category1s_to_mapped_category2s[product1.DbProduct.Category] = mapped_category2s;
            }
            mapped_category2s.Add(product2.DbProduct.Category);
        }

        Dictionary<string, Dictionary<string, HashSet<string>>> company1_id_company2_id_to_category1s_to_mapped_category2s = new Dictionary<string, Dictionary<string, HashSet<string>>>();

        public bool DoCategoriesBelong2MappedOnes(Product product1, Product product2)
        {
            if (product2.DbProduct.CompanyId < product1.DbProduct.CompanyId)
            {
                Product p = product1;
                product1 = product2;
                product2 = p;
            }
            Dictionary<string, HashSet<string>> category1s_to_mapped_category2s;
            string company1_id_company2_id = product1.DbProduct.CompanyId.ToString() + "," + product2.DbProduct.CompanyId;
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
              if(is_child_of_category(product1.DbProduct.Category, c1))
                  if (category1s_to_mapped_category2s.TryGetValue(c1, out mapped_category2s))
                      foreach(string c2 in mapped_category2s)
                          if(is_child_of_category(product2.DbProduct.Category, c2))
                              return true;
            return false;
        }

        bool is_child_of_category(string child_category, string category)
        {
            return Regex.IsMatch(child_category, @"^\s*" + Regex.Escape(category), RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        public double GetCategoryScore(Product product1, Product product2)
        {
            if (product2.DbProduct.CompanyId < product1.DbProduct.CompanyId)
            {
                Product p = product1;
                product1 = product2;
                product2 = p;
            }
            Dictionary<string, double> category1_category2_to_score;
            string company1_id_company2_id = product1.DbProduct.CompanyId.ToString() + "," + product2.DbProduct.CompanyId;
            if (!company1_id_company2_id_to_category1_category2_to_score.TryGetValue(company1_id_company2_id, out category1_category2_to_score))
            {
                category1_category2_to_score = new Dictionary<string, double>();
                company1_id_company2_id_to_category1_category2_to_score[company1_id_company2_id] = category1_category2_to_score;
            }
            double score;
            string category1_category2 = product1.DbProduct.Category + Fhr.ProductOffice.DataApi.Product.CATEGORY_SEPARATOR + product2.DbProduct.Category;
            if (!category1_category2_to_score.TryGetValue(category1_category2, out score))
            {
                score = get_category_score(product1, product2);
                category1_category2_to_score[category1_category2] = score;
            }
            return score;
        }
        Dictionary<string, Dictionary<string, double>> company1_id_company2_id_to_category1_category2_to_score = new Dictionary<string, Dictionary<string, double>>();

        double get_category_score(Product product1, Product product2)
        {
            Dictionary<string, double> word2category_score = new Dictionary<string, double>();
            foreach (string word in product1.Words(Field.Category))
            {
                if (product2.Words2Count(Field.Category).ContainsKey(word))
                {
                    Word w = engine.Words.Get(word);
                    word2category_score[word] = w.Get(product1.DbProduct.CompanyId).Weight * w.Get(product2.DbProduct.CompanyId).Weight;
                }
            }

            double score = 0;
            if (word2category_score.Count > 0)
            {
                score = ((double)word2category_score.Values.Sum() / word2category_score.Count)
                    * ((double)word2category_score.Count / product1.Words(Field.Category).Count)
                    * ((double)word2category_score.Count / product2.Words(Field.Category).Count)
                    * (1 - 0.3 * word2category_score.Count);
            }
            score = (engine.Configuration.DoCategoriesBelong2MappedOnes(product1, product2) ? 1 : 0.5) * score;
            return score;
        }

        public void SaveMappedCategories()
        {
            foreach (string company1_id_company2_id in company1_id_company2_id_to_category1s_to_mapped_category2s.Keys)
                Cliver.Bot.DbSettings.Save(engine.Dbc, SettingsKey.SCOPE, SettingsKey.CATEGORY_MAP + company1_id_company2_id, company1_id_company2_id_to_category1s_to_mapped_category2s[company1_id_company2_id]);
        }
        #endregion

        #region API for editing configuration

        public void GetMappedCategoriesAsString(int company1_id, int company2_id, out string mapped_categories)
        {
            if (company2_id < company1_id)
                throw new Exception("company1_id must be < company2_id");
            string company1_id_company2_id = company1_id.ToString() + "," + company2_id;
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
            if (company2_id < company1_id)
                throw new Exception("company1_id must be < company2_id");
            string company1_id_company2_id = company1_id.ToString() + "," + company2_id;
            company1_id_company2_id_to_category1s_to_mapped_category2s.Remove(company1_id_company2_id);
            Dictionary<string, HashSet<string>> category1s_to_mapped_category2s = Cliver.Bot.SerializationRoutines.Json.Get<Dictionary<string, HashSet<string>>>(mapped_categories);
            company1_id_company2_id_to_category1s_to_mapped_category2s[company1_id_company2_id] = category1s_to_mapped_category2s;

            Cliver.Bot.DbSettings.Save(engine.Dbc, SettingsKey.SCOPE, SettingsKey.CATEGORY_MAP + company1_id_company2_id, company1_id_company2_id_to_category1s_to_mapped_category2s[company1_id_company2_id]);
        }
        #endregion

        //Dictionary<string, double> default_word_weights = new Dictionary<string, double>()
        //{
        //    {"g4",.5},
        //    {"g3",.5},
        //    {"g2",.5}, 
        //    {"sony", .7},
        //    {"Samsung", .7},
        //    {"Galaxy", .7},
        //    {"HTC", .7},
        //    {"EVO", .7},
        //    {"Xperia", .7},
        //    {"iPad", .7},     
        //};

        HashSet<string> default_ignored_words = new HashSet<string>() { 
            "the", "a", "an", "some","this","there","their","it",
            "and", "or",
            "so", "that", "then","only","currently","available","big","new","reasonable","advisable","more","much","as","such","with","other","each","not","during",
            "of", "in", "on", "at", "by", "off","for",
            "is", "are","be","was", "were",
            ".", ",", ";", "'", "\"", "!", "?", ":", "-",            
            "size", "inch", "inches", "cm","pixel","pixels",
            "kg",            
            "type","types", "resolution", "resolutions", "version","versions","brand","brands","price","prices","quality","qualities","service","services",
            "specifications", "specification",
            "advantage","advantages","product","products","feature","features","difference","differences",
            
            "prevent","prevents","provide","provides","get",
            
            "compared","easier","makes","design","designed","compatible","used","origin","certificated",
            
            "industry","system","functionality","test","before","package","we","also",
        };
        //,,,iso9001,,qc,,conducts,strict,visual,inspection,,,,,,transportation,,,invent,our,own,super,packaging,method,give,maximum,protection,therefore,assured,apart,from,part,supplies,related,replacement,purchasing,notes,closely,connected,digitizer,touch,panel,frame,when,replacing,lcd,opening,tool,should,separate,process,excellent,skill,good,master,force,highly,required,leaving,feature,advantage,damages,has,appearance,function,shipment,testing,will,avoid,unnecessary,waste,your,valuable,time,money,ship,fedex,ups,dhl,express,within,working,days,after,confirming,customer,s,payment,you,can,still,purchase,cell,phone,item,screen,assembly,following,includes,component,home,button,keyboard,buckle,5,2048,1536,display,material,led,backlit,connector,clip,plug,components,multi,glass,lens,soft,which,adhered,together,special,machining,if,broken,replace,well,flex,cable,ribbon,pretty,supple,corrosion,resistant,end,metal,piece,instead,slots,ribbons,coated,layer,anti,fingerprint,necessary,put,film,order,protect,being,blemished,besides,please,rest,guaranteed,make,sure,choose,right,model,fear,causing,any,trouble,web,installing,both,sides,scratching,must,bonding,large,space,between,may,contaminated,dust,even,slip,wide,breach,looks,like,divided,into,posts,difficulty,putting,through,hole,bent,180,degree,connecting,kindly,noted,cannot,responsible,possible,caused,personal,factors,antenna

        Dictionary<string, string> default_synonyms = new Dictionary<string, string>()
            {
                {"rear", "back"},
                {"gray", "grey"},
            };
    }
}