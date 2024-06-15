using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _20203160
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        public Image origin { get; set; }
        Image changed { get; set; }
        public Form1 fm1;
        private void Form3_Load(object sender, EventArgs e)
        {
            changed = origin;
            hScrollBar2.Maximum = 100;
            hScrollBar2.Minimum = 0;
            hScrollBar3.Maximum = 100;
            hScrollBar3.Minimum = 0;
            hScrollBar4.Maximum = 100;
            hScrollBar4.Minimum = 0;
            hScrollBar2.Value = 100;
            hScrollBar3.Value = 100;
            hScrollBar4.Value = 100;
            textBox1.Text = hScrollBar2.Value.ToString();
            textBox2.Text = hScrollBar3.Value.ToString();
            textBox3.Text = hScrollBar4.Value.ToString();

        }

        private void Form3_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(origin, 30, 30, 200, 200);
            e.Graphics.DrawImage(changed, 270, 30, 200, 200);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 Child = new Form2();
            Child.image = changed;
            Child.MdiParent = fm1;
            Child.SendImage(changed);
            Child.Show();
            this.Close();
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            int value = 182;
            int Rvalue = hScrollBar2.Value;
            int Gvalue = hScrollBar3.Value;
            int Bvalue = hScrollBar4.Value;
            if ((HScrollBar)sender == hScrollBar2)
            {
                textBox1.Text = hScrollBar2.Value.ToString();
            }
            else if((HScrollBar)sender == hScrollBar3)
            {
                textBox2.Text = hScrollBar3.Value.ToString();
            }
            else
            {
                textBox3.Text = hScrollBar4.Value.ToString();
            }
            changed = changerbit();
            this.Invalidate();
        }

        private void hScrollBar3_Scroll(object sender, ScrollEventArgs e)
        {
            textBox2.Text = hScrollBar3.Value.ToString();
        }

        private void hScrollBar4_Scroll(object sender, ScrollEventArgs e)
        {
            textBox3.Text = hScrollBar4.Value.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox text = sender as TextBox;
            int value;
            if (int.TryParse(text.Text, out value)) // 백스페이스 예외처리
            {
                if ((TextBox)sender == textBox1)
                {
                    if (int.Parse(textBox1.Text) > 100)
                    {
                        hScrollBar2.Value = 100;
                        textBox1.Text = "100";
                    }
                    else if (int.Parse(textBox1.Text) < 0)
                    {
                        hScrollBar2.Value = 0;
                        textBox1.Text = "0";
                    }
                    else
                    {
                        hScrollBar2.Value = int.Parse(textBox1.Text);
                    }
                }
                else if ((TextBox)sender == textBox2)
                {
                    if (int.Parse(textBox2.Text) > 100)
                    {
                        hScrollBar3.Value = 100;
                        textBox2.Text = "100";
                    }
                    else if (int.Parse(textBox1.Text) < 0)
                    {
                        hScrollBar3.Value = 0;
                        textBox2.Text = "0";
                    }
                    else
                    {
                        hScrollBar3.Value = int.Parse(textBox2.Text);
                    }
                }
                else
                {
                    if (int.Parse(textBox3.Text) > 100)
                    {
                        hScrollBar4.Value = 100;
                        textBox3.Text = "100";
                    }
                    else if (int.Parse(textBox3.Text) < 0)
                    {
                        hScrollBar4.Value = 0;
                        textBox3.Text = "0";
                    }
                    else
                    {
                        hScrollBar4.Value = int.Parse(textBox3.Text);
                    }
                }
                changed = changerbit();
                this.Invalidate();
            }
        }
        private Bitmap changerbit()
        {
            int tvalue = 182;
            int Rvalue = hScrollBar2.Value;
            int Gvalue = hScrollBar3.Value;
            int Bvalue = hScrollBar4.Value;
            Bitmap B = new Bitmap(origin);
            for (int y = 0; y < B.Height; y++)
                for (int x = 0; x < B.Width; x++)
                {
                    Color color = B.GetPixel(x, y);
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;
                    // Saturation

                    tvalue = Math.Max(0, Math.Min(256, tvalue));



                    r = Math.Max(0, Math.Min(255, r + tvalue - 128));
                    g = Math.Max(0, Math.Min(255, g + tvalue - 128));
                    b = Math.Max(0, Math.Min(255, b + tvalue - 128));

                    r = Math.Max(0, Math.Min(255, r + (Rvalue - 128) * 2));
                    g = Math.Max(0, Math.Min(255, g + (Gvalue - 128) * 2));
                    b = Math.Max(0, Math.Min(255, b + (Bvalue - 128) * 2));

                    B.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            return B;
        }
    }
}