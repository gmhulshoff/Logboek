using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace logboek
{
    public enum BandField
    {
        Call,
        Freq,
        Datum,
        Tijd,
        Rapport,
        Qsl,
    }
    public class Band : ITableRow<BandField, Band>
    {
        string call;
        string freq;
        string datum;
        string tijd;
        string rapport;
        string qsl;

        public Band()
        {
            call = "";
            freq = "";
            datum = "";
            tijd = "";
            rapport = "";
            qsl = "";
        }

        public override string ToString()
        {
            string[] vals = new string[] { 
                    Setting.Quote(call), 
                    Setting.Quote(freq), 
                    Datum.Replace("-", ""), 
                    Setting.Quote(tijd), 
                    Setting.Quote(rapport), 
                    Setting.Quote(qsl)};
            return string.Join(",", vals);
        }

        public static int toHz(string freq)
        {
            int mhz = 0;
            if (freq != null && freq.Length > 3 && int.TryParse(freq.Substring(0, freq.Length - 3), out mhz))
            {
                string unit = freq.Substring(freq.Length -3).ToLower();
                mhz = mhz * (unit == "mhz" ? 1000 : unit == "ghz" ? 1000000 : 1);
            }
            return mhz;
        }

        public int Hz { get { return toHz(freq); } }
        public string Call { get { return call; } set { call = value; } }
        public string Freq { get { return freq; } set { freq = value; } }
        public string Datum 
        {
            get 
            { 
                string dat = (!datum.StartsWith("20") && !string.IsNullOrEmpty(datum.Trim('-').Trim()) && string.Compare(datum, "1910") < 0) ? "20" + datum.Substring(2) : datum;
                if (dat.Length == 8)
                    dat = dat.Substring(0, 4) + "-" + dat.Substring(4, 2) + "-" + dat.Substring(6);
                return dat;
            } 
            set { datum = value; } 
        }
        public string Tijd { get { return tijd; } set { tijd = value; } }
        public string Rapport { get { return rapport; } set { rapport = value; } }
        public string Qsl { get { return qsl; } set { qsl = value; } }

        public static void RemoveCallBanden(string call)
        {
            TableRow<BandField, Band>.Rows.RemoveAll(band => string.Compare(band.Call, call, true) == 0);
        }

        public static void InitBandRows(string call, List<BandRow> bandRows)
        {
            foreach (Band band in TableRow<BandField, Band>.Rows.Where(b => b.Call == call))
                bandRows[BandRow.GetRow(band.Freq)].Read(band, TableRow<BandField, Band>.Rows.Count(b => string.Compare(b.Freq, band.Freq, false) == 0));
        }

        #region ITableRow<BandField,Band> Members

        public Band Create(string value)
        {
            string[] fields = value.Split(',');
            Band band = new Band();
            string[] names = Enum.GetNames(typeof(BandField));
            for (int i = 0; i < names.Count(); i++)
                band.Set(TableRow<BandField, Band>.GetFld(names[i]), fields.Length > i ? Setting.Unquote(fields[i]) : "");
            return band;
        }

        public string Get(BandField fld)
        {
            string result = "";
            switch (fld)
            {
                case BandField.Call: result = call; break;
                case BandField.Datum: result = datum; break;
                case BandField.Freq: result = freq; break;
                case BandField.Qsl: result = qsl; break;
                case BandField.Rapport: result = rapport; break;
                case BandField.Tijd: result = tijd; break;
            }
            return result ?? "";
        }

        public void Set(BandField fld, string value)
        {
            switch (fld)
            {
                case BandField.Call: call = value; break;
                case BandField.Datum: datum = value; break;
                case BandField.Freq: freq = value; break;
                case BandField.Qsl: qsl = value; break;
                case BandField.Rapport: rapport = value; break;
                case BandField.Tijd: tijd = value; break;
            }
        }

        #endregion
    }
    public struct BandRow
    {
        public static readonly string[] freqs = new string[] { "50MHz", "144MHz", "432MHz", "1296MHz", "2320MHz", "3400MHz", "5700MHz", "10GHz" };
        public TextBox txtFreq;
        public MaskedTextBox txtDatum;
        public MaskedTextBox txtTijd;
        public TextBox txtRapport;
        public CheckBox chkQsl;
        public TextBox txtQSO;
        public static bool beep = false;

        public BandRow(TextBox txtFreq, MaskedTextBox txtDatum, MaskedTextBox txtTijd, TextBox txtRapport, CheckBox chkQsl, TextBox txtQSO)
        {
            this.txtFreq = txtFreq;
            this.txtDatum = txtDatum;
            this.txtTijd = txtTijd;
            this.txtRapport = txtRapport;
            this.chkQsl = chkQsl;
            this.txtQSO = txtQSO;
            this.txtRapport.TextChanged += new EventHandler(txtRapport_TextChanged);
            this.txtDatum.TextChanged += new EventHandler(txtDatum_TextChanged);
            this.txtTijd.TextChanged += new EventHandler(txtTijd_TextChanged);
        }

        public static DataSet CreateDataSet(bool addCall)
        {
            DataSet ds = new DataSet();
            DataTable table = ds.Tables.Add(typeof(Band).Name);
            if (addCall)
                table.Columns.Add(BandField.Call.ToString(), typeof(string));
            table.Columns.Add(BandField.Freq.ToString(), typeof(string));
            table.Columns.Add(BandField.Datum.ToString(), typeof(string));
            table.Columns.Add(BandField.Tijd.ToString(), typeof(string));
            table.Columns.Add(BandField.Rapport.ToString(), typeof(string));
            table.Columns.Add(BandField.Qsl.ToString(), typeof(bool));
            return ds;
        }

        public static Band ToBand(DataRow row, string call)
        {
            Band newBand = new Band();
            newBand.Call = call;
            newBand.Freq = Setting.GetObject<string>(row, BandField.Freq.ToString());
            newBand.Datum = Setting.GetObject<string>(row, BandField.Datum.ToString());
            newBand.Tijd = Setting.GetObject<string>(row, BandField.Tijd.ToString());
            newBand.Rapport = Setting.GetObject<string>(row, BandField.Rapport.ToString());
            newBand.Qsl = Setting.GetObject<bool>(row, BandField.Qsl.ToString()) ? "J" : "N";
            return newBand;
        }

        public static void AddRow(DataSet ds, Band band, bool useCall)
        {
            DataRow row = ds.Tables[typeof(Band).Name].Rows.Add();
            if (useCall)
                row[BandField.Call.ToString()] = band.Call;
            row[BandField.Freq.ToString()] = band.Freq;
            row[BandField.Datum.ToString()] = band.Datum;
            row[BandField.Tijd.ToString()] = band.Tijd;
            row[BandField.Rapport.ToString()] = band.Rapport;
            row[BandField.Qsl.ToString()] = band.Qsl == "J";
            row.AcceptChanges();
        }

        void txtRapport_TextChanged(object sender, EventArgs e)
        {
            TextBox mt = sender as TextBox;
            if (mt.Text.Length == 5 && BandRow.beep)
            {
                Setting.Beep();
                mt.Parent.GetNextControl(mt, true).Focus();
            }
        }

        static void txtDatum_TextChanged(object sender, EventArgs e)
        {
            MaskedTextBox mt = sender as MaskedTextBox;
            if (mt.Text.Length == 10 && BandRow.beep)
            {
                Setting.Beep();
                mt.Parent.GetNextControl(mt, true).Focus();
            }
        }

        static void txtTijd_TextChanged(object sender, EventArgs e)
        {
            MaskedTextBox mt = sender as MaskedTextBox;
            if (BandRow.beep && mt.Text.Length == 5)
            {
                Setting.Beep();
                mt.Parent.GetNextControl(mt, true).Focus();
            }
        }

        public void SetReadOnly(bool readOnly)
        {
            txtRapport.ReadOnly = readOnly;
            txtDatum.ReadOnly = readOnly;
            txtFreq.ReadOnly = readOnly;
            txtFreq.Enabled = true;
            txtTijd.ReadOnly = readOnly;
            txtQSO.ReadOnly = true;
            chkQsl.AutoCheck = !readOnly;
            txtFreq.BackColor = readOnly ? TextBoxBase.DefaultBackColor:  System.Drawing.Color.White;
            txtFreq.ForeColor = readOnly ? TextBoxBase.DefaultForeColor : System.Drawing.Color.Black;
        }

        public bool Filled
        {
            get { return !string.IsNullOrEmpty(txtDatum.Text.Replace("-", "").Trim() + txtTijd.Text.Replace(":", "").Trim() + txtRapport.Text); }
        }

        public static int GetRow(string freq)
        {
            for (int i = 0; i < freqs.Count(); i++)
                if (freq == freqs[i])
                    return i;
            return 8;
        }

        string ToYearOnEnd(string startWithYear)
        {
            startWithYear = startWithYear.Replace("-", "");
            return startWithYear.Substring(6, 2) + startWithYear.Substring(4, 2) + startWithYear.Substring(0, 4);
        }

        string ToYearOnStart(string startWithDay)
        {
            startWithDay = startWithDay.Replace("-", "");
            return startWithDay.Substring(4, 4) + startWithDay.Substring(2, 2) + startWithDay.Substring(0, 2);
        }

        public void Read(Band band, int qso)
        {
            BandRow.beep = false;
            txtFreq.Text = band.Freq;
            txtDatum.Text = ToYearOnEnd(band.Datum); // YYYYMMDD => DDMMYYYY 01234567
            txtTijd.Text = band.Tijd;
            txtRapport.Text = band.Rapport;
            chkQsl.Checked = band.Qsl == "J";
            txtQSO.Text = qso.ToString();
            BandRow.beep = true;
        }

        public Band Write(int i, string call)
        {
            Band band = new Band();
            band.Call = call;
            band.Datum = ToYearOnStart(txtDatum.Text);
            band.Tijd = txtTijd.Text.Replace(":", "").Replace(".", "");
            band.Freq = i < freqs.Count() ? freqs[i] : txtFreq.Text;
            band.Qsl = chkQsl.Checked ? "J" : "N";
            band.Rapport = txtRapport.Text;
            return band;
        }

        public void Clear()
        {
            txtRapport.Text = "";
            txtDatum.Text = "";
            //txtFreq.Text = "";
            txtTijd.Text = "";
            txtQSO.Text = "";
            chkQsl.Checked = false;
        }
    }
}
