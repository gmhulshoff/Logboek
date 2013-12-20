using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace logboek
{
    public enum LogField
    {
        Call,
        Naam,
        Qth,
        Qra,
        Dok,
        Cert,
        Opm1,
        Opm2,
    }
    public class Log : ITableRow<LogField, Log>
    {
        string call;
        string naam;
        string qth;
        string qra;
        string dok;
        string cert;
        string opm1;
        string opm2;

        public Log() 
        {
            call = "";
            naam = "";
            qth = "";
            qra = "";
            dok = "";
            cert = "";
            opm1 = "";
            opm2 = "";
        }

        public override string ToString()
        {
            return string.Join(",", new string[] { 
                    Setting.Quote(call), 
                    Setting.Quote(naam), 
                    Setting.Quote(qth), 
                    Setting.Quote(qra), 
                    Setting.Quote(dok), 
                    Setting.Quote(cert), 
                    Setting.Quote(opm1), 
                    Setting.Quote(opm2)});
        }
        public string Call { get { return call; } set { call = value; } }
        public string Naam { get { return naam; } set { naam = value; } }
        public string Qth { get { return qth; } set { qth = value; } }
        public string Qra { get { return qra; } set { qra = value; } }
        public string Dok { get { return dok; } set { dok = value; } }
        public string Cert { get { return cert; } set { cert = value; } }
        public string Opm1 { get { return opm1; } set { opm1 = value; } }
        public string Opm2 { get { return opm2; } set { opm2 = value; } }

        #region ITableRow<LogField,Log> Members

        public Log Create(string value)
        {
            string[] fields = value.Split(',');
            Log log = new Log();
            string[] names = Enum.GetNames(typeof(LogField));
            for (int i = 0; i < names.Count(); i++)
                log.Set(TableRow<LogField, Log>.GetFld(names[i]), fields.Length > i ? Setting.Unquote(fields[i]) : "");
            return log;
        }

        public string Get(LogField fld)
        {
            string result = "";
            switch (fld)
            {
                case LogField.Call: result = call; break;
                case LogField.Cert: result = cert; break;
                case LogField.Dok: result = dok; break;
                case LogField.Naam: result = naam; break;
                case LogField.Opm1: result = opm1; break;
                case LogField.Opm2: result = opm2; break;
                case LogField.Qra: result = qra; break;
                case LogField.Qth: result = qth; break;
            }
            return result ?? "";
        }

        public void Set(LogField fld, string value)
        {
            switch (fld)
            {
                case LogField.Call: call = value; break;
                case LogField.Cert: cert = value; break;
                case LogField.Dok: dok = value; break;
                case LogField.Naam: naam = value; break;
                case LogField.Opm1: opm1 = value; break;
                case LogField.Opm2: opm2 = value; break;
                case LogField.Qra: qra = value; break;
                case LogField.Qth: qth = value; break;
            }
        }

        #endregion
    }

    struct LogRow
    {
        public TextBox txtCall;
        public TextBox txtCert;
        public TextBox txtDok;
        public TextBox txtOpm;
        public TextBox txtQRA;
        public TextBox txtQTH;
        public TextBox txtMemo;
        public TextBox txtNaam;
        public Label lblAfstand;

        public void Clear()
        {
            lblAfstand.Text = "";
            txtCall.Text = "";
            txtCert.Text = "";
            txtDok.Text = "";
            txtOpm.Text = "";
            txtQRA.Text = "";
            txtQTH.Text = "";
            txtMemo.Text = "";
            txtNaam.Text = "";
        }

        public void Read(Log log)
        {
            log.Cert = txtCert.Text;
            log.Dok = txtDok.Text;
            log.Naam = txtNaam.Text;
            log.Opm1 = txtOpm.Text;
            log.Opm2 = txtMemo.Text;
            log.Qra = txtQRA.Text;
            log.Qth = txtQTH.Text;
        }

        public void SetDefaultFocus()
        {
            txtCall.Focus();
        }

        public void SetReadOnly(bool readOnly)
        {
            txtQRA.ReadOnly = readOnly;
            txtQTH.ReadOnly = readOnly;
            txtOpm.ReadOnly = readOnly;
            txtNaam.ReadOnly = readOnly;
            txtDok.ReadOnly = readOnly;
            txtCert.ReadOnly = readOnly;
            txtMemo.ReadOnly = readOnly;
            txtCall.ReadOnly = readOnly;
        }

        public void Write(Log log)
        {
            txtCall.Text = log.Call;
            txtCert.Text = log.Cert;
            txtDok.Text = log.Dok;
            txtOpm.Text = log.Opm1;
            txtQRA.Text = log.Qra;
            txtQTH.Text = log.Qth;
            txtMemo.Text = log.Opm2;
            txtNaam.Text = log.Naam;
            string baseQra = Setting.Get(eSetting.QraBase).Waarde;
            lblAfstand.Text = "";
            try
            {
                if (baseQra != "" && log.Qra != "")
                    lblAfstand.Text = MaidenheadLocator.Distance(baseQra, log.Qra).ToString("#.##") + " km";
            }
            catch
            {
                lblAfstand.Text = "";
            }
        }
    }
}
