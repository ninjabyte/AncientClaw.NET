using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Claw.Imaging.Palettes;
using Claw.Imaging.Colorspaces;

namespace Claw.Imaging.Image
{
    public class PaletteImage8 : IImage
    {
        public uint Width { get; private set; }

        public uint Height { get; private set; }

        public IPalette Palette { get; set; }

        public PaletteFormat Format
        {
            get { return PaletteFormat.Palette8bit; }
        }

        public byte[] Data { get; private set; }

        public PaletteImage8(uint Width, uint Height, IPalette Palette)
        {
            this.Width = Width;
            this.Height = Height;
            this.Palette = Palette;
            Data = new byte[Width * Height];
        }

        public byte this[uint Column, uint Row]
        {
            get
            {
                return Data[Row * Width + Column];
            }
            set
            {
                Data[Row * Width + Column] = value;
            }
        }

        public Colorspaces.RGB565 GetColorAt(uint Column, uint Row)
        {
            return Palette[Data[Row * Width + Column]];
        }

        public MemoryBitmap ToBitmap()
        {
            var mbmp = new MemoryBitmap(Width, Height);
            mbmp.Lock();

            for (uint y = 0; y < Height; y++) {
                for (uint x = 0; x < Width; x++) {
                    byte val = Data[y * Width + x];
                    var color = new RGB888(Palette[val]);
                    mbmp[x, y] = color;
                }
            }

            return mbmp;
        }
    }
}
