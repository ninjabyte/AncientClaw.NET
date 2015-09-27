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
    }
}
