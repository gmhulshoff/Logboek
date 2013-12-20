using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Deployment.Application;

namespace logboek
{
    public enum eSetting
    {
        AppPath,
        BeepOnConfirm,
        BeepDuration,
        BeepFrequency,
        BijGewerktTot,
        ConfirmLineDelete,
        CallToUpperCase,
        DataPath,
        DataPathLocalUser,
        QraBase,
        Show7ZLogs,
        UpdateDataPath,
        UpdateUrl,
    }
    public enum SettingField
    {
        Naam,
        Omschrijving,
        Waarde
    }
    public class Setting : ITableRow<SettingField, Setting>
    {
        string naam;
        string waarde;
        string omschrijving;

        public Setting()
        {
            naam = "";
            waarde = "";
            omschrijving = "";
        }

        public override string ToString()
        {
            return string.Join(",", new string[] { Quote(naam), Quote(omschrijving), Quote(waarde) });
        }

        static string DefaultValue(eSetting setting)
        {
            switch (setting)
            {
                case eSetting.AppPath : return Backup.AppPath;
                case eSetting.BeepOnConfirm: return "J";
                case eSetting.BeepDuration: return "1600";
                case eSetting.BeepFrequency: return "262";
                case eSetting.CallToUpperCase: return "J";
                case eSetting.ConfirmLineDelete: return "N";
                case eSetting.DataPath: return Backup.DataPath;
                case eSetting.DataPathLocalUser: return Backup.DataPathLocalUser;
                case eSetting.Show7ZLogs: return "N";
                case eSetting.UpdateDataPath: return ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.DataDirectory : Backup.DataPath;
            }
            return "";
        }

        internal static string DefaultDescription(eSetting setting)
        {
            switch (setting)
            {
                case eSetting.AppPath: return "Programma directory";
                case eSetting.BeepOnConfirm: return "Piep bij veld verlaten";
                case eSetting.BeepDuration: return "Duur piep";
                case eSetting.BeepFrequency: return "Frequentie piep";
                case eSetting.CallToUpperCase: return "Call in hoofdletters";
                case eSetting.ConfirmLineDelete: return "Bevestig verwijderen regel";
                case eSetting.Show7ZLogs: return "Laat backup info zien";
                case eSetting.DataPath: return "Data directory";
                case eSetting.DataPathLocalUser: return "Data directory local user";
                case eSetting.BijGewerktTot: return "Bijgewerkt tot";
                case eSetting.UpdateDataPath: return "Data directory voor automatisch bijwerken";
            }
            return "";
        }

        public static Setting Get(eSetting name)
        {
            Setting setting = TableRow<SettingField, Setting>.Rows != null ? TableRow<SettingField, Setting>.Rows.FirstOrDefault(s => s.Naam == name.ToString()) : null;
            return setting ?? (new Setting() { Naam = name.ToString(), Waarde = Setting.DefaultValue(name), Omschrijving = DefaultDescription(name) });
        }

        public string Waarde { get { return waarde; } set { this.waarde = value; } }
        public string Naam { get { return naam; } set { naam = value; } }
        public string Omschrijving { get { return omschrijving; } set { omschrijving = value; } }

        public static void Beep()
        {
            if (Setting.Get(eSetting.BeepOnConfirm).Waarde == "J")
            {
                int freq;
                int duration;
                if (int.TryParse(Setting.Get(eSetting.BeepFrequency).Waarde, out freq) &&
                    int.TryParse(Setting.Get(eSetting.BeepDuration).Waarde, out duration))
                    Console.Beep(freq, duration);
            }
        }

        public static T GetObject<T>(DataRow row, string col)
        {
            return row[col] != DBNull.Value ? (T)row[col] : default(T);
        }

        public static string Unquote(string val) { return (val.Length > 1 && val[0] == '"') ? val.Substring(1, val.Length - 2) : val; }
        public static string Quote(string val) { return "\"" + val + "\""; }

        #region ITableRow<SettingField,Setting> Members

        public Setting Create(string value)
        {
            string[] fields = value.Split(',');
            Setting setting = new Setting();
            string[] names = Enum.GetNames(typeof(SettingField));
            for (int i = 0; i < names.Count(); i++)
                setting.Set(TableRow<SettingField, Setting>.GetFld(names[i]), fields.Length > i ? Setting.Unquote(fields[i]) : "");
            return setting;
        }

        public string Get(SettingField fld)
        {
            string result = "";
            switch (fld)
            {
                case SettingField.Naam: result = naam; break;
                case SettingField.Omschrijving: result = omschrijving; break;
                case SettingField.Waarde: result = waarde; break;
            }
            return result ?? "";
        }

        public void Set(SettingField fld, string value)
        {
            switch (fld)
            {
                case SettingField.Naam: naam = value; break;
                case SettingField.Omschrijving: omschrijving = value; break;
                case SettingField.Waarde: this.waarde = value; break;
            }
        }

        #endregion
    }

    struct SettingRow
    {
        public static DataSet CreateDataSet()
        {
            DataSet ds = new DataSet();
            DataTable table = ds.Tables.Add(typeof(Setting).Name);
            table.Columns.Add(SettingField.Naam.ToString(), typeof(string));
            table.Columns.Add(SettingField.Omschrijving.ToString(), typeof(string));
            table.Columns.Add(SettingField.Waarde.ToString(), typeof(string));
            return ds;
        }

        public static void AddRow(DataSet ds, Setting setting)
        {
            DataRow row = ds.Tables[typeof(Setting).Name].Rows.Add();
            row[SettingField.Naam.ToString()] = setting.Naam;

            string omschr = Setting.DefaultDescription((eSetting)Enum.Parse(typeof(eSetting), setting.Naam));
            row[SettingField.Omschrijving.ToString()] = omschr;
            row[SettingField.Waarde.ToString()] = setting.Waarde;
            row.AcceptChanges();
        }

        public static Setting ToSetting(DataRow row)
        {
            Setting newRow = new Setting();
            newRow.Naam = Setting.GetObject<string>(row, SettingField.Naam.ToString());
            newRow.Omschrijving = Setting.GetObject<string>(row, SettingField.Omschrijving.ToString());
            newRow.Waarde = Setting.GetObject<string>(row, SettingField.Waarde.ToString());
            return newRow;
        }
    }
}
