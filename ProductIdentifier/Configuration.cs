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
    public class Configuration
    {
        public const int NO_COMPANY_DEPENDENT = -1;

        public Configuration(Engine engine)
        {
            this.engine = engine;

            List<int> company_ids = engine.Db2Api.Context.Companies.Select(x => x.Id).ToList();
           List<FhrCrawlerHost.Db2.ProductIdentifierData> unlinked_pids=null;
           try
           {
               unlinked_pids = engine.Db2Api.Context.ProductIdentifierDatas.Where(x => x.CompanyId != NO_COMPANY_DEPENDENT && !company_ids.Contains(x.CompanyId)).ToList();
           }
           catch (SqlException e)
           {
               engine.Db2Api.Connection.Get(@"
CREATE TABLE [dbo].[ProductIdentifierData] (
[CompanyId]   INT   NOT NULL,
[WordWeights] NTEXT DEFAULT (NULL) NOT NULL,
[Synonyms]    NTEXT DEFAULT (NULL) NOT NULL,
CONSTRAINT [PK_ProductIdentifierData] PRIMARY KEY CLUSTERED ([CompanyId] ASC)
);").Execute();
               unlinked_pids = engine.Db2Api.Context.ProductIdentifierDatas.Where(x => x.CompanyId != NO_COMPANY_DEPENDENT && !company_ids.Contains(x.CompanyId)).ToList();
           }
           engine.Db2Api.Context.ProductIdentifierDatas.DeleteAllOnSubmit(unlinked_pids);
           engine.Db2Api.Context.SubmitChanges();

            default_word_weights = default_word_weights.ToDictionary(x => x.Key.Trim().ToLower(), x => x.Value);

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

        public void Save()
        {
            company_ids2Company.Values.ToList().ForEach(x => x.Save());
        }

        public class Company
        {
            public Company(Configuration configuration, int company_id)
            {
                this.configuration = configuration;
                this.company_id = company_id;

                FhrCrawlerHost.Db2.ProductIdentifierData pid = configuration.engine.Db2Api.Context.ProductIdentifierDatas.Where(x => x.CompanyId == company_id).FirstOrDefault();
                if (pid == null)
                {
                    word_weights = new Dictionary<string, double>();
                    configuration.default_word_weights.Keys.ToList().ForEach(x => word_weights[x] = configuration.default_word_weights[x]);
                    foreach (string iw in configuration.default_ignored_words)
                        word_weights[iw] = -1;

                    synonyms = new Dictionary<string, string>();
                    configuration.default_synonyms.Keys.ToList().ForEach(x => synonyms[x] = configuration.default_synonyms[x]);
                }
                else
                {
                    //ignored_words = Cliver.Bot.SerializationRoutines.Json.Get<HashSet<string>>(pid.IgnoredWords);
                    word_weights = Cliver.Bot.SerializationRoutines.Json.Get<Dictionary<string, double>>(pid.WordWeights);
                    synonyms = Cliver.Bot.SerializationRoutines.Json.Get<Dictionary<string, string>>(pid.Synonyms);
                }
                ignored_words_regex = create_ignored_words_regex();
                synonyms_regex = create_synonyms_regex();
            }
            readonly int company_id;
            readonly Configuration configuration;
            
            public void Save()
            {
                //configuration.engine.Db2Api.RenewContext();                
                FhrCrawlerHost.Db2.ProductIdentifierData pid = configuration.engine.Db2Api.Context.ProductIdentifierDatas.Where(x => x.CompanyId == company_id).FirstOrDefault();
                if (pid == null)
                {
                    pid = new FhrCrawlerHost.Db2.ProductIdentifierData();
                    pid.CompanyId = company_id;
                    configuration.engine.Db2Api.Context.ProductIdentifierDatas.InsertOnSubmit(pid);
                }
                pid.WordWeights = Cliver.Bot.SerializationRoutines.Json.Get(word_weights);

                List<string> looped_synonyms = synonyms.Keys.Intersect(synonyms.Values).ToList();
                synonyms = synonyms.Where(x => !looped_synonyms.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                pid.Synonyms = Cliver.Bot.SerializationRoutines.Json.Get(synonyms);
                configuration.engine.Db2Api.Context.SubmitChanges();
            }

            public double GetWordWeight(string word)
            {
                word = GetSynonym(word);
                double weight = 0;
                if (!word_weights.TryGetValue(word, out weight))
                {
                    weight = 1;
                    Word w = configuration.engine.Words.Get(word);
                    //if (w.IsInDictionary)
                    //    weight *= 0.3;
                    weight *= (1 - w.Get(company_id).ProductFrequency(Field.Name));
                    weight *= (1 - w.Get(company_id).WordDensity(Field.Name));
                    weight *= 0.3 * (1 - w.Get(company_id).ProductFrequency(Field.Description));
                    weight *= 0.3 * (1 - w.Get(company_id).WordDensity(Field.Description));

                    if (Regex.IsMatch(word, @"\d"))
                        weight *= .7;
                    else
                        weight *= .1;

                    word_weights[word] = weight;
                }
                return weight;
            }

            internal string StripOfIgnoredWords(string text)
            {
                return ignored_words_regex.Replace(text, "");
            }
            Regex ignored_words_regex;

            Regex create_ignored_words_regex()
            {
                List<string> iws = word_weights.Where(x => x.Value < 0).Select(x => x.Key).ToList();
                return new Regex(@"(?<!\w)(" + iws.Aggregate((x, y) => x + "|" + Regex.Escape(y.Trim().ToLower())) + @")(?!\w)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                //return new Regex(@"(?<!\w)(" + ignored_words.Aggregate((x, y) => x + "|" + Regex.Escape(y.Trim().ToLower())) + @")(?!\w)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            Dictionary<string, double> word_weights;
            Dictionary<string, string> synonyms;

            public void SetWordWeight(string word, double weight)
            {
                word = word.Trim().ToLower();
                word = GetSynonym(word);
                if (weight < 0)
                {
                    word_weights[word] = -1;

                    ignored_words_regex = create_ignored_words_regex();
                    //UnSetSynonym(word);
                    return;
                }
                word_weights[word] = weight;
            }

            public void SetIgnoredWord(string word)
            {
                SetWordWeight(word, -1);
            }

            public void UnSetWord(string word)
            {
                word = GetSynonym(word);
                word = word.Trim().ToLower();
                word_weights.Remove(word);

                ignored_words_regex = create_ignored_words_regex();
            }

            internal string ReplaceWithSynonyms(string text)
            {
                //foreach (KeyValuePair<string, string> kvp in synonyms)
                //    text = Regex.Replace(text, @"(?<!\w)(" + kvp.Key + @")(?!\w)", kvp.Value, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                text = synonyms_regex.Replace(text, (Match m) => { 
                    return GetSynonym(m.Groups["Word"].Value); 
                });
                return text;
            }
            Regex synonyms_regex;

            Regex create_synonyms_regex()
            {
                return new Regex(@"(?<!\w)(?'Word'" + synonyms.Keys.Aggregate((x, y) => x + "|" + Regex.Escape(y)) + @")(?!\w)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            public string GetSynonym(string word)
            {
                string synonym;
                if (synonyms.TryGetValue(word, out synonym))
                    return synonym;
                return word;
            }

            public void SetSynonym(string word, string synonym)
            {
                word = word.Trim().ToLower();
                synonym = synonym.Trim().ToLower();
                if (word == synonym)
                    return;
                synonyms.Where(x => x.Value == word).ToList().ForEach(x => synonyms[x.Key] = synonym);
                synonyms[word] = synonym;

                synonyms_regex = create_synonyms_regex();
                //UnSetWord(word);
            }

            public void UnSetSynonym(string word)
            {
                word = word.Trim().ToLower();
                synonyms.Remove(word);

                synonyms_regex = create_synonyms_regex();
            }

            #region API for editing configuration

            public void GetConfigurationAsString(out string word_weights, out string ignored_words, out string synonyms)
            {
                List<string> wws = new List<string>();
                List<string> iws = new List<string>();
                foreach (var kv in this.word_weights)
                    if (kv.Value >= 0)
                        wws.Add(kv.Key + ">" + kv.Value);
                    else
                        iws.Add(kv.Key);
                word_weights = string.Join(" ", wws.OrderBy(x => x));
                ignored_words = string.Join(" ", iws.OrderBy(x => x));

                List<string> ss = new List<string>();
                foreach (var kv in this.synonyms)
                    ss.Add(kv.Key + ">" + kv.Value);
                synonyms = string.Join(" ", ss.OrderBy(x => x));
            }

            public void SetConfigurationFromString(string word_weights, string ignored_words, string synonyms)
            {
                this.word_weights.Clear();
                this.synonyms.Clear();

                for (Match m = Regex.Match(word_weights, @"(?'Word'[^\s>]+)\s*>\s*(?'Weight'[^\s>]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase); m.Success; m = m.NextMatch())
                    SetWordWeight(m.Groups["Word"].Value, double.Parse(m.Groups["Weight"].Value));

                for (Match m = Regex.Match(ignored_words, @"(?'Word'[^\s]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase); m.Success; m = m.NextMatch())
                    SetIgnoredWord(m.Groups["Word"].Value);

                for (Match m = Regex.Match(synonyms, @"(?'Word'[^\s>]+)\s*>\s*(?'Synonym'[^\s>]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase); m.Success; m = m.NextMatch())
                    SetSynonym(m.Groups["Word"].Value, m.Groups["Synonym"].Value);
            }

            #endregion
        }

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