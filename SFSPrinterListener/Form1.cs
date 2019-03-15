using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SFSPrinterListener
{
    public partial class Form1 : Form
    {

        public void DisplayImage(Image image)
        {
            //string tempImgName = $"temp{DateTime.Now.ToBinary()}.png";
            //image.Save(tempImgName);
            //pictureBox1.Image = Image.FromFile(tempImgName);
            pictureBox1.Image = image;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            pictureBox1.Refresh();
            //pictureBox1.Show();
        }

        public Form1()
        {
            InitializeComponent();
            this.SizeChanged += new EventHandler((s, e) =>
            {
                



            });
        }
    }
}
