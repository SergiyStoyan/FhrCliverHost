//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Runtime.Serialization;

namespace Cliver.DataSifter
{
    static internal class FilterApi
    {
        static FilterApi()
        {
            assembly = Assembly.GetExecutingAssembly();
            filter_type_name2filter_type = (from x in assembly.GetTypes() where x.IsSubclassOf(typeof(Filter)) select x).ToDictionary(x => x.Name, x => x);

            foreach (string ftn in filter_type_name2filter_type.Keys)
            {
                ConstructorInfo ci = filter_type_name2filter_type[ftn].GetConstructor(new Type[] { typeof(Version), typeof(string), typeof(string), typeof(string) });
                if (ci == null)
                    throw new Exception("Filter " + ftn + " has no constructor with parameters: Version, string");
            }
        }
        static readonly Assembly assembly;
        readonly static Dictionary<string, Type> filter_type_name2filter_type = new Dictionary<string, Type>();

        internal static List<Type> GetFilterTypes()
        {
            return filter_type_name2filter_type.Values.ToList();
        }

        internal static Filter CreateFilter(string filter_type_name, string version, string filter_node_definition, string input_group_name, string comment)
        {
            Type ft;
            if (!filter_type_name2filter_type.TryGetValue(filter_type_name, out ft))
                throw new Exception("Filter " + filter_type_name + " is not defined");
            Version v;
            if (!Version.TryParse(version, out v))
                throw new Exception("Version string '" + version + "' could not be recornized");
            return CreateFilter(ft, v, filter_node_definition, input_group_name, comment);
        }

        internal static Filter CreateFilter(Type filter_type, Version version, string filter_node_definition, string input_group_name, string comment)
        {
            try
            {
                return (Filter)Activator.CreateInstance(filter_type, version, filter_node_definition, input_group_name, comment);
            }
            catch (Exception e)
            {
                for (; e.InnerException != null; e = e.InnerException) ;
                throw new Exception("The filter is not valid.\n" + e.Message);
            }
        }

        internal static Filter CreateDefaultFilter(Type filter_type)
        {
            Filter f = (Filter)FormatterServices.GetUninitializedObject(filter_type);
            return FilterApi.CreateFilter(filter_type, f.Version, null, null, null);
        }

        internal static string GetFilterReadableTypeName(Type filter_type)
        {
            return ((Filter)FormatterServices.GetUninitializedObject(filter_type)).ReadableTypeName;
            //return (string)filter_type.GetField("Name", BindingFlags.Static | BindingFlags.NonPublic| BindingFlags.Public).GetValue(null);
        }

        //internal static string GetFilterTypeName(Type filter_type)
        //{
        //    return ((Filter)FormatterServices.GetUninitializedObject(filter_type)).TypeName;
        //}

        internal static Version GetFilterVersion(Type filter_type)
        {
            return ((Filter)FormatterServices.GetUninitializedObject(filter_type)).Version;
            //return (string)filter_type.GetField("Version", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).GetValue(null);
        }
    }
}
