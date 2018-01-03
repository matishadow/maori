using System;
using System.Drawing;

namespace Maori.Interfaces
{
    public interface IDirectBitmap : IDisposable
    {
        Bitmap Bitmap { get; }
        byte[] Bits { get; }
        bool Disposed { get;  }
        int Height { get;  }
        int Width { get;  }
    }
}