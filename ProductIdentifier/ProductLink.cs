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
    public class ProductLink
    {
        public class PairStatistics
        {
            public Dictionary<Field, List<string>> MatchedWords = new Dictionary<Field, List<string>>();
            public double Score = 0;

            public PairStatistics(Engine engine, Product product1, Product product2)
            {
                this.engine = engine;

                Dictionary<string, double> word2score = new Dictionary<string, double>();
                foreach (string word in product1.Words(Field.Name))
                {
                    if (product2.Words2Count(Field.Name).ContainsKey(word))
                    {
                        Word w = engine.Words.Get(word);
                        word2score[word] = w.Get(product1.DbProduct.CompanyId).Weight * w.Get(product2.DbProduct.CompanyId).Weight;
                    }
                }
                MatchedWords[Field.Name] = word2score.Keys.ToList();
                if (word2score.Count < 1)
                    return;
                Score = ((double)word2score.Values.Sum() / word2score.Count)
                    * ((double)word2score.Count / product1.Words(Field.Name).Count)
                    * ((double)word2score.Count / product2.Words(Field.Name).Count);

                //    decimal p1 = product1.DbProduct.Price;

                //HashSet<string> matched_words = new HashSet<string>();
                //foreach (string word in product1.Words(Field.Description))
                //{
                //    if (product2.Word2Count(Field.Description).ContainsKey(word))
                //        matched_words.Add(word);
                //}
                //foreach (string word in matched_words)
                //{
                //    double score = get_word_score(word, product1, product2);
                //    SecondaryScore += score;
                //}
            }
            readonly Engine engine;
        }

        public PairStatistics Get(int product1_id, int product2_id)
        {
            PairStatistics ps = null;
            product1_id_product2_id_s2PairStatistics.TryGetValue(new IdPair { Id1 = product1_id, Id2 = product2_id }, out ps);
            return ps;
        }

        public PairStatistics GetByCompanies(int company1_id, int company2_id)
        {
            Product p1 = Product1s.Where(x => x.DbProduct.CompanyId == company1_id).FirstOrDefault();
            if (p1 == null)
                throw new Exception("No PairStatistics found for company Id=" + company1_id);
            Product p2 = Product2s.Where(x => x.DbProduct.CompanyId == company2_id).FirstOrDefault();
            if (p2 == null)
                throw new Exception("No PairStatistics found for company Id=" + company2_id);
            PairStatistics ps = null;
            product1_id_product2_id_s2PairStatistics.TryGetValue(new IdPair { Id1 = p1.DbProduct.Id, Id2 = p1.DbProduct.Id }, out ps);
            return ps;
        }

        public void Set(int product1_id, int product2_id, PairStatistics ps)
        {
            product1_id_product2_id_s2PairStatistics[new IdPair { Id1 = product1_id, Id2 = product2_id }] = ps;
        }
        Dictionary<IdPair, PairStatistics> product1_id_product2_id_s2PairStatistics = new Dictionary<IdPair, PairStatistics>();

        public struct IdPair
        {
            public int Id1;
            public int Id2;
        }

        public readonly double Score = 0;

        public double SecondaryScore
        {
            get
            {
                return 0;
            }
        }

        public readonly Product[] Product1s;
        public readonly Product[] Product2s;

        readonly List<int> MutualProductIds = new List<int>();
        readonly List<int> MutualCompanyIds;

        //public readonly double LogicalScore;

        public ProductLink(Engine engine, Product[] product1s, Product[] product2s)
        {
            this.Engine = engine;
            Product1s = product1s;
            Product2s = product2s;

            MutualCompanyIds = (from p1 in product1s join p2 in product2s on p1.DbProduct.CompanyId equals p2.DbProduct.CompanyId where p1 != null && p2 != null select p1.DbProduct.CompanyId).ToList();
            if (MutualCompanyIds.Count > 0)
            {
                MutualProductIds = (from p1 in product1s join p2 in product2s on p1.DbProduct.Id equals p2.DbProduct.Id where p1 != null && p2 != null select p1.DbProduct.Id).ToList();
                if (MutualProductIds.Count < MutualCompanyIds.Count)//chains contain different products of the same company so cannot be linked
                    Score = -1.1;
                else  //chains contain the same products so should be considered linked
                    Score = 1.1;
                return;
            }

            foreach (Product product1 in Product1s)
                foreach (Product product2 in Product2s)
                    Set(product1.DbProduct.Id, product2.DbProduct.Id, new PairStatistics(engine, product1, product2));

            //word order
            //word density

            Score = (double)product1_id_product2_id_s2PairStatistics.Values.Select(x => x.Score).Sum() / product1_id_product2_id_s2PairStatistics.Count;
        }
        readonly public Engine Engine;

        /// <summary>
        /// used for self-training
        /// </summary>
        /// <param name="product1"></param>
        /// <param name="product2"></param>
        public ProductLink(Engine engine, Product product1, Product product2)
        {
            this.Engine = engine;
            Product1s = new Product[] { product1 };
            Product2s = new Product[] { product2 };

            if (product1.DbProduct.Id != product2.DbProduct.Id)
                Set(product1.DbProduct.Id, product2.DbProduct.Id, new PairStatistics(engine, product1, product2));

            //word order
            //word density

            Score = (double)product1_id_product2_id_s2PairStatistics.Values.Select(x => x.Score).Sum() / product1_id_product2_id_s2PairStatistics.Count;
        }
    }
}
