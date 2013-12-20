namespace logboek
{
    partial class frmQSO
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridViewQSO = new System.Windows.Forms.DataGridView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewQSO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewQSO
            // 
            this.dataGridViewQSO.AllowUserToAddRows = false;
            this.dataGridViewQSO.AllowUserToDeleteRows = false;
            this.dataGridViewQSO.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewQSO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewQSO.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewQSO.Name = "dataGridViewQSO";
            this.dataGridViewQSO.ReadOnly = true;
            this.dataGridViewQSO.Size = new System.Drawing.Size(244, 387);
            this.dataGridViewQSO.TabIndex = 0;
            // 
            // frmQSO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 387);
            this.Controls.Add(this.dataGridViewQSO);
            this.Name = "frmQSO";
            this.Text = "frmQSO";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewQSO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewQSO;
        private System.Windows.Forms.BindingSource bindingSource1;
    }
}