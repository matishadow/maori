using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Maori.Interfaces;

namespace Maori.Implementations
{
    public class MaoriBitmap : DirectBitmap
    {
        public Pixel[] Pixels { get; protected set; }

        private void CreatePixels()
        {
            Pixels = new Pixel[Width * Height];
            BitsHandle = GCHandle.Alloc(Pixels, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb,
                BitsHandle.AddrOfPinnedObject());
        }

        public MaoriBitmap(int width, int height) : base(width, height)
        {
            CreatePixels();
        }

        public MaoriBitmap(Image image)
        {
            Width = image.Width;
            Height = image.Height;

            CreatePixels();

            using (Graphics g = Graphics.FromImage(Bitmap))
            {
                g.DrawImage(image, new Rectangle(Point.Empty, Bitmap.Size));
            }
        }

        public Pixel this[int x, int y]
        {
            get => Pixels[x + y * Width];
            set => Pixels[x + y * Width] = value;
        }

        public void ConvertToGrayscale()
        {
            for (var i = 0; i < Pixels.Length; i++)
            {
                Pixel p = Pixels[i];
                var gray = (byte) ((p.R + p.G + p.B) / 3);
                p.R = gray;
                p.G = gray;
                p.B = gray;

                Pixels[i] = p;
            }
        }

        public MaoriBitmap ApplyKernel2D(double[,] kernelX, double[,] kernelY)
        {
            var bitmap = new MaoriBitmap(Width, Height);
            int kernelSize = kernelX.GetLength(0);
            int pixelsToSkip = kernelSize / 2;

            for (int i = 0 + pixelsToSkip; i < Width - pixelsToSkip; i++)
            {
                for (int j = 0 + pixelsToSkip; j < Height - pixelsToSkip; j++)
                {
                    double sumX = 0;
                    double sumY = 0;

                    for (var k = 0; k < kernelSize; k++)
                    {
                        for (var l = 0; l < kernelSize; l++)
                        {
                            sumX += this[i - pixelsToSkip + k, j - pixelsToSkip + l].R * kernelX[k, l];
                            sumY += this[i - pixelsToSkip + k, j - pixelsToSkip + l].R * kernelY[k, l];
                        }
                    }

                    var intSum = (int)Math.Sqrt(sumX * sumX + sumY * sumY);
                    byte byteSaturatedSum = intSum > byte.MaxValue ? byte.MaxValue : (byte)intSum;

                    bitmap.Pixels[i + j * Width].R = byteSaturatedSum;
                    bitmap.Pixels[i + j * Width].G = byteSaturatedSum;
                    bitmap.Pixels[i + j * Width].B = byteSaturatedSum;
                    bitmap.Pixels[i + j * Width].A = byte.MaxValue;
                }
            }

            return bitmap;
        }
    }
}