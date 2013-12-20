using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace logboek
{
    interface ITableRow<E, T>
    {
        T Create(string value);
        string Get(E fld);
        void Set(E fld, string value);
    }
    class TableRow<E, T> where T : ITableRow<E, T>, new()
    {
        public static readonly string fileName = typeof(T).Name + ".cvs";
        public static E PrimaryKey { get { return (E)Enum.Parse(typeof(E), Enum.GetNames(typeof(E))[0]); } }
        public static E GetFld(string name) { return (E)Enum.Parse(typeof(E), name); }
        public static List<T> Rows = new List<T>();

        public static void Laad()
        {
            string fName = Path.Combine(Setting.Get(eSetting.DataPath).Waarde, fileName);
            if (fileName.ToLower() == "setting.cvs")
                fName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), fileName);
            if (File.Exists(fName))
            {
                StreamReader tableFile = File.OpenText(fName);
                Rows.Clear();
                while (!tableFile.EndOfStream)
                    Rows.Add(new T().Create(tableFile.ReadLine()));
                Rows.Sort(new TableRowComparer<E, T>(PrimaryKey));
                tableFile.Close();
            }
        }

        public static void Bewaar()
        {
            string fName = Path.Combine(Setting.Get(eSetting.DataPath).Waarde, fileName);
            if (fileName.ToLower() == "setting.cvs")
                fName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), fileName);
            StreamWriter tableFile = File.CreateText(fName);
            Rows.Sort(new TableRowComparer<E, T>(PrimaryKey));
            foreach (T row in Rows)
                tableFile.WriteLine(row.ToString());
            tableFile.Close();
        }

        public static void InitOvz(BindingSource bs, DataGridView dgv, List<Log> logs)
        {
            dgv.DataSource = bs;
            DataSet ds = new DataSet();
            DataTable table = ds.Tables.Add(typeof(E).Name);
            string[] names = Enum.GetNames(typeof(E));
            for (int i = 0; i < names.Length; i++)
                table.Columns.Add(names[i], typeof(string));
            foreach (T row in Rows)
            {
                DataRow dataRow = table.Rows.Add();
                for (int i = 0; i < names.Length; i++)
                    dataRow[i] = row.Get(GetFld(names[i]));
                dataRow.AcceptChanges();
            }
            bs.DataSource = table;
        }
    }
    class TableRowComparer<E, T> : IComparer<T> where T : ITableRow<E, T>, new()
    {
        // Create a field for the desired sort option
        E _sortOption = TableRow<E, T>.PrimaryKey;

        // Restrict the scope of the default constructor
        protected TableRowComparer()
        {
        }

        // Create a constructor that specifies the desired sort option
        public TableRowComparer(E sortOption)
        {
            _sortOption = sortOption;
        }

        #region IComparer<TableRow<E> Members

        public int Compare(T x, T y)
        {
            return x.Get(_sortOption).CompareTo(y.Get(_sortOption));
        }

        #endregion
    }
}
