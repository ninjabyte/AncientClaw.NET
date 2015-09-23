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
        private RGB565[] rgb_entries;
        private CIELab[] lab_entries;

        public byte Size { get { return (byte)rgb_entries.Length; } }

        public static FullPalette DefaultColors
        {
            get
            {
                return new FullPalette(System.Drawing.Image.FromStream(typeof(FullPalette).Assembly.GetManifestResourceStream("Claw.Imaging.Palettes.Color.bmp")));
            }
        }

        public FullPalette(byte Size)
        {
            rgb_entries = new RGB565[Size];
            lab_entries = new CIELab[Size];
        }

        public FullPalette(System.Drawing.Image PaletteImage)
        {
            if (PaletteImage == null)
                throw new ArgumentNullException("PaletteImage");
            if ((PaletteImage.Width * PaletteImage.Height) > 256)
                throw new OverflowException("Too many palette entries! Maximum is 256.");

            var bmp = new Bitmap(PaletteImage);
            rgb_entries = new RGB565[PaletteImage.Width * PaletteImage.Height];
            lab_entries = new CIELab[PaletteImage.Width * PaletteImage.Height];

            for (int x = 0; x < bmp.Width; x++) {
                for (int y = 0; y < bmp.Height; y++) {
                    rgb_entries[y * bmp.Width + x] = new RGB565(bmp.GetPixel(x, y));
                    lab_entries[y * bmp.Width + x] = new CIELab(bmp.GetPixel(x, y));
                }
            }
        }

        public RGB565 this[byte Index]
        {
            get
            {
                return rgb_entries[Index];
            }

            set
            {
                rgb_entries[Index] = value;
                lab_entries[Index] = new CIELab(value.Color);
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
                double deltaE = searchColor.CalculateDeltaE(lab_entries[i]);

                if (Math.Abs(deltaE) < Math.Abs(closestDeltaE) || i == 0) {
                    closestPaletteEntry = (byte)i;
                    closestDeltaE = deltaE;
                }
            }

            return closestPaletteEntry;
        }
    }
}
