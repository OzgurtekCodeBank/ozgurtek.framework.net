using System;

namespace ozgurtek.framework.core.Data
{
    public struct GdColor
    {
        private byte _r;
        private byte _g;
        private byte _b;
        private byte _a;
        private static readonly Random Random = new Random();

        public GdColor(byte r, byte g, byte b)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = 255;
        }

        public GdColor(byte r, byte g, byte b, byte a)
            : this(r, g, b)
        {
            _a = a;
        }

        public byte R
        {
            get { return _r; }
            set { _r = value; }
        }

        public byte G
        {
            get { return _g; }
            set { _g = value; }
        }

        public byte B
        {
            get { return _b; }
            set { _b = value; }
        }

        public byte A
        {
            get { return _a; }
            set { _a = value; }
        }

        public static GdColor FromHsl(double h, double s, double l)
        {
            double fh = h / 255.0;
            double fs = s / 255.0;
            double fl = l / 255.0;

            // default to gray
            double fr = fl;
            double fg = fl;
            double fb = fl;

            double v = (fl <= 0.5) ? (fl * (1 + fs)) : (fl + fs - (fl * fs));

            if (v > 0)
            {
                double m = fl + fl - v;
                double sv = (v - m) / v;
                fh *= 6.0;
                int sextant = (int)fh;
                double fract = fh - sextant;
                double vsf = v * sv * fract;
                double mid1 = m + vsf;
                double mid2 = v - vsf;

                switch (sextant)
                {
                    case 0:
                        fr = v;
                        fg = mid1;
                        fb = m;
                        break;

                    case 1:
                        fr = mid2;
                        fg = v;
                        fb = m;
                        break;

                    case 2:
                        fr = m;
                        fg = v;
                        fb = mid1;
                        break;

                    case 3:
                        fr = m;
                        fg = mid2;
                        fb = v;
                        break;

                    case 4:
                        fr = mid1;
                        fg = m;
                        fb = v;
                        break;

                    case 5:
                        fr = v;
                        fg = m;
                        fb = mid2;
                        break;
                }
            }

            int r = (int)Math.Round(fr * 255.0);
            int g = (int)Math.Round(fg * 255.0);
            int b = (int)Math.Round(fb * 255.0);
            return new GdColor((byte)r, (byte)g, (byte)b);
        }

        public static void FromRgb(GdColor rgb, out double h, out double s, out double l)
        {
            double r = rgb.R / 255.0;
            double g = rgb.G / 255.0;
            double b = rgb.B / 255.0;
            double v;
            double m;
            double vm;
            double r2, g2, b2;

            h = 0; // default to black
            s = 0;
            l = 0;

            v = Math.Max(r, g);
            v = Math.Max(v, b);
            m = Math.Min(r, g);
            m = Math.Min(m, b);
            l = (m + v) / 2.0;

            if (l <= 0.0)
            {
                return;
            }

            vm = v - m;
            s = vm;

            if (s > 0.0)
            {
                s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
            }

            else
            {
                return;
            }

            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;

            if (r == v)
            {
                h = (g == m ? 5.0 + b2 : 1.0 - g2);
            }

            else if (g == v)
            {
                h = (b == m ? 1.0 + r2 : 3.0 - b2);
            }

            else
            {
                h = (r == m ? 3.0 + g2 : 5.0 - r2);
            }

            h /= 6.0;
        }

        public static GdColor FromRgbRange(byte minValue, byte maxValue)
        {
            int r = Random.Next(minValue, maxValue);
            int g = Random.Next(minValue, maxValue);
            int b = Random.Next(minValue, maxValue);
            return new GdColor((byte)r, (byte)g, (byte)b);
        }

        public static GdColor FromHslRange(byte minValue, byte maxValue)
        {
            int h = Random.Next(minValue, maxValue);
            int s = Random.Next(minValue, maxValue);
            int l = Random.Next(minValue, maxValue);
            return FromHsl(h, s, l);
        }

        public static GdColor Sharp
        {
            get { return FromRgbRange(0, 128); }
        }

