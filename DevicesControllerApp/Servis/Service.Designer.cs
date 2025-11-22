namespace DevicesControllerApp.Servis
{
    partial class Service
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Service));
            this.splitLogMain = new System.Windows.Forms.SplitContainer();
            this.dgvLoglar = new System.Windows.Forms.DataGridView();
            this.txtLogDetay = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pnlLogFiltre = new System.Windows.Forms.Panel();
            this.btnLogExport = new System.Windows.Forms.Button();
            this.btnLogGetir = new System.Windows.Forms.Button();
            this.txtLogArama = new System.Windows.Forms.TextBox();
            this.cmbLogSeviye = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpLogBitis = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpLogBaslangic = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbLogTipi = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.splitLogMain)).BeginInit();
            this.splitLogMain.Panel1.SuspendLayout();
            this.splitLogMain.Panel2.SuspendLayout();
            this.splitLogMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoglar)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.pnlLogFiltre.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitLogMain
            // 
            resources.ApplyResources(this.splitLogMain, "splitLogMain");
            this.splitLogMain.Name = "splitLogMain";
            // 
            // splitLogMain.Panel1
            // 
            resources.ApplyResources(this.splitLogMain.Panel1, "splitLogMain.Panel1");
            this.splitLogMain.Panel1.Controls.Add(this.dgvLoglar);
            // 
            // splitLogMain.Panel2
            // 
            resources.ApplyResources(this.splitLogMain.Panel2, "splitLogMain.Panel2");
            this.splitLogMain.Panel2.Controls.Add(this.txtLogDetay);
            // 
            // dgvLoglar
            // 
            resources.ApplyResources(this.dgvLoglar, "dgvLoglar");
            this.dgvLoglar.AllowUserToAddRows = false;
            this.dgvLoglar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoglar.Name = "dgvLoglar";
            this.dgvLoglar.ReadOnly = true;
            this.dgvLoglar.RowTemplate.Height = 24;
            this.dgvLoglar.SelectionChanged += new System.EventHandler(this.dgvLoglar_SelectionChanged);
            // 
            // txtLogDetay
            // 
            resources.ApplyResources(this.txtLogDetay, "txtLogDetay");
            this.txtLogDetay.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.txtLogDetay.Name = "txtLogDetay";
            this.txtLogDetay.ReadOnly = true;
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.splitLogMain);
            this.tabPage2.Controls.Add(this.pnlLogFiltre);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pnlLogFiltre
            // 
            resources.ApplyResources(this.pnlLogFiltre, "pnlLogFiltre");
            this.pnlLogFiltre.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.pnlLogFiltre.Controls.Add(this.btnLogExport);
            this.pnlLogFiltre.Controls.Add(this.btnLogGetir);
            this.pnlLogFiltre.Controls.Add(this.txtLogArama);
            this.pnlLogFiltre.Controls.Add(this.cmbLogSeviye);
            this.pnlLogFiltre.Controls.Add(this.label5);
            this.pnlLogFiltre.Controls.Add(this.label4);
            this.pnlLogFiltre.Controls.Add(this.dtpLogBitis);
            this.pnlLogFiltre.Controls.Add(this.label3);
            this.pnlLogFiltre.Controls.Add(this.dtpLogBaslangic);
            this.pnlLogFiltre.Controls.Add(this.label2);
            this.pnlLogFiltre.Controls.Add(this.cmbLogTipi);
            this.pnlLogFiltre.Controls.Add(this.label1);
            this.pnlLogFiltre.Name = "pnlLogFiltre";
            // 
            // btnLogExport
            // 
            resources.ApplyResources(this.btnLogExport, "btnLogExport");
            this.btnLogExport.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnLogExport.Name = "btnLogExport";
            this.btnLogExport.UseVisualStyleBackColor = false;
            this.btnLogExport.Click += new System.EventHandler(this.btnLogExport_Click);
            // 
            // btnLogGetir
            // 
            resources.ApplyResources(this.btnLogGetir, "btnLogGetir");
            this.btnLogGetir.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnLogGetir.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnLogGetir.Name = "btnLogGetir";
            this.btnLogGetir.UseVisualStyleBackColor = false;
            this.btnLogGetir.Click += new System.EventHandler(this.btnLogGetir_Click);
            // 
            // txtLogArama
            // 
            resources.ApplyResources(this.txtLogArama, "txtLogArama");
            this.txtLogArama.BackColor = System.Drawing.SystemColors.MenuBar;
            this.txtLogArama.Name = "txtLogArama";
            // 
            // cmbLogSeviye
            // 
            resources.ApplyResources(this.cmbLogSeviye, "cmbLogSeviye");
            this.cmbLogSeviye.BackColor = System.Drawing.SystemColors.MenuBar;
            this.cmbLogSeviye.FormattingEnabled = true;
            this.cmbLogSeviye.Items.AddRange(new object[] {
            resources.GetString("cmbLogSeviye.Items"),
            resources.GetString("cmbLogSeviye.Items1"),
            resources.GetString("cmbLogSeviye.Items2"),
            resources.GetString("cmbLogSeviye.Items3"),
            resources.GetString("cmbLogSeviye.Items4")});
            this.cmbLogSeviye.Name = "cmbLogSeviye";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // dtpLogBitis
            // 
            resources.ApplyResources(this.dtpLogBitis, "dtpLogBitis");
            this.dtpLogBitis.Name = "dtpLogBitis";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // dtpLogBaslangic
            // 
            resources.ApplyResources(this.dtpLogBaslangic, "dtpLogBaslangic");
            this.dtpLogBaslangic.Name = "dtpLogBaslangic";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cmbLogTipi
            // 
            resources.ApplyResources(this.cmbLogTipi, "cmbLogTipi");
            this.cmbLogTipi.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbLogTipi.FormattingEnabled = true;
            this.cmbLogTipi.Items.AddRange(new object[] {
            resources.GetString("cmbLogTipi.Items"),
            resources.GetString("cmbLogTipi.Items1")});
            this.cmbLogTipi.Name = "cmbLogTipi";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tabPage3
            // 
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // Service
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.checkBox1);
            this.Name = "Service";
            this.splitLogMain.Panel1.ResumeLayout(false);
            this.splitLogMain.Panel2.ResumeLayout(false);
            this.splitLogMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitLogMain)).EndInit();
            this.splitLogMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoglar)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.pnlLogFiltre.ResumeLayout(false);
            this.pnlLogFiltre.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel pnlLogFiltre;
        private System.Windows.Forms.Button btnLogExport;
        private System.Windows.Forms.Button btnLogGetir;
        private System.Windows.Forms.TextBox txtLogArama;
        private System.Windows.Forms.ComboBox cmbLogSeviye;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpLogBitis;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpLogBaslangic;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbLogTipi;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitLogMain;
        private System.Windows.Forms.DataGridView dgvLoglar;
        private System.Windows.Forms.TextBox txtLogDetay;
    }
}
