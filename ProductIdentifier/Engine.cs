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
    public class Engine
    {
        public Engine()
        {
            this.Db2Api = new Cliver.FhrCrawlerHost.Db2Api();
            Configuration = new Configuration(this);
            Companies = new Companies(this);
            Products = new Products(this);
            Words = new Words(this);
        }

        public readonly Configuration Configuration;
        internal readonly Companies Companies;
        internal readonly Products Products;
        internal readonly Words Words;
        internal readonly Cliver.FhrCrawlerHost.Db2Api Db2Api;

        public List<ProductLink> CreateProductLinkList(int[] product1_ids, int company2_id/*, string[] keyword2s = null*/)
        {
            FhrCrawlerHost.Db2.Product p1 = Db2Api.Context.Products.Where(p => product1_ids.Contains(p.Id) && p.CompanyId == company2_id).FirstOrDefault();
            if (p1 != null)
                throw new Exception("Product Id:" + p1.Id + " already belongs to company Id:" + p1.CompanyId + " " + p1.Company.Name + " so no more link can be found.");

            Product[] product1s = (from x in product1_ids select Products.Get(x)).ToArray();
            List<ProductLink> pls;
            //if (keyword2s != null && keyword2s.Length > 0)
            //{
            //    keyword2s = keyword2s.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim().ToLower()).ToArray();
            //    HashSet<int> product2_ids = null;
            //    foreach (string keyword2 in keyword2s)
            //    {
            //        HashSet<int> p2_ids = Company.Get(company2_id).Words2ProductIds(Field.Name)[keyword2];
            //        if (product2_ids == null)
            //            product2_ids = p2_ids;
            //        else
            //            product2_ids = (HashSet<int>)product2_ids.Intersect(p2_ids);
            //    }

            //    List<int> link_ids = (from p2_id in product2_ids let link_id = Product.Get(p2_id).DbProduct.LinkId where link_id > 0 group link_id by link_id into g select (int)g.Key).ToList();
            //    pls = (from x in link_ids select new ProductLink(product1s, Product.GetLinked(x))).ToList();

            //    List<int> free_product_ids = (from p2_id in product2_ids let link_id = Product.Get(p2_id).DbProduct.LinkId where link_id == null || link_id <= 0 select p2_id).ToList();
            //    List<ProductLink> pls2 = (from x in free_product_ids select new ProductLink(product1s, new Product[] { Product.Get(x) })).ToList();
            //    pls.AddRange(pls2);
            //}
            //else
            //{
            List<int> link_ids = (from p in Companies.Get(company2_id).DbCompany.Products where p.LinkId > 0 group p by p.LinkId into g select (int)g.Key).ToList();
            pls = (from x in link_ids select new ProductLink(this, product1s, Products.GetLinked(x))).ToList();

            List<int> free_product_ids = (from p in Companies.Get(company2_id).DbCompany.Products where p.LinkId == null || p.LinkId <= 0 select p.Id).ToList();
            List<ProductLink> pls2 = (from x in free_product_ids select new ProductLink(this, product1s, new Product[] { Products.Get(x) })).ToList();
            pls.AddRange(pls2);
            //}
            pls = pls.OrderByDescending(x => x.Score).OrderByDescending(x => x.SecondaryScore).ToList();
            return pls;
        }

        static int get_minimal_free_link_id(Cliver.Bot.DbConnection dbc)
        {
            int link_id = 1;
            if (null != dbc.Get(@"SELECT LinkId FROM Products WHERE LinkId=@LinkId").GetSingleValue("@LinkId", link_id))
                link_id = (int)dbc.Get(@"SELECT MIN(a.LinkId + 1)
FROM (SELECT LinkId FROM Products WHERE LinkId>0) a LEFT OUTER JOIN (SELECT LinkId FROM Products WHERE LinkId>0) b ON (a.LinkId + 1 = b.LinkId)
WHERE b.LinkId IS NULL").GetSingleValue();
            return link_id;
        }

        public void SaveLink(int[] product_ids)
        {
            product_ids = product_ids.Distinct().ToArray();
            
            int link_id = get_minimal_free_link_id(Db2Api.Connection);

            //FhrCrawlerHost.Db2.ProductOfficeDataContext db = Db2Api.RenewContext();
            Dictionary<int, int> company_ids2product_id = new Dictionary<int, int>();
            foreach (int product_id in product_ids)
            {
                FhrCrawlerHost.Db2.Product p = Db2Api.Context.Products.Where(r => r.Id == product_id).FirstOrDefault();
                if (p == null)
                    continue;
                if (company_ids2product_id.ContainsKey(p.CompanyId))
                    throw new Exception("Products with Id: " + p.Id + " and " + company_ids2product_id[p.CompanyId] + " belong to the same company: " + p.Company.Name + " and so cannot be linked");
                company_ids2product_id[p.CompanyId] = p.Id;
                p.LinkId = link_id;
            }
            Db2Api.Context.SubmitChanges();

            self_training_analysis(product_ids);
        }

        void self_training_analysis(int[] linked_product_ids)
        {
            for (int i = 0; i < linked_product_ids.Length; i++)
                for (int j = i + 1; j < linked_product_ids.Length; j++)
                {
                    int product1_id = linked_product_ids[i];
                    int product2_id = linked_product_ids[j];
                    Product product1 = Products.Get(product1_id);
                    Product product2 = Products.Get(product2_id);
                    Dictionary<Field, HashSet<string>> matched_words = new Dictionary<Field, HashSet<string>>();
                    matched_words[Field.Name] = new HashSet<string>();
                    foreach (string word in product1.Words(Field.Name))
                        if (product2.Words2Count(Field.Name).ContainsKey(word))
                            matched_words[Field.Name].Add(word);

                    List<ProductLink> pls = create_identical_Product_list_for_training(product1_id, product2.DbProduct.CompanyId);
                    foreach (ProductLink pl in pls)
                    {
                        if (null != pl.Product2s.Where(x => x.DbProduct.Id == product2_id).FirstOrDefault())
                            break;
                        foreach (Product p2 in pl.Product2s)
                        {
                            if (product1_id == p2.DbProduct.Id)
                                continue;
                            Dictionary<Field, List<string>> mws = pl.Get(product1_id, p2.DbProduct.Id).MatchedWords;
                            List<string> week_mws = mws[Field.Name].Where(x => !matched_words[Field.Name].Contains(x)).ToList();
                            foreach (string word in week_mws)
                            {
                                Configuration.Get(product1).SetWordWeight(word, 0.9 * Configuration.Get(product1).GetWordWeight(word));
                                Configuration.Get(product2).SetWordWeight(word, 0.9 * Configuration.Get(product2).GetWordWeight(word));
                            }
                        }
                    }
                }
            Configuration.Save();
        }

        List<ProductLink> create_identical_Product_list_for_training(int product1_id, int company2_id)
        {
            List<ProductLink> pls = (from x in Companies.Get(company2_id).DbCompany.Products select new ProductLink(this, Products.Get(product1_id), Products.Get(x.Id))).ToList();
            pls = pls.OrderByDescending(x => x.Score).OrderByDescending(x => x.SecondaryScore).ToList();
            return pls;
        }
    }
}