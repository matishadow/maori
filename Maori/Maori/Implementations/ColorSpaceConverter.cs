using System;
using Maori.Interfaces;

namespace Maori.Implementations
{
    public class ColorSpaceConverter : IColorSpaceConverter
    {
        public void HsvToRgb(double hue, double saturation, double value, out byte r, out byte g, out byte b)
        {
            double h_ = hue / (2 * Math.PI) * 6;

            double c = saturation * value;
            double x = c * (1 - Math.Abs((h_ % 2) - 1));
            double r_, g_, b_;
            if (h_ < 1)
            {
                r_ = c;
                g_ = x;
                b_ = 0;
            }
            else if (h_ < 2)
            {
                r_ = x;
                g_ = c;
                b_ = 0;
            }
            else if (h_ < 3)
            {
                r_ = 0;
                g_ = c;
                b_ = x;
            }
            else if (h_ < 4)
            {
                r_ = 0;
                g_ = x;
                b_ = c;
            }
            else if (h_ < 5)
            {
                r_ = x;
                g_ = 0;
                b_ = c;
            }
            else
            {
                r_ = c;
                g_ = 0;
                b_ = x;
            }

            double m = value - c;

            r_ += m;
            g_ += m;
            b_ += m;

            r = (byte)(r_ * byte.MaxValue);
            g = (byte)(g_ * byte.MaxValue);
            b = (byte)(b_ * byte.MaxValue);
        }
    }
}