using System.Drawing;
using Maori.Interfaces;

namespace Maori.Implementations
{
    public class BitmapFileGetter : IBitmapFileGetter
    {
        public Image GetBitmap(string filePath)
        {
            return Image.FromFile(filePath);
        }
    }
}