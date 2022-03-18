using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using dnp;

namespace MaiJian
{
    public partial class Form1 : Form
    {
        //初始化5个位置的模板
        List<Position> woJun = new List<Position>();
        List<Position> dong = new List<Position>();
        List<Position> nan = new List<Position>();
        List<Position> xi = new List<Position>();
        List<Position> bei = new List<Position>();
        List<Position> woJunPeng = new List<Position>();
        private bool woJunEnd = false;
        private bool dongEnd = false;
        private bool nanEnd = false;
        private bool xiEnd = false;
        private bool beiEnd = false;
        private bool woJunPengEnd = false;
        private bool IsStart = false;
        private List<string> gang = new List<string>();
        private Queue<Pai> queue = new Queue<Pai>();

        public Form1()
        {
            InitializeComponent();
            CenterToScreen();
            CheckForIllegalCrossThreadCalls = false;
            //麻将图片模板
            pb1tong.ImageLocation = "show/1tong.png";
            pb2tong.ImageLocation = "show/2tong.png";
            pb3tong.ImageLocation = "show/3tong.png";
            pb4tong.ImageLocation = "show/4tong.png";
            pb5tong.ImageLocation = "show/5tong.png";
            pb6tong.ImageLocation = "show/6tong.png";
            pb7tong.ImageLocation = "show/7tong.png";
            pb8tong.ImageLocation = "show/8tong.png";
            pb9tong.ImageLocation = "show/9tong.png";
            pb1tiao.ImageLocation = "show/1tiao.png";
            pb2tiao.ImageLocation = "show/2tiao.png";
            pb3tiao.ImageLocation = "show/3tiao.png";
            pb4tiao.ImageLocation = "show/4tiao.png";
            pb5tiao.ImageLocation = "show/5tiao.png";
            pb6tiao.ImageLocation = "show/6tiao.png";
            pb7tiao.ImageLocation = "show/7tiao.png";
            pb8tiao.ImageLocation = "show/8tiao.png";
            pb9tiao.ImageLocation = "show/9tiao.png";
            pbDong.ImageLocation = "show/dong.png";
            pbNan.ImageLocation = "show/nan.png";
            pbXi.ImageLocation = "show/xi.png";
            pbBei.ImageLocation = "show/bei.png";
            pbZhong.ImageLocation = "show/zhong.png";
            pbFa.ImageLocation = "show/fa.png";
            pbBa.ImageLocation = "show/ba.png";
            //pb1wan.ImageLocation = "show/1wan.png";
            //pb9wan.ImageLocation = "show/9wan.png";

        }

        private List<Position> LoadAllPai(string dirPath)
        {
            var filePath = $"image/{dirPath}/";
            var files = Directory.GetFiles(filePath, "*.png");
            var list = new List<Position>();
            foreach (var file in files)
            {
                var fileName = file.Substring(file.LastIndexOf('/') + 1, file.Length - file.LastIndexOf('/') - 1);
                if (file.IndexOf('-') <= -1)
                {
                    list.Add(new Position { FullPath = file, FileName = fileName.Replace(".png", "").Replace("-hsv", "").Replace("-ycc", ""), Type = "bgr", BgrImage = new Image<Bgr, byte>(file) });
                }
                else
                {
                    var arr = file.Split('-');
                    var position = new Position { FullPath = arr[0], FileName = fileName.Replace(".png", "").Replace("-hsv", "").Replace("-ycc", ""), Type = arr[1] };
                    if (arr[1] == "hsv")
                    {
                        position.HsvImage = new Image<Hsv, byte>(file);
                    }
                    else
                    {
                        position.YccImage = new Image<Ycc, byte>(file);
                    }
                    list.Add(position);
                }
            }
            return list;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //将模板全部加载出来
            woJun = LoadAllPai("wojun");
            dong = LoadAllPai("dong");
            nan = LoadAllPai("nan");
            xi = LoadAllPai("xi");
            bei = LoadAllPai("bei");
            woJunPeng = LoadAllPai("wojunpeng");

            //为picbox绑定事件
            foreach (var item in woJun)
            {
                var pb = (PictureBox)this.Controls.Find("pb" + item.FileName, false).First();
                pb.DoubleClick += Pb_DoubleClick;
            }
        }

