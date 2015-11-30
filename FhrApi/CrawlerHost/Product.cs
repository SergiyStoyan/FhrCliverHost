using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
//using System.Data.Odbc;
using System.Web.Script.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Cliver.Bot;
using Cliver.CrawlerHost;

namespace Cliver.FhrApi.CrawlerHost
{
    public class Product : Cliver.CrawlerHost.Product
    {
        public Product() { }

        static Product()
        {
            ValidateProductClass(typeof(Product));
        }

        public Product(string id, string url, string name, string sku, string price, string[] category_branch, string[] image_urls, decimal? stock, string description)
            : base(id, url)
        {
            Name = name;
            Sku = sku;
            Price = price;
            CategoryBranch = category_branch;
            ImageUrls = image_urls;
            Description = description;

            if (stock == null)
                Stock = (decimal)ProductStock.NOT_SET;
            else
                Stock = (decimal)stock;
        }
        
        readonly public string Name;
        readonly public string Sku;
        readonly public string Price;
        readonly public string Description;
        readonly public string[] ImageUrls;
        readonly public string[] CategoryBranch;
        readonly public decimal Stock;

        override public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Id))
                Error("Id is empty.");
            if (string.IsNullOrWhiteSpace(Url))
                Error("Url is empty.");

            if (string.IsNullOrWhiteSpace(Sku))
                Warning("Sku is empty.");
            if (string.IsNullOrWhiteSpace(Name))
                Warning("Name is empty.");
            if (string.IsNullOrWhiteSpace(Sku))
                Warning("Sku is empty.");
            if (string.IsNullOrWhiteSpace(Price))
                Warning("Price is empty.");
            if (CategoryBranch.Length < 1)
                Warning("CategoryBranch is empty.");
            if (string.IsNullOrWhiteSpace(Description))
                Warning("Description is empty.");
            if (Stock == (decimal)ProductStock.NOT_SET || Stock == (decimal)ProductStock.CANNOT_PARSE)
                Warning("Stock is empty.");
            if (ImageUrls.Length < 1)
                Warning("ImageUrls is empty.");
        }

        public class PreparedProduct
        {
            public readonly string Id;
            public readonly string Url;
            public readonly DateTime CrawlTime; 
            public readonly DateTime ChangeTime;
            readonly public string Name;
            readonly public string Sku;
            readonly public string Price;
            readonly public string Description;
            readonly public string ImageUrls;
            readonly public string Category;
            readonly public decimal Stock;

            internal PreparedProduct(Product p)
            {
                Id = p.Id;
                Url = p.Url;
                CrawlTime = p.CrawlTime;
                ChangeTime = p.ChangeTime;
                Name = Cliver.PrepareField.Html.GetDbField(p.Name);
                Sku = Cliver.PrepareField.Html.GetDbField(p.Sku);
                Price = Cliver.PrepareField.Html.GetDbField(p.Price);
                Description = Cliver.PrepareField.Html.GetDbField(p.Description);
                ImageUrls = string.Join(Cliver.FhrApi.ProductOffice.DataApi.Product.IMAGE_URL_SEPARATOR, p.ImageUrls);
                List<string> cs = new List<string>();
                foreach (string c in p.CategoryBranch)
                    cs.Add(Cliver.PrepareField.Html.GetDbField(Regex.Replace(c, Regex.Escape(Cliver.FhrApi.ProductOffice.DataApi.Product.CATEGORY_SEPARATOR), "-", RegexOptions.Singleline | RegexOptions.Compiled)));
                Category = string.Join(Cliver.FhrApi.ProductOffice.DataApi.Product.CATEGORY_SEPARATOR, cs);
                Stock = p.Stock;
            }
        }

        public PreparedProduct GetPreparedProduct()
        {
            return new PreparedProduct(this);
        }
    }

    public enum ProductStock : int
    {
        NOT_IN_STOCK = 0,
        NOT_SET = -1,
        CANNOT_PARSE = -2,
        IN_STOCK = -3,
    }
}

