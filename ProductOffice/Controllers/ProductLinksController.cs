using System;
using System.Collections.Generic;
//using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cliver.FhrApi.ProductOffice.Models;
using System.Text.RegularExpressions;

namespace Cliver.ProductOffice.Controllers
{
    [HandleActionError]
    [Authorize]
    public class ProductLinksController : Controller
    {
        private DbApi db = FhrApi.ProductOffice.Models.DbApi.Create();

        List<object> CompanySelect
        {
            get
            {
                var cs = (from r in db.Companies select new { Value = r.Id, Name = r.Name }).ToList<object>();
                cs.Insert(0, new { Value = -1, Name = "-- ALL --" });
                return cs;
            }
        }
        
        public ActionResult Index()
        {
            ViewBag.Companies = db.Companies;
            ViewBag.CompanyId = new SelectList(CompanySelect, "Value", "Name");

            //List<int> invisible_column_ids = new List<int>();
            //int i = -2;
            //foreach (var c in db.Companies)
            //    invisible_column_ids.Add(i += 2);
            //ViewBag.InvisibleColumnIds = "[" + string.Join(", ", invisible_column_ids) + "]";

            return View(db.Companies);
        }

        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            //            string sql = @"SELECT
            //CASE WHEN a.LinkId>=0 THEN a.LinkId END AS LinkId,"
            //                + string.Join(",", (from c in db.Companies select "MAX([" + c.Name + "_ProductId]) AS [" + c.Name + "_ProductId], MAX([" + c.Name + "_ProductName]) AS [" + c.Name + "_ProductName]"))
            //                + @" INTO #ProductLinks FROM (
            //SELECT
            //CASE WHEN LinkId>0 THEN LinkId ELSE -Id END AS LinkId,"
            //                + string.Join(",", (from c in db.Companies select "CASE WHEN CompanyId = " + c.Id + " THEN Id END AS [" + c.Name + "_ProductId], CASE WHEN CompanyId = " + c.Id + " THEN Name END AS [" + c.Name + "_ProductName]"))
            //                + @" FROM Products
            //) a
            //GROUP BY a.LinkId";
            string sql = @"SELECT
LinkId,"
                + string.Join(",", (from c in db.Companies select "MAX([" + c.Name + "_ProductName]) AS [" + c.Name + "_ProductName]"))
                + @" INTO #ProductLinks FROM (
SELECT
CASE WHEN LinkId>0 THEN LinkId ELSE -Id END AS LinkId,"
                + string.Join(",", (from c in db.Companies select "CASE WHEN CompanyId = " + c.Id + " THEN Name END AS [" + c.Name + "_ProductName]"))
                + @" FROM Products
) a
GROUP BY a.LinkId";
            //SELECT 
            //CASE WHEN a.LinkId>=0 THEN a.LinkId END AS LinkId, MAX(A) AS A, MAX(B) AS B
            //FROM (
            //    SELECT
            //    CASE WHEN LinkId>0 THEN LinkId ELSE -Id END AS LinkId, Id AS A, NULL AS B
            //    FROM Products
            //    WHERE CompanyId=1
            //    UNION
            //    SELECT
            //    CASE WHEN LinkId>0 THEN LinkId ELSE -Id END AS LinkId, NULL, Id
            //    FROM Products
            //    WHERE CompanyId=2
            //) a
            //GROUP BY a.LinkId

            //SELECT 
            //CASE WHEN a.LinkId>=0 THEN a.LinkId END AS LinkId, MAX(A_ProductId) AS A_ProductId, MAX(A_ProductName) AS A_ProductName, MAX(B_ProductId) AS B_ProductId, MAX(B_ProductName) AS B_ProductName
            //FROM (
            //    SELECT
            //    CASE WHEN LinkId>0 THEN LinkId ELSE -Id END AS LinkId, Id AS A_ProductId, Name AS A_ProductName, NULL AS B_ProductId, NULL AS B_ProductName
            //    FROM Products
            //    WHERE CompanyId=1
            //    UNION
            //    SELECT
            //    CASE WHEN LinkId>0 THEN LinkId ELSE -Id END AS LinkId, NULL, NULL, Id, Name
            //    FROM Products
            //    WHERE CompanyId=2
            //) a
            //GROUP BY a.LinkId

