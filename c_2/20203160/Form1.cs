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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == DialogResult.OK)
            {
                Image I = Image.FromFile(op.FileName);
                Form2 Child = new Form2();
                Child.image = I;
                Child.MdiParent = this;
                Child.Show();
            }
        }

        private void 대화상자ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 Child = (Form2)this.ActiveMdiChild;
            if (Child != null)
            {
                Image I = Child.image;
                Form3 dig = new Form3();
                dig.origin = Child.image;
                dig.fm1 = this;
                dig.Show();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