        private void Pb_DoubleClick(object sender, EventArgs e)
        {
            var pb = sender as PictureBox;
            var name = pb.Name.Replace("pb", "");
            woJun.FirstOrDefault(r => r.FileName == name).Number = 0;
            var lb = (Label)this.Controls.Find("lb" + name, false).First();
            lb.ForeColor = Color.Red;
            lb.Text = "0";
        }

        public int GetMatchPos(string src, string template)
        {
            int index = 0;
            MatchInfo myMatchInfo = new MatchInfo();
            Image<Bgr, byte> a = new Image<Bgr, byte>(src);
            Image<Bgr, byte> b = new Image<Bgr, byte>(template);
            Image<Bgr, byte> aa = new Image<Bgr, byte>(src);
            var c = a.MatchTemplate(b, TemplateMatchingType.CcorrNormed);
            //CvInvoke.Normalize(c, c, 255, 0, Emgu.CV.CvEnum.NormType.DiffC);
            double min = 0, max = 0;
            Point maxp = new Point(0, 0);
            Point minp = new Point(0, 0);
            CvInvoke.MinMaxLoc(c, ref min, ref max, ref minp, ref maxp);
            //for (int i =0;i< c.; i++) {

            //}
            if (max >= 0.99)
            {
                index++;
            }
            else
            {
                return index;
            }
            Console.WriteLine(max + "-" + maxp.X + "," + maxp.Y);
            Console.WriteLine(min + "-" + minp.X + "," + minp.Y);
            CvInvoke.Rectangle(aa, new Rectangle(maxp, new Size(b.Width, b.Height)), new MCvScalar(0, 0, 255), 3);
            //CvInvoke.Rectangle(aa, new Rectangle(minp, new Size(b.Width + 13, b.Height + 13)), new MCvScalar(0, 255, 255), 3);
            Image<Gray, float> c1 = c;
            int getcnt = 0;
            Lab1:
            c1 = GetNextMinLoc(c1, maxp, (int)max, b.Width, b.Height);
            //CvInvoke.Normalize(c1, c1, 255, 0, Emgu.CV.CvEnum.NormType.DiffC);
            CvInvoke.MinMaxLoc(c1, ref min, ref max, ref minp, ref maxp);
            if (max < 0.99)
            {
                goto Lab2;
            }
            else
            {
                index++;
            }
            getcnt++;
            if (getcnt > 10)
            {
                goto Lab2;
            }
            Console.WriteLine(max + "-" + maxp.X + "," + maxp.Y);
            CvInvoke.Rectangle(aa, new Rectangle(maxp, new Size(b.Width, b.Height)), new MCvScalar(0, 255, 0), 3);
            goto Lab1;
            Lab2:
            Console.WriteLine(index);
            //myMatchInfo.matchMax = max;
            //myMatchInfo.matchMin = min;
            //myMatchInfo.matchPicture = aa;
            //myMatchInfo.matchRectangle = new Rectangle(maxp, new Size(b.Width, b.Height));
            //myMatchInfo.matchMaxLoc = maxp;
            //myMatchInfo.matchMinLoc = minp;
            pb2tong.Image = aa.Bitmap;
            return index;
        }

        #region 模板匹配

        /// <summary>
        /// 模板匹配
        /// </summary>
        /// <param name="a">源图</param>
        /// <param name="b">模板图</param>
        /// <returns></returns>
        public int GetMatchPosBgr(Image<Bgr, byte> a, Image<Bgr, byte> b)
        {
            if (!IsStart)
            {
                return 0;
            }
            int index = 0;
            var c = a.MatchTemplate(b, TemplateMatchingType.CcorrNormed);
            double min = 0, max = 0;
            Point maxp = new Point(0, 0);
            Point minp = new Point(0, 0);
            CvInvoke.MinMaxLoc(c, ref min, ref max, ref minp, ref maxp);
            if (max >= 0.99)
            {
                index++;
            }
            else
            {
                return index;
            }
            Image<Gray, float> c1 = c;
            int getcnt = 0;
            Lab1:
            c1 = GetNextMinLoc(c1, maxp, (int)max, b.Width, b.Height);
            CvInvoke.MinMaxLoc(c1, ref min, ref max, ref minp, ref maxp);
            if (max < 0.99)
            {
                goto Lab2;
            }
            else
            {
                index++;
            }
            getcnt++;
            if (getcnt >= 3)
            {
                goto Lab2;
            }
            goto Lab1;
            Lab2:
            return index;
        }

