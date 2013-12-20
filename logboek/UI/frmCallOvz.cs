using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace logboek
{
    public partial class frmCallOvz : Form
    {
        public frmCallOvz()
        {
            InitializeComponent();
            this.Text = "Overzicht calls (" + TableRow<LogField, Log>.Rows.Count + ")";
            TableRow<LogField, Log>.InitOvz(bindingSource1, dataGridView1, TableRow<LogField, Log>.Rows);
        }
    }
}
