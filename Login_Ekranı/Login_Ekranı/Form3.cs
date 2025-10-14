using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Login_Ekranı
{
    public partial class Form3 : Form
    {
        private Size originalFormSize;
        private Rectangle originalPictureBoxBounds;
        public Form3()
        {
            InitializeComponent();

            originalFormSize = this.Size;
            originalPictureBoxBounds = pictureBox1.Bounds;

            // Resize eventini ekle
            this.Resize += Form3_Resize;

            // GIF'in düzgün ölçeklenmesi için
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void Form3_Resize(object sender, EventArgs e)
        {
            float xRatio = (float)this.Width / originalFormSize.Width;
            float yRatio = (float)this.Height / originalFormSize.Height;

            pictureBox1.SetBounds(
                (int)(originalPictureBoxBounds.X * xRatio),
                (int)(originalPictureBoxBounds.Y * yRatio),
                (int)(originalPictureBoxBounds.Width * xRatio),
                (int)(originalPictureBoxBounds.Height * yRatio)
            );
        }

        private async void Form3_Load(object sender, EventArgs e)
        {



            pictureBox1.Image = Image.FromFile("Trail loading.gif");
            pictureBox1.Visible = true;

            await Task.Delay(10000); // 3 saniye bekle

            // GIF ve yazıyı gizle
            pictureBox1.Visible = false;

 

        }

        private void lblDurum_Click(object sender, EventArgs e)
        {

        }
    }
}
