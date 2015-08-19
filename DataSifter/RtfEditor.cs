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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Cliver
{
    /// <summary>
    /// Edit rtf text
    /// </summary>
    internal class RtfEditor
    {
        StringBuilder rtf_head = null;
        StringBuilder rtf_body = null;
        List<Color> color_table = new List<Color>();

        public RtfEditor(string rtf_source)
        {
            Match m;
            int colortbl_start = rtf_source.IndexOf(@"{\colortbl");
            if (colortbl_start < 0)
            {
                m = Regex.Match(rtf_source, @".*\\(?:fonttbl).*?(?'Open'\{).*?(?'-Open'\})}(?'Point'.*)", RegexOptions.Compiled | RegexOptions.Singleline);
                colortbl_start = m.Groups["Point"].Index;
                rtf_source = rtf_source.Insert(colortbl_start, @"{\colortbl;}");
            }
            colortbl_end = rtf_source.IndexOf('}', colortbl_start);

            string colors = rtf_source.Substring(colortbl_start, colortbl_end - colortbl_start);
            colors = colors.Substring(colors.IndexOf(";"));
            m = Regex.Match(colors, @"(?'Color'.+?;)", RegexOptions.Compiled | RegexOptions.Singleline);
            while (m.Success)
            {
                color_table.Add(Color.Empty);
                m = m.NextMatch();
            }

            //m = Regex.Match(rtf_source, @".*\\(?:deff|fonttbl|filetbl|colortbl|stylesheet|listtables|revtbl).*?(?'Open'\{).*?(?'-Open'\})(?'Body'}.*)", RegexOptions.Compiled | RegexOptions.Singleline);            
            m = Regex.Match(rtf_source, @".*?{\\colortbl.*?}\r\n(?'Body'.*)", RegexOptions.Compiled | RegexOptions.Singleline);
            Group g = m.Groups["Body"];
            rtf_head = new StringBuilder(rtf_source, 0, g.Index, g.Index);
            rtf_body = new StringBuilder(g.Value);
            //int head_end = rtf_source.IndexOf("\r\n");
            //rtf_head = new StringBuilder(rtf_source.Substring(0, head_end));
            //rtf_body = new StringBuilder(rtf_source.Substring(head_end));
        }

        public string RtfSource
        {
            get
            {
                StringBuilder rtf_source = new StringBuilder();
                return rtf_source.Append(rtf_head).Append(rtf_body).ToString();
            }
        }

        int colortbl_end = -1;

        int get_color_index(Color color)
        {
            int color_index = color_table.IndexOf(color);
            if (color_index >= 0)
                return color_index + 1;

            string color_rtf_code = @"\red" + color.R.ToString() + @"\green" + color.G.ToString() + @"\blue" + color.B.ToString() + ";";
            rtf_head.Insert(colortbl_end, color_rtf_code);
            colortbl_end += color_rtf_code.Length;
            color_table.Add(color);
            return color_table.Count + 1;
        }

        int pass_tags(int rtf_start)
        {
            int body_length = rtf_body.Length;
            for (int i = rtf_start; i < body_length; i++)
            {
                char c = rtf_body[i];
                if (c == '\\')
                {
                    if (i + 1 < body_length)
                    {
                        char c2 = rtf_body[i + 1];
                        if (c2 == '\\'
                            || c2 == '{' || c == '}'
                            )
                        {//encoded char
                            return i;
                        }
                        else
                        {//tag
                            for (i++; i < body_length; i++)
                            {
                                c = rtf_body[i];
                                if (c == ' '//end of control word
                                    || (c != '\\' && (c < '0' || c > '9') && (c < 'A' || c > 'Z') && (c < 'a' || c > 'z'))//end of control symbol 
                                    )
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (c == '{' || c == '}')// || c == '\r' || c == '\n')
                    {
                    }
                    else
                        return i;
                }
            }
            return -1;
        }

        void insert_tag(int plain_start, int plain_end, string tag)
        {
            int rtf_start = 0;
            int rtf_end = 0;
            int current_plain_position = 0;
            int body_length = rtf_body.Length;
            for (int i = 0; i < body_length; i++)
            {
                char c = rtf_body[i];
                if (current_plain_position == plain_start)
                {
                    i = pass_tags(i);
                    rtf_start = i;
                }
                if (current_plain_position == plain_end)
                {
                    rtf_end = i;
                    break;
                }
                i = pass_tags(i);
                if (i < 0)
                    break;
                current_plain_position++;
            }

            rtf_body.Insert(rtf_end, "}");
            rtf_body.Insert(rtf_start, "{" + tag);
        }

        //public void SetBackColor(int plain_start, int plain_end, Color color)
        //{
        //    string color_tag = @"\highlight" + get_color_index(color).ToString() + " ";
        //    insert(plain_start, "{" + color_tag);
        //    insert(plain_end, "}");
        //}

        //public void SetForeColor(int plain_start, int plain_end, Color color)
        //{
        //    string color_tag = @"\cf" + get_color_index(color).ToString() + " ";
        //    insert(plain_start, "{" + color_tag);
        //    insert(plain_end, "}");
        //}

        public void InsertText(int plain_start, string text, Color fore_color, Color back_color)
        {
            string color_tags = @"\highlight" + get_color_index(back_color).ToString() + @"\cf" + get_color_index(fore_color).ToString() + " ";
            insert_tag(plain_start, plain_start, color_tags + text);
        }

        internal void MarkCapture(int plain_start, string label_with_font_tags, Color label_fore_color, int plain_end, Color back_color)
        {
            string capture_tags = @"\highlight" + get_color_index(back_color).ToString() + " ";
            string label_with_tags = @"\cf" + get_color_index(label_fore_color).ToString() + label_with_font_tags;

            insert_tag(plain_start, plain_end, capture_tags + "{" + label_with_tags + "}");
        }

        /// <summary>
        /// Analog string.IndexOf for StringBuilder
        /// </summary>
        /// <param name="str">string where to search</param>
        /// <param name="value">string what to search</param>
        /// <returns>-1 if not found or value is empty</returns>
        static public int IndexOf(StringBuilder str, string value)
        {
            return RtfEditor.IndexOf(str, value, 0, str.Length);
        }

        /// <summary>
        /// Analog string.IndexOf for StringBuilder
        /// </summary>
        /// <param name="str">string where to search</param>
        /// <param name="value">string what to search</param>
        /// <param name="start_index">where to start serch within str</param>
        /// <returns>-1 if not found or value is empty</returns>
        static public int IndexOf(StringBuilder str, string value, int start_index)
        {            
            return RtfEditor.IndexOf(str, value, start_index, str.Length);
        }

        /// <summary>
        /// Analog string.IndexOf for StringBuilder
        /// </summary>
        /// <param name="str">string where to search</param>
        /// <param name="value">string what to search</param>
        /// <param name="start_index">where to start serch within str</param>
        /// <param name="count">the number of charakter positions to examine</param>
        /// <returns>-1 if not found or value is empty</returns>
        static public int IndexOf(StringBuilder str, string value, int start_index, int count)
        {
            if (string.IsNullOrEmpty(value))
                return -1;

            int length = str.Length;
            if (start_index + count < length)
                length = start_index + count;
            for (int i = start_index; i < length; i++)
            {
                if (str[i] == value[0])
                {
                    for (int j = 1; i + j < length; j++)
                    {
                        if (j == value.Length)
                            return i;
                        else if (str[i + j] != value[j])
                            break;
                    }
                }
            }

            return -1;
        }

        static public bool IsEqualIgnoringCase(StringBuilder str, int start_index, string value)
        {
            if (str == null || string.IsNullOrEmpty(value))
                return false;
            if (start_index < 0)
                return false;
            if (start_index + value.Length > str.Length)
                return false;

            char [] cs = new char[value.Length];
            str.CopyTo(start_index, cs, 0, value.Length);
            string v = new string(cs);
            return string.Compare(v, value, true) == 0;
        }
    }
}