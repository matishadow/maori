using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Maori.Interfaces;

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

        public MaoriBitmap(int width, int height, IColorSpaceConverter colorSpaceConverter) 
            : base(width, height)
        {
            this.colorSpaceConverter = colorSpaceConverter;
            CreatePixels();
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

        public MaoriBitmap DrawLine(int r, double theta)
        {
            var bitmap = new MaoriBitmap(Width, Height, colorSpaceConverter);
            double thetaRad = DegsToRad(theta);
            double valueCos = Math.Cos(thetaRad);
            double valueSin = Math.Sin(thetaRad);

            var white = new Pixel {A = 255, B = 255, G = 255, R = 255};
            var black = new Pixel { A = 255, B = 0, G = 0, R = 0 };

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (r == (int) (i * valueCos + j * valueSin))
                    {
                        bitmap[i, j] = black;
                    }
                    else
                    {
                        bitmap[i, j] = white;
                    }
                }
            }

            return bitmap;
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

            for (int i = 0 + pixelsToSkip; i < Width - pixelsToSkip; i++)
            {
                for (int j = 0 + pixelsToSkip; j < Height - pixelsToSkip; j++)
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
                }
            }

            return bitmap;
        }

        public MaoriBitmap ApplyKernel2D(double[,] kernelX, double[,] kernelY, bool drawAngle)
        {
            var bitmap = new MaoriBitmap(Width, Height, colorSpaceConverter);
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
                    sumX /= 8;
                    sumY /= 8;

                    var byteSaturatedSum = (byte)Math.Sqrt(sumX * sumX + sumY * sumY);
                    //byte byteSaturatedSum = intSum > byte.MaxValue ? byte.MaxValue : (byte)intSum;

                    double angle = Math.Atan2(sumX, sumY) + Math.PI;
                    colorSpaceConverter
                        .HsvToRgb(angle, 1, (double) byteSaturatedSum / byte.MaxValue, out byte r, out byte g,
                            out byte b);

                    bitmap.Pixels[i + j * Width].R = drawAngle ? r : byteSaturatedSum;
                    bitmap.Pixels[i + j * Width].G = drawAngle ? g : byteSaturatedSum;
                    bitmap.Pixels[i + j * Width].B = drawAngle ? b : byteSaturatedSum;
                    bitmap.Pixels[i + j * Width].A = byte.MaxValue;
                }
            }

            return bitmap;
        }

        public void DetectCircle()
        {
            var accumulator = new int[Width, Height, Width];

            Parallel.For(0, Width, x =>
            {
                Parallel.For(0, Height, y =>
                {
                    var p = this[x, y];

                    if (p.R != 255)
                    {
                        Parallel.For(1, Width, r =>
                        {
                            Parallel.For(0, 360, t =>
                            {
                                int a = (int) (x - r * Math.Cos(t * Math.PI / 180));
                                int b = (int) (y - r * Math.Sin(t * Math.PI / 180));

                                if (a > 0 && a < Width && b < Height && b > 0)
                                    accumulator[a, b, r] += 1;
                            });
                        });
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

        }

        public void DetectLines()
        {
            byte[,] accumulator = CreateAccumulator(Width, Height);

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    byte pixel = this[i, j].R;

                    if (pixel != 255)
                        DrawSin(i, j, accumulator);
                }
            }

            int m = 0;
            int mi = -1;
            int mj = -1;
            for (int i = 0; i < accumulator.GetLength(0); i++)
            {
                for (int j = 0; j < accumulator.GetLength(1); j++)
                {
                    byte b = accumulator[i, j];
                    if (b > m)
                    {
                        m = b;
                        mi = i;
                        mj = j;
                    }
                }
            }

        }

        private int CalculateR(int x, int y, double phi)
        {
            double cos = y * Math.Cos(phi);
            double sin = x * Math.Sin(phi);

            double R = cos + sin;

            return Convert.ToInt32(R);
        }

        private byte[,] CreateAccumulator(int r, int c)
        {
            const int degs = 360;
            int R = Convert.ToInt32(Math.Sqrt(r * r + c * c));

            var acc = new byte[degs, R];

            return acc;
        }

        private void DrawSin(int x, int y, byte[,] acc)
        {
            int degOffset = -90;

            if (acc == null) return;

            for (int i = 0; i < acc.GetLength(0); i++)
            {
                int r = CalculateR(x, y, DegsToRad(i + degOffset));

                if (r < acc.GetLength(1) && r >= 0)
                    acc[i, r]++;
            }
        }

        private double DegsToRad(double angleInDegrees)
        {
            double rad = angleInDegrees * (Math.PI / 180.0);

            return rad;
        }
    }
}