using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.FhrCrawlerHost;
using Cliver.Bot;
using System.Text.RegularExpressions;

namespace Cliver.FhrEventMonitor
{
    class Replicator
    {
        static Cliver.FhrCrawlerHost.Db2.ProductOfficeDataContext dc2;

        public static void Do()
        {
            dc2 = new FhrCrawlerHost.Db2.ProductOfficeDataContext(Db2Api.ConnectionString);
        }

        //static void replicate_product_table(string crawler_id, string products_table)
        //{
        //    try
        //    {
        //        LogMessage.Inform("Processing '" + products_table + "' table.");

        //        Company company = (from r in dc2.Companies where r.CrawlerId == crawler_id select r).FirstOrDefault();
        //        if (company == null)
        //        {
        //            DbApi.Message(CrawlerHost.DbApi.MessageType.ERROR, "Could not find company for crawler_id:" + crawler_id);
        //            return;
        //        }

        //        int replicated_count = 0;
        //        Recordset products;
        //        do
        //        {
        //            dc2 = new FhrDbDataContext(Db2Api.ConnectionString);
        //            products = dbc1["SELECT TOP 100 Id, CrawlTime, ChangeTime, Url, Data, State FROM " + products_table + " WHERE State=" + (int)Crawler.ProductState.NEW].GetRecordset();
        //            foreach (Record record in products)
        //            {
        //                FhrCrawlerHost.Product p = Cliver.CrawlerHost.Product.Restore<FhrCrawlerHost.Product>(record);
        //                p.Prepare();
        //                LogMessage.Inform("Replicating id: '" + p.Id + "'");
        //                Product product = (from x in dc2.Products where x.CompanyId == company.Id && x.CompanyProductId == p.Id select x).FirstOrDefault();
        //                if (product != null)
        //                {
        //                    product.Category = p.Category;
        //                    product.Description = p.Description;
        //                    product.ImageUrls = string.Join("\n", p.ImageUrls);
        //                    product.ModifiedTime = p.CrawlTime;
        //                    product.Name = p.Name;
        //                    product.Sku = p.Sku;
        //                    product.Source = "crawler:" + crawler_id;
        //                    product.Stock = p.Stock;
        //                    product.UpdatedTime = p.CrawlTime;
        //                    product.Url = p.Url;
        //                }
        //                else
        //                {
        //                    product = new Product();
        //                    product.Category = p.Category;
        //                    product.CompanyId = company.Id;
        //                    product.CompanyProductId = p.Id;
        //                    product.CreatedTime = p.CrawlTime;
        //                    product.Description = p.Description;
        //                    product.ImageUrls = string.Join("\n", p.ImageUrls);
        //                    product.ModifiedTime = p.ChangeTime;
        //                    product.Name = p.Name;
        //                    product.Sku = p.Sku;
        //                    product.Source = "crawler:" + crawler_id;
        //                    product.Stock = p.Stock;
        //                    product.UpdatedTime = p.CrawlTime;
        //                    product.Url = p.Url;
        //                    dc2.Products.InsertOnSubmit(product);
        //                }

        //                decimal price_value;
        //                Db2Api.Currency currency_id_;
        //                if (FhrDbReplicator.Parser.ParsePrice(p.Price, out currency_id_, out price_value))
        //                {
        //                    int currency_id = (int)currency_id_;
        //                    Price price = product.Prices.SingleOrDefault(x => x.Time == p.CrawlTime);
        //                    //Price price = (from x in dc2.Prices where x.ProductId == product.Id && x.Time == (DateTime)r["CrawlTime"] && x.Quantity==1 select x).FirstOrDefault();
        //                    if (price != null)
        //                    {
        //                        price.CurrencyId = currency_id;
        //                        price.Value = price_value;
        //                    }
        //                    else
        //                    {
        //                        price = new Price();
        //                        price.CurrencyId = currency_id;
        //                        price.ProductId = product.Id;
        //                        price.Time = p.CrawlTime;
        //                        price.Value = price_value;
        //                        product.Prices.Add(price);
        //                    }
        //                }
        //                dc2.SubmitChanges();

        //                if (1 > dbc1["UPDATE " + products_table + " SET State=" + (int)Crawler.ProductState.REPLICATED + " WHERE Id=@Id"].Execute("@Id", p.Id))
        //                    LogMessage.Error("Could not update State for product id:" + p.Id);
        //            }
        //            replicated_count += products.Count;
        //        }
        //        while (products.Count > 0);
        //        LogMessage.Inform("Replicated products: " + replicated_count);

        //        int deleted_count = 0;
        //        do
        //        {
        //            dc2 = new FhrDbDataContext(Db2Api.ConnectionString);
        //            products = dbc1["SELECT TOP 100 Id, CrawlTime, ChangeTime, Url, Data, State FROM " + products_table + " WHERE State=" + (int)Crawler.ProductState.DELETED].GetRecordset();
        //            foreach (Record record in products)
        //            {
        //                LogMessage.Inform("Deleting id: '" + (string)record["Id"] + "'");
        //                Product product = (from x in dc2.Products where x.CompanyId == company.Id && x.CompanyProductId == (string)record["Id"] select x).FirstOrDefault();
        //                if (product == null)
        //                    DbApi.Message(CrawlerHost.DbApi.MessageType.WARNING, "Could not find Product [CompanyId=" + company.Id + "CompanyProductId=" + (string)record["Id"] + "] while deleting.");
        //                else
        //                {
        //                    foreach(Price p in product.Prices)
        //                        dc2.Prices.DeleteOnSubmit(p);

        //                    dc2.Products.DeleteOnSubmit(product);
        //                    dc2.SubmitChanges();

        //                    int u = dbc1["DELETE FROM " + products_table + " WHERE Id=@Id"].Execute("@Id", (string)record["Id"]);
        //                    if (u < 1)
        //                        LogMessage.Error("Could not delete record for product id:" + (string)record["Id"]);
        //                }
        //            }
        //            deleted_count += products.Count;
        //        }
        //        while (products.Count > 0);
        //        LogMessage.Inform("Deleted products: " + deleted_count);
        //    }
        //    catch (Exception e)
        //    {
        //        DbApi.Message(e);
        //    }
        //}
    }
}