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
    public partial class frmBandOvz : Form
    {
        public frmBandOvz(string freq, string qsl)
        {
            InitializeComponent();
            if (freq == "<leeg>")
                freq = "";
            dataGridView1.DataSource = bindingSource1;
            DataSet ds = BandRow.CreateDataSet(true);
            foreach (Band band in TableRow<BandField, Band>.Rows.Where(band => band.Freq == freq && band.Qsl == qsl).OrderBy(band => band.Call))
                BandRow.AddRow(ds, band, true);
            bindingSource1.DataSource = ds.Tables[0];
        }
    }
}
