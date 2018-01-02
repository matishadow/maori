using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Maori.Interfaces;
using Point = System.Drawing.Point;

namespace Maori.Implementations
{
    public class MaoriBitmap : DirectBitmap
    {
        private readonly IColorSpaceConverter colorSpaceConverter;
        public Pixel[] Pixels { get; protected set; }

        private void CreatePixels()
        {
            Pixels = new Pixel[Width * Height];
            BitsHandle = GCHandle.Alloc(Pixels, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb,
                BitsHandle.AddrOfPinnedObject());
        }

        public BitmapImage ToWpfImage()
        {
            var ms = new MemoryStream();
            Bitmap.Save(ms, ImageFormat.Bmp);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public static MaoriBitmap FromWpfImage(BitmapSource image, IColorSpaceConverter colorSpaceConverter)
        {
            var bmp = new Bitmap(
                image.PixelWidth,
                image.PixelHeight,
                PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
                new Rectangle(Point.Empty, bmp.Size),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppPArgb);
            image.CopyPixels(
                Int32Rect.Empty,
                data.Scan0,
                data.Height * data.Stride,
                data.Stride);
            bmp.UnlockBits(data);
            return new MaoriBitmap(bmp, colorSpaceConverter);
        }

        public MaoriBitmap(int width, int height, IColorSpaceConverter colorSpaceConverter) 
            : base(width, height)
        {
            this.colorSpaceConverter = colorSpaceConverter;
            CreatePixels();
        }

        public MaoriBitmap ConvertToEdgesSobel(bool showAngle = false)
        {
            double[,] xKernel = {
                {-1, 0, 1},
                {-2, 0, 2},
                {-1, 0, 1}
            };
            double[,] yKernel = {
                {-1, -2, -1},
                {0, 0, 0},
                {1, 2, 1}
            };

            return ApplyKernel2D(xKernel, yKernel, showAngle);
        }

        public MaoriBitmap(Image image, IColorSpaceConverter colorSpaceConverter)
        {
            this.colorSpaceConverter = colorSpaceConverter;
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
            Parallel.For(0, Pixels.Length, i =>
            {
                Pixel p = Pixels[i];
                var gray = (byte)((p.R + p.G + p.B) / 3);
                p.R = gray;
                p.G = gray;
                p.B = gray;

                Pixels[i] = p;
            });
        }

        public void ConvertToBinary(double threshold)
        {
            var blackPixel = new Pixel {A = 255, B = 0, G = 0, R = 0};
            var whitePixel = new Pixel {A = 255, B = 255, G = 255, R = 255};
            var thresholdValue = (byte) (255 * threshold);


            Parallel.For(0, Pixels.Length, i =>
            {
                Pixel p = Pixels[i];
                double luminance = (0.2126 * p.R + 0.7152 * p.G + 0.0722 * p.B);
                if (luminance > thresholdValue)
                    Pixels[i] = whitePixel;
                else
                    Pixels[i] = blackPixel;
            });
        }

        public MaoriBitmap GaussianBlur(int kernelSize, double ro)
        {
            var kernel = new double[kernelSize, kernelSize];
            int center = kernelSize / 2;

            for (var i = 0; i < kernelSize; i++)
            {
                for (var j = 0; j < kernelSize; j++)
                {
                    kernel[i, j] = CalculateGaussianValue(
                        DistanceFromCenter(center, i), 
                        DistanceFromCenter(center, j), ro);
                } 
            }
            double sum = kernel.Cast<double>().Sum();

            for (int i = 0; i < kernelSize; i++)
            {
                for (int j = 0; j < kernelSize; j++)
                {
                    kernel[i, j] = kernel[i, j] * (1 / sum);
                }
            }

            return ApplyKernel(kernel);
        }

        private int DistanceFromCenter(int center, int place)
        {
            return Math.Abs(center - place);
        }

        private double CalculateGaussianValue(int x, int y, double ro)
        {
            double fraction = 1 / (2 * Math.PI * ro * ro);
            double exponential = -(x * x + y * y) / (2 * ro * ro);

            return  (fraction * Math.Exp(exponential));
        }

        public MaoriBitmap ApplyKernel(double[,] kernel)
        {
            var bitmap = new MaoriBitmap(Bitmap, colorSpaceConverter);
            int kernelSize = kernel.GetLength(0);
            int pixelsToSkip = kernelSize / 2;
            double totalValueKernel = kernel.Cast<double>().Sum();

            Parallel.For(0 + pixelsToSkip, Width - pixelsToSkip, i =>
            {
                Parallel.For(0 + pixelsToSkip, Height - pixelsToSkip, j =>
                {
                        double sumR = 0;
                        double sumG = 0;
                        double sumB = 0;

                        for (var k = 0; k < kernelSize; k++)
                        {
                            for (var l = 0; l < kernelSize; l++)
                            {
                                sumR += this[i - pixelsToSkip + k, j - pixelsToSkip + l].R * kernel[k, l];
                                sumG += this[i - pixelsToSkip + k, j - pixelsToSkip + l].G * kernel[k, l];
                                sumB += this[i - pixelsToSkip + k, j - pixelsToSkip + l].B * kernel[k, l];
                            }
                        }

                        bitmap.Pixels[i + j * Width].R = (byte)(sumR / totalValueKernel);
                        bitmap.Pixels[i + j * Width].G = (byte)(sumG / totalValueKernel);
                        bitmap.Pixels[i + j * Width].B = (byte)(sumB / totalValueKernel);
                        bitmap.Pixels[i + j * Width].A = byte.MaxValue;
                });
            });

            return bitmap;
        }

        public MaoriBitmap ApplyKernel2D(double[,] kernelX, double[,] kernelY, bool drawAngle)
        {
            var bitmap = new MaoriBitmap(Width, Height, colorSpaceConverter);
            int kernelSize = kernelX.GetLength(0);
            int pixelsToSkip = kernelSize / 2;

            Parallel.For(0 + pixelsToSkip, Width - pixelsToSkip, i =>
            {
                Parallel.For(0 + pixelsToSkip, Height - pixelsToSkip, j =>
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

                    var byteSaturatedSum = (byte) Math.Sqrt(sumX * sumX + sumY * sumY);

                    double angle = Math.Atan2(sumX, sumY) + Math.PI;
                    colorSpaceConverter
                        .HsvToRgb(angle, 1, (double) byteSaturatedSum / byte.MaxValue, out byte r, out byte g,
                            out byte b);

                    bitmap.Pixels[i + j * Width].R = drawAngle ? r : byteSaturatedSum;
                    bitmap.Pixels[i + j * Width].G = drawAngle ? g : byteSaturatedSum;
                    bitmap.Pixels[i + j * Width].B = drawAngle ? b : byteSaturatedSum;
                    bitmap.Pixels[i + j * Width].A = byte.MaxValue;
                });
            });

            return bitmap;
        }

