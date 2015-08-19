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


namespace Cliver.DataSifter
{
    /// <summary>
    /// Node of tree of captures being a node of a data parsed tree (result of parsing data with a filter tree).
    /// </summary>
    public class Capture
    {
        /// <summary>
        /// Child Capture's for the key. 
        /// </summary>
        /// <param name="key">group name of child filter</param>
        /// <returns>child Capture's</returns>
        public Capture[] this[string key]
        {
            get
            {
                List<Capture> a;
                if (!child_group_captures.TryGetValue(key, out a))
                    throw (new Exception("Key '" + key + "' specified in the Capture path could not be found."));
                return a.ToArray();
            }
        }

        /// <summary>
        /// Child Capture for the key and the index. 
        /// It is almost the same like [key][index] 
        /// but more efficient since does not build an array of strings.
        /// Also it is more safe in index use: returns null if index is out of range.
        /// </summary>
        /// <param name="key">group name of child filter</param>
        /// <param name="index">index within array of captures</param>
        /// <returns>child Capture</returns>
        public Capture this[string key, int index]
        {
            get
            {
                List<Capture> al;
                if (!child_group_captures.TryGetValue(key, out al))
                    throw (new Exception("Key '" + key + "' does NOT exist."));

                if (al.Count <= index || index < 0)
                    return null;

                return al[index];
            }
        }

        /// <summary>
        /// Length of array of child Capture's for the key. 
        /// It is the same like [key].Length but more efficient.
        /// </summary>
        /// <param name="key">group name of child filter</param>
        /// <returns>length of array of child Capture's</returns>
        public int LengthOf(string key)
        {
            List<Capture> a;
            if (!child_group_captures.TryGetValue(key, out a))
                throw (new Exception("Key '" + key + "' does NOT exist."));
            return a.Count;
        }

        ///// <summary>
        ///// Length of array of child Capture's for the path.
        ///// Usually it makes sense when path is a key or ends with a key.
        ///// </summary>
        ///// <param name="path">path specified in the following manner: "key[, index][/ key[, index]]...", 
        ///// where key is a group name of child filter, index is integer or '*'</param>
        ///// <returns>length of array of child Capture's</returns>
        //public int LengthOf(string path)
        //{
        //    return get_GroupCaptures_by_path(path).Count;
        //}

        /// <summary>
        /// Capture string of this Capture.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Text position of the capture string of this Capture.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Length of the capture string of this Capture.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// Key of this Capture. (Group name in the filter.)
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// Keys of child Capture's. (Group names of child filters.)
        /// </summary>
        public string[] Keys
        {
            get
            {
                return child_group_capture_keys.ToArray();
            }
        }

        ///// <summary>
        ///// Values of child Capture's for the key. These are captured strings of named group of child filter.
        ///// </summary>
        ///// <param name="key">group name of child filter</param>
        ///// <returns>values of child Capture's</returns>
        //public string[] ValuesOf(string key)
        //{
        //    Capture[] gcs = this[key];
        //    string[] vs = new string[gcs.Length];
        //    int i = 0;
        //    foreach (Capture gc in gcs)
        //        vs[i++] = gc.Value;

        //    return vs;
        //}

        /// <summary>
        /// Value of child Capture's for the key and the index. It is captured string of named group of child filter.
        /// </summary>
        /// <param name="key">group name of child filter</param>
        /// <param name="index">index within array of captures</param>
        /// <returns>value of child Capture</returns>
        public string ValueOf(string key, int index)
        {
            Capture gc = this[key, index];
            if (gc == null)
                return null;
            return gc.Value;
        }

        /// <summary>
        /// Values of last descendant GroupCaptures for the path. These are captured strings of named group of last descendant filter.
        /// It is almost the same like this.[key][index]...[key] but more efficient and safe.
        /// It is safe: ignores if some key is empty or some index is out of range in the path.
        /// </summary>
        /// <param name="path">path specified in the following manner: "key[, index][/ key[, index]]...", 
        /// where key is a group name of child filter, index is integer or '*'</param>
        /// <returns>values of last-descendant Capture's</returns>
        public string[] ValuesOf(string path)
        {
            List<Capture> gcs = get_GroupCaptures_by_path(path);
            string[] vs = new string[gcs.Count];
            int i = 0;
            foreach (Capture gc in gcs)
                vs[i++] = gc.Value;

            return vs;
        }

        /// <summary>
        /// First value of last-descendant Capture for the path. It is captured string of named group of last descendant filter.
        /// Used when an only value needed. If the path specifies array of values, the first one is returned.
        /// It is almost the same like this.[key][index]....Value but more efficient and safe.
        /// It returns null if some key is empty or some index is out of range.
        /// </summary>
        /// <param name="path">path specified in the following manner: "key[, index][/ key[, index]]...", 
        /// where key is a group name of child filter, index is integer or '*'</param>
        /// <returns>first value of last-descendant GroupCaptures</returns>
        public string FirstValueOf(string path)
        {
            List<Capture> gcs = get_GroupCaptures_by_path(path);
            if (gcs.Count < 1)
                return null;
            return gcs[0].Value;
        }

