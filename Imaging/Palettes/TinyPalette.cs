using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Claw.Imaging.Colorspaces;

namespace Claw.Imaging.Palettes
{
    public class TinyPalette : IPalette
    {
        private RGB565[] rgb_entries;
        private CIELab[] lab_entries;

        public byte Size
        {
            get { return 16; }
        }

        public TinyPalette()
        {
            rgb_entries = new RGB565[16];
            lab_entries = new CIELab[16];
        }

        public Colorspaces.RGB565 this[byte Index]
        {
            get
            {
                return rgb_entries[(byte)(Index & 0x0F)];
            }

            set
            {
                rgb_entries[(byte)(Index & 0x0F)] = value;
                lab_entries[(byte)(Index & 0x0F)] = new CIELab(value.Color);
            }
        }

        public byte FindClosestEntry(System.Drawing.Color Color)
        {
            var searchColor = new CIELab(Color);
            byte closestPaletteEntry = 0;
            double closestDeltaE = 0d;

            for (int i = 0; i < 16; i++) {
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
