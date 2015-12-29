﻿using System;
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
    public partial class Company
    {
        void initialize_settings()
        {
            word_weights = Cliver.Bot.DbSettings.Get<Dictionary<string, double>>(engine.Dbc, SettingsKey.SCOPE, SettingsKey.COMPANY + DbCompany.Id + SettingsKey.WORD_WEIGHTS);
            if (word_weights == null)
            {
                word_weights = new Dictionary<string, double>();
                //configuration.default_word_weights.Keys.ToList().ForEach(x => word_weights[x] = configuration.default_word_weights[x]);
                foreach (string iw in engine.DefaultIgnoredWords)
                    this.word_weights[iw] = -1;
            }

            synonyms = Cliver.Bot.DbSettings.Get<Dictionary<string, string>>(engine.Dbc, SettingsKey.SCOPE, SettingsKey.COMPANY + DbCompany.Id + SettingsKey.SYNONYMS);
            if (synonyms == null)
            {
                this.synonyms = new Dictionary<string, string>();
                engine.DefaultSynonyms.Keys.ToList().ForEach(x => synonyms[x] = engine.DefaultSynonyms[x]);
            }

            ignored_words_regex = create_ignored_words_regex();
            synonyms_regex = create_synonyms_regex();
        }

        internal bool IsDataAnalysisRequired()
        {
            DateTime t = Cliver.Bot.DbSettings.Get<DateTime>(engine.Dbc, Cliver.ProductIdentifier.SettingsKey.SCOPE, Cliver.ProductIdentifier.SettingsKey.COMPANY + DbCompany.Id + Cliver.ProductIdentifier.SettingsKey.ANALYSIS_TIME);
            return t == null || t <= engine.Db.Products.Max(p => p.UpdateTime).Value;
        }

        public void SaveWordWeights()
        {
            Cliver.Bot.DbSettings.Save(engine.Dbc, SettingsKey.SCOPE, SettingsKey.COMPANY + DbCompany.Id + SettingsKey.WORD_WEIGHTS, word_weights);
        }

        public void SaveSynonyms()
        {
            List<string> looped_synonyms = synonyms.Keys.Intersect(synonyms.Values).ToList();
            synonyms = synonyms.Where(x => !looped_synonyms.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            Cliver.Bot.DbSettings.Save(engine.Dbc, SettingsKey.SCOPE, SettingsKey.COMPANY + DbCompany.Id + SettingsKey.SYNONYMS, synonyms);
        }

        internal double GetWordWeight(string word)
        {
            word = GetSynonym(word);
            double weight = 0;
            if (!word_weights.TryGetValue(word, out weight))
            {
                //if (w.IsInDictionary)
                //    weight *= 0.5;

                int containing_category_count = 0;
                foreach (string c in Categories)
                    if (NormalizedCategory(c).Contains(word))
                        containing_category_count++;
                double category_frequency = (double)containing_category_count / Categories.Count;

                weight =
                    1 * (containing_category_count > 0 && Regex.IsMatch(word, @"\d") ? 1 : 0) +
                    30 * category_frequency +
                    10 * WordProductFrequency(Field.Name, word) +
                    10 * (1 - WordDensity(Field.Name, word)) +
                    10 * (1 - WordProductFrequency(Field.Description, word)) +
                    10 * (1 - WordDensity(Field.Description, word));

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

        internal void SetWordWeight(string word, double weight)
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

        internal void SetIgnoredWord(string word)
        {
            SetWordWeight(word, -1);
        }

        internal void UnSetWord(string word)
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

            text = synonyms_regex.Replace(text, (Match m) =>
            {
                return GetSynonym(m.Groups["Word"].Value);
            });
            return text;
        }
        Regex synonyms_regex;

        Regex create_synonyms_regex()
        {
            return new Regex(@"(?<!\w)(?'Word'" + synonyms.Keys.Aggregate((x, y) => x + "|" + Regex.Escape(y)) + @")(?!\w)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        internal string GetSynonym(string word)
        {
            string synonym;
            if (synonyms.TryGetValue(word, out synonym))
                return synonym;
            return word;
        }

        internal void SetSynonym(string word, string synonym)
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

        internal void UnSetSynonym(string word)
        {
            word = word.Trim().ToLower();
            synonyms.Remove(word);

            synonyms_regex = create_synonyms_regex();
        }

        #region API for data analysis

        internal void PrepareForDataAnalysis()
        {
            Dictionary<string, double> wws = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> w2w in word_weights)
                if (w2w.Value < 0)
                    wws[w2w.Key] = w2w.Value;
            word_weights = wws;
        }

        internal void DefineWordWeight(string word)
        {
            GetWordWeight(word);
        }

        internal void SaveAfterDataAnalysis()
        {
            SaveWordWeights();
            Cliver.Bot.DbSettings.Save(engine.Dbc, SettingsKey.SCOPE, SettingsKey.COMPANY + DbCompany.Id + SettingsKey.ANALYSIS_TIME, DateTime.Now);
        }

        #endregion

        #region API for editing configuration

        public string GetWordWeightsAsString()
        {
            List<string> wws = new List<string>();
            foreach (var kv in this.word_weights)
                if (kv.Value >= 0)
                    wws.Add(kv.Key + ">" + kv.Value);
            return string.Join(" ", wws.OrderBy(x => x));
        }

        public string GetIgnoredWordsAsString()
        {
            List<string> iws = new List<string>();
            foreach (var kv in this.word_weights)
                if (kv.Value < 0)
                    iws.Add(kv.Key);
            return string.Join(" ", iws.OrderBy(x => x));
        }

        public string GetSynonymsAsString()
        {
            List<string> ss = new List<string>();
            foreach (var kv in this.synonyms)
                ss.Add(kv.Key + ">" + kv.Value);
            return string.Join(" ", ss.OrderBy(x => x));
        }

        public void SetWordWeightsFromString(string word_weights)
        {
            Dictionary<string, double> wws = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> w2w in this.word_weights)
                if (w2w.Value < 0)
                    wws[w2w.Key] = w2w.Value;
            this.word_weights = wws;

            for (Match m = Regex.Match(word_weights, @"(?'Word'[^\s>]+)\s*>\s*(?'Weight'[^\s>]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase); m.Success; m = m.NextMatch())
            {
                string word = m.Groups["Word"].Value;
                double weight = 0;
                if (this.word_weights.TryGetValue(word, out weight))
                    if (weight < 1)
                        continue;
                SetWordWeight(word, double.Parse(m.Groups["Weight"].Value));
            }
        }

        public void SetIgnoredWordsFromString(string ignored_words)
        {
            Dictionary<string, double> wws = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> w2w in this.word_weights)
                if (w2w.Value >= 0)
                    wws[w2w.Key] = w2w.Value;
            this.word_weights = wws;

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