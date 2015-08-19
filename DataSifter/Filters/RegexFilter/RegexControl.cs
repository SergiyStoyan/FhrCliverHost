using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Cliver.DataSifter
{
    public partial class RegexControl : FilterControl
    {
        //needed for VS designer
        public RegexControl()
        {
            InitializeComponent();
        }

        internal RegexControl(RegexFilter filter = null)
            : base(filter)
        {
            InitializeComponent();
            RegexBox.Text = filter.Regex.ToString();

            checkIgnoreCase.Checked = (filter.Regex.Options & RegexOptions.IgnoreCase) != 0;
            checkSingleLine.Checked = (filter.Regex.Options & RegexOptions.Singleline) != 0;
            checkMultiline.Checked = (filter.Regex.Options & RegexOptions.Multiline) != 0;
            checkIgnorePatternWhitespace.Checked = (filter.Regex.Options & RegexOptions.IgnorePatternWhitespace) != 0;
            checkExplicitCapture.Checked = (filter.Regex.Options & RegexOptions.ExplicitCapture) != 0;
            checkRightToLeft.Checked = (filter.Regex.Options & RegexOptions.RightToLeft) != 0;
        }

        override internal string GetUpdatedFilterDefinition()
        {
            RegexOptions ro = RegexOptions.Compiled;
            ro = ro | RegexOptions.Singleline;
            if (!checkSingleLine.Checked)
                ro = ro ^ RegexOptions.Singleline;
            ro = ro | RegexOptions.IgnoreCase;
            if (!checkIgnoreCase.Checked)
                ro = ro ^ RegexOptions.IgnoreCase;
            ro = ro | RegexOptions.ExplicitCapture;
            if (!checkExplicitCapture.Checked)
                ro = ro ^ RegexOptions.ExplicitCapture;
            ro = ro | RegexOptions.IgnorePatternWhitespace;
            if (!checkIgnorePatternWhitespace.Checked)
                ro = ro ^ RegexOptions.IgnorePatternWhitespace;
            ro = ro | RegexOptions.Multiline;
            if (!checkMultiline.Checked)
                ro = ro ^ RegexOptions.Multiline;
            ro = ro | RegexOptions.RightToLeft;
            if (!checkRightToLeft.Checked)
                ro = ro ^ RegexOptions.RightToLeft;

            return RegexBox.Text + "\n" + ro.ToString();
        }
    }
}
