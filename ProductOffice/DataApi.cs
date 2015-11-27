using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cliver.ProductOffice.Models;

namespace Cliver.ProductOffice
{
    public class DataApi
    {
        public static class Products
        {
            //public static ProductOffice.Models.ProductOfficeEntities GetDb()
            //{
            //    return new ProductOffice.Models.ProductOfficeEntities();
            //}

            public static void Delete(ProductOfficeEntities db, int product_id, bool delete_group = false)
            {
                db.Prices.RemoveRange(db.Prices.Where(i => i.ProductId == product_id));
                if (delete_group)
                    db.Products.RemoveRange(db.Products.Where(i => i.MainProductId == product_id));
                db.Products.RemoveRange(db.Products.Where(i => i.Id == product_id));
                db.SaveChanges();
            }

            public static void Delete(FhrCrawlerHost.Db2.ProductOfficeDataContext dc, int product_id, bool delete_group = false)
            {
                dc.Prices.DeleteAllOnSubmit(dc.Prices.Where(i => i.ProductId == product_id));
                if (delete_group)
                    dc.Products.DeleteAllOnSubmit(dc.Products.Where(i => i.MainProductId == product_id));
                dc.Products.DeleteAllOnSubmit(dc.Products.Where(i => i.Id == product_id));                            
                dc.SubmitChanges();
            }
        }

        public static class Companies
        {
            public static void Delete(ProductOfficeEntities db, int product_id)
            {
                db.SaveChanges();
            }

            public static void Delete(FhrCrawlerHost.Db2.ProductOfficeDataContext dc, int product_id)
            {
                dc.SubmitChanges();
            }
        }
    }
}