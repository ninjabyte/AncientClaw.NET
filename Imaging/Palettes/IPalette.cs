using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Claw.Imaging.Colorspaces;

namespace Claw.Imaging.Palettes
{
    public interface IPalette
    {
        byte Size { get; }
        RGB565 this[byte Index] { get; set; }
        byte FindClosestEntry(Color Color);
    }
}
