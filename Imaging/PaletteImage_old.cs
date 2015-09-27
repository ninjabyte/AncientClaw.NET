using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Claw.Imaging.Colorspaces;
using Claw.Imaging.Palettes;

namespace Claw.Imaging
{
    public class PaletteImage_old
    {
        public PixelFormat Format { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public byte[] Data { get; private set; } 

        public PaletteImage_old(byte[] Bytes, PixelFormat Format, uint Width, uint Height)
        {
            Data = Bytes;
            this.Format = Format;
            this.Width = Width;
            this.Height = Height;
        }

        public PaletteImage_old FromStream(Stream Stream)
        {
            return null;
        }

        public void ToStream(Stream Stream)
        {

        }

        public System.Drawing.Image ToImage(FullPalette Palette)
        {
            var bmp = new Bitmap((int)Width, (int)Height);

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    bmp.SetPixel(x, y, Palette[Data[y * Width + x]].Color);
                }
            }

            return bmp;
        }

        public enum PixelFormat : byte
        {
            Monochrome1bit,
            Palette4bit,
            Palette8bit,
            RGB16bit
        }
    }
}
