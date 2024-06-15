using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _20203160
{
    public partial class Form2 : Form
    {
        private TcpClient tclient; //상대방 정보를 담아둘 TcpClient
        private Thread th1; //백그라운드로 서버를 돌릴 Thread
        public Form2()
        {
            InitializeComponent();
        }
        public Image image { get; set; }  // Child.image = I; 때문에 선언해야함.

        private void Form2_Load(object sender, EventArgs e)// load문을 선언해야함 + form3도
        {
            this.ClientSize = image.Size;
            th1 = new Thread(new ThreadStart(socketSendReceive));
            th1.IsBackground = true;
            th1.Start();
        }
        private void socketSendReceive() // 스레드 용 함수
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 8000);
            listener.Start();
            TcpClient serverClient = listener.AcceptTcpClient();
            if (serverClient.Connected)
            {
                NetworkStream ns = serverClient.GetStream();
                while (true)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    Image img = (Image)bf.Deserialize(ns);
                    this.image = img;
                    Invalidate();
                }
            }
        }
        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(image, 0, 0, image.Width, image.Height);
        }
        public void SendImage(Image img)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            tclient = new TcpClient("127.0.0.1", 9000);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NetworkStream ns = tclient.GetStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ns, image);
        }
    }
}
