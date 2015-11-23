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
            this.settings = new Bot.DbSettings(engine.Db2Api.Connection);
            
            default_word_weights = default_word_weights.ToDictionary(x => x.Key.Trim().ToLower(), x => x.Value);

            List<string> iws = (from x in default_ignored_words select x.Trim()).ToList();
            default_ignored_words = new HashSet<string>(iws);

            default_synonyms = default_synonyms.ToDictionary(x => x.Key.Trim().ToLower(), x => x.Value.Trim().ToLower());            
        }
        readonly Engine engine;
        readonly Bot.DbSettings settings;

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

        public void Save(bool update_traning_time = true)
        {
            settings.Delete(SettingsKey.SCOPE, "%");
            company_ids2Company.Values.ToList().ForEach(x => { x.SaveWordWeights(); x.SaveSynonyms(); });
            SaveMappedCategories();
            if (update_traning_time)
                settings.Save(SettingsKey.SCOPE, SettingsKey.TRAINING_TIME, DateTime.Now);
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

        public bool AreCategoriesMapped(Product product1, Product product2)
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
                var category1s_to_mapped_category2s_ = settings.Get<Dictionary<string, List<string>>>(SettingsKey.SCOPE, SettingsKey.CATEGORY_MAP + company1_id_company2_id);
                category1s_to_mapped_category2s = new Dictionary<string, HashSet<string>>();
                foreach (string c1 in category1s_to_mapped_category2s_.Keys)
                    category1s_to_mapped_category2s[c1] = new HashSet<string>(category1s_to_mapped_category2s_[c1]);
                company1_id_company2_id_to_category1s_to_mapped_category2s[company1_id_company2_id] = category1s_to_mapped_category2s;
            }
            if (category1s_to_mapped_category2s == null)
                return false;
            HashSet<string> mapped_category2s;
            if (!category1s_to_mapped_category2s.TryGetValue(product1.DbProduct.Category, out mapped_category2s))
                return false;
            return mapped_category2s.Contains(product1.DbProduct.Category);
        }

        public void SaveMappedCategories()
        {
            foreach (string company1_id_company2_id in company1_id_company2_id_to_category1s_to_mapped_category2s.Keys)
                settings.Save(SettingsKey.SCOPE, SettingsKey.CATEGORY_MAP + company1_id_company2_id, company1_id_company2_id_to_category1s_to_mapped_category2s[company1_id_company2_id]);
        }

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
            Dictionary<string, List<string>> category1s_to_mapped_category2s = settings.Get<Dictionary<string, List<string>>>(SettingsKey.SCOPE, SettingsKey.CATEGORY_MAP + company1_id_company2_id);
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

            settings.Save(SettingsKey.SCOPE, SettingsKey.CATEGORY_MAP + company1_id_company2_id, company1_id_company2_id_to_category1s_to_mapped_category2s[company1_id_company2_id]);
        }
        #endregion

        Dictionary<string, double> default_word_weights = new Dictionary<string, double>()
        {
            {"g4",.5},
            {"g3",.5},
            {"g2",.5}, 
            {"sony", .7},
            {"Samsung", .7},
            {"Galaxy", .7},
            {"HTC", .7},
            {"EVO", .7},
            {"Xperia", .7},
            {"iPad", .7},     
        };

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