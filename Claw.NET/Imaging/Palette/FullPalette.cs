using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Claw.Imaging.Colorspaces;
using System.Resources;

namespace Claw.Imaging.Palette
{
    public class FullPalette : IPalette
    {
        private RGB565[] entries;

        public byte Size { get { return (byte)entries.Length; } }

        public static FullPalette DefaultColors
        {
            get
            {
                return new FullPalette(System.Drawing.Image.FromStream(typeof(FullPalette).Assembly.GetManifestResourceStream("Claw.Imaging.Palette.Palette.png")));
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
                for (int y = 0; y < bmp.Height; y++) {
                    entries[y * bmp.Width + x] = new RGB565(bmp.GetPixel(x, y));
                }
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
    }
}
