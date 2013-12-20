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
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
            Text = "Instellingen logboek";
            dataGridView1.DataSource = bindingSource1;
            DataSet ds = SettingRow.CreateDataSet();
            foreach (string settingName in Enum.GetNames(typeof(eSetting)))
            {
                Setting setting = Setting.Get((eSetting)Enum.Parse(typeof(eSetting), settingName));
                SettingRow.AddRow(ds, setting);
            }
            bindingSource1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].ReadOnly = true;
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
            TableRow<SettingField, Setting>.Rows.Clear();
            DataTable table = (DataTable)bindingSource1.DataSource;
            foreach (DataRow row in table.Rows)
                if (row.RowState != DataRowState.Deleted)
                    TableRow<SettingField, Setting>.Rows.Add(SettingRow.ToSetting(row));
            TableRow<SettingField, Setting>.Bewaar();
        }
    }
}
