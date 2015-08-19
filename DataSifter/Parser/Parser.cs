//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        17 February 2008
//Copyright: (C) 2008, Sergey Stoyan
//********************************************************************************************

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;

namespace Cliver.DataSifter
{
    /// <summary>
    /// Engine for parsing text with filter tree.
    /// </summary>
    public class Parser
    {
        #region constructors
        /// <summary>
        /// Create Parser for the filter tree file.
        /// </summary>
        /// <param name="filter_file">filter tree file</param>
        public Parser(string filter_file)
        {
            if (filter_file == null)
                throw (new Exception("Tree file name passed to the constructor is null."));

            RootFilters = read_filter_tree_from_xml_file(filter_file);
            build_node_name_tree();
        }

        /// <summary>
        /// used by DataSifter
        /// </summary>
        /// <param name="root_filters"></param>
        internal Parser(Filter[] root_filters)
        {
            if (root_filters == null)
                throw (new Exception("Filter array passed to the constructor is null."));

            RootFilters = root_filters;
            Filter[] fs = RootFilters;
            while (fs.Length > 0)
            {
                Filter f = fs[0];
                f.group_name2child_filters_group_names.Clear();
                fs = f.Next;
            }

            build_node_name_tree();

            DisplayMode = true;
        }
        bool DisplayMode = false;
        
        /// <summary>
        /// Creates a tree of named groups so that each Capture to have a complete name collection 
        /// independently on parse results.
        /// ALSO(!) it checks filter tree for group name uniquness within the same branch-level.
        /// </summary>
        void build_node_name_tree()
        {
            foreach (Filter f in RootFilters)
                f.InputGroupName = ROOT_INPUT_GROUP_NAME;

            group_name2child_filters_group_names0[ROOT_INPUT_GROUP_NAME] = new System.Collections.Generic.List<string>();
            //warning = null;
            get_next_level_group_names(RootFilters, ROOT_INPUT_GROUP_NAME, group_name2child_filters_group_names0[ROOT_INPUT_GROUP_NAME]);
            //if (warning != null)
            //    Message.Ok(System.Drawing.SystemIcons.Warning, warning);
            //warning = null;
            check_input_names_existance(RootFilters);

            //convert_child_group_names2string_array(FILTER_NODE0);
        }
        Dictionary<string, List<string>> group_name2child_filters_group_names0 = new Dictionary<string, List<string>>();
        //string warning = null;

        /// <summary>
        /// Create a tree of named groups so that each Capture to have a complete name collection independently on parse results. 
        /// ALSO(!) it checks the filter tree for group name uniquness within the same branch-level.
        /// </summary>
        void get_next_level_group_names(Filter[] filters, string parent_filter_output_group_name, List<string> filters_output_group_names)
        {
            foreach (Filter f in filters)
            {
                if (parent_filter_output_group_name != f.InputGroupName)
                    continue;

                string[] gns = f.GetGroupRawNames();
                for (int i = 0; i < gns.Length; i++)
                {
                    string gn = f.GetGroupNameByIndex(i);
                    if (gn == null) //not named group
                    {
                        //if (f.Next.Length == 0)
                        //    warning = "Last level filter '" + f.GetDefinition() + "' has no named group so it is useless as can't be referenced.";
                        get_next_level_group_names(f.Next, f.GetGroupRawNameByIndex(i), filters_output_group_names);
                    }
                    else
                    {//named group
                        if (filters_output_group_names.Contains(gn))
                            throw new Exception("Named group '" + gn + "' is used twice within the same filter tree branch and level. It will bring to incorrect parsing.");
                        filters_output_group_names.Add(gn);

                        f.group_name2child_filters_group_names[gn] = new List<string>();
                        get_next_level_group_names(f.Next, gn, f.group_name2child_filters_group_names[gn]);
                    }
                }
            }
        }

        void check_input_names_existance(Filter[] filters)
        {
            foreach (Filter f in filters)
            {
                foreach (Filter cf in f.Next)
                {
                    if (f.GetGroupIndexByRawName(cf.InputGroupName) < 0)
                        throw new Exception("Input group name '" + cf.InputGroupName + "' does not exists among group names of the parent node.\nParent node: " + f.GetDefinition());
                }
                check_input_names_existance(f.Next);
            }
        }

        //void convert_child_group_names2string_array(Filter filter)
        //{
        //        string[] keys = new string[filter.child_filters_group_names.Keys.Count];
        //        filter.child_filters_group_names.Keys.CopyTo(keys, 0);
        //        foreach (string group_name in keys)
        //        {
        //            List<string> cgns = ((List<string>)filter.child_filters_group_names[group_name]);
        //            filter.child_filters_group_names[group_name] = (string[])cgns.ToArray(typeof(string));
        //        }

        //        foreach (Filter tn in filter.Next)
        //            convert_child_group_names2string_array(tn);        //   
        //}

        /// <summary>
        /// Start level TlNodes collection.
        /// (!)Changes in this data will bring to error or incorrect parsing.
        /// </summary>
        internal readonly Filter[] RootFilters;
        
        internal const string ROOT_INPUT_GROUP_NAME = "";

        #endregion

        #region Parse method

