using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cliver.DataSifter
{
    public partial class FilterControl : BaseControl
    {
        //needed for VS designer
        public FilterControl()
        {
        }

        public FilterControl(Filter filter)
        {
            this.filter = filter;
        }
        protected readonly Filter filter;

        private void FilterControl_Load(object sender, EventArgs e)
        {
            ToolTip.AutoPopDelay = 100000;
            set_tool_tip();
        }

        protected System.Windows.Forms.ToolTip ToolTip = new System.Windows.Forms.ToolTip();

        protected virtual void set_tool_tip()
        {
            //ToolTip.SetToolTip(this.TreeName, "Name of " + Program.FilterTreeFileExtension + " file");
            throw new Exception("Not implemented");
        }

        virtual internal string GetUpdatedFilterDefinition()
        {
            throw new Exception("Not implemented");
        }      

        virtual public bool CanUndo
        {
            get
            {
                return false;
            }
        }

        virtual public bool CanRedo
        {
            get
            {
                return false;
            }
        }

        virtual public string SelectedText
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        virtual public string Copy()
        {
            throw new Exception("Not implemented");
        }

        virtual public string Cut()
        {
            throw new Exception("Not implemented");
        }

        virtual public string Undo()
        {
            throw new Exception("Not implemented");
        }

        virtual public string Redo()
        {
            throw new Exception("Not implemented");
        }
    }
}
