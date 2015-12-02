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

namespace Cliver.ProductOffice
{
    public static class JqueryDataTable
    {
        //public static IQueryable<T> Odrer<T>(DataTables.AspNet.Core.IDataTablesRequest request, IQueryable<T> query, string[] fields/*, string[] searchable_fields*/)
        //{
        //    //int total_count = query.Count();

        //    //    if (!string.IsNullOrEmpty(request.Search.Value))
        //    //    {
        //    //        string s = request.Search.Value.ToLower();
        //    //        foreach (string sf in searchable_fields)
        //    //            query = query.Where(sf + ".ToLower().Contains(" + s + ")", searchable_fields);

        //    //List<string> conditions = new List<string>();
        //    //    query = query.Select("Regex.IsMatch(" + f + ", Regex.Escape(request.Search.Value), RegexOptions.IgnoreCase| RegexOptions.Singleline)");

        //    List<string> ofs = new List<string>();
        //    foreach (var column in request.Columns)
        //    {
        //        if (column.Sort == null)
        //            continue;
        //        string f = column.Field;
        //        int n;
        //        if (int.TryParse(f, out n))
        //            f = fields[n];
        //        if (column.Sort.Direction == DataTables.AspNet.Core.SortDirection.Ascending)
        //            ofs.Add(f + " asc");
        //        else
        //            ofs.Add(f + " desc");
        //    }
        //    if (ofs.Count > 0)
        //        query = query.OrderBy(string.Join(", ", ofs));
        //    else
        //        query = query.OrderBy(fields[0]);

        //    query = query.Skip(request.Start).Take(request.Length);

        //    return query;
        //}

        public class Field
        {
            public readonly string Name;
            public string Entity
            {
                get
                {
                    return Expression != null ? Expression : Name;
                }
            }            
            public readonly bool Searchable = false;
            public readonly string Expression = null;
            public enum OrderMode
            {
                NONE = 0,
                ASC = 1,
                DESC = -1
            }
            public readonly OrderMode Order = OrderMode.NONE;
            public Field(string name, bool searchable = false, OrderMode order = OrderMode.NONE, string expression = null)
            {
                Name = name;
                Searchable = searchable;
                Order = order;
                Expression = expression;
            }
        }

        public static JsonResult Index(DataTables.AspNet.Core.IDataTablesRequest request, DbConnection connection, string from_sql, Field[] fields, bool ignore_first_column_search = true)
        {
            Cliver.Bot.DbConnection dbc = Cliver.Bot.DbConnection.CreateFromNativeConnection(connection);
            return Index(request, dbc, from_sql, fields, ignore_first_column_search);
        }

