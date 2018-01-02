using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Maori.Implementations;
using Maori.Interfaces;

namespace Maori.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            IBitmapFileGetter bitmapFileGetter = new BitmapFileGetter();
            IColorSpaceConverter colorSpaceConverter = new ColorSpaceConverter();

            Image img = bitmapFileGetter.GetBitmap(@"C:\Users\matishadow\Desktop\b.bmp");
            var bmp = new MaoriBitmap(img, colorSpaceConverter);
            bmp.DetectCircle(50, 70);

            int n = 372;

            bmp.Bitmap.Save(@"C:\Users\matishadow\Desktop\rrr.bmp");

            System.Console.WriteLine("awd");
        }
    }
}