            //SELECT MAX(a.COUNT_) FROM (SELECT COUNT(Id) AS COUNT_, LinkId FROM Products WHERE LinkId>0 GROUP BY LinkId) a
            //SELECT * FROM Products WHERE LinkId=(SELECT CASE WHEN LinkId>0 THEN LinkId ELSE -1 END FROM Products WHERE Id=1);

            Cliver.Bot.DbConnection dbc = Cliver.Bot.DbConnection.CreateFromNativeConnection(db.Database.Connection);
            dbc.Get(sql).Execute();

            List<JqueryDataTable.Field> fields = new List<JqueryDataTable.Field> { new JqueryDataTable.Field("LinkId", false) };
            fields.AddRange(from c in db.Companies.ToList() select new JqueryDataTable.Field("[" + c.Name + "_ProductName]", true, 0, null));
            return JqueryDataTable.Index(request, db.Database.Connection, "FROM #ProductLinks", fields.ToArray());
        }

        public ActionResult Edit([Bind(Prefix = "id")]int? link_id)
        {//link_id can also be negative in which case it contains id of the first product
            if (link_id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            List<Product> products;
            if (link_id <= 0)//no link yet exists so link_id is ProductId
                products = db.Products.Where(p => p.Id == -link_id).ToList();
            else
                products = db.Products.Where(p => p.LinkId == link_id).ToList();
            ViewBag.LinkId = link_id > 0 ? link_id : -products[0].Id;
            ViewBag.Companies = db.Companies;
            ViewBag.LinkedProductsJson = Cliver.Bot.SerializationRoutines.Json.Get(get_product_objects(products));
            ViewBag.CATEGORY_SEPARATOR = FhrApi.ProductOffice.DataApi.Product.CATEGORY_SEPARATOR;
            if (Request.IsAjaxRequest())
                return PartialView(products);
            return View();
        }

        object get_product_objects(List<Product> products)
        {
            if (products == null)
                return null;

            List<object> ds = new List<object>();
            foreach (Product p in products)
            {
                dynamic d = new
                {
                    Id = p.Id,
                    CompanyId = p.CompanyId,
                    ExternalId = p.ExternalId,
                    CreateTime = p.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ModifyTime = p.ModifyTime != null ? ((DateTime)p.ModifyTime).ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdateTime = p.UpdateTime != null ? ((DateTime)p.UpdateTime).ToString("yyyy-MM-dd HH:mm:ss") : null,
                    Name = p.Name,
                    Url = p.Url,
                    ImageUrls = p.ImageUrls != null ? p.ImageUrls.Split('\n') : new string[] { },
                    Sku = p.Sku,
                    Category = p.Category,
                    Description = p.Description,
                    LinkId = p.LinkId != null ? p.LinkId : -p.Id,
                    Source = p.Source,
                    Stock = p.Stock,
                    CompanyName = p.Company.Name,
                    Price = (from r in p.Prices orderby r.Time descending select r.Currency.Name + "'" + r.Currency.Symbol + "'" + r.Value).FirstOrDefault()
                };
                d = Cliver.PrepareField.Html.GetDbObject(d);
                ds.Add(d);
            }
            return ds;
        }        

        class IdenticalProductList
        {
            readonly List<Cliver.ProductIdentifier.ProductLink> product_links;
            readonly public int[] Product1Ids;
            readonly public int Company2Id;
            readonly Cliver.ProductIdentifier.Engine engine;

            public List<Cliver.ProductIdentifier.ProductLink> CurrentProductLinks
            {
                get { return filtered_product_links; }
            }
            List<Cliver.ProductIdentifier.ProductLink> filtered_product_links = null;

            public static IdenticalProductList GetMovingToNextRange(ProductLinksController controller, int[] product1_ids, int company2_id, string[] keyword2s, string category2, int range_size, bool forward, out bool is_CurrentProductLinks_changed)
            {
                product1_ids = product1_ids.Distinct().ToArray();

                IdenticalProductList ipl = get_from_session(controller, product1_ids, company2_id);
                ipl.set_pls_filtered(keyword2s, category2);
                is_CurrentProductLinks_changed = ipl.CurrentProductLinkRangeStartIndex == INITIAL_INDEX && ipl.CurrentProductLinkRangeEndIndex == INITIAL_INDEX;

                if (forward)
                {
                    ipl.CurrentProductLinkRangeStartIndex = ipl.CurrentProductLinkRangeEndIndex + 1;
                    ipl.CurrentProductLinkRangeEndIndex = ipl.CurrentProductLinkRangeStartIndex + range_size - 1;
                    if (ipl.CurrentProductLinkRangeEndIndex >= ipl.CurrentProductLinks.Count)
                        ipl.CurrentProductLinkRangeEndIndex = ipl.CurrentProductLinks.Count - 1;
                    if (ipl.CurrentProductLinkRangeStartIndex >= ipl.CurrentProductLinks.Count)
                        ipl.CurrentProductLinkRangeStartIndex = ipl.CurrentProductLinks.Count;
                }
                else
                {
                    ipl.CurrentProductLinkRangeEndIndex = ipl.CurrentProductLinkRangeStartIndex - 1;
                    ipl.CurrentProductLinkRangeStartIndex = ipl.CurrentProductLinkRangeEndIndex - range_size + 1;
                    if (ipl.CurrentProductLinkRangeStartIndex < 0)
                        ipl.CurrentProductLinkRangeStartIndex = 0;
                    if (ipl.CurrentProductLinkRangeEndIndex < 0)
                        ipl.CurrentProductLinkRangeEndIndex = -1;
                }
                return ipl;
            }
            public int CurrentProductLinkRangeStartIndex = INITIAL_INDEX;
            public int CurrentProductLinkRangeEndIndex = INITIAL_INDEX;
            const int INITIAL_INDEX = -1;

            void set_pls_filtered(string[] keyword2s, string category2)
            {
                if (keyword2s == null)
                    keyword2s = new string[] { string.Empty };

                keyword2s = keyword2s.Select(x => x.Trim().ToLower()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).ToArray();
                string keyword2s_ = string.Join(" ", keyword2s).Trim();
                if (this.keyword2s_ == keyword2s_ && this.category2 == category2)
                {
                    if (filtered_product_links == null)
                        filtered_product_links = product_links;
                    return;
                }

                this.keyword2s_ = keyword2s_;
                this.category2 = category2;
                CurrentProductLinkRangeStartIndex = INITIAL_INDEX;
                CurrentProductLinkRangeEndIndex = INITIAL_INDEX;
                if (keyword2s.Length < 1 && string.IsNullOrEmpty(category2))
                {
                    filtered_product_links = product_links;
                    return;
                }

                filtered_product_links = new List<Cliver.ProductIdentifier.ProductLink>();
                List<Regex> rs = new List<Regex>();
                foreach (string k in keyword2s)
                    rs.Add(new Regex(Regex.Escape(k), RegexOptions.IgnoreCase | RegexOptions.Singleline));
                foreach (Cliver.ProductIdentifier.ProductLink l in this.product_links)
                {
                    Cliver.ProductIdentifier.Product p = l.Product2s.Where(x => x.DbProduct.CompanyId == this.Company2Id).First();
                    
                    if (!string.IsNullOrEmpty(category2))
                        if (!p.DbProduct.Category.StartsWith(category2))
                            continue;
                    
                    bool match = true;
                    foreach (Regex r in rs)
                    {
                        if (!r.IsMatch(p.NormalizedText(ProductIdentifier.Field.Name)))
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                        filtered_product_links.Add(l);
                }
            }
            string keyword2s_ = string.Empty;
            string category2 = null;

            public double ScoreFactor = 1;

            static IdenticalProductList get_from_session(ProductLinksController controller, int[] product1_ids, int company2_id)
            {
                IdenticalProductList ipl = (IdenticalProductList)controller.Session[IdenticalProductList.SESSION_KEY];
                //IdenticalProductList ipl = (IdenticalProductList)System.Runtime.Caching.MemoryCache.Default.Get(IdenticalProductList.CacheKey);
                if (product1_ids == null)
                    return ipl;
                if (ipl == null)
                    ipl = new IdenticalProductList(controller, product1_ids, company2_id, new Cliver.ProductIdentifier.Engine());
                else if (!ipl.is_corresponding(controller, product1_ids, company2_id))
                    ipl = new IdenticalProductList(controller, product1_ids, company2_id, ipl.engine);
                controller.Session[IdenticalProductList.SESSION_KEY] = ipl;
                //System.Runtime.Caching.MemoryCache.Default.Set(IdenticalProductList.CacheKey, ipl, DateTimeOffset.Now.AddSeconds(3600));
                if (ipl.product_links.Count > 0)
                    ipl.ScoreFactor = 1D / ipl.product_links[0].Score;
                return ipl;
            }
            const string SESSION_KEY = "IDENTICAL_PRODUCT_LIST";
            bool is_corresponding(ProductLinksController controller, int[] product1_ids, int company2_id)
            {
                if (Company2Id != company2_id)
                    return false;

                Product p2 = controller.db.Products.Where(p => p.CompanyId == company2_id && product1_ids.Contains(p.Id)).FirstOrDefault();
                if (p2 != null)
                {
                    if (p2.LinkId > 0)
                    {
                        int[] p2_ids = controller.db.Products.Where(x => x.LinkId == p2.LinkId).Select(x => x.Id).ToArray();
                        product1_ids = product1_ids.Where(x => !p2_ids.Contains(x)).ToArray();
                    }
                    else
                        product1_ids = product1_ids.Where(x => x != p2.Id).ToArray();
                }
                if (!Product1Ids.OrderBy(x => x).SequenceEqual(product1_ids.OrderBy(x => x)))
                    return false;
                return true;
            }
            IdenticalProductList(ProductLinksController controller, int[] product1_ids, int company2_id, Cliver.ProductIdentifier.Engine engine)
            {
                Product1Ids = (from x in product1_ids where controller.db.Products.Where(p => p.Id == x).First().CompanyId != company2_id select x).ToArray();
                Company2Id = company2_id;
                this.engine = engine;
                product_links = engine.CreateProductLinkList(Product1Ids, Company2Id);
            }

            public static void SaveLink(ProductLinksController controller, int[] product_ids)
            {               
                Cliver.ProductIdentifier.Engine engine;
                IdenticalProductList ipl = get_from_session(controller, null, 0);
                if (ipl == null)
                    engine = new Cliver.ProductIdentifier.Engine();
                else
                    engine = ipl.engine;
                engine.SaveLink(product_ids);
            }
        }

        public ActionResult GetNextProductLinks(
            [Bind(Prefix = "product1_ids[]")]string[] product1_ids_,
            [Bind(Prefix = "company2_id")]string company2_id_,
            [Bind(Prefix = "keyword2s")]string keyword2s_,
            string category2,
            int range_size,
            bool forward
            )
        {
            int[] product1_ids = (from x in product1_ids_ where !string.IsNullOrWhiteSpace(x) select int.Parse(x)).ToArray();
            int company2_id = int.Parse(company2_id_);
            string[] keyword2s = keyword2s_.Split(' ');
            bool is_CurrentProductLinks_changed;
            IdenticalProductList ipl = IdenticalProductList.GetMovingToNextRange(this, product1_ids, company2_id, keyword2s, category2, range_size, forward, out is_CurrentProductLinks_changed);
            List<List<object>> pss = new List<List<object>>();
            for (int i = ipl.CurrentProductLinkRangeStartIndex; i <= ipl.CurrentProductLinkRangeEndIndex; i++)
                pss.Add(get_ProductLink_objects(ipl.CurrentProductLinks[i], i, ipl.ScoreFactor));
            Dictionary<string, object> json_o = new Dictionary<string, object>();
            json_o["Product2s"] = pss;
            json_o["Product2sCount"] = ipl.CurrentProductLinks.Count;
            json_o["Is_CurrentProductLinks_Changed"] = is_CurrentProductLinks_changed;
            return Json(json_o, JsonRequestBehavior.AllowGet);
        }

        List<object> get_ProductLink_objects(Cliver.ProductIdentifier.ProductLink pl, int index, double score_factor)
        {
            if (pl == null)
                return null;

            List<object> ds = new List<object>();
            foreach (Cliver.ProductIdentifier.Product product in pl.Product2s)
            {
                Cliver.FhrApi.ProductOffice.Models.Product p = product.DbProduct;

                object d = new
                {
                    Id = p.Id,
                    CompanyId = p.CompanyId,
                    ExternalId = p.ExternalId,
                    CreateTime = p.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ModifyTime = p.ModifyTime != null ? ((DateTime)p.ModifyTime).ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdateTime = p.UpdateTime != null ? ((DateTime)p.UpdateTime).ToString("yyyy-MM-dd HH:mm:ss") : null,
                    Name = p.Name,
                    Url = p.Url,
                    ImageUrls = p.ImageUrls != null ? p.ImageUrls.Split('\n') : new string[] { },
                    Sku = p.Sku,
                    Category = p.Category,
                    Description = p.Description,
                    LinkId = p.LinkId != null ? p.LinkId : -p.Id,
                    Source = p.Source,
                    Stock = p.Stock,
                    CompanyName = p.Company.Name,
                    Price = (from r in p.Prices orderby r.Time descending select (string)r.Currency.Symbol + r.Value.ToString()).FirstOrDefault(),
                    //_MatchedWords = (from x in pl.Product1s select new { x.DbProduct.Id, pl.Get(x.DbProduct.Id, p.Id).MatchedWords }).ToDictionary(x => x.Id, x => x.MatchedWords),
                    _Index = index,
                    _Score = (!double.IsNaN(pl.Score) ? pl.Score : 1) * score_factor,
                    _CategoryScore = (!double.IsNaN(pl.CategoryScore) ? pl.CategoryScore : 1) * score_factor,
                    _NameScore = (!double.IsNaN(pl.NameScore) ? pl.NameScore : 1) * score_factor,
                    _SecondaryScore = (!double.IsNaN(pl.SecondaryScore) ? pl.SecondaryScore : 1) * score_factor,
                };
                d = Cliver.PrepareField.Html.GetDbObject(d);
                ds.Add(d);
            }
            return ds;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Prefix = "ProductId")]string[] product_ids_)
        {
            int[] product_ids = product_ids_.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => int.Parse(x)).ToArray();
            IdenticalProductList.SaveLink(this, product_ids);
            if (Request.IsAjaxRequest())
                return Content(null);
            return RedirectToAction("Index");
        }

        public ActionResult GetCompanyCategories(int company_id)
        {         
            Dictionary<string, dynamic> tree = CategoryRoutines.GetCompanyCategories(db, company_id);
            return Json(tree, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    if (Request.IsAjaxRequest())    
        //    {
        //        Exception e = filterContext.Exception;
        //        filterContext.ExceptionHandled = true; 
        //        filterContext.Result = Content(Cliver.Bot.Log.GetExceptionMessage(e));
        //    }
        //}
    }
}
