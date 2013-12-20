using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Deployment.Application;

namespace logboek
{
    public partial class frmLogboek : Form
    {
        LogRow logRow;
        List<BandRow> bandRows;
        int currentCall = 0;

        public frmLogboek()
        {
            InitializeComponent();
            if (ApplicationDeployment.IsNetworkDeployed)
                this.Text += " " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            else
                this.Text += " " + Application.ProductVersion;
            logRow = new LogRow() { txtCall = txtCall, txtCert = txtCert, txtDok = txtDok, txtMemo = txtMemo, txtNaam = txtNaam, txtOpm = txtOpm, txtQRA = txtQRA, txtQTH = txtQTH, lblAfstand = lblBase };
            bandRows = new List<BandRow>();
            bandRows.Add(new BandRow(txtFreq1, txtDatum1, txtTijd1, txtRapp1, chkQSL1, txtQSO1));
            bandRows.Add(new BandRow(txtFreq2, txtDatum2, txtTijd2, txtRapp2, chkQSL2, txtQSO2));
            bandRows.Add(new BandRow(txtFreq3, txtDatum3, txtTijd3, txtRapp3, chkQSL3, txtQSO3));
            bandRows.Add(new BandRow(txtFreq4, txtDatum4, txtTijd4, txtRapp4, chkQSL4, txtQSO4));
            bandRows.Add(new BandRow(txtFreq5, txtDatum5, txtTijd5, txtRapp5, chkQSL5, txtQSO5));
            bandRows.Add(new BandRow(txtFreq6, txtDatum6, txtTijd6, txtRapp6, chkQSL6, txtQSO6));
            bandRows.Add(new BandRow(txtFreq7, txtDatum7, txtTijd7, txtRapp7, chkQSL7, txtQSO7));
            bandRows.Add(new BandRow(txtFreq8, txtDatum8, txtTijd8, txtRapp8, chkQSL8, txtQSO8));
            bandRows.Add(new BandRow(txtFreq9, txtDatum9, txtTijd9, txtRapp9, chkQSL9, txtQSO9));
        }

        #region Helper function

        void TurnOnAutoComplete(TextBox txt, string[] vals)
        {
            txt.AutoCompleteMode = AutoCompleteMode.Suggest;
            txt.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txt.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            txt.AutoCompleteCustomSource.AddRange(vals);
        }

        void SetDefaultFreqs()
        {
            for (int i = 0; i < bandRows.Count; i++)
                bandRows[i].txtFreq.Text = BandRow.freqs[i];
            txtFreq8.Text = "Anders";
        }

        void DisplayLog(int index)
        {
            if (TableRow<LogField, Log>.Rows.Count > index)
            {
                currentCall = Math.Min(TableRow<LogField, Log>.Rows.Count, Math.Max(0, index));
                statusStrip1.Items[1].Text = (currentCall + 1).ToString();
                statusStrip1.Items[3].Text = TableRow<LogField, Log>.Rows.Count.ToString();

                foreach (BandRow row in bandRows)
                    row.Clear();
                if (currentCall > -1 && currentCall < TableRow<LogField, Log>.Rows.Count)
                {
                    logRow.Write(TableRow<LogField, Log>.Rows[currentCall]);
                    Band.InitBandRows(TableRow<LogField, Log>.Rows[currentCall].Call, bandRows);
                }
            }
        }

        void LaadBand()
        {
            TableRow<BandField, Band>.Laad();
            InitBandenMenu();

            foreach (BandRow row in bandRows)
                TurnOnAutoComplete(row.txtFreq, TableRow<BandField, Band>.Rows.GroupBy(band => band.Freq).Select(grp => grp.Key).ToArray());
        }

        void LaadLog()
        {
            TableRow<LogField, Log>.Laad();
            TurnOnAutoComplete(txtCall, TableRow<LogField, Log>.Rows.Select(log => log.Call).ToArray());
            TurnOnAutoComplete(txtNaam, TableRow<LogField, Log>.Rows.Select(log => log.Naam).ToArray());
            TurnOnAutoComplete(txtQRA, TableRow<LogField, Log>.Rows.Select(log => log.Qra).ToArray());
            TurnOnAutoComplete(txtQTH, TableRow<LogField, Log>.Rows.Select(log => log.Qth).ToArray());
            TurnOnAutoComplete(txtDok, TableRow<LogField, Log>.Rows.Select(log => log.Dok).ToArray());
            TurnOnAutoComplete(txtCert, TableRow<LogField, Log>.Rows.Select(log => log.Cert).ToArray());
        }

