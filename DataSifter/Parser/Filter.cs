//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        17 February 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Cliver.DataSifter
{
    public abstract class Filter
    {
        abstract public Version Version
        {
            get;
        }

        public Filter(Version version, string input_group_name, string comment)
        {
            if (version > Version)
                throw new Exception("Version of the filter " + version.ToString() + " is older then version of the node definition:\n" + Version.ToString() + "<" + version.ToString());           
            
            InputGroupName = input_group_name;
            Comment = comment;
        }
        
        /// <summary>
        /// must be invoked by constructor of inheriting class
        /// </summary>
        protected void fill_group_index2raw_group_names()
        {
            string[] gns = GetGroupRawNames();
            for (int i = 0; i < gns.Length; i++)
            {
                string gn = gns[i];
                group_index2group_raw_names[i] = gn;
                //int gn_i;
                //if (!int.TryParse(gn, out gn_i))
                //    group_index2group_names[i] = gn;
            }
        }

        public Filter[] Next = new Filter[0];

        public string InputGroupName;

        public string Comment;

        /// <summary>
        /// If group has no name, its name is its index
        /// </summary>
        /// <returns></returns>
        abstract public string[] GetGroupRawNames();
        
        public int GetGroupIndexByRawName(string group_raw_name)
        {
            for (int i = 0; i < group_index2group_raw_names.Count; i++)
                if (group_index2group_raw_names[i] == group_raw_name)
                    return i;
            return -1;
        }

        /// <summary>
        /// If no name, return null
        /// </summary>
        /// <param name="group_index"></param>
        /// <returns></returns>
        public string GetGroupNameByIndex(int group_index)
        {
            string gn = group_index2group_raw_names[group_index];
            int gi;
            if (string.IsNullOrWhiteSpace(gn) || int.TryParse(gn, out gi))
                return null;
            return gn;
        }

        public string GetGroupRawNameByIndex(int group_index)
        {
            return group_index2group_raw_names[group_index];
        }

        /// <summary>
        /// Keys are indexes of named groups. 
        /// i => output_group_name
        /// </summary>
        //Dictionary<int, string> group_index2group_names = new Dictionary<int, string>();

        Dictionary<int, string> group_index2group_raw_names = new Dictionary<int, string>();
        
        abstract public string GetDefinition();
        abstract public FilterMatchCollection Matches(FilterGroup parent_group);
        
        protected string get_default_definition()
        {
            string d = null;
            Settings.Default.FilterTypeName2NewFilter.TryGetValue(this.GetType().Name, out d);
            return d;
        }

        /// <summary>
        /// Keys are names of named groups.
        /// output_group_name => List
        /// </summary>
        internal Dictionary<string, List<string>> group_name2child_filters_group_names = new Dictionary<string, List<string>>();

        #region used by DataSifter

        internal int Level = 0;

        abstract internal FilterControl CreateControl();
        
        abstract internal string ReadableTypeName
        {
            get;
        }

        //public static bool operator ==(Filter a, Filter b)
        //{
        //    return a.GetDefinition() == b.GetDefinition();
        //}

        //public static bool operator !=(Filter a, Filter b)
        //{
        //    return !(a == b);
        //}

        abstract public string GetVisibleDefinition();

        #endregion
    }

    public class FilterMatch
    {
        readonly public List<FilterGroup> Groups;

        public FilterMatch(List<FilterGroup> groups)
        {
            Groups = groups;
        }
    }
    
    public class FilterGroup
    {
        readonly internal int Index;

        internal int Length
        {
            get
            {
                return Text.Length;
            }
        }
        readonly internal string Text;

        readonly internal FilterGroup ParentGroup;
        readonly internal int RelativeIndex;

        public FilterGroup(FilterGroup parent_group, int relative_index, string text)
        {
            RelativeIndex = relative_index;
            Text = text;
            ParentGroup = parent_group;
            Index = relative_index;
            if (parent_group != null)
                Index += parent_group.Index;
        }
    }
    
     abstract public class FilterMatchCollection : IEnumerable<FilterMatch>, IEnumerator<FilterMatch>
     {
         public FilterMatchCollection(FilterGroup parent_group)
         {
             ParentGroup = parent_group;
         }
         
         readonly internal FilterGroup ParentGroup;

         public IEnumerator<FilterMatch> GetEnumerator()
         {
             return (IEnumerator<FilterMatch>)this;
         }
         System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
         {
             return GetEnumerator();
         }

         abstract public void Reset();
         abstract public FilterMatch Current
         {
             get;
         }
         object System.Collections.IEnumerator.Current
         {
             get
             {
                 return this.Current;
             }
         }
         abstract public bool MoveNext();

         public void Dispose()
         {
         }
     }

     //public interface IParsableObject
     //{
     //    string ToString();
     //}
}
