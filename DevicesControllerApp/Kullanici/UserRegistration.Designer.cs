namespace DevicesControllerApp.Kullanici
{
    partial class UserRegistration
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
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.button1.Location = new System.Drawing.Point(525, 285);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(338, 142);
            this.button1.TabIndex = 2;
            this.button1.Text = "Kullanıcı Kayıt";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // UserRegistration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Name = "UserRegistration";
            this.Size = new System.Drawing.Size(1389, 712);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
    }
}
