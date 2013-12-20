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
    public partial class frmQSO : Form
    {
        public frmQSO()
        {
            InitializeComponent();
            dataGridViewQSO.DataSource = bindingSource1;
            DataSet dsLog = new DataSet();
            DataTable table = dsLog.Tables.Add("qso");
            table.Columns.Add("freq", typeof(string));
            table.Columns.Add("qso", typeof(int));
            foreach (IGrouping<string, Band> group in TableRow<BandField, Band>.Rows.GroupBy(band => band.Freq).OrderBy(grp => Band.toHz(grp.Key)))
            {
                DataRow row = table.Rows.Add();
                row["freq"] = group.Key;
                row["qso"] = group.Count();
                row.AcceptChanges();
            }
            bindingSource1.DataSource = table;
        }
    }
}
