using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Entity;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Specialized;
using System.Xml;
using Cliver.Fhr.ProductOffice.Models;

namespace Cliver.ProductOffice
{
    class CategoryRoutines
    {
        static public Dictionary<string, dynamic> GetCompanyCategories(DbApi db, int company_id)
        {
            List<string> categories = db.Products.Where(p => p.CompanyId == company_id).GroupBy(p => p.Category).Select(c => c.Key).ToList();
            Dictionary<string, dynamic> tree = build_tree_from_paths(categories);
            return tree;
        }

        static Dictionary<string, dynamic> build_tree_from_paths(List<string> paths)
        {
            Dictionary<string, dynamic> tree = new Dictionary<string, dynamic>();
            foreach (string path in paths)
                if (path != null)
                    add_path(path, tree);
            return tree;
        }

        static void add_path(string path, Dictionary<string, dynamic> tree)
        {
            Match m = Regex.Match(path, "^(.*?)" + Regex.Escape(Fhr.ProductOffice.DataApi.Product.CATEGORY_SEPARATOR) + "+(.+)", RegexOptions.Compiled | RegexOptions.Singleline);
            if (m.Success)
            {
                string name = m.Groups[1].Value;
                dynamic child_tree;
                if (!tree.TryGetValue(name, out child_tree))
                {
                    child_tree = new Dictionary<string, object>();
                    tree[name] = child_tree;
                }
                add_path(m.Groups[2].Value, child_tree);
            }
            else
                tree[path] = new Dictionary<string, object>();
        }
    }
}