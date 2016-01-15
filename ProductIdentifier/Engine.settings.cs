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
        readonly string DefaultConfiguration_PATH = Cliver.CrawlerHost.Api.GetCrawlerHostDirectory() + @"/ProductIdentifier/DefaultConfiguration/";
        
        public string GetDefaultIgnoredWordsAsString()
        {
            return File.ReadAllText(DefaultConfiguration_PATH + "DefaultIgnoredWords.txt");
        }

        public string GetDefaultSynonymsAsString()
        {
            return File.ReadAllText(DefaultConfiguration_PATH + "DefaultSynonyms.txt");
        }

        public void SaveDefaultIgnoredWordsFromString(string ignored_words)
        {
            File.WriteAllText(DefaultConfiguration_PATH + "DefaultIgnoredWords.txt", ignored_words);
        }

        public void SaveDefaultSynonymsFromString(string synonyms)
        {
            File.WriteAllText(DefaultConfiguration_PATH + "DefaultSynonyms.txt", synonyms);
        }
        
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

      //internal HashSet<string> DefaultIgnoredWords = new HashSet<string>() { 
      //      "the", "a", "an", "some","this","there","their","it",
      //      "and", "or",
      //      "so", "that", "then","only","currently","available","big","new","reasonable","advisable","more","much","as","such","with","other","each","not","during",
      //      "of", "in", "on", "at", "by", "off","for","all",
      //      "is", "are","be","was", "were",
      //      ".", ",", ";", "'", "\"", "!", "?", ":", "-",            
      //      "size", "inch", "inches", "cm","pixel","pixels",
      //      "kg",            
      //      "type","types", "resolution", "resolutions", "version","versions","brand","brands","price","prices","quality","qualities","service","services",
      //      "specifications", "specification", "series",
      //      "advantage","advantages","product","products","feature","features","difference","differences",
            
      //      "prevent","prevents","provide","provides","get",
            
      //      "compared","easier","makes","design","designed","compatible","used","origin","certificated",
            
      //      "industry","system","functionality","test","before","package","we","also",
      //  };
      //  //,,,iso9001,,qc,,conducts,strict,visual,inspection,,,,,,transportation,,,invent,our,own,super,packaging,method,give,maximum,protection,therefore,assured,apart,from,part,supplies,related,replacement,purchasing,notes,closely,connected,digitizer,touch,panel,frame,when,replacing,lcd,opening,tool,should,separate,process,excellent,skill,good,master,force,highly,required,leaving,feature,advantage,damages,has,appearance,function,shipment,testing,will,avoid,unnecessary,waste,your,valuable,time,money,ship,fedex,ups,dhl,express,within,working,days,after,confirming,customer,s,payment,you,can,still,purchase,cell,phone,item,screen,assembly,following,includes,component,home,button,keyboard,buckle,5,2048,1536,display,material,led,backlit,connector,clip,plug,components,multi,glass,lens,soft,which,adhered,together,special,machining,if,broken,replace,well,flex,cable,ribbon,pretty,supple,corrosion,resistant,end,metal,piece,instead,slots,ribbons,coated,layer,anti,fingerprint,necessary,put,film,order,protect,being,blemished,besides,please,rest,guaranteed,make,sure,choose,right,model,fear,causing,any,trouble,web,installing,both,sides,scratching,must,bonding,large,space,between,may,contaminated,dust,even,slip,wide,breach,looks,like,divided,into,posts,difficulty,putting,through,hole,bent,180,degree,connecting,kindly,noted,cannot,responsible,possible,caused,personal,factors,antenna
        
      //  internal  Dictionary<string, string> DefaultSynonyms = new Dictionary<string, string>()
      //      {
      //          {"rear", "back"},
      //          {"gray", "grey"},
      //      };
    }
}