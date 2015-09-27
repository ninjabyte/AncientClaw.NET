using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Claw.Imaging.Palette;
using Claw.Imaging.Colorspaces;
using Claw.Imaging.Image;

namespace Claw.Imaging
{
    public static class ColorConverter
    {
        public static void ConvertImage(System.Drawing.Image Image, IImage TargetImage, bool DitheringEnabled)
        {
            var sourceBitmap = new MemoryBitmap(Image);
            sourceBitmap.Lock();

            DateTime startTime = DateTime.Now;

            for (uint y = 0; y < sourceBitmap.Height; y++) {
                for (uint x = 0; x < sourceBitmap.Width; x++) {
                    byte old_r, old_g, old_b, old_a, new_r, new_g, new_b;

                    sourceBitmap.GetRGBA(x, y, out old_r, out old_g, out old_b, out old_a);
                    byte paletteColorIndex = FindClosestPaletteEntry(old_r, old_g, old_b, TargetImage.Palette);

                    TargetImage[x, y] = paletteColorIndex;

                    if (DitheringEnabled) {
                        byte qerr_r, qerr_g, qerr_b, res_r, res_g, res_b;
                        RGB888.UnpackValueFromRGB565(TargetImage.Palette[paletteColorIndex], out new_r, out new_g, out new_b);

                        RGB888.Subtract(old_r, old_g, old_b, new_r, new_g, new_b, out qerr_r, out qerr_g, out qerr_b);

                        if (qerr_r > 0 && qerr_g > 0 && qerr_b > 0) {
                            RGB888.Multiply(qerr_r, qerr_g, qerr_b, 7 / 16f, out res_r, out res_g, out res_b);
                            AddPixel(sourceBitmap, x + 1, y, res_r, res_g, res_b);

                            RGB888.Multiply(qerr_r, qerr_g, qerr_b, 3 / 16f, out res_r, out res_g, out res_b);
                            AddPixel(sourceBitmap, x - 1, y + 1, res_r, res_g, res_b);

                            RGB888.Multiply(qerr_r, qerr_g, qerr_b, 5 / 16f, out res_r, out res_g, out res_b);
                            AddPixel(sourceBitmap, x, y + 1, res_r, res_g, res_b);

                            RGB888.Multiply(qerr_r, qerr_g, qerr_b, 1 / 16f, out res_r, out res_g, out res_b);
                            AddPixel(sourceBitmap, x + 1, y + 1, res_r, res_g, res_b);
                        }
                    }
                }
            }

            TimeSpan totalTime = DateTime.Now - startTime;

            Console.WriteLine(totalTime.TotalSeconds.ToString());

            sourceBitmap.Unlock();
            sourceBitmap.Dispose();
        }

        private static void AddPixel(MemoryBitmap Bitmap, uint X, uint Y, RGB888 Color)
        {
            if (Bitmap == null)
                throw new ArgumentNullException("Bitmap");
            if (Color == null)
                throw new ArgumentNullException("Color");
            if (X >= Bitmap.Width || Y >= Bitmap.Height)
                return;

            Bitmap[X, Y] = Bitmap[X, Y].Add(Color);
        }

        private static void AddPixel(MemoryBitmap Bitmap, uint X, uint Y, byte R, byte G, byte B)
        {
            if (Bitmap == null)
                throw new ArgumentNullException("Bitmap");
            if (X >= Bitmap.Width || Y >= Bitmap.Height)
                return;

            Bitmap[X, Y] = Bitmap[X, Y].Add(R, G, B);
        }

        private static byte FindClosestPaletteEntry(byte R, byte G, byte B, IPalette Palette)
        {
            int closestDst = int.MaxValue, dst;
            byte palColor = 0;

            for (byte i = 0; i < Palette.Size; i++) {
                dst = RGB888.SquareDifference(R, G, B, Palette[i].R, Palette[i].G, Palette[i].B);

                if (dst < closestDst) {
                    closestDst = dst;
                    palColor = i;

                    if (closestDst == 0)
                        break;
                }
            }

            return palColor;
        }

        private static byte FindClosestPaletteEntry(RGB888 Color, IPalette Palette)
        {
            int closestDst = int.MaxValue, dst;
            byte palColor = 0;

            for (byte i = 0; i < Palette.Size; i++) {
                dst = Color.SquareDifference(new RGB888(Palette[i]));

                if (dst < closestDst) {
                    closestDst = dst;
                    palColor = i;
                }
            }

            return palColor;
        }
    }
}