        void InitBandenMenu()
        {
            ToolStripMenuItem[] items = TableRow<BandField, Band>.Rows.GroupBy(band => band.Freq).OrderBy(grp => Band.toHz(grp.Key)).Select(group => new ToolStripMenuItem(group.Key) { Name = group.Key, Size = new Size(152, 22), Text = (!string.IsNullOrEmpty(group.Key) ? group.Key : "<leeg>"), ToolTipText = group.Count(band => band.Qsl == "J").ToString() + ";" + group.Count(band => band.Qsl != "J").ToString() }).ToArray();
            foreach (ToolStripMenuItem item in items)
            {
                string countYes = item.ToolTipText.Split(';')[0];
                string countNo = item.ToolTipText.Split(';')[1];
                ToolStripMenuItem qslYes = new ToolStripMenuItem("QSL ja: (" + countYes + ")") { Size = new Size(152, 22), Name = "QSLJ" };
                ToolStripMenuItem qslNo = new ToolStripMenuItem("QSL nee: (" + countNo + ")") { Size = new Size(152, 22), Name = "QSLN" };
                ToolStripMenuItem[] subItems =
                    countNo != "0" && countYes != "0" ? new ToolStripMenuItem[] { qslYes, qslNo } :
                    countNo != "0" ? new ToolStripMenuItem[] { qslNo } : new ToolStripMenuItem[] { qslYes };
                foreach (ToolStripMenuItem subItem in subItems)
                    subItem.Click += new EventHandler(tiItem_Click);
                item.DropDownItems.AddRange(subItems);
            }
            this.mnuOvzBand.DropDownItems.Clear();
            this.mnuOvzBand.DropDownItems.AddRange(items);
        }

        void tiItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem ti = sender as ToolStripMenuItem;
            frmBandOvz bandOvz = new frmBandOvz(ti.OwnerItem.Text, ti.Name == "QSLJ" ? "J" : "N");
            bandOvz.ShowDialog();
        }

        void SetButtonEdit(bool edit)
        {
            btnCancel.Enabled = edit;
            btnFirst.Enabled = !edit;
            btnLast.Enabled = !edit;
            btnNext.Enabled = !edit;
            btnPrev.Enabled = !edit;
            btnSave.Enabled = edit;
            btnSearch.Enabled = !edit;
        }

        void ShowSearchResult(LogField sortOptions, string search, int index)
        {
            if (btnSearch.Checked && !string.IsNullOrEmpty(search))
            {
                btnSearch.Checked = false;
                Log found = index > -1 ? TableRow<LogField, Log>.Rows.ElementAt(index) : null;
                TableRow<LogField, Log>.Rows.Sort(new TableRowComparer<LogField, Log>(sortOptions));
                if (found != null)
                    DisplayLog(TableRow<LogField, Log>.Rows.IndexOf(found));
                else
                {
                    currentCall = -1;
                    //DisplayLog(currentCall);
                }

                logRow.SetReadOnly(sortOptions != LogField.Call);
                int i = 0;
                foreach (BandRow row in bandRows)
                {
                    row.SetReadOnly(sortOptions != LogField.Call);
                    if (i < BandRow.freqs.Count() && sortOptions == LogField.Call)
                    {
                        row.txtFreq.Text = BandRow.freqs[i++];
                        row.txtFreq.Enabled = false;
                        row.txtFreq.BackColor = Color.White;
                        row.txtFreq.ForeColor = Color.Black;
                    }
                }
                if (sortOptions == LogField.Call)
                    ModifyCurrentCall();
            }
        }