        /// <summary>
        /// 模板匹配
        /// </summary>
        /// <param name="a">源图</param>
        /// <param name="b">模板图</param>
        /// <returns></returns>
        public int GetMatchPosHsv(Image<Hsv, byte> a, Image<Hsv, byte> b)
        {
            if (!IsStart)
            {
                return 0;
            }
            int index = 0;
            var c = a.MatchTemplate(b, TemplateMatchingType.CcorrNormed);
            double min = 0, max = 0;
            Point maxp = new Point(0, 0);
            Point minp = new Point(0, 0);
            CvInvoke.MinMaxLoc(c, ref min, ref max, ref minp, ref maxp);
            if (max >= 0.99)
            {
                index++;
            }
            else
            {
                return index;
            }
            Image<Gray, float> c1 = c;
            int getcnt = 0;
            Lab1:
            c1 = GetNextMinLoc(c1, maxp, (int)max, b.Width, b.Height);
            CvInvoke.MinMaxLoc(c1, ref min, ref max, ref minp, ref maxp);
            if (max < 0.99)
            {
                goto Lab2;
            }
            else
            {
                index++;
            }
            getcnt++;
            if (getcnt >= 3)
            {
                goto Lab2;
            }
            goto Lab1;
            Lab2:
            return index;
        }

        /// <summary>
        /// 模板匹配
        /// </summary>
        /// <param name="a">源图</param>
        /// <param name="b">模板图</param>
        /// <returns></returns>
        public int GetMatchPosYcc(Image<Ycc, byte> a, Image<Ycc, byte> b)
        {
            if (!IsStart)
            {
                return 0;
            }
            int index = 0;
            var c = a.MatchTemplate(b, TemplateMatchingType.CcorrNormed);
            double min = 0, max = 0;
            Point maxp = new Point(0, 0);
            Point minp = new Point(0, 0);
            CvInvoke.MinMaxLoc(c, ref min, ref max, ref minp, ref maxp);
            if (max >= 0.99)
            {
                index++;
            }
            else
            {
                return index;
            }
            Image<Gray, float> c1 = c;
            int getcnt = 0;
            Lab1:
            c1 = GetNextMinLoc(c1, maxp, (int)max, b.Width, b.Height);
            CvInvoke.MinMaxLoc(c1, ref min, ref max, ref minp, ref maxp);
            if (max < 0.99)
            {
                goto Lab2;
            }
            else
            {
                index++;
            }
            getcnt++;
            if (getcnt >= 3)
            {
                goto Lab2;
            }
            goto Lab1;
            Lab2:
            return index;
        }
        #endregion

        Image<Gray, float> GetNextMinLoc(Image<Gray, float> result, Point maxLoc, int maxVaule, int templatW, int templatH)
        {

            // 先将第一个最大值点附近模板宽度和高度的都设置为最大值防止产生干扰
            int startX = maxLoc.X - (templatW >> 2);//
            int startY = maxLoc.Y - (templatH >> 2);//
            int endX = maxLoc.X + (templatW >> 2);//
            int endY = maxLoc.Y + (templatH >> 2);//
            if (startX < 0 || startY < 0)
            {
                startX = 0;
                startY = 0;
            }
            if (endX > result.Width - 1 || endY > result.Height - 1)
            {
                endX = result.Width - 1;
                endY = result.Height - 1;
            }
            int y, x;
            for (y = startY; y < endY; y++)
            {
                for (x = startX; x < endX; x++)
                {
                    CvInvoke.cvSetReal2D(result, y, x, maxVaule);
                }
            }

            return result;

        }

