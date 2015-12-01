using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.Fhr;
using Cliver.Bot;
using System.Text.RegularExpressions;
using Cliver.Fhr.ProductOffice.Models;

namespace Cliver.FhrCleaner
{
    public class Cleaner : Cliver.CrawlerHostCleaner.Cleaner
    {
        override protected void Do()
        {
            base.Do();

            DbApi db = Fhr.ProductOffice.Models.DbApi.Create();
            if (Properties.Settings.Default.DeletePricesOlderThanDays > 0)
            {
                DateTime old_time = DateTime.Now.AddDays(-Properties.Settings.Default.DeletePricesOlderThanDays);
                IQueryable<Cliver.Fhr.ProductOffice.Models.Price> prices = db.Prices.Where(p => p.Time < old_time);
                db.Prices.RemoveRange(prices);
                Log.Main.Write("Deleting Prices older than " + old_time.ToShortDateString() + ": " + prices.Count());
                db.SaveChanges();
            }
            DbApi.RenewContext(ref db);
            if (Properties.Settings.Default.DeleteProductsOlderThanDays > 0)
            {
                DateTime old_time = DateTime.Now.AddDays(-Properties.Settings.Default.DeleteProductsOlderThanDays);
                IQueryable<Fhr.ProductOffice.Models.Product> products = db.Products.Where(p => p.UpdateTime == null || p.UpdateTime < old_time);
                foreach (Fhr.ProductOffice.Models.Product product in products)
                    Fhr.ProductOffice.DataApi.Product.Delete(db, product.Id);
                Log.Main.Write("Deleting Products older than " + old_time.ToShortDateString() + ": " + products.Count());
            }
        }

        //override protected void Do()
        //{
        //    base.Do();

        //    ProductOffice.Models.ProductOfficeEntities db = ProductOffice.DataApi.Products.GetDb();
        //    if (Properties.Settings.Default.DeletePricesOlderThanDays > 0)
        //    {
        //        DateTime old_time = DateTime.Now.AddDays(-Properties.Settings.Default.DeletePricesOlderThanDays);
        //        IQueryable<ProductOffice.Models.Price> prices = db.Prices.Where(p => p.Time < old_time);
        //        db.Prices.RemoveRange(prices);
        //        Log.Main.Write("Deleting Prices older than " + old_time.ToShortDateString() + ": " + prices.Count());
        //        db.SaveChanges();
        //    }
        //    db.Dispose();
        //    db = new ProductOffice.Models.ProductOfficeEntities();
        //    if (Properties.Settings.Default.DeleteProductsOlderThanDays > 0)
        //    {
        //        DateTime old_time = DateTime.Now.AddDays(-Properties.Settings.Default.DeleteProductsOlderThanDays);
        //        IQueryable<ProductOffice.Models.Product> products = db.Products.Where(p => p.UpdateTime == null || p.UpdateTime < old_time);
        //        Log.Main.Write("Deleting Products older than " + old_time.ToShortDateString() + ": " + products.Count());
        //        foreach (ProductOffice.Models.Product product in products)
        //            ProductOffice.DataApi.Products.Delete(db, product.Id);
        //    }
        //}
    }
}