using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Claw.Imaging.Colorspaces;
using System.Resources;

namespace Claw.Imaging.Palettes
{
    public class FullPalette : IPalette
    {
        private RGB565[] entries;

        public byte Size { get { return (byte)entries.Length; } }

        public static IPalette Default
        {
            get
            {
                return new FullPalette(System.Drawing.Image.FromStream(typeof(FullPalette).Assembly.GetManifestResourceStream("Claw.Imaging.Palettes.Color.bmp")));
            }
        }

        public FullPalette(byte Size)
        {
            entries = new RGB565[Size];
        }

        public FullPalette(System.Drawing.Image PaletteImage)
        {
            if (PaletteImage == null)
                throw new ArgumentNullException("PaletteImage");
            if ((PaletteImage.Width * PaletteImage.Height) > 256)
                throw new OverflowException("Too many palette entries! Maximum is 256.");

            var bmp = new Bitmap(PaletteImage);
            entries = new RGB565[PaletteImage.Width * PaletteImage.Height];

            for (int x = 0; x < bmp.Width; x++) {
                for (int y = 0; y < bmp.Height; y++)
                    entries[y * bmp.Width + x] = new RGB565(bmp.GetPixel(x, y));
            }
        }

        public RGB565 this[byte Index]
        {
            get
            {
                return entries[Index];
            }

            set
            {
                entries[Index] = value;
            }
        }

        /// <summary>
        /// Finds the closest palette entry for the specified RGB color
        /// </summary>
        /// <param name="Color">Search color</param>
        /// <returns>Index of the closest matching color entry</returns>
        public byte FindClosestEntry(Color Color)
        {
            var searchColor = new CIELab(Color);
            byte closestPaletteEntry = 0;
            double closestDeltaE = 0d;

            for (int i = 0; i < Size; i++) {
                CIELab color = new CIELab(this[(byte)i].Color);
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
