using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cliver.Fhr.ProductOffice.DataApi
{
    public class Product
    {
        public static void Delete(Cliver.Fhr.ProductOffice.Models.ProductOfficeEntities db, int product_id, bool delete_group = false)
        {
            db.Prices.RemoveRange(db.Prices.Where(i => i.ProductId == product_id));
            if (delete_group)
                db.Products.RemoveRange(db.Products.Where(i => i.MainProductId == product_id));
            db.Products.RemoveRange(db.Products.Where(i => i.Id == product_id));
            db.SaveChanges();
        }

        public const string CATEGORY_SEPARATOR = @">";
        public const string IMAGE_URL_SEPARATOR = "\n";

        //public static void Add(Cliver.Fhr.ProductOffice.Models.ProductOfficeEntities db, Product p)
        //{
        //    db.Prices.RemoveRange(db.Prices.Where(i => i.ProductId == product_id));
        //    if (delete_group)
        //        db.Products.RemoveRange(db.Products.Where(i => i.MainProductId == product_id));
        //    db.Products.RemoveRange(db.Products.Where(i => i.Id == product_id));
        //    db.SaveChanges();
        //}
    }
}