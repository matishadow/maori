namespace Maori.Interfaces
{
    public interface IColorSpaceConverter
    {
        void HsvToRgb(double hue, double saturation, double value, out byte r, out byte g, out byte b);
    }
}