        /// <summary>
        /// Perform parsing.
        /// </summary>
        /// <param name="text">text that is to be parsed with the tree</param>
        /// <returns>Zero Capture that the tree of CroupCapture's starts from</returns>
        public Capture Parse(string text)
        {
            //if (parsable_object == null)
            //    throw new Exception("The input parsable_object is null");
            //string text = parsable_object.ToString();
            Capture capture0 = new Capture(ROOT_INPUT_GROUP_NAME, text, 0, group_name2child_filters_group_names0[ROOT_INPUT_GROUP_NAME]); 
            FilterGroup fg = new FilterGroup(null, 0, text);
            foreach (Filter f in RootFilters)
                parse(capture0, fg, f);
            return capture0;
        }

        void parse(Capture parent_capture, FilterGroup parent_filter_group, Filter filter)
        {
            foreach (FilterMatch fm in filter.Matches(parent_filter_group))
            {
                for (int group_i = 0; group_i < fm.Groups.Count; group_i++)
                {
                    FilterGroup fg = fm.Groups[group_i];

                    if (DisplayMode)
                        SourceForm.This.AddCaptureLabel(fg.Index, fg.Length, filter.Level, group_i, filter.GetGroupRawNameByIndex(group_i), fm.Groups.Count);

                    Capture gc = null;
                    string output_gn = filter.GetGroupNameByIndex(group_i);
                    if (output_gn != null)
                    {//it is named group
                        //get named groups
                        gc = new Capture(output_gn, fg.Text, fg.Index, filter.group_name2child_filters_group_names[output_gn]);
                        parent_capture.AddChildGroupCapture(gc);
                    }

                    string grn = filter.GetGroupRawNameByIndex(group_i);
                    foreach (Filter child_f in filter.Next)
                    {
                        if (grn != child_f.InputGroupName)
                            continue;
                        if (gc != null)//it is output(named) group
                            parse(gc, fg, child_f);
                        else
                            parse(parent_capture, fg, child_f);
                    }
                }
            }
        }

        #endregion

        #region reading from/writing to a tree file

        Filter[] read_filter_tree_from_xml_file(string filter_file)
        {
            try
            {
                XmlDocument x = new XmlDocument();
                x.Load(filter_file);

                XmlNode xn = x.SelectSingleNode("FilterTree");
                
                return get_filters(xn, null);
            }
            catch (Exception e)
            {
                throw new Exception("Could not load '" + filter_file + "'\r\n\r\n" + e.Message);
            }
            //catch (Exception e)
            //{
            //    throw new Exception("Could not load '" + filter_tree_file + "' because of unexpected format.\r\n\r\n" + e.Message);
            //}
        }

        Filter[] get_filters(XmlNode xml_node, Filter parent_filter)
        {
            XmlNodeList xns = xml_node.SelectNodes("Filter");
            Filter[] fs = new Filter[xns.Count];
            int i = 0;
            foreach (XmlNode xn in xns)
            {
                string input_group_name = null;
                XmlAttribute xa = xn.Attributes["input_group"];
                if (xa != null && xa.Value != ROOT_INPUT_GROUP_NAME)
                {//it is not zero node
                    if (parent_filter == null)
                        throw new Exception("Input group name is sepcified while no parent filter exists");
                    input_group_name = xa.Value.Trim();
                    if (input_group_name.StartsWith("$"))
                    {
                        input_group_name = input_group_name.Substring(1);
                        int igi = parent_filter.GetGroupIndexByRawName(input_group_name);
                        if (igi < 0)
                            throw new Exception("Input group name does not exists among group names of parent filter.\nParent filter: " + parent_filter.GetDefinition());
                    }
                }

                string comment = "";
                xa = xn.Attributes["comment"];
                if (xa != null)
                    comment = xa.Value;

                Filter f = FilterApi.CreateFilter(xn.Attributes["type"].Value.Trim(), xn.Attributes["version"].Value.Trim(), xn.Attributes["definition"].Value, input_group_name, comment);

                fs[i++] = f;
                f.Next = get_filters(xn, f);
            }
            return fs;
        }

        internal void WriteFilterTreeToXmlFile(string filter_file)
        {
            XmlDocument xd = new XmlDocument();

            xd.AppendChild(xd.CreateXmlDeclaration("1.0", "utf-8", null));
            xd.AppendChild(xd.CreateComment("Filter tree file. Produced by "  + Program.AppTitle + ", http://www.cliversoft.com"));

            XmlElement pxe = xd.CreateElement("FilterTree");
            
            add_xml_nodes(xd, pxe, RootFilters);

            xd.AppendChild(pxe);

            xd.Save(filter_file);
        }

        void add_xml_nodes(XmlDocument xd, XmlElement pxe, Filter[] filters)
        {
            foreach (Filter tn in filters)
            {
                XmlElement xe = xd.CreateElement("Filter");
                xe.SetAttribute("type", tn.GetType().Name);
                xe.SetAttribute("version", tn.Version.ToString());
                xe.SetAttribute("input_group", tn.InputGroupName);
                xe.SetAttribute("definition", tn.GetDefinition());
                xe.SetAttribute("comment", tn.Comment);
                pxe.AppendChild(xe);

                add_xml_nodes(xd, xe, tn.Next);
            }
        }
        #endregion
    }
}



