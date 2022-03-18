using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace MaiJian
{
    public class Position
    {
        public string FullPath { get; set; }

        public string FileName { get; set; }

        public string Type { get; set; }

        public Image<Bgr, byte> BgrImage { get; set; }

        public Image<Hsv, byte> HsvImage { get; set; }

        public Image<Ycc, byte> YccImage { get; set; }

        public int Number { get; set; }
    }
}
