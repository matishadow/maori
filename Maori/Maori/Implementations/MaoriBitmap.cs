using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Maori.Interfaces;

namespace Maori.Implementations
{
    public class MaoriBitmap : DirectBitmap
    {
        public Pixel[] Pixels { get; protected set; }

        public MaoriBitmap(int width, int height) : base(width, height)
        {
        }

        public MaoriBitmap(Image image)
        {
            Width = image.Width;
            Height = image.Height;
            Pixels = new Pixel[Width * Height];
            BitsHandle = GCHandle.Alloc(Pixels, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb,
                BitsHandle.AddrOfPinnedObject());

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
    }
}