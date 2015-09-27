using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Claw.Imaging.Colorspaces
{
    public class RGB565
    {
        private byte r;
        private byte g;
        private byte b;

        public byte R
        {
            get
            {
                return (byte)(r << 3);
            }
            set
            {
                r = (byte)(value >> 3);
            }
        }

        public byte G
        {
            get
            {
                return (byte)(g << 2);
            }

            set
            {
                g = (byte)(value >> 2);
            }
        }

        public byte B
        {
            get
            {
                return (byte)(b << 3);
            }

            set
            {
                b = (byte)(value >> 3);
            }
        }

        public ushort Value
        {
            get
            {
                return Pack(R, G, B);
            }

            set
            {
                Unpack(value, out r, out g, out b);
            }
        }

        public Color Color
        {
            get
            {
                return Color.FromArgb(R, G, B);
            }

            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
        }

        public RGB565(byte R, byte G, byte B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public RGB565(ushort Value)
        {
            this.Value = Value;
        }

        public RGB565(Color Color)
        {
            this.Color = Color;
        }

        private static ushort Pack(byte R, byte G, byte B)
        {
            return (ushort)((((ushort)R & 0x1F) << 11) | (((ushort)G & 0x3F) << 5) | ((ushort)B & 0x1F));
        }

        private static void Unpack(ushort Value, out byte R, out byte G, out byte B)
        {
            R = (byte)(Value >> 11);
            G = (byte)((Value >> 5) & 0x3F);
            B = (byte)(Value & 0x1F);
        }

        public static ushort PackValueFromColor(Color Color) {
            return PackValueFromRGB888(Color.R, Color.G, Color.B);
        }

        public static ushort PackValueFromColor888(RGB888 Color)
        {
            return PackValueFromRGB888(Color.R, Color.G, Color.B);
        }

        public static ushort PackValueFromRGB888(byte R, byte G, byte B)
        {
            return Pack((byte)(R >> 3), (byte)(G >> 2), (byte)(B >> 3));
        }

        public static ushort PackValueFromRGB565(byte R, byte G, byte B)
        {
            return Pack(R, G, B);
        }

        public static Color UnpackValueToColor(ushort Value)
        {
            byte r, g, b;
            Unpack(Value, out r, out g, out b);
            r = (byte)(r << 3);
            g = (byte)(g << 2);
            b = (byte)(b << 3);

            return Color.FromArgb(r, g, b);
        }

        public static RGB888 UnpackValueToColor888(ushort Value)
        {
            byte r, g, b;
            Unpack(Value, out r, out g, out b);
            r = (byte)(r << 3);
            g = (byte)(g << 2);
            b = (byte)(b << 3);

            return new RGB888(r, g, b);
        }

        public static void UnpackValueToRGB888(ushort Value, out byte R, out byte G, out byte B)
        {
            byte r, g, b;
            Unpack(Value, out r, out g, out b);
            R = (byte)(r << 3);
            G = (byte)(g << 2);
            B = (byte)(b << 3);
        }

        public static void UnpackValueToRGB565(ushort Value, out byte R, out byte G, out byte B)
        {
            Unpack(Value, out R, out G, out B);
        }
    }
}
