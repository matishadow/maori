using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Maori.Interfaces;

namespace Maori.Implementations
{
    public class MaoriBitmap : DirectBitmap
    {
        public MaoriBitmap(int width, int height) : base(width, height)
        {
        }

        public MaoriBitmap(Image image)
        {
            Width = image.Width;
            Height = image.Height;
            Bits = new byte[Width * Height * 4];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb,
                BitsHandle.AddrOfPinnedObject());

            using (Graphics g = Graphics.FromImage(Bitmap))
            {
                g.DrawImage(image, new Rectangle(Point.Empty, Bitmap.Size));
            }
        }

        public byte this[int x, int y]
        {
            get => Bits[x + y * Width];
            set => Bits[x + y * Width] = value;
        }
    }
}