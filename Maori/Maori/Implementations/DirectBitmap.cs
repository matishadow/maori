using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Maori.Interfaces;

namespace Maori.Implementations
{
    /// <summary>
    /// Copied from stack
    /// https://stackoverflow.com/questions/24701703/c-sharp-faster-alternatives-to-setpixel-and-getpixel-for-bitmaps-for-windows-f
    /// </summary>
    public class DirectBitmap : IDirectBitmap 
    {
        public Bitmap Bitmap { get; protected set; }
        public byte[] Bits { get; protected set; }
        public bool Disposed { get; private set; }
        public int Height { get; protected set; }
        public int Width { get; protected set; }

        protected GCHandle BitsHandle { get; set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new byte[width * height * 4];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public DirectBitmap()
        {
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}