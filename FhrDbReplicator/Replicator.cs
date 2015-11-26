using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.FhrCrawlerHost;
using Cliver.Bot;
using System.Text.RegularExpressions;

namespace Cliver.FhrDbReplicator
{
    public class Replicator : CrawlerHost.Service
    {
        Db2Api db2_api = new Db2Api();

        override protected void Do()
        {
            Recordset products_tables = DbApi.Connection["SELECT Id, Site, _ProductsTable FROM Crawlers"].GetRecordset();
            foreach (Record r in products_tables)
                replicate_product_table((string)r["Id"], (string)r["_ProductsTable"]);
        }

        void replicate_product_table(string crawler_id, string products_table)
        {
            try
            {
                Log.Write("Processing '" + products_table + "' table.");

                FhrCrawlerHost.Db2.Company company = (from r in db2_api.Context.Companies where r.CrawlerId == crawler_id select r).FirstOrDefault();
                if (company == null)
                {
                    Log.Error("Could not find company for crawler_id:" + crawler_id);
                    return;
                }

                int replicated_count = 0;
                Recordset products;
                do
                {
                    db2_api.RenewContext();
                    products = DbApi.Connection["SELECT TOP 100 Id, CrawlTime, ChangeTime, Url, Data, State FROM " + products_table + " WHERE State=" + (int)Crawler.ProductState.NEW].GetRecordset();
                    foreach (Record record in products)
                    {
                        FhrCrawlerHost.Product p = Cliver.CrawlerHost.Product.Restore<FhrCrawlerHost.Product>(record);
                        p.Prepare();
                        LogMessage.Write("Replicating id: '" + p.Id + "'");
                        FhrCrawlerHost.Db2.Product product = (from x in db2_api.Context.Products where x.CompanyId == company.Id && x.ExternalId == p.Id select x).FirstOrDefault();
                        if (product != null)
                        {
                            product.Category = p.Category;
                            product.Description = p.Description;
                            product.ImageUrls = string.Join("\n", p.ImageUrls);
                            product.ModifyTime = p.ChangeTime;
                            product.Name = p.Name;
                            product.Sku = p.Sku;
                            product.Source = "crawler:" + crawler_id;
                            product.Stock = p.Stock;
                            product.UpdateTime = p.CrawlTime;
                            product.Url = p.Url;
                        }
                        else
                        {
                            product = new FhrCrawlerHost.Db2.Product();
                            product.Category = p.Category;
                            product.CompanyId = company.Id;
                            product.ExternalId = p.Id;
                            product.CreateTime = p.CrawlTime;
                            product.Description = p.Description;
                            product.ImageUrls = string.Join("\n", p.ImageUrls);
                            product.LinkId = null;
                            product.ModifyTime = p.ChangeTime;
                            product.Name = p.Name;
                            product.Sku = p.Sku;
                            product.Source = "crawler:" + crawler_id;
                            product.Stock = p.Stock;
                            product.UpdateTime = p.CrawlTime;
                            product.Url = p.Url;
                            db2_api.Context.Products.InsertOnSubmit(product);
                        }

                        decimal price_value;
                        Db2Api.Currency currency_id_;
                        if (FhrDbReplicator.Parser.ParsePrice(p.Price, out currency_id_, out price_value))
                        {
                            int currency_id = (int)currency_id_;
                            FhrCrawlerHost.Db2.Price price = product.Prices.SingleOrDefault(x => x.Time == p.CrawlTime);
                            //Price price = (from x in dc2.Prices where x.ProductId == product.Id && x.Time == (DateTime)r["CrawlTime"] && x.Quantity==1 select x).FirstOrDefault();
                            if (price != null)
                            {
                                price.CurrencyId = currency_id;
                                price.Value = price_value;
                            }
                            else
                            {
                                price = new FhrCrawlerHost.Db2.Price();
                                price.CurrencyId = currency_id;
                                price.ProductId = product.Id;
                                price.Time = p.CrawlTime;
                                price.Value = price_value;
                                product.Prices.Add(price);
                            }
                        }
                        db2_api.Context.SubmitChanges();

                        if (1 > DbApi.Connection["UPDATE " + products_table + " SET State=" + (int)Crawler.ProductState.REPLICATED + " WHERE Id=@Id"].Execute("@Id", p.Id))
                            LogMessage.Error("Could not update State for product id:" + p.Id);
                    }
                    replicated_count += products.Count;
                }
                while (products.Count > 0);
                LogMessage.Inform("Replicated products: " + replicated_count);

                int deleted_count = 0;
                do
                {
                   db2_api.RenewContext();
                    products = DbApi.Connection["SELECT TOP 100 Id, CrawlTime, ChangeTime, Url, Data, State FROM " + products_table + " WHERE State=" + (int)Crawler.ProductState.DELETED].GetRecordset();
                    foreach (Record record in products)
                    {
                        LogMessage.Inform("Deleting id: '" + (string)record["Id"] + "'");
                        FhrCrawlerHost.Db2.Product product = (from x in db2_api.Context.Products where x.CompanyId == company.Id && x.ExternalId == (string)record["Id"] select x).FirstOrDefault();
                        if (product == null)
                            Log.Warning("Could not find Product [CompanyId=" + company.Id + "CompanyProductId=" + (string)record["Id"] + "] while deleting.");
                        else
                        {
                            ProductOffice.DataApi.Products.Delete(db2_api.Context, product.Id);

                            int u = DbApi.Connection["DELETE FROM " + products_table + " WHERE Id=@Id"].Execute("@Id", (string)record["Id"]);
                            if (u < 1)
                                LogMessage.Error("Could not delete record for product id:" + (string)record["Id"]);
                        }
                    }
                    deleted_count += products.Count;
                }
                while (products.Count > 0);
                LogMessage.Inform("Deleted products: " + deleted_count);
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }
    }
}