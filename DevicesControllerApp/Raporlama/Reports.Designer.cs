namespace DevicesControllerApp.Raporlama
{
    partial class Reports
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_Isim = new System.Windows.Forms.TextBox();
            this.txt_Soyisim = new System.Windows.Forms.TextBox();
            this.txt_TcNo = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dtBaslangic = new System.Windows.Forms.DateTimePicker();
            this.dtBitis = new System.Windows.Forms.DateTimePicker();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnRaporOlustur = new System.Windows.Forms.Button();
            this.rbHastalariListele = new System.Windows.Forms.RadioButton();
            this.rbSeanslarıListele = new System.Windows.Forms.RadioButton();
            this.rbOperatorleriListele = new System.Windows.Forms.RadioButton();
            this.rbLoglariListele = new System.Windows.Forms.RadioButton();
            this.btnYazdir = new System.Windows.Forms.Button();
            this.btnMail = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnMail);
            this.panel1.Controls.Add(this.btnYazdir);
            this.panel1.Controls.Add(this.btnRaporOlustur);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(700, 868);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(700, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1091, 868);
            this.panel2.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txt_TcNo);
            this.groupBox1.Controls.Add(this.txt_Soyisim);
            this.groupBox1.Controls.Add(this.txt_Isim);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(48, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(605, 214);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "İsim";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Soyisim";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Tc No";
            // 
            // txt_Isim
            // 
            this.txt_Isim.Location = new System.Drawing.Point(149, 46);
            this.txt_Isim.Name = "txt_Isim";
            this.txt_Isim.Size = new System.Drawing.Size(100, 22);
            this.txt_Isim.TabIndex = 3;
            // 
            // txt_Soyisim
            // 
            this.txt_Soyisim.Location = new System.Drawing.Point(149, 101);
            this.txt_Soyisim.Name = "txt_Soyisim";
            this.txt_Soyisim.Size = new System.Drawing.Size(100, 22);
            this.txt_Soyisim.TabIndex = 4;
            // 
            // txt_TcNo
            // 
            this.txt_TcNo.Location = new System.Drawing.Point(149, 159);
            this.txt_TcNo.Name = "txt_TcNo";
            this.txt_TcNo.Size = new System.Drawing.Size(100, 22);
            this.txt_TcNo.TabIndex = 5;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dtBitis);
            this.groupBox2.Controls.Add(this.dtBaslangic);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(48, 293);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(605, 217);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Başlangıç:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 16);
            this.label5.TabIndex = 1;
            this.label5.Text = "Bitiş:";
            // 
            // dtBaslangic
            // 
            this.dtBaslangic.Location = new System.Drawing.Point(149, 68);
            this.dtBaslangic.Name = "dtBaslangic";
            this.dtBaslangic.Size = new System.Drawing.Size(200, 22);
            this.dtBaslangic.TabIndex = 2;
            // 
            // dtBitis
            // 
            this.dtBitis.Location = new System.Drawing.Point(149, 133);
            this.dtBitis.Name = "dtBitis";
            this.dtBitis.Size = new System.Drawing.Size(200, 22);
            this.dtBitis.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbLoglariListele);
            this.groupBox3.Controls.Add(this.rbOperatorleriListele);
            this.groupBox3.Controls.Add(this.rbSeanslarıListele);
            this.groupBox3.Controls.Add(this.rbHastalariListele);
            this.groupBox3.Location = new System.Drawing.Point(48, 534);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(605, 232);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "groupBox3";
            // 
            // btnRaporOlustur
            // 
            this.btnRaporOlustur.Location = new System.Drawing.Point(55, 787);
            this.btnRaporOlustur.Name = "btnRaporOlustur";
            this.btnRaporOlustur.Size = new System.Drawing.Size(213, 59);
            this.btnRaporOlustur.TabIndex = 3;
            this.btnRaporOlustur.Text = "RAPOR AL";
            this.btnRaporOlustur.UseVisualStyleBackColor = true;
            // 
            // rbHastalariListele
            // 
            this.rbHastalariListele.AutoSize = true;
            this.rbHastalariListele.Location = new System.Drawing.Point(7, 34);
            this.rbHastalariListele.Name = "rbHastalariListele";
            this.rbHastalariListele.Size = new System.Drawing.Size(124, 20);
            this.rbHastalariListele.TabIndex = 0;
            this.rbHastalariListele.TabStop = true;
            this.rbHastalariListele.Text = "Hastaları Listele";
            this.rbHastalariListele.UseVisualStyleBackColor = true;
            // 
            // rbSeanslarıListele
            // 
            this.rbSeanslarıListele.AutoSize = true;
            this.rbSeanslarıListele.Location = new System.Drawing.Point(7, 78);
            this.rbSeanslarıListele.Name = "rbSeanslarıListele";
            this.rbSeanslarıListele.Size = new System.Drawing.Size(127, 20);
            this.rbSeanslarıListele.TabIndex = 1;
            this.rbSeanslarıListele.TabStop = true;
            this.rbSeanslarıListele.Text = "Seansları Listele";
            this.rbSeanslarıListele.UseVisualStyleBackColor = true;
            // 
            // rbOperatorleriListele
            // 
            this.rbOperatorleriListele.AutoSize = true;
            this.rbOperatorleriListele.Location = new System.Drawing.Point(7, 126);
            this.rbOperatorleriListele.Name = "rbOperatorleriListele";
            this.rbOperatorleriListele.Size = new System.Drawing.Size(141, 20);
            this.rbOperatorleriListele.TabIndex = 2;
            this.rbOperatorleriListele.TabStop = true;
            this.rbOperatorleriListele.Text = "Operatörleri Listele";
            this.rbOperatorleriListele.UseVisualStyleBackColor = true;
            // 
            // rbLoglariListele
            // 
            this.rbLoglariListele.AutoSize = true;
            this.rbLoglariListele.Location = new System.Drawing.Point(7, 168);
            this.rbLoglariListele.Name = "rbLoglariListele";
            this.rbLoglariListele.Size = new System.Drawing.Size(111, 20);
            this.rbLoglariListele.TabIndex = 3;
            this.rbLoglariListele.TabStop = true;
            this.rbLoglariListele.Text = "Logları Listele";
            this.rbLoglariListele.UseVisualStyleBackColor = true;
            // 
            // btnYazdir
            // 
            this.btnYazdir.Location = new System.Drawing.Point(284, 787);
            this.btnYazdir.Name = "btnYazdir";
            this.btnYazdir.Size = new System.Drawing.Size(144, 62);
            this.btnYazdir.TabIndex = 4;
            this.btnYazdir.Text = "YAZDIR";
            this.btnYazdir.UseVisualStyleBackColor = true;
            // 
            // btnMail
            // 
            this.btnMail.Location = new System.Drawing.Point(445, 787);
            this.btnMail.Name = "btnMail";
            this.btnMail.Size = new System.Drawing.Size(166, 62);
            this.btnMail.TabIndex = 5;
            this.btnMail.Text = "Mail Oluştur";
            this.btnMail.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(27, 27);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(817, 819);
            this.dataGridView1.TabIndex = 0;
            // 
            // Reports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Reports";
            this.Size = new System.Drawing.Size(1791, 868);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRaporOlustur;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DateTimePicker dtBitis;
        private System.Windows.Forms.DateTimePicker dtBaslangic;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_TcNo;
        private System.Windows.Forms.TextBox txt_Soyisim;
        private System.Windows.Forms.TextBox txt_Isim;
        private System.Windows.Forms.Button btnMail;
        private System.Windows.Forms.Button btnYazdir;
        private System.Windows.Forms.RadioButton rbLoglariListele;
        private System.Windows.Forms.RadioButton rbOperatorleriListele;
        private System.Windows.Forms.RadioButton rbSeanslarıListele;
        private System.Windows.Forms.RadioButton rbHastalariListele;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}
