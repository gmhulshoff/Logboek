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
    public partial class frmDate : Form
    {
        public string ChosenDate {get {return (string)listBox1.SelectedItem;}}

        public frmDate()
        {
            InitializeComponent();
            listBox1.Items.AddRange(TableRow<BandField, Band>.Rows.GroupBy(band => band.Datum.Length > 7 ? band.Datum.Substring(0, 7) : "").OrderByDescending(grp => grp.Key).Select(grp => grp.Key).ToArray());
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = true;
        }
    }
}
