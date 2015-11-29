using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cliver.FhrApi.ProductOffice
{
    public class DataApi
    {
        public class Products
        {
            public static void Delete(Cliver.FhrApi.ProductOffice.Models.ProductOfficeEntities db, int product_id, bool delete_group = false)
            {
                db.Prices.RemoveRange(db.Prices.Where(i => i.ProductId == product_id));
                if (delete_group)
                    db.Products.RemoveRange(db.Products.Where(i => i.MainProductId == product_id));
                db.Products.RemoveRange(db.Products.Where(i => i.Id == product_id));
                db.SaveChanges();
            }
        }

        public class Companies
        {
            public static void Delete(Cliver.FhrApi.ProductOffice.Models.ProductOfficeEntities db, int product_id)
            {
                db.SaveChanges();
            }
        }
    }
}