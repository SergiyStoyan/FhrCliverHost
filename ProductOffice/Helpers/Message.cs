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
    public class StringOrderedDictionary : OrderedDictionary
    {
        public List<string> this[string key]
        {
            get
            {
                List<string> l = (List<string>)base[key];
                if (l == null)
                {
                    l = new List<string>();
                    base[key] = l;
                }
                return l;
            }
        }
    }

    public class Errors
    {
        public static string Expand(Exception e)
        {
            List<string> ms = new List<string>();
            for (; e != null; e = e.InnerException)
                ms.Insert(0, e.Message);
            return string.Join("<br/>\r\n", ms);
        }

        public static void Add(string message)
        {
            ((List<string>)Dictionary[""]).Add(message);
        }

        public static void Add(string title, string message)
        {
            ((List<string>)Dictionary[title]).Add(message);
        }

        public static void Insert(string message)
        {
            ((List<string>)Dictionary[""]).Insert(0, message);
        }

        public static void Insert(string title, string message)
        {
            ((List<string>)Dictionary[title]).Insert(0, message);
        }

        public static void Insert(string title, int index, string message)
        {
            ((List<string>)Dictionary[title]).Insert(index, message);
        }

        public static StringOrderedDictionary Dictionary
        {
            get
            {
                StringOrderedDictionary title2ms = (StringOrderedDictionary)System.Web.HttpContext.Current.Items["Errors"];
                if (title2ms == null)
                {
                    title2ms = new StringOrderedDictionary();
                    System.Web.HttpContext.Current.Items["Errors"] = title2ms;
                }
                return title2ms;
            }
        }

        public static System.Collections.ICollection Titles
        {
            get
            {
                return Dictionary.Keys;
            }
        }
    }

    public class Messages
    {
        public static void Add(string message)
        {
            ((List<string>)Dictionary[""]).Add(message);
        }

        public static void Add(string title, string message)
        {
            ((List<string>)Dictionary[title]).Add(message);
        }

        public static void Insert(string message)
        {
            ((List<string>)Dictionary[""]).Insert(0, message);
        }

        public static void Insert(string title, string message)
        {
            ((List<string>)Dictionary[title]).Insert(0, message);
        }

        public static void Insert(string title, int index, string message)
        {
            ((List<string>)Dictionary[title]).Insert(index, message);
        }

        public static StringOrderedDictionary Dictionary
        {
            get
            {
                StringOrderedDictionary title2ms = (StringOrderedDictionary)System.Web.HttpContext.Current.Items["Messages"];
                if (title2ms == null)
                {
                    title2ms = new StringOrderedDictionary();
                    System.Web.HttpContext.Current.Items["Messages"] = title2ms;
                }
                return title2ms;
            }
        }

        public static System.Collections.ICollection Titles
        {
            get
            {
                return Dictionary.Keys;
            }
        }
    }
}