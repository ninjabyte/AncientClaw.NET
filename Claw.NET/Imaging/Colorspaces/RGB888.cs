using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Claw.Imaging.Colorspaces
{
    public class RGB888
    {
        public byte R {get; set;}

        public byte G {get; set;}

        public byte B {get; set;}

        public bool Transparent { get; set; }

        public uint Value
        {
            get
            {
                return Pack(R, G, B);
            }

            set
            {
                byte r,g,b;
                Unpack(value, out r, out g, out b);
                R = r;
                G = g;
                B = b;
            }
        }

        public Color Color
        {
            get
            {
                return Color.FromArgb(Transparent ? 0 : 0xFF, R, G, B);
            }

            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
                Transparent = (value.A > 127) ? false : true;
            }
        }

        public RGB888(byte R, byte G, byte B, bool Transparent = false)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.Transparent = Transparent;
        }

        public RGB888(uint Value, bool Transparent = false)
        {
            this.Value = Value;
            this.Transparent = Transparent;
        }

        public RGB888(Color Color)
        {
            this.Color = Color;
        }

        public RGB888(RGB565 Color)
        {
            this.R = Color.R;
            this.G = Color.G;
            this.B = Color.B;
            this.Transparent = false;
        }

        public RGB565 ToRGB565()
        {
            return ToRGB565(this);
        }

        public static RGB565 ToRGB565(RGB888 Color)
        {
            return new RGB565((byte)(Color.R >> 3), (byte)(Color.G >> 2), (byte)(Color.B >> 3));
        }

        private static uint Pack(byte R, byte G, byte B)
        {
            return (uint)((((uint)R) << 16) | (((uint)G) << 8) | ((uint)B));
        }

        private static void Unpack(uint Value, out byte R, out byte G, out byte B)
        {
            R = (byte)(Value >> 16);
            G = (byte)((Value >> 8) & 0xFF);
            B = (byte)(Value & 0xFF);
        }

        public static uint PackValueFromColor(Color Color)
        {
            return PackValueFromRGB888(Color.R, Color.G, Color.B);
        }

        public static uint PackValueFromColor565(RGB565 Color)
        {
            return PackValueFromRGB888(Color.R, Color.G, Color.B);
        }

        public static uint PackValueFromRGB888(byte R, byte G, byte B)
        {
            return Pack(R, G, B);
        }

        public static uint PackValueFromRGB565(byte R, byte G, byte B)
        {
            return Pack((byte)((R & 0x1F) << 3), (byte)((G & 0x3F) << 2), (byte)((B & 0x1F) << 3));
        }

        public static Color UnpackValueToColor(uint Value)
        {
            byte r, g, b;
            Unpack(Value, out r, out g, out b);

            return Color.FromArgb(r, g, b);
        }

        public static RGB565 UnpackValueToColor565(uint Value)
        {
            byte r, g, b;
            Unpack(Value, out r, out g, out b);

            return new RGB565(r, g, b);
        }

        public static void UnpackValueToRGB888(uint Value, out byte R, out byte G, out byte B)
        {
            Unpack(Value, out R, out G, out B);
        }

        public static void UnpackValueToRGB565(uint Value, out byte R, out byte G, out byte B)
        {
            byte r, g, b;
            Unpack(Value, out r, out g, out b);
            R = (byte)(r >> 3);
            G = (byte)(g >> 2);
            B = (byte)(b >> 3);
        }



        public void Add(byte R1, byte G1, byte B1, out byte Rout, out byte Gout, out byte Bout)
        {
            Add(R, G, B, R1, G1, B1, out Rout, out Gout, out Bout);
        }

        public RGB888 Add(RGB888 Color)
        {
            return Add(this, Color);
        }

        public static RGB888 Add(RGB888 Color0, RGB888 Color1)
        {
            byte r, g, b;
            Add(Color0.R, Color0.G, Color0.B, Color1.R, Color1.G, Color1.B, out r, out g, out b);
            return new RGB888(r, g, b, Color0.Transparent || Color1.Transparent);
        }

        public static void Add(byte R0, byte G0, byte B0, byte R1, byte G1, byte B1, out byte Rout, out byte Gout, out byte Bout)
        {
            Rout = (byte)(R0 + UnscaleComponent(R1));
            Gout = (byte)(G0 + UnscaleComponent(G1));
            Bout = (byte)(B0 + UnscaleComponent(B1));
        }

        public void Subtract(byte R1, byte G1, byte B1, out byte Rout, out byte Gout, out byte Bout)
        {
            Subtract(R, G, B, R1, G1, B1, out Rout, out Gout, out Bout);
        }

        public RGB888 Subtract(RGB888 Color)
        {
            return Subtract(this, Color);
        }

        public static RGB888 Subtract(RGB888 Color0, RGB888 Color1)
        {
            byte r, g, b;
            Subtract(Color0.R, Color0.G, Color0.B, Color1.R, Color1.G, Color1.B, out r, out g, out b);
            return new RGB888(r, g, b, Color0.Transparent || Color1.Transparent);
        }

        public static void Subtract(byte R0, byte G0, byte B0, byte R1, byte G1, byte B1, out byte Rout, out byte Gout, out byte Bout)
        {
            Rout = ClampComponent(ScaleComponent((short)(R0 - R1)));
            Gout = ClampComponent(ScaleComponent((short)(G0 - G1)));
            Bout = ClampComponent(ScaleComponent((short)(B0 - B1)));
        }

        public void Multiply(float Multiplier, out byte Rout, out byte Gout, out byte Bout)
        {
            Multiply(R, G, B, Multiplier, out Rout, out Gout, out Bout);
        }

        public RGB888 Multiply(float Multiplier)
        {
            return Multiply(this, Multiplier);
        }

        public static RGB888 Multiply(RGB888 Color, float Multiplier)
        {
            byte r, g, b;
            Multiply(Color.R, Color.G, Color.B, Multiplier, out r, out g, out b);
            return new RGB888(r, g, b, Color.Transparent);
        }

        public static void Multiply(byte R0, byte G0, byte B0, float Multiplier, out byte Rout, out byte Gout, out byte Bout)
        {
            Rout = ClampComponent(ScaleComponent((short)(UnscaleComponent(R0) * Multiplier)));
            Gout = ClampComponent(ScaleComponent((short)(UnscaleComponent(G0) * Multiplier)));
            Bout = ClampComponent(ScaleComponent((short)(UnscaleComponent(B0) * Multiplier)));
        }

        public int SquareDifference(RGB888 Color)
        {
            return SquareDifference(this, Color);
        }

        public static int SquareDifference(RGB888 Color0, RGB888 Color1)
        {
            return SquareDifference(Color0.R, Color0.G, Color0.B, Color1.R, Color1.G, Color1.B);
        }

        public static int SquareDifference(byte R0, byte G0, byte B0, byte R1, byte G1, byte B1)
        {
            int r = R0 - R1;
            int g = G0 - G1;
            int b = B0 - B1;
            return r * r + g * g + b * b;
        }

        /// <summary>
        /// Scales an unscaled component to a [-256...256] range.
        /// </summary>
        /// <param name="Component">Color component</param>
        /// <returns>Scaled component</returns>
        private static short ScaleComponent(short Component)
        {
            return (short)(Component * 2 - 256);
        }

        /// <summary>
        /// Scales a previously range scaled component down to its original scale.
        /// </summary>
        /// <param name="Component">Color component</param>
        /// <returns>Unscaled component</returns>
        private static byte UnscaleComponent(short ScaledComponent)
        {
            return (byte)ClampComponent((short)(ScaledComponent / 2 + 128));
        }

        private static byte ClampComponent(short Component)
        {
            return (byte)((Component > 0xFF) ? 0xFF : (Component < 0) ? 0 : Component);
        }
    }
}
