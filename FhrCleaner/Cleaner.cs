using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.FhrCrawlerHost;
using Cliver.Bot;
using System.Text.RegularExpressions;

namespace Cliver.FhrCleaner
{
    public class Cleaner : Cliver.CrawlerHostCleaner.Cleaner
    {
        override protected void Do()
        {
            base.Do();

            FhrCrawlerHost.Db2.ProductOfficeDataContext dc = new FhrCrawlerHost.Db2.ProductOfficeDataContext(Db2Api.ConnectionString);
            if (Properties.Settings.Default.DeletePricesOlderThanDays > 0)
            {
                DateTime old_time = DateTime.Now.AddDays(-Properties.Settings.Default.DeletePricesOlderThanDays);
                IQueryable<FhrCrawlerHost.Db2.Price> prices = dc.Prices.Where(p => p.Time < old_time);
                dc.Prices.DeleteAllOnSubmit(prices);
                Log.Main.Write("Deleting Prices older than " + old_time.ToShortDateString() + ": " + prices.Count());
                dc.SubmitChanges();
            }
            dc.Dispose();
            dc = new FhrCrawlerHost.Db2.ProductOfficeDataContext(Db2Api.ConnectionString);
            if (Properties.Settings.Default.DeleteProductsOlderThanDays > 0)
            {
                DateTime old_time = DateTime.Now.AddDays(-Properties.Settings.Default.DeleteProductsOlderThanDays);
                IQueryable<FhrCrawlerHost.Db2.Product> products = dc.Products.Where(p => p.UpdateTime == null || p.UpdateTime < old_time);
                foreach (FhrCrawlerHost.Db2.Product product in products)
                    dc.Prices.DeleteAllOnSubmit(product.Prices);
                dc.Products.DeleteAllOnSubmit(products);
                Log.Main.Write("Deleting Products older than " + old_time.ToShortDateString() + ": " + products.Count());
                dc.SubmitChanges();
            }
        }
    }
}