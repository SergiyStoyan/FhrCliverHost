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

namespace Cliver.FhrCrawlerHost
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
            List<string> cs = new List<string>();
            foreach (string c in category_branch)
                cs.Add(Regex.Replace(c, Regex.Escape(CATEGORY_SEPARATOR), " ", RegexOptions.Singleline | RegexOptions.Compiled).Trim());
            Category = string.Join(CATEGORY_SEPARATOR, cs);
            ImageUrls = image_urls;
            Description = description;

            if (stock == null)
                Stock = (decimal)ProductStock.NOT_SET;
            else
                Stock = (decimal)stock;
        }

        public const string CATEGORY_SEPARATOR = @">";

        readonly public string Name;
        readonly public string Sku;
        readonly public string Price;
        readonly public string Description;
        readonly public string[] ImageUrls;
        readonly public string Category;
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
            if (string.IsNullOrWhiteSpace(Category))
                Warning("Category is empty.");
            if (string.IsNullOrWhiteSpace(Description))
                Warning("Description is empty.");
            if (Stock == (decimal)ProductStock.NOT_SET || Stock == (decimal)ProductStock.CANNOT_PARSE)
                Warning("Stock is empty.");
            if (ImageUrls.Length < 1)
                Warning("ImageUrls is empty.");
        }

        //public static Product GetPreparedProduct(Product p)
        //{
        //    List<string> cs = new List<string>();
        //    foreach (string c in p.Category.Split('\\'))
        //        cs.Add(FileWriter.PrepareField(c, FileWriter.FieldFormat.DB_TABLE));

        //    return new Product(
        //        id: p.Id,
        //        url: p.Url,
        //        name: FileWriter.PrepareField(p.Name, FileWriter.FieldFormat.DB_TABLE),
        //        sku: FileWriter.PrepareField(p.Sku, FileWriter.FieldFormat.DB_TABLE),
        //        price: FileWriter.PrepareField(p.Price, FileWriter.FieldFormat.DB_TABLE),
        //        category_branch: cs.ToArray(),
        //        image_urls: p.ImageUrls,
        //        description: FileWriter.PrepareField(p.Description, FileWriter.FieldFormat.DB_TABLE),
        //        stock: p.Stock.ToString()
        //    );
        //}

        public void Prepare()
        {
            this.Set("Name", Cliver.PrepareField.Html.GetDbField(this.Name));
            this.Set("Sku", Cliver.PrepareField.Html.GetDbField(this.Sku));
            this.Set("Price", Cliver.PrepareField.Html.GetDbField(this.Price));
            this.Set("Category", Cliver.PrepareField.Html.GetDbField(this.Category));
            this.Set("Description", Cliver.PrepareField.Html.GetDbField(this.Description));
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

