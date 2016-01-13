using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;

namespace Cliver.ProductIdentifier
{
    internal class Products
    {
        internal Products(Engine engine)
        {
            this.engine = engine;
        }
        readonly Engine engine;

        internal Product Get(int product_id)
        {
            engine.sw9.Start();
            Product p = null;
            if (!product_ids2Product.TryGetValue(product_id, out p))
            {
                Fhr.ProductOffice.Models.Product product = engine.Db.Products.Where(x => x.Id == product_id).FirstOrDefault();
                if (product == null)
                    throw new Exception("No Product with Id=" + product_id);
                p = new Product(engine, product);
                product_ids2Product[product.Id] = p;
            }
            engine.sw9.Stop();
            return p;
        }

        internal Product[] GetLinked(int link_id)
        {
            List<Product> ps = new List<Product>();
            foreach (Fhr.ProductOffice.Models.Product product in engine.Db.Products.Where(p => p.LinkId == link_id))
            {
                Product p = null;
                if (!product_ids2Product.TryGetValue(product.Id, out p))
                {
                    p = new Product(engine, product);
                    product_ids2Product[product.Id] = p;
                }
                ps.Add(p);
            }
            return ps.ToArray();
        }

        //public static Product Get(int product_id)
        //{
        //    Product p = null;
        //    if (!product_ids2Product.TryGetValue(product_id, out p))
        //    {
        //        Fhr.ProductOffice.Product dp = Fhr.Db.Products.Where(x => x.Id == product_id).FirstOrDefault();
        //        p = new Product(dp);
        //        product_ids2Product[product_id] = p;
        //    }
        //    return p;
        //}

        //public static Product Get(Fhr.ProductOffice.Product product)
        //{
        //    return Get(product.Id);
        //}
        Dictionary<int, Product> product_ids2Product = new Dictionary<int, Product>();

        internal void Ininitalize(ICollection<Fhr.ProductOffice.Models.Product> products)
        {
            foreach (Fhr.ProductOffice.Models.Product product in products)
            {
                if (product == null)
                    throw new Exception("product is null");
                Product p = null;
                if (!product_ids2Product.TryGetValue(product.Id, out p))
                {
                    p = new Product(engine, product);
                    product_ids2Product[product.Id] = p;
                }
            }
        }

        internal List<Product> All
        {
            get
            {
                return product_ids2Product.Values.ToList();
            }
            set
            {
                foreach (Product p in value)
                    product_ids2Product[p.DbProduct.Id] = p;
            }
        }
    }
}