        void RemoveCurrentCall()
        {
            if (MessageBox.Show("Call " + txtCall.Text + " verwijderen", "Verwijderen", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                TableRow<LogField, Log>.Rows.RemoveAll(log => string.Compare(log.Call, txtCall.Text, true) == 0);
                TurnOnAutoComplete(txtCall, TableRow<LogField, Log>.Rows.Select(l => l.Call).ToArray());
                TableRow<LogField, Log>.Bewaar();
                Band.RemoveCallBanden(txtCall.Text);
                TableRow<BandField, Band>.Bewaar();
                DisplayLog(0);
            }
        }

        void AddNewCall()
        {
            currentCall = -1;
            logRow.Clear();
            foreach (BandRow row in bandRows)
                row.Clear();
            ModifyCurrentCall();
            logRow.SetDefaultFocus();
        }

        void PopulateBackupMenu()
        {
            Backup.PopulateBackupMenu(mnuBackup, item_RestoreClick, delItem_Click, copyToUSBItem_Click, restoreItem_Click);
        }

        void delItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem delItem = sender as ToolStripMenuItem;
            if (MessageBox.Show("Verwijderen " + delItem.OwnerItem.Text, "Vraag", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                File.Delete(delItem.OwnerItem.Name);
                PopulateBackupMenu();
            }
        }

        void restoreItem_Click(object sender, EventArgs e)
        {
            if (Backup.Restore(openFileDialog1))
            {
                LaadBand();
                LaadLog();
                DisplayLog(0);
            }
        }

        void ModifyBanden()
        {
            frmBandEdit bEdit = new frmBandEdit(txtCall.Text);
            bEdit.ShowDialog();

            if (bEdit.IsModified && MessageBox.Show("Wijzigingen opslaan?", "Vraag?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bEdit.Save();
                TableRow<BandField, Band>.Bewaar();
                InitBandenMenu();
                DisplayLog(currentCall);
            }
        }

        void ModifyCurrentCall()
        {
            logRow.SetReadOnly(false);
            int i = 0;
            foreach (BandRow row in bandRows)
            {
                row.SetReadOnly(false);
                if (i < BandRow.freqs.Count())
                {
                    row.txtFreq.Text = BandRow.freqs[i++];
                    row.txtFreq.Enabled = false;
                    row.txtFreq.BackColor = Color.White;
                    row.txtFreq.ForeColor = Color.Black;
                }

            }
            SetButtonEdit(true);
        }

        #endregion

        #region Event handlers
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyValue)
            {
                case 40:
                    GotoNextInput();
                    break;
                case 38:
                    GotoPrevInput();
                    break;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (btnNext.Enabled && !btnSearch.Checked)
            {
                try
                {
                    bool handled = true;
                    switch (e.KeyChar)
                    {
                        case 'N':
                        case 'n':
                            AddNewCall();
                            break;
                        case 'V':
                        case 'v':
                            RemoveCurrentCall();
                            break;
                        case 'W':
                        case 'w':
                            ModifyCurrentCall();
                            break;
                        case 'B':
                        case 'b':
                            ModifyBanden();
                            break;
                        case '7':
                            DisplayLog(0);
                            break;
                        case '4':
                            DisplayLog(currentCall - 1);
                            break;
                        case '6':
                            DisplayLog(currentCall + 1);
                            break;
                        case '1':
                            DisplayLog(TableRow<LogField, Log>.Rows.Count - 1);
                            break;
                        default:
                            handled = false;
                            break;
                    }
                    e.Handled = handled;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
            else if ((int)e.KeyChar == 27)
            {
                btnCancel_Click(null, null);
                e.Handled = true;
            }
        }

        void copyToUSBItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            folderBrowserDialog1.ShowNewFolderButton = false;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string usbDir = folderBrowserDialog1.SelectedPath.TrimEnd('\\');
                File.Copy(item.OwnerItem.Name, usbDir + "\\" + item.OwnerItem.Text, true);
            }
        }

        void item_RestoreClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (MessageBox.Show(item.OwnerItem.Text + " terugzetten", "Vraag", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Backup.CreateDateBackup();
                Backup.RunProcess("7za.exe", "e " + item.OwnerItem.Text + " -y");
                LaadBand();
                LaadLog();
                DisplayLog(0);
            }
        }

        private void frmLogboek_Load(object sender, EventArgs e)
        {
            Backup.CheckDirs();

            TableRow<SettingField, Setting>.Laad();
            PopulateBackupMenu();

            LaadBand();
            LaadLog();

            DisplayLog(0);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            DisplayLog(currentCall + 1);
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            DisplayLog(currentCall - 1);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            RemoveCurrentCall();
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            DisplayLog(0);
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            DisplayLog(TableRow<LogField, Log>.Rows.Count - 1);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            logRow.SetReadOnly(true);
            foreach (BandRow row in bandRows)
                row.SetReadOnly(true);
            SetButtonEdit(false);
            if (currentCall >= 0)
            {
                string orgCall = TableRow<LogField, Log>.Rows[currentCall].Call;
                if (string.Compare(orgCall, txtCall.Text, true) != 0)
                {
                    if (MessageBox.Show("Call " + orgCall + " hernoemen naar " + txtCall.Text + " (nee is kopie maken)?", "Vraag", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        TableRow<LogField, Log>.Rows.RemoveAll(l => string.Compare(l.Call, orgCall, true) == 0);
                        Band.RemoveCallBanden(orgCall);
                    }
                }
            }
            Log log = TableRow<LogField, Log>.Rows.FirstOrDefault(l => string.Compare(l.Call, txtCall.Text, true) == 0);
            if (log == null)
            {
                log = new Log();
                log.Call = txtCall.Text;
                TableRow<LogField, Log>.Rows.Add(log);
                TableRow<LogField, Log>.Rows.Sort(new TableRowComparer<LogField, Log>(LogField.Call));
            }
            logRow.Read(log);
            Band.RemoveCallBanden(log.Call);
            for (int i = 0; i < bandRows.Count; i++)
                if (bandRows[i].Filled)
                    TableRow<BandField, Band>.Rows.Add(bandRows[i].Write(i, txtCall.Text));
            DisplayLog(TableRow<LogField, Log>.Rows.IndexOf(log));
            TurnOnAutoComplete(txtCall, TableRow<LogField, Log>.Rows.Select(l => l.Call).ToArray());
            TableRow<BandField, Band>.Bewaar();
            TableRow<LogField, Log>.Bewaar();
        }

        void Cancel()
        {
            //if (Setting.Get(eSetting.ConfirmLineDelete).Waarde == "J" && MessageBox.Show("Afbreken?", "Afbreken", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //{
            logRow.SetReadOnly(true);
            foreach (BandRow row in bandRows)
                row.SetReadOnly(true);
            DisplayLog(currentCall);
            SetButtonEdit(false);
            //}
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //SetButtonEdit(btnSearch.Checked);
            logRow.SetReadOnly(!btnSearch.Checked);
            if (btnSearch.Checked)
                logRow.Clear();
            foreach (BandRow row in bandRows)
            {
                if (btnSearch.Checked)
                    row.Clear();
                row.SetReadOnly(true);
            }
            logRow.SetDefaultFocus();
        }

        private void txtCall_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCall.Text))
            {
                if (Setting.Get(eSetting.CallToUpperCase).Waarde == "J")
                    txtCall.Text = txtCall.Text.ToUpper();
                int index = TableRow<LogField, Log>.Rows.FindIndex(log => string.Compare(log.Call, txtCall.Text, true) == 0);
                if (btnSearch.Checked)
                    ShowSearchResult(LogField.Call, txtCall.Text, index);
                else if (index > -1)
                    DisplayLog(index);
            }
        }

        private void txtOpm_Leave(object sender, EventArgs e)
        {
            ShowSearchResult(LogField.Opm1, txtOpm.Text, TableRow<LogField, Log>.Rows.Count(l => string.Compare(l.Opm1, txtOpm.Text, true) < 0));
        }

        private void txtNaam_Leave(object sender, EventArgs e)
        {
            ShowSearchResult(LogField.Naam, txtNaam.Text, TableRow<LogField, Log>.Rows.Count(l => string.Compare(l.Naam, txtNaam.Text, true) < 0));
        }

        private void txtQTH_Leave(object sender, EventArgs e)
        {
            ShowSearchResult(LogField.Qth, txtQTH.Text, TableRow<LogField, Log>.Rows.Count(l => string.Compare(l.Qth, txtQTH.Text, true) < 0));
        }

        private void txtQRA_Leave(object sender, EventArgs e)
        {
            ShowSearchResult(LogField.Qra, txtQRA.Text, TableRow<LogField, Log>.Rows.Count(l => string.Compare(l.Qra, txtQRA.Text, true) < 0));
            try
            {
                string baseQra = Setting.Get(eSetting.QraBase).Waarde;
                if (baseQra != "" && txtQRA.Text != "")
                    lblBase.Text = MaidenheadLocator.Distance(baseQra, txtQRA.Text).ToString("#.##") + " km";
            }
            catch
            {
                lblBase.Text = "";
            }

        }

        private void txtDok_Leave(object sender, EventArgs e)
        {
            ShowSearchResult(LogField.Dok, txtDok.Text, TableRow<LogField, Log>.Rows.Count(l => string.Compare(l.Dok, txtDok.Text, true) < 0));
        }

        private void txtCert_Leave(object sender, EventArgs e)
        {
            ShowSearchResult(LogField.Cert, txtCert.Text, TableRow<LogField, Log>.Rows.Count(l => string.Compare(l.Cert, txtCert.Text, true) < 0));
        }

        private void txtMemo_Leave(object sender, EventArgs e)
        {
            ShowSearchResult(LogField.Opm2, txtMemo.Text, TableRow<LogField, Log>.Rows.Count(l => string.Compare(l.Opm2, txtMemo.Text, true) < 0));
        }

        private void overzichtQSOsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmQSO qso = new frmQSO();
            qso.ShowDialog();
        }

        private void verzendenQSLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            frmDate datePicker = new frmDate();
            if (datePicker.ShowDialog() == DialogResult.OK)
            {
                frmBandQsl bShowQsl = new frmBandQsl(datePicker.ChosenDate);
                bShowQsl.ShowDialog();
            }
        }

        private void btnBandEdit_Click(object sender, EventArgs e)
        {
            ModifyBanden();
        }

        private void overzichtCallsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCallOvz frmCalls = new frmCallOvz();
            frmCalls.ShowDialog();
        }

        private void frmLogboek_FormClosing(object sender, FormClosingEventArgs e)
        {
            Backup.CreateDateBackup();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            ModifyCurrentCall();
        }

        bool WalkControls(Panel pnl, string stopControl, Control nextControl, bool forward)
        {
            Control c;
            TextBox t;
            foreach (Control ctl in pnl.Controls)
                if (ctl.Focused)
                {
                    c = ctl;
                    t = c as TextBox;
                    if (ctl.Name != stopControl)
                        do
                        {
                            c = GetNextControl(c, forward);
                            t = c as TextBox;
                        }
                        while ((t != null ? (t.ReadOnly || !t.Enabled) : !c.CanFocus) && c.Name != stopControl);
                    else
                    {
                        c = nextControl;
                        t = c as TextBox;
                    }
                    c.Focus();
                    return true;
                }
            return false;
        }

        void GotoNextInput()
        {
            if (!WalkControls(pnlCall, "txtMemo", txtDatum1, true))
                WalkControls(pnlBanden, "chkQSL9", txtCall, true);
        }

        void GotoPrevInput()
        {
            if (!WalkControls(pnlBanden, "txtDatum1", txtMemo, false))
                WalkControls(pnlCall, "txtCall", chkQSL9, false);
        }

        #endregion

        private void btnEnter_Click(object sender, EventArgs e)
        {
            GotoNextInput();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettings frm = new frmSettings();
            frm.ShowDialog();
            if (frm.IsModified && MessageBox.Show("Wijzigingen opslaan?", "Vraag?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                frm.Save();
        }

        private void btnCancel2_Click(object sender, EventArgs e)
        {
            //if (!btnNext.Enabled && !btnSearch.Checked)
            Cancel();
        }

        private void instellingenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSettings frm = new frmSettings();
            frm.ShowDialog();
            if (frm.IsModified && MessageBox.Show("Wijzigingen opslaan?", "Vraag?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                frm.Save();
        }

        private void nieuwToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (btnNext.Enabled && !btnSearch.Checked)
                AddNewCall();
        }

        private void verwijderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (btnNext.Enabled && !btnSearch.Checked)
                RemoveCurrentCall();
        }

        private void wijzigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (btnNext.Enabled && !btnSearch.Checked)
                ModifyCurrentCall();
        }

        private void bandenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (btnNext.Enabled && !btnSearch.Checked)
                ModifyBanden();
        }

        private void frmLogboek_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
            e.Cancel = true;
        }

        private void frmLogboek_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
            hlpevent.Handled = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Setting.Beep();
        }

        private void pieptestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting.Beep();
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }
    }

    // Define the frequencies of notes in an octave, as well as 
    // silence (rest).
    enum Tone
    {
        REST = 0,
        GbelowC = 196,
        A = 220,
        Asharp = 233,
        B = 247,
        C = 262,
        Csharp = 277,
        D = 294,
        Dsharp = 311,
        E = 330,
        F = 349,
        Fsharp = 370,
        G = 392,
        Gsharp = 415,
    }

    // Define the duration of a note in units of milliseconds.
    enum Duration
    {
        WHOLE = 1600,
        HALF = WHOLE / 2,
        QUARTER = HALF / 2,
        EIGHTH = QUARTER / 2,
        SIXTEENTH = EIGHTH / 2,
    }

    // Define a note as a frequency (tone) and the amount of 
    // time (duration) the note plays.
    struct Note
    {
        Tone toneVal;
        Duration durVal;

        // Define a constructor to create a specific note.
        public Note(Tone frequency, Duration time)
        {
            toneVal = frequency;
            durVal = time;
        }

        // Define properties to return the note's tone and duration.
        public Tone NoteTone { get { return toneVal; } }
        public Duration NoteDuration { get { return durVal; } }
    }
}