        //点击开始按钮
        private void btnStart_Click(object sender, EventArgs e)
        {
            IsStart = true;

            ThreadPool.QueueUserWorkItem(a =>
            {
                while (IsStart)
                {
                    if (queue.Any())
                    {
                        var item = queue.Dequeue();
                        var list = new List<Pai>();
                        if (list.Exists(r => r.Name == item.Name))
                        {
                            list.FirstOrDefault(r => r.Name == item.Name).Number++;
                        }
                        else
                        {
                            list.Add(new Pai { Name = item.Name, Number = item.Number });
                        }

                        foreach (var pai in list)
                        {
                            Label lb = (Label)this.Controls.Find("lb" + pai.Name, false).First();
                            var result = 4 - pai.Number;
                            lb.ForeColor = result <= 0 ? Color.Red : Color.Black;
                            lb.Text = result.ToString();
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
            });

            ThreadPool.QueueUserWorkItem(a =>
            {
                while (IsStart)
                {
                    var number = dong.Where(r => !gang.Contains(r.FileName)).Sum(r => r.Number);
                    if (number < dongNumber)
                    {
                        gang = new List<string>();
                        dongNumber = 0;
                        foreach (var item in woJun)
                        {
                            item.Number = 0;
                        }
                        foreach (var item in dong)
                        {
                            item.Number = 0;
                        }
                        foreach (var item in nan)
                        {
                            item.Number = 0;
                        }
                        foreach (var item in xi)
                        {
                            item.Number = 0;
                        }
                        foreach (var item in bei)
                        {
                            item.Number = 0;
                        }
                        foreach (var item in woJunPeng)
                        {
                            item.Number = 0;
                        }
                    }
                    else
                    {
                        dongNumber = number;
                    }
                    Thread.Sleep(500);
                }
            });

            dnp.dnp d = new dnp.dnp(@"D:\ChangZhi\dnplayer2\");
            ThreadPool.QueueUserWorkItem(a =>
            {
                while (IsStart)
                {
                    //发送指令截张图片
                    var datetime = DateTime.Now;
                    var fileName = $"{datetime:yyyyMMddhhmmss}.png";
                    var filePath = $"/sdcard/Pictures/{fileName}";
                    var cmd = d.Cmd($"adb shell screencap -p {filePath}");
                    Console.WriteLine(cmd);
                    //生成三张原型图片 
                    var locationFile = $"screencap/{fileName}";
                    Image<Bgr, byte> yuanBgr = new Image<Bgr, byte>(locationFile);
                    Image<Hsv, byte> yuanHsv = new Image<Hsv, byte>(locationFile);
                    Image<Ycc, byte> yuanYcc = new Image<Ycc, byte>(locationFile);

                    ThreadPool.QueueUserWorkItem(b =>
                    {
                        foreach (var item in woJun)
                        {
                            if (gang.Exists(r => r == item.FileName))
                            {
                                continue;
                            }
                            //ThreadPool.QueueUserWorkItem(c =>
                            //{
                            if (item.Type == "bgr")
                            {
                                var number = GetMatchPosBgr(yuanBgr, item.BgrImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "hsv")
                            {
                                var number = GetMatchPosHsv(yuanHsv, item.HsvImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "ycc")
                            {
                                var number = GetMatchPosYcc(yuanYcc, item.YccImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            //});
                        }

                        woJunEnd = true;
                    });

                    ThreadPool.QueueUserWorkItem(b =>
                    {
                        foreach (var item in dong)
                        {
                            if (gang.Exists(r => r == item.FileName))
                            {
                                continue;
                            }

                            //ThreadPool.QueueUserWorkItem(c =>
                            //{
                            if (item.Type == "bgr")
                            {
                                var number = GetMatchPosBgr(yuanBgr, item.BgrImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "hsv")
                            {
                                var number = GetMatchPosHsv(yuanHsv, item.HsvImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "ycc")
                            {
                                var number = GetMatchPosYcc(yuanYcc, item.YccImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            //});
                        }

                        dongEnd = true;
                    });

                    ThreadPool.QueueUserWorkItem(b =>
                    {
                        foreach (var item in nan)
                        {
                            if (gang.Exists(r => r == item.FileName))
                            {
                                continue;
                            }

                            //ThreadPool.QueueUserWorkItem(c =>
                            //{
                            if (item.Type == "bgr")
                            {
                                var number = GetMatchPosBgr(yuanBgr, item.BgrImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "hsv")
                            {
                                var number = GetMatchPosHsv(yuanHsv, item.HsvImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "ycc")
                            {
                                var number = GetMatchPosYcc(yuanYcc, item.YccImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            //});
                        }

                        nanEnd = true;
                    });

                    ThreadPool.QueueUserWorkItem(b =>
                    {
                        foreach (var item in xi)
                        {
                            if (gang.Exists(r => r == item.FileName))
                            {
                                continue;
                            }

                            //ThreadPool.QueueUserWorkItem(c =>
                            //{
                            if (item.Type == "bgr")
                            {
                                var number = GetMatchPosBgr(yuanBgr, item.BgrImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "hsv")
                            {
                                var number = GetMatchPosHsv(yuanHsv, item.HsvImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "ycc")
                            {
                                var number = GetMatchPosYcc(yuanYcc, item.YccImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            //});
                        }

                        xiEnd = true;
                    });

                    ThreadPool.QueueUserWorkItem(b =>
                    {
                        foreach (var item in bei)
                        {
                            if (gang.Exists(r => r == item.FileName))
                            {
                                continue;
                            }

                            //ThreadPool.QueueUserWorkItem(c =>
                            //{
                            if (item.Type == "bgr")
                            {
                                var number = GetMatchPosBgr(yuanBgr, item.BgrImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "hsv")
                            {
                                var number = GetMatchPosHsv(yuanHsv, item.HsvImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "ycc")
                            {
                                var number = GetMatchPosYcc(yuanYcc, item.YccImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            //});
                        }

                        beiEnd = true;
                    });

                    ThreadPool.QueueUserWorkItem(b =>
                    {
                        foreach (var item in woJunPeng)
                        {
                            if (gang.Exists(r => r == item.FileName))
                            {
                                continue;
                            }

                            //ThreadPool.QueueUserWorkItem(c =>
                            //{
                            if (item.Type == "bgr")
                            {
                                var number = GetMatchPosBgr(yuanBgr, item.BgrImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "hsv")
                            {
                                var number = GetMatchPosHsv(yuanHsv, item.HsvImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            else if (item.Type == "ycc")
                            {
                                var number = GetMatchPosYcc(yuanYcc, item.YccImage);
                                if (number > 0 && item.Number != number)
                                {
                                    item.Number = number;
                                    var pai = new Pai
                                    {
                                        Name = item.FileName,
                                        Number = number
                                    };
                                    queue.Enqueue(pai);
                                }
                            }
                            //});
                        }

                        woJunPengEnd = true;
                    });
                    //while (true)
                    //{
                    //    if (woJunEnd && dongEnd && nanEnd && xiEnd && beiEnd && woJunPengEnd)
                    //    {
                    //        var number = dong.Where(r => !gang.Contains(r.FileName)).Sum(r => r.Number);
                    //        if (number < dongNumber)
                    //        {
                    //            gang = new List<string>();
                    //        }

                    //        foreach (var item in woJun)
                    //        {
                    //            var dongNum = dong.FirstOrDefault(r => r.FileName == item.FileName)?.Number ?? 0;
                    //            var nanNum = nan.FirstOrDefault(r => r.FileName == item.FileName)?.Number ?? 0;
                    //            var xiNum = xi.FirstOrDefault(r => r.FileName == item.FileName)?.Number ?? 0;
                    //            var beiNum = bei.FirstOrDefault(r => r.FileName == item.FileName)?.Number ?? 0;
                    //            var woJunPengNum = woJunPeng.FirstOrDefault(r => r.FileName == item.FileName)?.Number ?? 0;
                    //            Label lb = (Label)this.Controls.Find("lb" + item.FileName, false).First();
                    //            var result = (4 - (item.Number + dongNum + nanNum + xiNum + beiNum + woJunPengNum));
                    //            lb.ForeColor = result <= 0 ? Color.Red : Color.Black;
                    //            lb.Text = result.ToString();
                    //        }
                    //        woJunEnd = false;
                    //        dongEnd = false;
                    //        nanEnd = false;
                    //        xiEnd = false;
                    //        beiEnd = false;
                    //        woJunPengEnd = false;
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        Thread.Sleep(200);
                    //    }
                    //}
                    Thread.Sleep(2000);
                    //yuanBgr.Dispose();
                    //yuanHsv.Dispose();
                    //yuanYcc.Dispose();
                }
            });
        }

        private int dongNumber = 0;

        private void btnEnd_Click(object sender, EventArgs e)
        {
            foreach (var item in woJun)
            {
                var lb = (Label)this.Controls.Find("lb" + item.FileName, false).First();
                lb.ForeColor = Color.Black;
                lb.Text = "4";
            }
            IsStart = false;
            queue = new Queue<Pai>();
            gang = new List<string>();
            woJunEnd = false;
            dongEnd = false;
            nanEnd = false;
            xiEnd = false;
            beiEnd = false;
            woJunPengEnd = false;
        }
    }
}
