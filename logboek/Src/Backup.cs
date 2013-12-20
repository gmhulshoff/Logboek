using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Deployment.Application;

namespace logboek
{
    class Backup
    {
        public static string DataPath = Application.CommonAppDataPath;
        public static string AppPath = Application.StartupPath;
        public static string DataPathLocalUser = Application.LocalUserAppDataPath;

        public static void PopulateBackupMenu(ToolStripDropDownButton mnuBackup, EventHandler item_RestoreClick, EventHandler delItem_Click, EventHandler copyToUSBItem_Click, EventHandler restoreItem_Click)
        {
            string dataPath = Setting.Get(eSetting.DataPath).Waarde;
            string[] files = Directory.GetFiles(dataPath, "logbackup*.7z").OrderBy(f => f).ToArray();
            mnuBackup.DropDownItems.Clear();
            mnuBackup.DropDownItems.AddRange(files.Select(file => new ToolStripMenuItem(Path.GetFileName(file)) { Size = new Size(152, 22), Name = file }).ToArray());

            foreach (ToolStripMenuItem item in mnuBackup.DropDownItems)
            {
                ToolStripMenuItem restoreItem = new ToolStripMenuItem("Terughalen") { Size = new Size(152, 22) };
                ToolStripMenuItem copyToUSBItem = new ToolStripMenuItem("Naar USB stick") { Size = new Size(152, 22) };
                ToolStripMenuItem delItem = new ToolStripMenuItem("Verwijderen") { Size = new Size(152, 22) };
                item.DropDownItems.AddRange(new ToolStripMenuItem[] { restoreItem, copyToUSBItem, delItem });
                restoreItem.Click += new EventHandler(item_RestoreClick);
                delItem.Click += new EventHandler(delItem_Click);
                copyToUSBItem.Click += new EventHandler(copyToUSBItem_Click);
            }
            ToolStripMenuItem selectItem = new ToolStripMenuItem("Terughalen van USB") { Size = new Size(152, 22) };
            selectItem.Click += new EventHandler(restoreItem_Click);
            mnuBackup.DropDownItems.Insert(0, selectItem);
        }

        //internal static string RegOldDataDir { get { return (string)Application.CommonAppDataRegistry.GetValue("DataDirOld"); } }
        //internal static string RegOldProgDir { get { return (string)Application.CommonAppDataRegistry.GetValue("ProgDirOld"); } }
        //internal static string RegDataDir { get { return (string)Application.CommonAppDataRegistry.GetValue("DataDirOld"); } }
        //internal static string RegProgDir { get { return (string)Application.CommonAppDataRegistry.GetValue("ProgDirOld"); } }

        static void StoreDirsInRegistry()
        {
            //string oldDataDir = (string)Application.CommonAppDataRegistry.GetValue("DataDir");
            //string oldProgDir = (string)Application.CommonAppDataRegistry.GetValue("ProgDir");

            //if (!string.IsNullOrEmpty(oldDataDir) && !string.IsNullOrEmpty(oldProgDir))
            //{
            //    if (oldDataDir != Backup.DataPath)
            //    {
            //        Application.CommonAppDataRegistry.SetValue("DataDirOld", oldDataDir);
            //        if (Directory.Exists(oldDataDir))
            //            foreach (string file in Directory.GetFiles(oldDataDir, "*.7z").Union(Directory.GetFiles(oldDataDir, "*.csv")))
            //                File.Copy(file, Path.Combine(Backup.DataPath, Path.GetFileName(file)), true);
            //    }
            //    if (oldProgDir != Backup.AppPath)
            //        Application.CommonAppDataRegistry.SetValue("ProgDirOld", oldProgDir);

            //}
            //Application.CommonAppDataRegistry.SetValue("DataDir", Backup.DataPath);
            //Application.CommonAppDataRegistry.SetValue("ProgDir", Backup.AppPath);
        }

        public static void CheckDirs()
        {
            // TODO if olddir different from current dir, get data from old dir to current dir
            //if (!ApplicationDeployment.IsNetworkDeployed)
            //    StoreDirsInRegistry();
            //else if (ApplicationDeployment.CurrentDeployment.IsFirstRun)
            //    StoreDirsInRegistry();
        }

        public static void RunProcess(string fileName, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(AppPath + "\\" + fileName);
            startInfo.Arguments = arguments;
            startInfo.WorkingDirectory = DataPath;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = false;
            try
            {
                Process proc = Process.Start(startInfo);

                StreamReader oReader2 = proc.StandardOutput;
                string sRes = oReader2.ReadToEnd();
                oReader2.Close();

                proc.WaitForExit();
                if (Setting.Get(eSetting.Show7ZLogs).Waarde == "J")
                    MessageBox.Show(AppPath + "\\" + fileName + " " + arguments + Environment.NewLine + sRes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(AppPath + "\\" + fileName + " " + ex.Message);
            }
        }

        public static bool Restore(OpenFileDialog fileDiag)
        {
            fileDiag.Multiselect = false;
            fileDiag.Filter = "7z files (*.7z)|*.7z";
            fileDiag.CheckFileExists = true;
            fileDiag.CheckPathExists = true;
            fileDiag.DefaultExt = "7z";
            fileDiag.FileName = "";
            fileDiag.InitialDirectory = Setting.Get(eSetting.DataPath).Waarde;
            fileDiag.RestoreDirectory = true;
            if (fileDiag.ShowDialog() == DialogResult.OK)
            {
                RunProcess("7za.exe", "e \"" + fileDiag.FileName + "\" -y");
                return true;
            }
            return false;
        }

        static DateTime Max(DateTime a, DateTime b)
        {
            return a > b ? a : b;
        }

        public static string CreateDateBackup()
        {
            string dataPath = Setting.Get(eSetting.DataPath).Waarde;

            string log = Path.Combine(dataPath, TableRow<LogField, Log>.fileName);
            string band = Path.Combine(dataPath, TableRow<BandField, Band>.fileName);
            string setting = AppPath + "\\" + TableRow<SettingField, Setting>.fileName;
            string dateStr = DateTime.Now.Year.ToString("0###") + DateTime.Now.Month.ToString("0#") + DateTime.Now.Day.ToString("0#");
            string check = Path.Combine(dataPath, "logbackup" + dateStr + ".7z");
            bool cvsPresent = File.Exists(log) && File.Exists(band);
            if (cvsPresent)
            {
                DateTime lastBackupped = File.Exists(check) ? File.GetLastWriteTime(check) : DateTime.MinValue;
                DateTime lastMutation = Max(File.GetLastWriteTime(setting), Max(File.GetLastWriteTime(log), File.GetLastWriteTime(band)));
                if (lastBackupped < lastMutation)
                {
                    string args = string.Format("a \"{0}\" \"{1}\" \"{2}\" \"{3}\"", check, log, band, setting);
                    RunProcess("7za.exe", args);
                }
            }
            return "logbackup" + dateStr;
        }

    }
}
