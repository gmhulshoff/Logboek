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
    public partial class frmBandEdit : Form
    {
        string call;

        public frmBandEdit(string call)
        {
            InitializeComponent();
            this.call = call;
            dataGridView1.DataSource = bindingSource1;
            DataSet dsBand = BandRow.CreateDataSet(false);
            foreach (Band band in TableRow<BandField, Band>.Rows.Where(band => band.Call == call).OrderBy(band => Band.toHz(band.Freq)))
                BandRow.AddRow(dsBand, band, false);
            bindingSource1.DataSource = dsBand.Tables[0];
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (Setting.Get(eSetting.ConfirmLineDelete).Waarde == "J" && MessageBox.Show("Rij verwijderen?", "Vraag", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                e.Cancel = true;
        }

        public bool IsModified
        {
            get
            {
                DataTable table = (DataTable)bindingSource1.DataSource;
                foreach (DataRow row in table.Rows)
                    if (row.RowState == DataRowState.Deleted || 
                        row.RowState == DataRowState.Added || 
                        row.RowState == DataRowState.Modified)
                        return true;
                return false;
            }
        }

        public void Save()
        {
            Band.RemoveCallBanden(call);
            DataTable table = (DataTable)bindingSource1.DataSource;
            foreach (DataRow row in table.Rows)
                if (row.RowState != DataRowState.Deleted)
                    TableRow<BandField, Band>.Rows.Add(BandRow.ToBand(row, call));
        }
    }
}
