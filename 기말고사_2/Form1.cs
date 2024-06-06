using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace DrawLines
{
    public partial class Form1 : Form
    {
        private LinkedList<CMyData> total_lines;
        private CMyData data;
        private CMyData1 data1;
        private int x, y;
        private Color CurrentPenColor;
        private int iCurrentPenWidth;
        ArrayList ar;
        private int option = 0;

        //통신을 위해 추가해야 할 객체들
        private TcpClient tclient; //상대방 정보를 담아둘 TcpClient
        private Thread th1; //백그라운드로 서버를 돌릴 Thread
        private CPoint cpoint; //전송에 필요한 CPoint
        public Form1()
        {
            total_lines = new LinkedList<CMyData>();
            CurrentPenColor = Color.Black;
            iCurrentPenWidth = 2;
            ar = new ArrayList();
            InitializeComponent();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && option == 0)
            {
                x = e.X;
                y = e.Y;
                data = new CMyData();
                data.Color = CurrentPenColor;
                data.Width = iCurrentPenWidth;
                data.AR.Add(new Point(x, y));
            }
        }

        /*private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Capture && e.Button == MouseButtons.Left)
            {
                Graphics G = CreateGraphics();
                //펜 설정하기(색깔과 굵기) 
                Pen p = new Pen(data.Color, data.Width);
                G.DrawLine(p, x, y, e.X, e.Y);
                x = e.X;
                y = e.Y;
                data.AR.Add(new Point(x, y));
                G.Dispose();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            total_lines.AddLast(data);
        }*/

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (CMyData line in total_lines)
            {
                Pen p = new Pen(line.Color, line.Width);
                for (int i = 1; i < line.AR.Count; i++)
                {
                    e.Graphics.DrawLine(p, (Point)line.AR[i - 1], (Point)line.AR[i]);
                }
            }
            foreach (CMyData1 c in ar) //ar안에 CMyData 추가.
            {
                e.Graphics.FillEllipse(Brushes.Red, c.Point.X, c.Point.Y, c.Size, c.Size);
                e.Graphics.DrawEllipse(Pens.Black, c.Point.X, c.Point.Y, c.Size, c.Size);
            }
        }

        private void 대화상자ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 dlg = new Form2();
            dlg.DialogPenColor = CurrentPenColor;  //set
            dlg.iDialogPenWidth = iCurrentPenWidth;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                CurrentPenColor = dlg.DialogPenColor;      //get
                iCurrentPenWidth = dlg.iDialogPenWidth;
            }
            dlg.Dispose();
        }

        // 통신을 위해 수정/추가된 메소드
        private void Form1_Load(object sender, EventArgs e) //백그라운드로 서버를 돌리기 위해 스레드 생성 및 실행
        {
            th1 = new Thread(new ThreadStart(socketSendReceive));
            th1.IsBackground = true;
            th1.Start();
        }

        private void socketSendReceive() // 스레드 용 함수
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 9000);
            listener.Start();
            TcpClient serverClient = listener.AcceptTcpClient();
            if (serverClient.Connected)
            {
                NetworkStream ns = serverClient.GetStream();
                while (true)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    object obj = bf.Deserialize(ns);

                    if (obj is CPoint cp)
                    {
                        if (cp.Option == 0)
                        {
                            if (cp.isMouseUp)
                            {
                                total_lines.AddLast(cp.ToCMyData());
                            }
                            else
                            {
                                CMyData c = total_lines.Last?.Value ?? new CMyData();
                                if (c.AR.Count == 0)
                                {
                                    c.AR.Add(cp.Start);
                                    c.Color = cp.Color;
                                    c.Width = cp.Width;
                                }
                                c.AR.Add(cp.End);

                                Graphics G = CreateGraphics();
                                Pen p = new Pen(c.Color, c.Width);
                                G.DrawLine(p, cp.Start, cp.End);
                                G.Dispose();
                            }
                        }
                    }
                    else if (obj is CMyData1 c1)
                    {
                        if (c1.Option == 1)
                        {
                            ar.Add(c1);
                            Graphics G = CreateGraphics();
                            G.FillEllipse(Brushes.Red, c1.Point.X, c1.Point.Y, c1.Size, c1.Size);
                            G.DrawEllipse(Pens.Black, c1.Point.X, c1.Point.Y, c1.Size, c1.Size);
                            G.Dispose();
                        }
                    }
                }
            }
        }


        private void button1_Click(object sender, EventArgs e) //버튼 클릭 시 지정된 서버:포트 로 연결해줌.
        {
            tclient = new TcpClient("127.0.0.1", 8000);
            if (tclient.Connected)
            {
                label1.Text = "연결 성공!";
            }
        }


        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Capture && e.Button == MouseButtons.Left)
            {
                if (tclient != null && tclient.Connected) // 연결이 되어 있어야만 그릴 수 있다.
                {
                    NetworkStream ns = tclient.GetStream();
                    BinaryFormatter bf = new BinaryFormatter();

                    if (data == null)
                    {
                        data = new CMyData();
                        data.Color = CurrentPenColor;
                        data.Width = iCurrentPenWidth;
                    }

                    cpoint = new CPoint(); //cpoint 생성
                    cpoint.Color = CurrentPenColor; //펜 색상
                    cpoint.Width = iCurrentPenWidth; //펜 굵기
                    cpoint.Start = new Point(x, y); //시작 점
                    cpoint.Option = option; // 현재 option 값 설정

                    Graphics G = CreateGraphics();
                    Pen p = new Pen(data.Color, data.Width);

                    if (option == 0)
                    {
                        G.DrawLine(p, x, y, e.X, e.Y);
                        x = e.X;
                        y = e.Y;

                        cpoint.End = new Point(x, y); // 끝 점

                        data.AR.Add(new Point(x, y));
                        bf.Serialize(ns, cpoint);
                    }
                    else if (option == 1)
                    {
                        Random random = new Random();
                        CMyData1 c = new CMyData1();
                        c.Size = random.Next(30, 100);
                        c.Point = new Point(e.X, e.Y);
                        c.Option = option; // 현재 option 값 설정

                        if (data1 == null)
                        {
                            data1 = new CMyData1();
                        }

                        data1.AR.Add(c); // 배열에 저장
                        G.FillEllipse(Brushes.Red, c.Point.X, c.Point.Y, c.Size, c.Size);
                        G.DrawEllipse(Pens.Black, c.Point.X, c.Point.Y, c.Size, c.Size);

                        bf.Serialize(ns, c);
                    }

                    G.Dispose();
                }
            }
        }


        private void 모양ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 원ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            option = 1;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void 사각형ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            option = 2;
        }

        private void 삼각형ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            option = 3;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (tclient.Connected && option == 0) // 이 프로그램은 연결이 되있어야만 그릴 수 있음
            {
                NetworkStream ns = tclient.GetStream();
                BinaryFormatter bf = new BinaryFormatter();
                total_lines.AddLast(data);
                cpoint.isMouseUp = true; //MouseUp했다
                bf.Serialize(ns, cpoint); //cpoint를 바이너리 포맷으로 시리얼라이징해서 ns로 전송
            }
        }
    }

    [Serializable]
    class CPoint //데이터를 전송하기 위한 Class
    {
        public bool isMouseUp { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public Color Color { get; set; }
        public int Width { get; set; }
        public int Option { get; set; } // 추가된 속성
        public CPoint()
        {
            isMouseUp = false;
        }

        public CMyData ToCMyData()
        {
            CMyData data = new CMyData();
            data.Color = this.Color;
            data.Width = this.Width;
            data.AR.Add(this.Start);
            data.AR.Add(this.End);
            return data;
        }
    }

    [Serializable]
    class CMyData1
    {
        private Point point;
        private int size;
        private ArrayList Ar;
        public int Option { get; set; } // 추가된 속성

        public CMyData1()  //생성자함수
        {
            Ar = new ArrayList();
        }
        public Point Point
        {
            get { return point; }
            set { point = value; }
        }
        public int Size
        {
            get { return size; }
            set { size = value; }
        }
        public ArrayList AR
        {
            get { return Ar; }
        }
    }

    [Serializable]
    class CMyData
    {
        private ArrayList Ar;

        public CMyData()  //생성자함수
        {
            Ar = new ArrayList();
        }
        public Color Color
        {
            get;
            set;
        }
        public int Width
        {
            get;
            set;
        }
        public ArrayList AR
        {
            get { return Ar; }
        }
    }

}
