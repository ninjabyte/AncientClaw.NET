using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Claw.Imaging.Palettes;
using Claw.Imaging.Colorspaces;

namespace Claw.Imaging
{
    public interface IImage
    {
        uint Width { get; }
        uint Height { set; }
        IPalette Palette { get; set; }
        PixelFormat Format { get; }
        byte[] Data { get; }
        byte this[uint Column, uint Row]
        {
            get;
            set;
        }
        RGB565 GetColorAt(uint Column, uint Row);
        MemoryBitmap ToMemoryBitmap();
    }
}
