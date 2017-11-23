namespace Maori
{
    public struct Pixel
    {
        public byte B { get; set; }
        public byte G { get; set; }
        public byte R { get; set; }
        public byte A { get; set; }

        public override string ToString()
        {
            return $"{nameof(B)}: {B}, {nameof(G)}: {G}, {nameof(R)}: {R}, {nameof(A)}: {A}";
        }
    }
}