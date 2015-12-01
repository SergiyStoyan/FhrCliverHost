using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using Cliver.Fhr.CrawlerHost;
//using Cliver.Fhr.ProductOffice.Models;
using Cliver.Fhr.ProductOffice.DataApi;
using Cliver.Bot;
using System.Text.RegularExpressions;

namespace Cliver.FhrDbReplicator
{
    public class Replicator : CrawlerHost.Service
    {
        Cliver.Fhr.ProductOffice.Models.DbApi db2 = Fhr.ProductOffice.Models.DbApi.Create();

        override protected void Do()
        {
            Recordset products_tables = this.Db["SELECT Id, Site, _ProductsTable FROM Crawlers"].GetRecordset();
            foreach (Record r in products_tables)
                replicate_product_table((string)r["Id"], (string)r["_ProductsTable"]);
        }

        void replicate_product_table(string crawler_id, string products_table)
        {
            Cliver.Fhr.ProductOffice.Models.DbApi.RenewContext(ref db2);
            try
            {
                Log.Write("Processing '" + products_table + "' table.");

                Fhr.ProductOffice.Models.Company company = (from r in db2.Companies where r.CrawlerId == crawler_id select r).FirstOrDefault();
                if (company == null)
                {
                    Log.Error("Could not find company for crawler_id:" + crawler_id);
                    return;
                }

                int replicated_count = 0;
                Recordset products;
                do
                {
                    Cliver.Fhr.ProductOffice.Models.DbApi.RenewContext(ref db2);
                    products = Db["SELECT TOP 100 Id, CrawlTime, ChangeTime, Url, Data, State FROM " + products_table + " WHERE State=" + (int)Crawler.ProductState.NEW].GetRecordset();
                    foreach (Record record in products)
                    {
                        Fhr.CrawlerHost.Product raw_p = Cliver.CrawlerHost.Product.Restore<Fhr.CrawlerHost.Product>(record);
                        Fhr.CrawlerHost.Product.PreparedProduct p = raw_p.GetPreparedProduct();
                        LogMessage.Write("Replicating id: '" + p.Id + "'");
                        Fhr.ProductOffice.Models.Product product = (from x in db2.Products where x.CompanyId == company.Id && x.ExternalId == p.Id select x).FirstOrDefault();
                        if (product != null)
                        {
                            product.Category = p.Category;
                            product.Description = p.Description;
                            product.ImageUrls = p.ImageUrls;
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
                            product = new Fhr.ProductOffice.Models.Product();

                            List<string> cs = new List<string>();
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
                            db2.Products.Add(product);
                        }

                        decimal price_value;
                        Currency currency_id_;
                        if (FhrDbReplicator.Parser.ParsePrice(p.Price, out currency_id_, out price_value))
                        {
                            int currency_id = (int)currency_id_;
                            Fhr.ProductOffice.Models.Price price = product.Prices.SingleOrDefault(x => x.Time == p.CrawlTime);
                            //Price price = (from x in dc2.Prices where x.ProductId == product.Id && x.Time == (DateTime)r["CrawlTime"] && x.Quantity==1 select x).FirstOrDefault();
                            if (price != null)
                            {
                                price.CurrencyId = currency_id;
                                price.Value = price_value;
                            }
                            else
                            {
                                price = new Fhr.ProductOffice.Models.Price();
                                price.CurrencyId = currency_id;
                                price.ProductId = product.Id;
                                price.Time = p.CrawlTime;
                                price.Value = price_value;
                                product.Prices.Add(price);
                            }
                        }
                        db2.Configuration.ValidateOnSaveEnabled = false;
                        db2.SaveChanges();

                        if (1 > Db["UPDATE " + products_table + " SET State=" + (int)Crawler.ProductState.REPLICATED + " WHERE Id=@Id"].Execute("@Id", p.Id))
                            LogMessage.Error("Could not update State for product id:" + p.Id);
                    }
                    replicated_count += products.Count;
                }
                while (products.Count > 0);
                LogMessage.Inform("Replicated products: " + replicated_count);

                int deleted_count = 0;
                do
                {
                    Cliver.Fhr.ProductOffice.Models.DbApi.RenewContext(ref db2);
                    products = Db["SELECT TOP 100 Id, CrawlTime, ChangeTime, Url, Data, State FROM " + products_table + " WHERE State=" + (int)Crawler.ProductState.DELETED].GetRecordset();
                    foreach (Record record in products)
                    {
                        LogMessage.Inform("Deleting id: '" + (string)record["Id"] + "'");
                        Fhr.ProductOffice.Models.Product product = (from x in db2.Products where x.CompanyId == company.Id && x.ExternalId == (string)record["Id"] select x).FirstOrDefault();
                        if (product == null)
                            Log.Warning("Could not find Product [CompanyId=" + company.Id + "CompanyProductId=" + (string)record["Id"] + "] while deleting.");
                        else
                        {
                            Cliver.Fhr.ProductOffice.DataApi.Product.Delete(db2, product.Id);

                            int u = Db["DELETE FROM " + products_table + " WHERE Id=@Id"].Execute("@Id", (string)record["Id"]);
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