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
            //IBitmapFileGetter bitmapFileGetter = new BitmapFileGetter();
            //IColorSpaceConverter colorSpaceConverter = new ColorSpaceConverter();

            //Image img = bitmapFileGetter.GetBitmap(@"C:\Users\matishadow\Desktop\b.bmp");
            //var bmp = new MaoriBitmap(img, colorSpaceConverter);

            //var random = new Random();

            //int gx = 120;
            //int gy = 83;
            //int gr = 66;

            //int i = 0;
            //int x = gx;
            //int y = gy;
            //int r = gr;

            //Pixel black = new Pixel() {A = 255, B = 0, G = 0, R = 0};
            //Pixel white = new Pixel() { A = 255, B = 255, G = 255, R = 255 };

            //var circle = new List<int>();

            //for (int j = 0; j < bmp.Pixels.Length; j++)
            //{
            //    if (bmp.Pixels[j].R == 0)
            //        circle.Add(j);
            //}

            //int len = bmp.Pixels.Length;
            //do
            //{
            //    int pickedIndex = random.Next(circle.Count);
            //    bmp.Pixels[circle[pickedIndex]] = white;
            //    circle.RemoveAt(pickedIndex);
            //    bmp.Pixels[random.Next(len)] = black;

            //    (x, y, r) = bmp.DetectCircle(50, 80);
            //    i++;
            //    bmp.Bitmap.Save($"C:\\Users\\matishadow\\Desktop\\cs\\{i}.bmp", ImageFormat.Bmp);
            //} while (x == gx && y == gy && r == gr);

            //int n = 372;


            //System.Console.WriteLine("awd");

        }
    }
}
