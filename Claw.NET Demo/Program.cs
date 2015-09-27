using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Claw.Imaging;
using Claw.Imaging.Palette;
using Claw.Imaging.Image;

namespace Claw.NET_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Drawing.Image sourceImg = System.Drawing.Image.FromFile("earth.jpg");
            IImage targetImg = new PaletteImage8((uint)sourceImg.Width, (uint)sourceImg.Height, FullPalette.DefaultColors);
            ColorConverter.ConvertImage(sourceImg, targetImg, true);
            
            MemoryBitmap targetBmp = targetImg.ToBitmap();
            targetBmp.Unlock();
            targetBmp.BitmapImage.Save("out.bmp");
        }
    }
}
