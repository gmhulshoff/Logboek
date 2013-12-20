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
    public partial class frmBandQsl : Form
    {
        public frmBandQsl(string dateString)
        {
            InitializeComponent();
            Text = "Bijwerken QSL's vanaf " + dateString;
            dataGridView1.DataSource = bindingSource1;
            DataSet ds = BandRow.CreateDataSet(true);
            foreach (Band band in TableRow<BandField, Band>.Rows.Where(band => string.Compare(band.Datum, dateString) > 0).OrderByDescending(band => band.Datum))
                BandRow.AddRow(ds, band, true);
            bindingSource1.DataSource = ds.Tables[0];
        }
    }
}
