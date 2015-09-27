using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Claw.Imaging.Palette;
using Claw.Imaging.Colorspaces;

namespace Claw.Imaging.Image
{
    public interface IImage
    {
        uint Width { get; }
        uint Height { get; }
        IPalette Palette { get; set; }
        PaletteFormat Format { get; }
        byte[] Data { get; }
        byte this[uint Column, uint Row]
        {
            get;
            set;
        }
        RGB565 GetColorAt(uint Column, uint Row);
        MemoryBitmap ToBitmap();
    }
}
