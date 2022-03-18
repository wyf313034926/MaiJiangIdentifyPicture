using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace MaiJian
{
    public class MatchInfo
    {
        public double matchMax { get; set; }

        public double matchMin { get; set; }

        public Image<Ycc, byte> matchPicture { get; set; }

        public Rectangle matchRectangle { get; set; }

        public Point matchMaxLoc { get; set; }

        public Point matchMinLoc { get; set; }
    }
}