        public static JsonResult Index(DataTables.AspNet.Core.IDataTablesRequest request, Cliver.Bot.DbConnection dbc, string from_sql, Field[] fields, bool ignore_first_column_search = true)
        {
            try
            {
                from_sql = " " + from_sql;

                //turn around as it is unclear how to pass additional parameters
                if (!string.IsNullOrWhiteSpace(request.Columns.First().Search.Value))
                {
                    List<string> wheres = new List<string>();
                    foreach (string f2c in Regex.Split(request.Columns.First().Search.Value, @"\|", RegexOptions.Singleline))
                    {
                        Match m = Regex.Match(f2c, @"(?'DbField'.*?)\=(?'SearchValue'.*)", RegexOptions.Singleline);
                        if (m.Success)
                        {
                            string search_value = m.Groups["SearchValue"].Value;
                            decimal sv;
                            if (!decimal.TryParse(search_value, out sv))
                                search_value = "'" + search_value + "'";
                            wheres.Add(m.Groups["DbField"].Value + "=" + search_value);
                        }
                        m = Regex.Match(f2c, @"(?'DbField'.*?)\s*LIKE\s*(?'SearchValue'.*)", RegexOptions.Singleline);
                        if (m.Success)
                        {
                            string search_value = m.Groups["SearchValue"].Value;
                            decimal sv;
                            if (!decimal.TryParse(search_value, out sv))
                                search_value = "'" + search_value + "'";
                            wheres.Add(m.Groups["DbField"].Value + " LIKE " + search_value);
                        }
                    }
                    if (wheres.Count > 0)
                    {
                        string select_sql = " WHERE " + string.Join(" AND ", wheres);
                        from_sql += select_sql;
                    }
                }

                int total_count = (int)dbc.Get("SELECT COUNT(ISNULL(" + fields[0].Entity + ", 0))" + from_sql).GetSingleValue();

                int filtered_count = total_count;

                string where_sql = null;
                if (!string.IsNullOrEmpty(request.Search.Value))
                {
                    string search = Regex.Replace(request.Search.Value.ToLower(), @"\'|\%|\\|_", @"\$0", RegexOptions.Compiled | RegexOptions.Singleline);
                    List<string> conditions = new List<string>();
                    foreach (Field f in fields.Where(r => r.Searchable))
                        conditions.Add("" + f.Entity + " LIKE '%" + search + "%'");
                    if (conditions.Count > 0)
                    {
                        if (from_sql.Contains(" WHERE "))
                            where_sql += " AND ";
                        else
                            where_sql += " WHERE "; 
                        where_sql += "(" + string.Join(" OR ", conditions) + @" ESCAPE '\')";
                    }
                }
                if (!string.IsNullOrWhiteSpace(where_sql))
                    filtered_count = (int)dbc.Get("SELECT COUNT(ISNULL(" + fields[0].Entity + ", 0))" + from_sql + where_sql).GetSingleValue();

                Dictionary<string, int> of2nothing = new Dictionary<string,int>(); 
                List<string> ofs = new List<string>();
                foreach (var column in request.Columns)
                {
                    if (ignore_first_column_search)
                    {
                        ignore_first_column_search = false;
                        continue;
                    }
                    if (column.Sort == null)
                        continue;
                    string f = column.Field;
                    int n;
                    if (int.TryParse(f, out n))
                        f = fields[n].Entity;
                    if (column.Sort.Direction == DataTables.AspNet.Core.SortDirection.Ascending)
                        ofs.Add(f);
                    else
                        ofs.Add(f + " DESC");
                    of2nothing[f] = 0;
                }
                foreach (Field field in fields)
                {
                    if (field.Order == Field.OrderMode.NONE)
                        continue;
                    if(of2nothing.ContainsKey(field.Name))
                        continue;
                    if (field.Order == Field.OrderMode.ASC)
                        ofs.Add(field.Entity);
                    else
                        ofs.Add(field.Entity + " DESC");
                    of2nothing[field.Name] = 0;
                }
                string order_sql = " ORDER BY ";
                if (ofs.Count > 0)
                    order_sql += string.Join(", ", ofs);
                else
                    order_sql += fields[0].Entity;

                string fields_sql = string.Join(",", (from r in fields select r.Expression != null ? r.Expression + " AS " + r.Name : r.Name));
                string sql = "SELECT " + fields_sql + from_sql + where_sql + order_sql + " OFFSET " + request.Start + " ROWS FETCH NEXT " + request.Length + " ROWS ONLY";
                List<object[]> array = new List<object[]>();
                using (DbDataReader r = (DbDataReader)dbc.Get(sql).GetReader())
                {
                    string search = null;
                    if (!string.IsNullOrEmpty(request.Search.Value))
                        search = Regex.Escape(request.Search.Value);
                    while (r.Read())
                    {
                        object[] vs = new object[fields.Length];
                        r.GetValues(vs);
                        if (search != null)
                        {
                            for (int i = 0; i < fields.Length; i++)
                                if (fields[i].Searchable)
                                    if(!(vs[i] is System.DBNull))
                                        vs[i] = Regex.Replace((string)vs[i], search, @"<span class='match'>$0</span>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        }
                        array.Add(vs);
                    }
                }

                DataTables.AspNet.Mvc5.DataTablesResponse response = DataTables.AspNet.Mvc5.DataTablesResponse.Create(request, total_count, filtered_count, array);
                return new DataTables.AspNet.Mvc5.DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                DataTables.AspNet.Mvc5.DataTablesResponse response = DataTables.AspNet.Mvc5.DataTablesResponse.Create(request, e.Message);
                return new DataTables.AspNet.Mvc5.DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            }
        }

        class Row
        {
            public string[] Values;

            public Row(params string[] values)
            {
                Values = values;
            }
        }
    }
}