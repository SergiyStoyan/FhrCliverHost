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

    public partial class Engine
    {
        public const int NO_COMPANY_ID = -1;

        void initialize_settings()
        {            
            //default_word_weights = default_word_weights.ToDictionary(x => x.Key.Trim().ToLower(), x => x.Value);

            List<string> iws = (from x in DefaultIgnoredWords select x.Trim()).ToList();
            DefaultIgnoredWords = new HashSet<string>(iws);

            DefaultSynonyms = DefaultSynonyms.ToDictionary(x => x.Key.Trim().ToLower(), x => x.Value.Trim().ToLower());            
        }

        #region API for self-training
        //!!!Update Configuration in Engine being kept for reuse, after performing self-training!!!
        public void PerformSelfTraining()
        {
            lock (this)
            {
                prepare_for_self_training();
                Dictionary<int?, List<Fhr.ProductOffice.Models.Product>> link_ids2linked_products = Db.Products.GroupBy(p => p.LinkId).Where(g => g.Key > 0 && g.Count() > 1).ToDictionary(g => g.Key, g => g.ToList());
                foreach (List<Fhr.ProductOffice.Models.Product> lps in link_ids2linked_products.Values)
                    analyse_link(lps.Select(p => p.Id).ToArray());
                save_after_self_training();
            }
        }

        public void prepare_for_self_training()
        {
            CompanyPairs.PrepareForSelfTraining();
        }

        public void save_after_self_training(bool update_traning_time = true)
        {
            Cliver.Bot.DbSettings.Delete(Dbc, SettingsKey.SCOPE, "%");
            Companies.All.ToList().ForEach(x => { x.SaveWordWeights(); x.SaveSynonyms(); });

            CompanyPairs.SaveAfterSelfTraining();

            if (update_traning_time)
                Cliver.Bot.DbSettings.Save(Dbc, SettingsKey.SCOPE, SettingsKey.TRAINING_TIME, DateTime.Now);
        }

        void analyse_link(int[] linked_product_ids)
        {
            lock (this)
            {
                for (int i = 0; i < linked_product_ids.Length; i++)
                    for (int j = i + 1; j < linked_product_ids.Length; j++)
                    {
                        int product1_id = linked_product_ids[i];
                        int product2_id = linked_product_ids[j];
                        Product product1 = Products.Get(product1_id);
                        Product product2 = Products.Get(product2_id);
                                                
                        CompanyPairs.MapCategories(product1, product2);

                        //Dictionary<Field, HashSet<string>> matched_words = new Dictionary<Field, HashSet<string>>();
                        ////matched_words[Field.Category] = new HashSet<string>();
                        ////foreach (string word in product1.Words(Field.Category))
                        ////    if (product2.Words2Count(Field.Category).ContainsKey(word))
                        ////        matched_words[Field.Category].Add(word);
                        //matched_words[Field.Name] = new HashSet<string>();
                        //foreach (string word in product1.Words(Field.Name))
                        //    if (product2.Words2Count(Field.Name).ContainsKey(word))
                        //        matched_words[Field.Name].Add(word);

                        //List<ProductLink> pls = create_identical_Product_list_for_training(product1_id, product2.DbProduct.CompanyId);
                        //foreach (ProductLink pl in pls)
                        //{
                        //    if (null != pl.Product2s.Where(x => x.DbProduct.Id == product2_id).FirstOrDefault())
                        //        break;
                        //    foreach (Product p2 in pl.Product2s)
                        //    {
                        //        if (product1_id == p2.DbProduct.Id)
                        //            continue;
                        //        Dictionary<Field, List<string>> mws = pl.Get(product1_id, p2.DbProduct.Id).MatchedWords;
                        //        //List<string> week_mws = mws[Field.Category].Where(x => !matched_words[Field.Category].Contains(x)).ToList();
                        //        //foreach (string word in week_mws)
                        //        //{
                        //        //    Configuration.Get(product1).SetWordWeight(word, 0.9 * Configuration.Get(product1).GetWordWeight(word));
                        //        //    Configuration.Get(product2).SetWordWeight(word, 0.9 * Configuration.Get(product2).GetWordWeight(word));
                        //        //}
                        //        List<string> week_mws = mws[Field.Name].Where(x => !matched_words[Field.Name].Contains(x)).ToList();
                        //        foreach (string word in week_mws)
                        //        {
                        //            Configuration.Company c1 = Configuration.Get(product1);
                        //            c1.SetWordWeight(word, 0.9 * c1.GetWordWeight(word));
                        //            Configuration.Company c2 = Configuration.Get(product2);
                        //            c2.SetWordWeight(word, 0.9 * c2.GetWordWeight(word));
                        //        }
                        //    }
                        //}
                    }
            }
        }

        //List<ProductLink> create_identical_Product_list_for_training(int product1_id, int company2_id)
        //{
        //    List<ProductLink> pls = (from x in Companies.Get(company2_id).DbCompany.Products select new ProductLink(this, Products.Get(product1_id), Products.Get(x.Id))).ToList();
        //    pls = pls.OrderByDescending(x => x.Score).OrderByDescending(x => x.SecondaryScore).ToList();
        //    return pls;
        //}
        #endregion

        #region API for analysing data
        //!!!Update Configuration in Engine being kept for reuse, after performing self-training!!!
        public void PerformDataAnalysis(int company_id)
        {
            lock (this)
            {
                Company c = Companies.Get(company_id);
                c.PrepareForDataAnalysis();
                foreach (string w in Companies.Get(company_id).Words2ProductIds(Field.Name).Keys)
                    c.DefineWordWeight(w);
                c.SaveAfterDataAnalysis();
            }
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

      internal HashSet<string> DefaultIgnoredWords = new HashSet<string>() { 
            "the", "a", "an", "some","this","there","their","it",
            "and", "or",
            "so", "that", "then","only","currently","available","big","new","reasonable","advisable","more","much","as","such","with","other","each","not","during",
            "of", "in", "on", "at", "by", "off","for","all",
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
        
        internal  Dictionary<string, string> DefaultSynonyms = new Dictionary<string, string>()
            {
                {"rear", "back"},
                {"gray", "grey"},
            };
    }
}