using System.Drawing;

namespace Maori.Interfaces
{
    public interface IBitmapFileGetter
    {
        Image GetBitmap(string filePath);
    }
}