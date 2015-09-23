using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Claw.Imaging.Colorspaces;
using System.Resources;

namespace Claw.Imaging
{
    public class Palette
    {
        // color format is RGB565
        const byte COLOR_DEPTH_R = 5;
        const byte COLOR_DEPTH_G = 6;
        const byte COLOR_DEPTH_B = 5;

        public ushort[] Colors { get; private set; }

        public byte Size { get { return (byte)Colors.Length; } }

        public static Palette DefaultPalette
        {
            get
            {
                return new Palette(System.Drawing.Image.FromStream(typeof(Palette).Assembly.GetManifestResourceStream("Claw.Palettes.Color.bmp")));
            }
        }

        public Palette(byte Size)
        {
            Colors = new ushort[Size];
        }

        public Palette(System.Drawing.Image PaletteImage)
        {
            var bmp = new Bitmap(PaletteImage);
            Colors = new ushort[bmp.Width * bmp.Height];

            for (int x = 0; x < bmp.Width; x++) {
                for (int y = 0; y < bmp.Height; y++) {
                    Color clr = bmp.GetPixel(x, y);
                    byte red = (byte)(clr.R >> (8 - COLOR_DEPTH_R));
                    byte green = (byte)(clr.G >> (8 - COLOR_DEPTH_G));
                    byte blue = (byte)(clr.B >> (8 - COLOR_DEPTH_B));

                    Colors[y * bmp.Width + x] = (ushort)((red << (COLOR_DEPTH_G + COLOR_DEPTH_B)) | (green << COLOR_DEPTH_B) | blue);
                }
            }
        }

        /// <summary>
        /// Returns the color at a specific index
        /// </summary>
        /// <param name="Index">Palette index</param>
        /// <returns>Color at the specified index</returns>
        public Color GetColorAt(byte Index)
        {
            ushort clr = Colors[(int)Index];
            byte red, green, blue;

            red = (byte)(clr >> (COLOR_DEPTH_G + COLOR_DEPTH_B));
            green = (byte)((clr >> COLOR_DEPTH_B) & (0xFF >> (8 - COLOR_DEPTH_G)));
            blue = (byte)(clr & (0xFF >> (8 - COLOR_DEPTH_B)));

            red = (byte)(red << (8 - COLOR_DEPTH_R));
            green = (byte)(green << (8 - COLOR_DEPTH_G));
            blue = (byte)(blue << (8 - COLOR_DEPTH_B));

            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// Finds the closest palette entry for the specified RGB color
        /// </summary>
        /// <param name="Color">Search color</param>
        /// <returns>Index of the closest matching color entry</returns>
        public byte FindClosestEntry(Color Color)
        {
            return FindClosestEntry(Color, this);
        }

        /// <summary>
        /// Searches for the closest matching palette entry for the RGB color
        /// </summary>
        /// <param name="Color">Input color</param>
        /// <param name="Palette">Palette to be used</param>
        /// <returns></returns>
        public static byte FindClosestEntry(Color Color, Palette Palette)
        {
            var searchColor = new CIELab(Color);
            byte closestPaletteEntry = 0;
            double closestDeltaE = 0d;

            for (uint i = 0; i < Palette.Size; i++) {
                Color rgb = Palette.GetColorAt((byte)i);
                CIELab color = new CIELab(rgb);
                double deltaE = searchColor.CalculateDeltaE(color);

                if (Math.Abs(deltaE) < Math.Abs(closestDeltaE) || i == 0) {
                    closestPaletteEntry = (byte)i;
                    closestDeltaE = deltaE;
                }
            }

            return closestPaletteEntry;
        }
    }
}
