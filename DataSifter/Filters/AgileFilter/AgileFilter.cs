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
    internal class AgileFilter : Filter
    {
        override public Version Version
        {
            get
            {
                return new Version(1, 0);
            }
        }

        public AgileFilter(Version version, string defintion, string input_group_name, string comment)
            : base(version, input_group_name, comment)
        {
            if (defintion == null)
                defintion = get_default_definition();
            if (defintion == null)
                defintion = "\n";
            Match m = Regex.Match(defintion, @"(?'Xpath'.*)\n(?'GroupName'.*)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (!m.Success)
                throw new Exception("Filter definition could not be parsed:\n" + defintion);
            Xpath = m.Groups["Xpath"].Value;
            GroupName = m.Groups["GroupName"].Value;
            InputGroupName = input_group_name;
            fill_group_index2raw_group_names();
        }

        internal string Xpath;
        internal string GroupName;

        override public string[] GetGroupRawNames()
        {
            if (string.IsNullOrWhiteSpace(GroupName))
                GroupName = "0";
            return new string[] { GroupName };
        }

        override public string GetDefinition()
        {
            if (string.IsNullOrWhiteSpace(GroupName))
                GroupName = "0";
            return Xpath + "\n" + GroupName;
        }

        override public FilterMatchCollection Matches(FilterGroup parent_group)
        {
            return new AgileMatches(parent_group, Xpath);
        }

        override internal FilterControl CreateControl()
        {
            AgileControl c = new AgileControl(this);
            return c;
        }

        //internal override bool IsEqual(Filter filter)
        //{
        //    if (filter == null)
        //        return false;
        //    AgileFilter node = filter as AgileFilter;
        //    if (node == null)
        //        return false;
        //    if (InputGroupName == node.InputGroupName
        //        && Xpath == node.Xpath
        //        )
        //        return true;
        //    return false;
        //}


        override internal string ReadableTypeName
        {
            get
            {
                return "Html Xpath";
            }
        }

        public override string GetVisibleDefinition()
        {
            return Xpath;
        }
    }

    public class AgileMatches : FilterMatchCollection
    {
        public AgileMatches(FilterGroup parent_group, string Xpath)
            : base(parent_group)
        {
            HtmlAgilityPack.HtmlNodeCollection ns = null;
            //if (parent_group is AgileFilterGroup)
            //{
            //    ns = ((AgileFilterGroup)parent_group).HtmlNode.SelectNodes(Xpath);
            //}
            //else
            //{
                HtmlAgilityPack.HtmlDocument d = new HtmlAgilityPack.HtmlDocument();
                d.LoadHtml(parent_group.Text);
                ns = d.DocumentNode.SelectNodes(Xpath);
            //}
            if (ns == null)
                return;
            node_enum = ns.AsEnumerable<HtmlAgilityPack.HtmlNode>().GetEnumerator();
        }
        readonly IEnumerator<HtmlAgilityPack.HtmlNode> node_enum = null;
                
        override public void Reset()
        {
            if (node_enum == null)
                return;
            node_enum.Reset();
        }

        override public FilterMatch Current
        {
            get
            {
                if (node_enum == null)
                    return null;
                //return new FilterMatch(new List<FilterGroup>() { new AgileFilterGroup(node_enum.Current.StreamPosition, node_enum.Current.OuterHtml, node_enum.Current) });
                List<HtmlAgilityPack.HtmlNode> ns = new List<HtmlAgilityPack.HtmlNode>();
                for (HtmlAgilityPack.HtmlNode n = node_enum.Current; n != null; n = n.ParentNode)
                {
                    ns.Add(n);
                    if (n.NextSibling != null)
                        break;
                }
                int end_position = -1;
                if (ns[ns.Count - 1].NextSibling != null)
                    end_position = ns[ns.Count - 1].NextSibling.StreamPosition;
                else
                    end_position = ParentGroup.Length;
                if (ns.Count > 1)
                {
                    string text = ParentGroup.Text.Substring(0, end_position);
                    for (int i = ns.Count - 1; i > 0; i--)
                    {
                        HtmlAgilityPack.HtmlNode n = ns[i];
                        if (n.Closed)
                        {
                            Match m = Regex.Match(text, @"\s*<\s*/\s*" + n.Name + @"\s*[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                            if (m.Success)
                            {
                                end_position = m.Index;
                                text = text.Substring(0, m.Index);
                            }
                        }
                    }
                }

                return new FilterMatch(new List<FilterGroup>() { new FilterGroup(ParentGroup, node_enum.Current.StreamPosition, ParentGroup.Text.Substring(node_enum.Current.StreamPosition, end_position - node_enum.Current.StreamPosition)) });
            }
        }

        override public bool MoveNext()
        {
            if (node_enum == null)
                return false;
            return node_enum.MoveNext();
        }
    }

    //public class AgileFilterGroup: FilterGroup
    //{
    //    readonly public HtmlAgilityPack.HtmlNode HtmlNode = null;

    //    public AgileFilterGroup(int index, string text, HtmlAgilityPack.HtmlNode html_node)
    //        : base(index, text)
    //    {
    //        HtmlNode = html_node;
    //    }
    //}

    //public class AgileParsableObject : IParsableObject
    //{
    //    public AgileParsableObject(HtmlAgilityPack.HtmlNode html_node)
    //    {
    //        this.html_node = html_node;
    //    }
        
    //    readonly HtmlAgilityPack.HtmlNode html_node;

    //    public string ToString()
    //    {
    //        return html_node.InnerHtml;
    //    }
    //}
}