        public void DetectCircle(int minR, int maxR)
        {
            var accumulator = new int[Width, Height, Width];

            Parallel.For(0, Width, x =>
            {
                Parallel.For(0, Height, y =>
                {
                    var p = this[x, y];

                    if (p.R != 255)
                    {
                        for (int r = minR; r < maxR; r++)
                        {
                            for (int t = 0; t < 360; t++)
                            {
                                int a = (int)(x - r * Math.Cos(t * Math.PI / 180));
                                int b = (int)(y - r * Math.Sin(t * Math.PI / 180));

                                if (a > 0 && a < Width && b < Height && b > 0)
                                    accumulator[a, b, r] += 1;
                            }
                        }
                    }
                });
            });

            int max = 0;
            int mx = 0;
            int my = 0;
            int mr = 0;

            Parallel.For(0, accumulator.GetLength(0), i =>
            {
                Parallel.For(0, accumulator.GetLength(1), j =>
                {
                    Parallel.For(0, accumulator.GetLength(2), k =>
                    {
                        int value = accumulator[i, j, k];
                        if (value > max)
                        {
                            max = value;
                            mx = i;
                            my = j;
                            mr = k;
                        }
                    });
                });
            });

            using (Graphics g = Graphics.FromImage(Bitmap))
            {
                g.DrawEllipse(new Pen(Color.Red, 2), mx - mr, my - mr, 2 * mr, 2 * mr);
            }
        }
    }
}