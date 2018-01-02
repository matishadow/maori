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

            //(int x, int y, int r, int max, int[,,] accumulator) = bmp.DetectCircle();
            //double scale = 255.0 / max;

            //for (int ri = 0; ri < accumulator.GetLength(2); ri++)
            //{
            //   var b = new MaoriBitmap(bmp.Width, bmp.Height, colorSpaceConverter);

            //    for (int i = 0; i < b.Width; i++)
            //    {
            //        for (int j = 0; j < b.Height; j++)
            //        {
            //            byte value =  (byte) (accumulator[i, j, ri] * scale);
            //            if (value > 0)
            //                b[i, j] = new Pixel{A = 255, B = value, G = value, R = value};
            //            else
            //                b[i, j] = new Pixel { A = 255, B = 0, G = 0, R = 0 };
            //        }
            //    }

            //    b.Bitmap.Save($"C:\\Users\\matishadow\\Desktop\\cs\\{ri}.bmp", ImageFormat.Bmp);
            //}

            System.Console.WriteLine("awd");
        }
    }
}
