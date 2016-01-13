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
using Cliver.Fhr;

namespace Cliver.ProductIdentifier
{
    public partial class Engine
    {
        public Engine(bool auto_data_analysing)
        {
            AutoDataAnalysing = auto_data_analysing;
            this.Db = Fhr.ProductOffice.Models.DbApi.Create();
            Dbc = Bot.DbConnection.Create(Fhr.ProductOffice.Models.DbApi.GetProviderConnectionString());
            Companies = new Companies(this);
            Products = new Products(this);
            CompanyPairs = new ProductIdentifier.CompanyPairs(this);

            initialize_settings();
        }

        public readonly Companies Companies;
        internal readonly Products Products;
        public readonly CompanyPairs CompanyPairs;
        internal readonly Cliver.Fhr.ProductOffice.Models.DbApi Db;
        internal readonly Cliver.Bot.DbConnection Dbc;
        internal readonly bool AutoDataAnalysing;

        //public void RenewConfiguration(bool auto_data_analysing)
        //{
        //    Configuration_ = new Configuration(this, auto_data_analysing);
        //}

        public List<ProductLink> CreateProductLinkList(int[] product1_ids, int company2_id/*, string[] keyword2s = null*/)
        {
            lock (this)
            {
                if (product1_ids.Length < 1)
                    throw new Exception("product1_ids is empty");

                Fhr.ProductOffice.Models.Product p1 = Db.Products.Where(p => product1_ids.Contains(p.Id) && p.CompanyId == company2_id).FirstOrDefault();
                if (p1 != null)
                    throw new Exception("Product Id:" + p1.Id + " already belongs to company Id:" + p1.CompanyId + " " + p1.Company.Name + " so no more link can be found.");

                //List<int> cis = (from x in Db.Products.Where(p => product1_ids.Contains(p.Id)) join y in Db.Products on x.LinkId equals y.LinkId select y.CompanyId).ToList();
                //cis.Add(company2_id);
                //HashSet<int> cis_ = new HashSet<int>(cis);
                //foreach (int company_id in cis_)
                //{
                //    Configuration.Company c = Configuration.Get(company_id);
                //    if (c.IsDataAnalysisRequired())
                //    {
                //        PerformDataAnalysis(company_id);
                //        c = new Company(this, company_id);
                //        company_ids2Company[company_id] = c;
                //    }
                //}

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
                //sw1.Start();
                List<int> company2_link_ids = new List<int>();
                IDataReader dr = Dbc["SELECT LinkId FROM Products WHERE CompanyId=@CompanyId AND LinkId>0 GROUP BY LinkId"].GetReader("@CompanyId", company2_id);
                //it's faster than LINQ
                while (dr.Read())
                    company2_link_ids.Add((int)dr[0]);
                dr.Close();
                pls = (from x in company2_link_ids select new ProductLink(this, product1s, Products.GetLinked(x))).ToList();
                List<int> free_product_ids = (from p in Companies.Get(company2_id).DbCompany.Products where p.LinkId == null || p.LinkId <= 0 select p.Id).ToList();
                List<ProductLink> pls2 = (from x in free_product_ids select new ProductLink(this, product1s, new Product[] { Products.Get(x) })).ToList();
                pls.AddRange(pls2);
                //}
                //pls = pls.OrderByDescending(x => x.Score).OrderByDescending(x => x.SecondaryScore).ToList();
                pls = pls.OrderByDescending(x => x.Score).ToList();
                //sw1.Stop();
                //string s = "1: " + sw1.ElapsedMilliseconds + ", 2: " + sw2.ElapsedMilliseconds + ", 3: " + sw3.ElapsedMilliseconds + ", 4: " + sw4.ElapsedMilliseconds
                //    + ", 5: " + sw5.ElapsedMilliseconds + ", 6: " + sw6.ElapsedMilliseconds + ", 7: " + sw7.ElapsedMilliseconds
                //    + ", 8: " + sw8.ElapsedMilliseconds + ", 9: " + sw9.ElapsedMilliseconds + ", 10: " + sw10.ElapsedMilliseconds;
                return pls;
            }
        }
        //public int c1 = 0;
        //public int c2 = 0;
        //public System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
        //public System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();
        //public System.Diagnostics.Stopwatch sw3 = new System.Diagnostics.Stopwatch();
        //public System.Diagnostics.Stopwatch sw4 = new System.Diagnostics.Stopwatch();
        //public System.Diagnostics.Stopwatch sw5 = new System.Diagnostics.Stopwatch();
        //public System.Diagnostics.Stopwatch sw6 = new System.Diagnostics.Stopwatch();
        //public System.Diagnostics.Stopwatch sw7 = new System.Diagnostics.Stopwatch();
        //public System.Diagnostics.Stopwatch sw8 = new System.Diagnostics.Stopwatch();
        //public System.Diagnostics.Stopwatch sw9 = new System.Diagnostics.Stopwatch();
        //public System.Diagnostics.Stopwatch sw10 = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// Valid LinkId is > 0
        /// If a product is not linked, its LinkId == null or < 0 (LinkId may be -Id to differ from all other LinkId's);
        /// </summary>
        /// <param name="dbc"></param>
        /// <returns></returns>
        static int get_minimal_free_link_id(Cliver.Bot.DbConnection dbc)
        {
            int? link_id = (int?)dbc[@"SELECT MIN(a.LinkId + 1)
FROM (SELECT LinkId FROM Products WHERE LinkId>0) a LEFT OUTER JOIN (SELECT LinkId FROM Products WHERE LinkId>0) b ON (a.LinkId + 1 = b.LinkId)
WHERE b.LinkId IS NULL"].GetSingleValue();
            if (link_id == null)
                link_id = 1;
            return (int)link_id;
        }

        public void SaveLink(int[] product_ids)
        {
            lock (this)
            {
                product_ids = product_ids.Distinct().ToArray();

                update_category_mapping(product_ids);

                int link_id = get_minimal_free_link_id(Dbc);

                //Fhr.ProductOffice.ProductOfficeDataContext db = DbApi.RenewContext();
                Dictionary<int, int> company_ids2product_id = new Dictionary<int, int>();
                foreach (int product_id in product_ids)
                {
                    Fhr.ProductOffice.Models.Product p = Db.Products.Where(r => r.Id == product_id).FirstOrDefault();
                    if (p == null)
                        continue;
                    if (company_ids2product_id.ContainsKey(p.CompanyId))
                        throw new Exception("Products with Id: " + p.Id + " and " + company_ids2product_id[p.CompanyId] + " belong to the same company: " + p.Company.Name + " and so cannot be linked");
                    company_ids2product_id[p.CompanyId] = p.Id;
                    p.LinkId = link_id;
                }
                Db.Configuration.ValidateOnSaveEnabled = false;
                Db.SaveChanges();
            }
        }

        void update_category_mapping(int[] product_ids)
        {
            foreach (int pi in product_ids)
            {
                Product product1 = Products.Get(pi);
                if (product1.DbProduct.LinkId == null || product1.DbProduct.LinkId < 0)
                    continue;
                var p1_category_ps = Db.Products.Where(p => p.Category == product1.DbProduct.Category && p.Id != pi).ToList();
                var p1_old_linked_ps = (from x in Db.Products where x.LinkId == product1.DbProduct.LinkId && !product_ids.Contains(x.Id) select x).ToList();
                foreach (Fhr.ProductOffice.Models.Product p1_old_linked_p in p1_old_linked_ps)
                {
                    if (null == (from x in p1_category_ps join y in Db.Products.Where(p => p.Category == p1_old_linked_p.Category && p.Id != p1_old_linked_p.Id) on x.LinkId equals y.LinkId select y).FirstOrDefault())
                        //remove mapping 
                        CompanyPairs.UnmapCategoriesAndSave(product1, Products.Get(p1_old_linked_p.Id));
                }
            }

            for (int i = 0; i < product_ids.Length; i++)
                for (int j = i + 1; j < product_ids.Length; j++)
                {
                    Product p1 = Products.Get(product_ids[i]);
                    Product p2 = Products.Get(product_ids[j]);
                    CompanyPairs.MapCategoriesAndSave(p1, p2);
                }
        }
    }
}