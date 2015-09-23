using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Claw.Imaging.Colorspaces;

namespace Claw.Imaging.Palettes
{
    public class TinyPalette : IPalette
    {
        private RGB565[] entries;

        public byte Size
        {
            get { return 16; }
        }

        public TinyPalette()
        {
            entries = new RGB565[16];
        }

        public Colorspaces.RGB565 this[byte Index]
        {
            get
            {
                return entries[(byte)(Index & 0x0F)];
            }

            set
            {
                entries[(byte)(Index & 0x0F)] = value;
            }
        }

        public byte FindClosestEntry(System.Drawing.Color Color)
        {
            var searchColor = new CIELab(Color);
            byte closestPaletteEntry = 0;
            double closestDeltaE = 0d;

            for (int i = 0; i < 16; i++) {
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