        ///// <summary>
        ///// Value of last descendant Capture for the path and the index. It is captured string of named group of last descendant filter.
        ///// Designed for the case when the path is a key.
        ///// It is almost the same like this.[key][index]....Value but more efficient.
        ///// It is safe in path use: returns null if some key is empty or some index is out of range.
        ///// </summary>
        ///// <param name="path">path specified in the following manner: "key[, index][/ key[, index]]...", 
        ///// where key is a group name of child filter, index is integer or '*'</param>
        ///// <param name="index">index within array of captures</param>
        ///// <returns>value of last descendant Capture</returns>
        //public string ValueOf(string path, int index)
        //{
        //    List<string> gcs = get_GroupCaptures_by_path(path);
        //    if (gcs.Count <= index || index < 0)
        //        return null;
        //    return ((Capture)gcs[index]).Value;
        //}

        /// <summary>
        /// Last value of last-descendant Capture for the path. It is captured string of named group of last descendant filter.
        /// Used when an only value needed. If the path specifies array of values, the first one is returned.
        /// It is almost the same like this.[key][index]....Value but more efficient and safe.
        /// It returns null if some key is empty or some index is out of range.
        /// </summary>
        /// <param name="path">path specified in the following manner: "key[, index][/ key[, index]]...", 
        /// where key is a group name of child filter, index is integer or '*'</param>
        /// <returns>last value of last-descendant GroupCaptures</returns>
        public string LastValueOf(string path)
        {
            List<Capture> gcs = get_GroupCaptures_by_path(path);
            if (gcs.Count < 1)
                return null;
            return gcs[gcs.Count - 1].Value;
        }

        /// <summary>
        /// Array of last descendant GroupCaptures for the path. These are captures of named group of last descendant filter.
        /// It is almost the same like this.[key][index]...[key] but more efficient and safe.
        /// It is safe: ignores if some key is empty or some index is out of range in the path.
        /// </summary>
        /// <param name="path">path specified in the following manner: "key[, index][/ key[, index]]...", 
        /// where key is a group name of child filter, index is integer or '*'</param>
        /// <returns>values of last-descendant Capture's</returns>
        public Capture[] CapturesOf(string path)
        {
            List<Capture> gcs = get_GroupCaptures_by_path(path);
            return gcs.ToArray();
        }

        //******************************************************************************************
        //aliases and shortcuts
        //******************************************************************************************

        /// <summary>
        /// Alias for [key, 0]
        /// </summary>
        /// <param name="key">group name of child filter</param>
        /// <returns>first child Capture</returns>
        public Capture FirstOf(string key)
        {
            return this[key, 0];
        }

        /// <summary>
        /// Alias for [key, LengthOf(key) - 1]
        /// </summary>
        /// <param name="key">group name of child filter</param>
        /// <returns>last child Capture</returns>
        public Capture LastOf(string key)
        {
            return this[key, LengthOf(key) - 1];
        }

        /// <summary>
        /// Alias for FirstValueOf(path)
        /// </summary>
        /// <param name="path">path specified in the following manner: "key[, index][/ key[, index]]...", 
        /// where key is a group name of child filter, index is integer or '*'</param>
        /// <returns>first value of last-descendant GroupCaptures</returns>
        public string ValueOf(string path)
        {
            return FirstValueOf(path);
        }

        //******************************************************************************************
        //internal methods
        //******************************************************************************************
        Dictionary<string, List<Capture>> child_group_captures = new Dictionary<string, List<Capture>>();
        List<string> child_group_capture_keys = new List<string>();//ordered keys

        internal Capture(string key, string value, int index, List<string> child_keys)
        {
            this.Key = key;
            this.Value = value;
            this.Index = index;
            this.Length = value.Length;

            if (child_keys == null)
                return;

            child_group_capture_keys = child_keys;

            foreach (string child_key in child_keys)
                child_group_captures[child_key] = new List<Capture>();
        }

        internal void AddChildGroupCapture(Capture group_capture)
        {
            List<Capture> group_captures = child_group_captures[group_capture.Key];
            group_captures.Add(group_capture);
        }

        List<Capture> get_GroupCaptures_by_path(string path)
        {
            string[] key_index_pairs = path.Split('/');
            List<Capture> level_gcs = new List<Capture>();
            level_gcs.Add(this);
            foreach (string key_index_pair in key_index_pairs)
            {
                string[] p = key_index_pair.Split(',');
                string key = p[0].Trim();
                string _index = null;
                if (p.Length < 2)
                    _index = "*";
                else
                    _index = p[1].Trim();
                List<Capture> child_gcs = new List<Capture>();
                if (_index == "*")
                {
                    foreach (Capture lgc in level_gcs)
                    {
                        List<Capture> gcs;
                        if (!lgc.child_group_captures.TryGetValue(key, out gcs))
                            throw (new Exception("Key '" + key + "' in the Capture path '" + path + "' was not found."));
                        child_gcs.AddRange(gcs);
                    }
                }
                else
                {
                    int index = -1;
                    if (!int.TryParse(_index, out index))
                        throw (new Exception("Index '" + _index + "' in the Capture path '" + path + "' is inadmissible. Index might be integer or '*' only."));

                    foreach (Capture lgc in level_gcs)
                    {
                        List<Capture> gcs;
                        if (!lgc.child_group_captures.TryGetValue(key, out gcs))
                            throw (new Exception("Key '" + key + "' in the Capture path '" + path + "' was not found."));
                        if (gcs.Count <= index || index < 0)
                            continue;
                        child_gcs.Add((Capture)gcs[index]);
                    }
                }
                level_gcs = child_gcs;
            }

            return level_gcs;
        }
    }
}