        public static GdColor Soft
        {
            get
            {
                int h = Random.Next(0, 255);
                int s = Random.Next(0, 255);
                int l = Random.Next(192, 224);
                return FromHsl(h, s, l);
            }
        }

        public static double[] Normalize(GdColor Color)
        {
            return new[] { Color.R / 255.0, Color.G / 255.0, Color.B / 255.0, Color.A / 255.0 };
        }

        public static GdColor FromNormalize(double r, double g, double b, double a)
        {
            return new GdColor(Convert.ToByte(r * 255), Convert.ToByte(g * 255), Convert.ToByte(b * 255),
                Convert.ToByte(a * 255));
        }

        public static readonly GdColor AliceBlue = new GdColor(240, 248, 255);
        public static readonly GdColor AntiqueWhite = new GdColor(250, 235, 215);
        public static readonly GdColor Aqua = new GdColor(0, 255, 255);
        public static readonly GdColor Aquamarine = new GdColor(127, 255, 212);
        public static readonly GdColor Azure = new GdColor(240, 255, 255);
        public static readonly GdColor Beige = new GdColor(245, 245, 220);
        public static readonly GdColor Bisque = new GdColor(255, 228, 196);
        public static readonly GdColor Black = new GdColor(0, 0, 0);
        public static readonly GdColor BlanchedAlmond = new GdColor(255, 235, 205);
        public static readonly GdColor Blue = new GdColor(0, 0, 255);
        public static readonly GdColor BlueViolet = new GdColor(138, 43, 226);
        public static readonly GdColor Brown = new GdColor(165, 42, 42);
        public static readonly GdColor BurlyWood = new GdColor(222, 184, 135);
        public static readonly GdColor CadetBlue = new GdColor(95, 158, 160);
        public static readonly GdColor Chartreuse = new GdColor(127, 255, 0);
        public static readonly GdColor Chocolate = new GdColor(210, 105, 30);
        public static readonly GdColor Coral = new GdColor(255, 127, 80);
        public static readonly GdColor CornflowerBlue = new GdColor(100, 149, 237);
        public static readonly GdColor Cornsilk = new GdColor(255, 248, 220);
        public static readonly GdColor Crimson = new GdColor(220, 20, 60);
        public static readonly GdColor Cyan = new GdColor(0, 255, 255);
        public static readonly GdColor DarkBlue = new GdColor(0, 0, 139);
        public static readonly GdColor DarkCyan = new GdColor(0, 139, 139);
        public static readonly GdColor DarkGoldenrod = new GdColor(184, 134, 11);
        public static readonly GdColor DarkGray = new GdColor(169, 169, 169);
        public static readonly GdColor DarkGreen = new GdColor(0, 100, 0);
        public static readonly GdColor DarkKhaki = new GdColor(189, 183, 107);
        public static readonly GdColor DarkMagenta = new GdColor(139, 0, 139);
        public static readonly GdColor DarkOliveGreen = new GdColor(85, 107, 47);
        public static readonly GdColor DarkOrange = new GdColor(255, 140, 0);
        public static readonly GdColor DarkOrchid = new GdColor(153, 50, 204);
        public static readonly GdColor DarkRed = new GdColor(139, 0, 0);
        public static readonly GdColor DarkSalmon = new GdColor(233, 150, 122);
        public static readonly GdColor DarkSeaGreen = new GdColor(143, 188, 143);
        public static readonly GdColor DarkSlateBlue = new GdColor(72, 61, 139);
        public static readonly GdColor DarkSlateGray = new GdColor(47, 79, 79);
        public static readonly GdColor DarkTurquoise = new GdColor(0, 206, 209);
        public static readonly GdColor DarkViolet = new GdColor(148, 0, 211);
        public static readonly GdColor DeepPink = new GdColor(255, 20, 147);
        public static readonly GdColor DeepSkyBlue = new GdColor(0, 191, 255);
        public static readonly GdColor DimGray = new GdColor(105, 105, 105);
        public static readonly GdColor DodgerBlue = new GdColor(30, 144, 255);
        public static readonly GdColor Firebrick = new GdColor(178, 34, 34);
        public static readonly GdColor FloralWhite = new GdColor(255, 250, 240);
        public static readonly GdColor ForestGreen = new GdColor(34, 139, 34);
        public static readonly GdColor Fuchsia = new GdColor(255, 0, 255);
        public static readonly GdColor Fuschia = new GdColor(255, 0, 255);
        public static readonly GdColor Gainsboro = new GdColor(220, 220, 220);
        public static readonly GdColor GhostWhite = new GdColor(248, 248, 255);
        public static readonly GdColor Gold = new GdColor(255, 215, 0);
        public static readonly GdColor Goldenrod = new GdColor(218, 165, 32);
        public static readonly GdColor Gray = new GdColor(128, 128, 128);
        public static readonly GdColor Green = new GdColor(0, 128, 0);
        public static readonly GdColor GreenYellow = new GdColor(173, 255, 47);
        public static readonly GdColor Honeydew = new GdColor(240, 255, 240);
        public static readonly GdColor HotPink = new GdColor(255, 105, 180);
        public static readonly GdColor IndianRed = new GdColor(205, 92, 92);
        public static readonly GdColor Indigo = new GdColor(75, 0, 130);
        public static readonly GdColor Ivory = new GdColor(255, 255, 240);
        public static readonly GdColor Khaki = new GdColor(240, 230, 140);
        public static readonly GdColor Lavender = new GdColor(230, 230, 250);
        public static readonly GdColor LavenderBlush = new GdColor(255, 240, 245);
        public static readonly GdColor LawnGreen = new GdColor(124, 252, 0);
        public static readonly GdColor LemonChiffon = new GdColor(255, 250, 205);
        public static readonly GdColor LightBlue = new GdColor(173, 216, 230);
        public static readonly GdColor LightCoral = new GdColor(240, 128, 128);
        public static readonly GdColor LightCyan = new GdColor(224, 255, 255);
        public static readonly GdColor LightGoldenrodYellow = new GdColor(250, 250, 210);
        public static readonly GdColor LightGray = new GdColor(211, 211, 211);
        public static readonly GdColor LightGreen = new GdColor(144, 238, 144);
        public static readonly GdColor LightPink = new GdColor(255, 182, 193);
        public static readonly GdColor LightSalmon = new GdColor(255, 160, 122);
        public static readonly GdColor LightSeaGreen = new GdColor(32, 178, 170);
        public static readonly GdColor LightSkyBlue = new GdColor(135, 206, 250);
        public static readonly GdColor LightSlateGray = new GdColor(119, 136, 153);
        public static readonly GdColor LightSteelBlue = new GdColor(176, 196, 222);
        public static readonly GdColor LightYellow = new GdColor(255, 255, 224);
        public static readonly GdColor Lime = new GdColor(0, 255, 0);
        public static readonly GdColor LimeGreen = new GdColor(50, 205, 50);
        public static readonly GdColor Linen = new GdColor(250, 240, 230);
        public static readonly GdColor Magenta = new GdColor(255, 0, 255);
        public static readonly GdColor Maroon = new GdColor(128, 0, 0);
        public static readonly GdColor MediumAquamarine = new GdColor(102, 205, 170);
        public static readonly GdColor MediumBlue = new GdColor(0, 0, 205);
        public static readonly GdColor MediumOrchid = new GdColor(186, 85, 211);
        public static readonly GdColor MediumPurple = new GdColor(147, 112, 219);
        public static readonly GdColor MediumSeaGreen = new GdColor(60, 179, 113);
        public static readonly GdColor MediumSlateBlue = new GdColor(123, 104, 238);
        public static readonly GdColor MediumSpringGreen = new GdColor(0, 250, 154);
        public static readonly GdColor MediumTurquoise = new GdColor(72, 209, 204);
        public static readonly GdColor MediumVioletRed = new GdColor(199, 21, 133);
        public static readonly GdColor MidnightBlue = new GdColor(25, 25, 112);
        public static readonly GdColor MintCream = new GdColor(245, 255, 250);
        public static readonly GdColor MistyRose = new GdColor(255, 228, 225);
        public static readonly GdColor Moccasin = new GdColor(255, 228, 181);
        public static readonly GdColor NavajoWhite = new GdColor(255, 222, 173);
        public static readonly GdColor Navy = new GdColor(0, 0, 128);
        public static readonly GdColor OldLace = new GdColor(253, 245, 230);
        public static readonly GdColor Olive = new GdColor(128, 128, 0);
        public static readonly GdColor OliveDrab = new GdColor(107, 142, 35);
        public static readonly GdColor Orange = new GdColor(255, 165, 0);
        public static readonly GdColor OrangeRed = new GdColor(255, 69, 0);
        public static readonly GdColor Orchid = new GdColor(218, 112, 214);
        public static readonly GdColor PaleGoldenrod = new GdColor(238, 232, 170);
        public static readonly GdColor PaleGreen = new GdColor(152, 251, 152);
        public static readonly GdColor PaleTurquoise = new GdColor(175, 238, 238);
        public static readonly GdColor PaleVioletRed = new GdColor(219, 112, 147);
        public static readonly GdColor PapayaWhip = new GdColor(255, 239, 213);
        public static readonly GdColor PeachPuff = new GdColor(255, 218, 185);
        public static readonly GdColor Peru = new GdColor(205, 133, 63);
        public static readonly GdColor Pink = new GdColor(255, 192, 203);
        public static readonly GdColor Plum = new GdColor(221, 160, 221);
        public static readonly GdColor PowderBlue = new GdColor(176, 224, 230);
        public static readonly GdColor Purple = new GdColor(128, 0, 128);
        public static readonly GdColor Red = new GdColor(255, 0, 0);
        public static readonly GdColor RosyBrown = new GdColor(188, 143, 143);
        public static readonly GdColor RoyalBlue = new GdColor(65, 105, 225);
        public static readonly GdColor SaddleBrown = new GdColor(139, 69, 19);
        public static readonly GdColor Salmon = new GdColor(250, 128, 114);
        public static readonly GdColor SandyBrown = new GdColor(244, 164, 96);
        public static readonly GdColor SeaGreen = new GdColor(46, 139, 87);
        public static readonly GdColor SeaShell = new GdColor(255, 245, 238);
        public static readonly GdColor Sienna = new GdColor(160, 82, 45);
        public static readonly GdColor Silver = new GdColor(192, 192, 192);
        public static readonly GdColor SkyBlue = new GdColor(135, 206, 235);
        public static readonly GdColor SlateBlue = new GdColor(106, 90, 205);
        public static readonly GdColor SlateGray = new GdColor(112, 128, 144);
        public static readonly GdColor Snow = new GdColor(255, 250, 250);
        public static readonly GdColor SpringGreen = new GdColor(0, 255, 127);
        public static readonly GdColor SteelBlue = new GdColor(70, 130, 180);
        public static readonly GdColor Tan = new GdColor(210, 180, 140);
        public static readonly GdColor Teal = new GdColor(0, 128, 128);
        public static readonly GdColor Thistle = new GdColor(216, 191, 216);
        public static readonly GdColor Tomato = new GdColor(255, 99, 71);
        public static readonly GdColor Transparent = new GdColor(255, 255, 255, 0);
        public static readonly GdColor Turquoise = new GdColor(64, 224, 208);
        public static readonly GdColor Violet = new GdColor(238, 130, 238);
        public static readonly GdColor Wheat = new GdColor(245, 222, 179);
        public static readonly GdColor White = new GdColor(255, 255, 255);
        public static readonly GdColor WhiteSmoke = new GdColor(245, 245, 245);
        public static readonly GdColor Yellow = new GdColor(255, 255, 0);
        public static readonly GdColor YellowGreen = new GdColor(154, 205, 50);
    }
}