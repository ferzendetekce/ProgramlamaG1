namespace DevicesControllerApp.Servis
{
    partial class Service
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        
        private void InitializeComponent()
        {
            // 1. Tüm Kontrolleri Tanımla
            this.gboxMotorControl = new System.Windows.Forms.GroupBox();
            this.lblMotorStatus = new System.Windows.Forms.Label();
            this.lblServoDistance = new System.Windows.Forms.Label();
            this.txtServoDistance = new System.Windows.Forms.TextBox();
            this.lblServoSpeed = new System.Windows.Forms.Label();
            this.txtServoSpeed = new System.Windows.Forms.TextBox();
            this.btnServoMove = new System.Windows.Forms.Button();
            this.gboxSensorData = new System.Windows.Forms.GroupBox();
            this.pnlLimitSwitches = new System.Windows.Forms.FlowLayoutPanel();
            this.lblLSXMin = new System.Windows.Forms.Label();
            this.lblLSXMax = new System.Windows.Forms.Label(); 
            this.lblLoadCellValue = new System.Windows.Forms.Label();
            this.lblLoadCellTitle = new System.Windows.Forms.Label();
            this.gboxOperations = new System.Windows.Forms.GroupBox();
            this.btnCalibration = new System.Windows.Forms.Button();
            this.btnHomingAll = new System.Windows.Forms.Button();
            
            // 2. Kontrollerin Düzenini Askıya Al
            this.gboxMotorControl.SuspendLayout();
            this.gboxSensorData.SuspendLayout();
            this.pnlLimitSwitches.SuspendLayout();
            this.gboxOperations.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // gboxMotorControl
            // 
            this.gboxMotorControl.Controls.Add(this.lblMotorStatus);
            this.gboxMotorControl.Controls.Add(this.lblServoDistance);
            this.gboxMotorControl.Controls.Add(this.lblServoSpeed);
            this.gboxMotorControl.Controls.Add(this.txtServoDistance);
            this.gboxMotorControl.Controls.Add(this.txtServoSpeed);
            this.gboxMotorControl.Controls.Add(this.btnServoMove);
            this.gboxMotorControl.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.gboxMotorControl.Location = new System.Drawing.Point(20, 20);
            this.gboxMotorControl.Name = "gboxMotorControl";
            this.gboxMotorControl.Size = new System.Drawing.Size(400, 250);
            this.gboxMotorControl.TabIndex = 0;
            this.gboxMotorControl.TabStop = false;
            this.gboxMotorControl.Text = "Motor Kontrol Paneli (Manuel)";
            // 
            // lblMotorStatus
            // 
            this.lblMotorStatus.AutoSize = true;
            this.lblMotorStatus.Location = new System.Drawing.Point(20, 160);
            this.lblMotorStatus.Name = "lblMotorStatus";
            this.lblMotorStatus.Size = new System.Drawing.Size(150, 16);
            this.lblMotorStatus.TabIndex = 6;
            this.lblMotorStatus.Text = "Servo 1 Durumu: Hazır";
            // 
            // lblServoDistance
            // 
            this.lblServoDistance.AutoSize = true;
            this.lblServoDistance.Location = new System.Drawing.Point(20, 80);
            this.lblServoDistance.Name = "lblServoDistance";
            this.lblServoDistance.Size = new System.Drawing.Size(55, 16);
            this.lblServoDistance.TabIndex = 5;
            this.lblServoDistance.Text = "Mesafe:";
            // 
            // txtServoDistance
            // 
            this.txtServoDistance.Location = new System.Drawing.Point(80, 77);
            this.txtServoDistance.Name = "txtServoDistance";
            this.txtServoDistance.Size = new System.Drawing.Size(100, 23);
            this.txtServoDistance.TabIndex = 4;
            this.txtServoDistance.Text = "5000";
            // 
            // lblServoSpeed
            // 
            this.lblServoSpeed.AutoSize = true;
            this.lblServoSpeed.Location = new System.Drawing.Point(20, 47);
            this.lblServoSpeed.Name = "lblServoSpeed";
            this.lblServoSpeed.Size = new System.Drawing.Size(48, 16);
            this.lblServoSpeed.TabIndex = 3;
            this.lblServoSpeed.Text = "Hız (Pulse/s):";
            // 
            // txtServoSpeed
            // 
            this.txtServoSpeed.Location = new System.Drawing.Point(80, 44);
            this.txtServoSpeed.Name = "txtServoSpeed";
            this.txtServoSpeed.Size = new System.Drawing.Size(100, 23);
            this.txtServoSpeed.TabIndex = 2;
            this.txtServoSpeed.Text = "1000";
            // 
            // btnServoMove
            // 
            this.btnServoMove.Location = new System.Drawing.Point(200, 40);
            this.btnServoMove.Name = "btnServoMove";
            this.btnServoMove.Size = new System.Drawing.Size(180, 30);
            this.btnServoMove.TabIndex = 1;
            this.btnServoMove.Text = "Servo 1 Hareket Başlat";
            this.btnServoMove.UseVisualStyleBackColor = true;
            this.btnServoMove.Click += new System.EventHandler(this.btnServoMove_Click);
            // 
            // gboxSensorData
            // 
            this.gboxSensorData.Controls.Add(this.pnlLimitSwitches);
            this.gboxSensorData.Controls.Add(this.lblLoadCellValue);
            this.gboxSensorData.Controls.Add(this.lblLoadCellTitle);
            this.gboxSensorData.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.gboxSensorData.Location = new System.Drawing.Point(450, 20);
            this.gboxSensorData.Name = "gboxSensorData";
            this.gboxSensorData.Size = new System.Drawing.Size(400, 250);
            this.gboxSensorData.TabIndex = 1;
            this.gboxSensorData.TabStop = false;
            this.gboxSensorData.Text = "Sensor ve Durum Bilgileri";
            // 
            // pnlLimitSwitches
            // 
            this.pnlLimitSwitches.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLimitSwitches.Controls.Add(this.lblLSXMin);
            this.pnlLimitSwitches.Controls.Add(this.lblLSXMax);
            this.pnlLimitSwitches.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.pnlLimitSwitches.Location = new System.Drawing.Point(20, 80);
            this.pnlLimitSwitches.Name = "pnlLimitSwitches";
            this.pnlLimitSwitches.Size = new System.Drawing.Size(360, 150);
            this.pnlLimitSwitches.TabIndex = 3;
            // 
            // lblLSXMin
            // 
            this.lblLSXMin.AutoSize = true;
            this.lblLSXMin.BackColor = System.Drawing.Color.Red;
            this.lblLSXMin.ForeColor = System.Drawing.Color.White;
            this.lblLSXMin.Margin = new System.Windows.Forms.Padding(5);
            this.lblLSXMin.Padding = new System.Windows.Forms.Padding(5);
            this.lblLSXMin.Size = new System.Drawing.Size(89, 26); 
            this.lblLSXMin.TabIndex = 0; 
            this.lblLSXMin.Text = "X Min PASİF";
            // 
            // lblLSXMax
            // 
            this.lblLSXMax.AutoSize = true;
            this.lblLSXMax.BackColor = System.Drawing.Color.Red;
            this.lblLSXMax.ForeColor = System.Drawing.Color.White;
            this.lblLSXMax.Margin = new System.Windows.Forms.Padding(5);
            this.lblLSXMax.Padding = new System.Windows.Forms.Padding(5);
            this.lblLSXMax.Size = new System.Drawing.Size(92, 26); 
            this.lblLSXMax.TabIndex = 1; 
            this.lblLSXMax.Text = "X Max PASİF";
            // 
            // lblLoadCellValue
            // 
            this.lblLoadCellValue.AutoSize = true;
            this.lblLoadCellValue.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.lblLoadCellValue.ForeColor = System.Drawing.Color.Blue;
            this.lblLoadCellValue.Location = new System.Drawing.Point(220, 40);
            this.lblLoadCellValue.Name = "lblLoadCellValue";
            this.lblLoadCellValue.Size = new System.Drawing.Size(59, 19);
            this.lblLoadCellValue.TabIndex = 1;
            this.lblLoadCellValue.Text = "0.00 kg";
            // 
            // lblLoadCellTitle
            // 
            this.lblLoadCellTitle.AutoSize = true;
            this.lblLoadCellTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.lblLoadCellTitle.Location = new System.Drawing.Point(20, 40);
            this.lblLoadCellTitle.Name = "lblLoadCellTitle";
            this.lblLoadCellTitle.Size = new System.Drawing.Size(149, 19);
            this.lblLoadCellTitle.TabIndex = 0;
            this.lblLoadCellTitle.Text = "LoadCell Ağırlığı:";
            // 
            // gboxOperations
            // 
            this.gboxOperations.Controls.Add(this.btnCalibration);
            this.gboxOperations.Controls.Add(this.btnHomingAll);
            this.gboxOperations.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.gboxOperations.Location = new System.Drawing.Point(20, 290);
            this.gboxOperations.Name = "gboxOperations";
            this.gboxOperations.Size = new System.Drawing.Size(400, 100);
            this.gboxOperations.TabIndex = 2;
            this.gboxOperations.TabStop = false;
            this.gboxOperations.Text = "Homing ve Kalibrasyon İşlemleri";
            // 
            // btnCalibration
            // 
            this.btnCalibration.Location = new System.Drawing.Point(200, 40);
            this.btnCalibration.Name = "btnCalibration";
            this.btnCalibration.Size = new System.Drawing.Size(180, 40);
            this.btnCalibration.TabIndex = 1;
            this.btnCalibration.Text = "Kalibrasyon Başlat";
            this.btnCalibration.UseVisualStyleBackColor = true;
            this.btnCalibration.Click += new System.EventHandler(this.btnCalibration_Click);
            // 
            // btnHomingAll
            // 
            this.btnHomingAll.Location = new System.Drawing.Point(20, 40);
            this.btnHomingAll.Name = "btnHomingAll";
            this.btnHomingAll.Size = new System.Drawing.Size(160, 40);
            this.btnHomingAll.TabIndex = 0;
            this.btnHomingAll.Text = "Tüm Eksenleri Sıfırla (Homing)";
            this.btnHomingAll.UseVisualStyleBackColor = true;
            this.btnHomingAll.Click += new System.EventHandler(this.btnHomingAll_Click);
            // 
            // Service
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gboxOperations);
            this.Controls.Add(this.gboxSensorData);
            this.Controls.Add(this.gboxMotorControl);

            this.Name = "Service"; 
            this.Size = new System.Drawing.Size(900, 700);
            
            // 3. Kontrolleri Devam Ettir
            this.gboxMotorControl.ResumeLayout(false);
            this.gboxMotorControl.PerformLayout();
            this.gboxSensorData.ResumeLayout(false);
            this.gboxSensorData.PerformLayout();
            this.pnlLimitSwitches.ResumeLayout(false);
            this.pnlLimitSwitches.PerformLayout();
            this.gboxOperations.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        
        #endregion

        // Tüm Kontrol Değişkenlerinin Tanımları
        private System.Windows.Forms.GroupBox gboxMotorControl;
        private System.Windows.Forms.Button btnServoMove;
        private System.Windows.Forms.GroupBox gboxSensorData;
        private System.Windows.Forms.Label lblServoSpeed;
        private System.Windows.Forms.TextBox txtServoSpeed;
        private System.Windows.Forms.Label lblMotorStatus;
        private System.Windows.Forms.Label lblLoadCellValue;
        private System.Windows.Forms.Label lblLoadCellTitle;
        private System.Windows.Forms.FlowLayoutPanel pnlLimitSwitches;
        private System.Windows.Forms.Label lblLSXMin;
        private System.Windows.Forms.Label lblLSXMax;
        private System.Windows.Forms.GroupBox gboxOperations;
        private System.Windows.Forms.Button btnCalibration;
        private System.Windows.Forms.Button btnHomingAll;
        private System.Windows.Forms.Label lblServoDistance;
        private System.Windows.Forms.TextBox txtServoDistance;
    }
}