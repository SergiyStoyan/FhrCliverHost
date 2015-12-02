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
    public partial class Configuration 
    {
        public class Company
        {
            public Company(Configuration configuration, int company_id)
            {
                this.configuration = configuration;
                this.company_id = company_id;

                word_weights = Cliver.Bot.DbSettings.Get<Dictionary<string, double>>(configuration.engine.Dbc, SettingsKey.SCOPE, SettingsKey.COMPANY + company_id + SettingsKey.WORD_WEIGHTS);
                if (word_weights == null)
                {
                    word_weights = new Dictionary<string, double>();
                    configuration.default_word_weights.Keys.ToList().ForEach(x => word_weights[x] = configuration.default_word_weights[x]);
                    foreach (string iw in configuration.default_ignored_words)
                        this.word_weights[iw] = -1;
                }

                synonyms = Cliver.Bot.DbSettings.Get<Dictionary<string, string>>(configuration.engine.Dbc, SettingsKey.SCOPE, SettingsKey.COMPANY + company_id + SettingsKey.SYNONYMS);
                if (synonyms == null)
                {
                    this.synonyms = new Dictionary<string, string>();
                    configuration.default_synonyms.Keys.ToList().ForEach(x => synonyms[x] = configuration.default_synonyms[x]);
                }

                ignored_words_regex = create_ignored_words_regex();
                synonyms_regex = create_synonyms_regex();
            }
            readonly int company_id;
            readonly Configuration configuration;

            public void SaveWordWeights()
            {
                Cliver.Bot.DbSettings.Save(configuration.engine.Dbc, SettingsKey.SCOPE, SettingsKey.COMPANY + company_id + SettingsKey.WORD_WEIGHTS, word_weights);
            }

            public void SaveSynonyms()
            {
                List<string> looped_synonyms = synonyms.Keys.Intersect(synonyms.Values).ToList();
                synonyms = synonyms.Where(x => !looped_synonyms.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                Cliver.Bot.DbSettings.Save(configuration.engine.Dbc, SettingsKey.SCOPE, SettingsKey.COMPANY + company_id + SettingsKey.SYNONYMS, synonyms);
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
                    weight *= (1 - w.Get(company_id).ProductFrequency(Field.Category));
                    weight *= (1 - w.Get(company_id).WordDensity(Field.Category));
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

            public void SetWordWeightsFromString(string word_weights, string ignored_words)
            {
                this.word_weights.Clear();
                for (Match m = Regex.Match(word_weights, @"(?'Word'[^\s>]+)\s*>\s*(?'Weight'[^\s>]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase); m.Success; m = m.NextMatch())
                    SetWordWeight(m.Groups["Word"].Value, double.Parse(m.Groups["Weight"].Value));

                for (Match m = Regex.Match(ignored_words, @"(?'Word'[^\s]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase); m.Success; m = m.NextMatch())
                    SetIgnoredWord(m.Groups["Word"].Value);
            }

            public void SetSynonymsFromString(string synonyms)
            {
                this.synonyms.Clear();
                for (Match m = Regex.Match(synonyms, @"(?'Word'[^\s>]+)\s*>\s*(?'Synonym'[^\s>]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase); m.Success; m = m.NextMatch())
                    SetSynonym(m.Groups["Word"].Value, m.Groups["Synonym"].Value);
            }

            #endregion
        }
    }
}