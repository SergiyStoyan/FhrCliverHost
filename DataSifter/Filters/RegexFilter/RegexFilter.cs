//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        17 February 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Xml;
using System.Configuration;
using Cliver.DataSifter;

namespace Cliver
{
    internal class RegexFilter : Filter
    {
        override public Version Version
        {
            get
            {
                return new Version(1, 0);
            }
        }

        public RegexFilter(Version version, string defintion, string input_group_name, string comment)
            : base(version, input_group_name, comment)
        {
            if (defintion == null)
                defintion = get_default_definition();
            if (defintion == null)
                defintion = "\n";
            Match m = Regex.Match(defintion, @"(?'Regex'.*)\n(?'RegexOptions'.*)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (!m.Success)
                throw new Exception("Filter definition could not be parsed:\n" + defintion);
            RegexOptions options = RegexOptions.None;
            Enum.TryParse(m.Groups["RegexOptions"].Value, out options);
            Regex = new Regex(m.Groups["Regex"].Value, options);
            fill_group_index2raw_group_names();
        }

        internal Regex Regex;

        override public string[] GetGroupRawNames()
        {
            return Regex.GetGroupNames();
        }

        override public string GetDefinition()
        {
            return Regex.ToString() + "\n" + Regex.Options.ToString();
        }

        override public FilterMatchCollection Matches(FilterGroup group)
        {
            return new RegexMatches(group, Regex);
        }

        override internal FilterControl CreateControl()
        {
            RegexControl c = new RegexControl(this);
            return c;
        }

        //internal override bool IsEqual(Filter filter)
        //{
        //    if (filter == null)
        //        return false;
        //    RegexFilter node = filter as RegexFilter;
        //    if (node == null)
        //        return false;
        //    if (InputGroupName == node.InputGroupName
        //        && Regex.ToString() == node.Regex.ToString()
        //        && Regex.Options == node.Regex.Options
        //        )
        //        return true;
        //    return false;
        //}  

        override internal string ReadableTypeName
        {
            get
            {
                return ".NET Regex";
            }
        }

        public override string GetVisibleDefinition()
        {
            return Regex.ToString();
        }
    }

    public class RegexMatches : FilterMatchCollection
    {
        public RegexMatches(FilterGroup parent_group, Regex Regex)
            : base(parent_group)
        {
            m0 = Regex.Match(parent_group.Text);
            if (!m0.Success)
                return;
            Reset();
        }
        readonly Match m0 = null;
        Match m;
        
        override public void Reset()
        {
            if (m0 == null)
                return;
            m = m0;
            reset = true;
        }
        bool reset = false;
        
        override public FilterMatch Current
        {
            get
            {
                if (m == null)
                    return null;
                return new FilterMatch((from x in m.Groups.Cast<Group>() select new FilterGroup(ParentGroup, x.Index, x.Value)).ToList<FilterGroup>());
            }
        }
        
        override public bool MoveNext()
        {
            if (m == null)
               return false;
            if (reset)
                reset = false;
            else
                m = m.NextMatch();
            return m.Success;
        }
    }

    //public class RegexFilterGroup : FilterGroup
    //{
    //}

    //public class StringParsableObject : IParsableObject
    //{
    //    public StringParsableObject(string text)
    //    {
    //        this.text = text;
    //    }
        
    //    readonly string text;

    //    public string ToString()
    //    {
    //        return text;
    //    }
    //}
}