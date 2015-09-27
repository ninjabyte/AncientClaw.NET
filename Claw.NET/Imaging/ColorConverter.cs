using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Claw.Imaging.Palettes;
using Claw.Imaging.Colorspaces;
using Claw.Imaging.Image;

namespace Claw.Imaging
{
    public static class ColorConverter
    {
        /* public ColorConverter(String ifile)
         {
             BufferedImage palImg = ImageIO.read(ColorConverter.class.getResource("/assets/palette.png"));
             BufferedImage srcImg = ImageIO.read(new File(ifile));
		
             width = srcImg.getWidth();
             height = srcImg.getHeight();
		
             pal = palette = toPixelArray(palImg);
             src = sourceimage = toPixelArray(srcImg);
             dst = destination = new int[src.length]; // the alpha component of the dst image is used to store the color's index on the palette.
         }*/

        public static void ConvertImage(System.Drawing.Image Image, IImage TargetImage)
        {
            var sourceBitmap = new MemoryBitmap(Image);
            sourceBitmap.Lock();

            /*
             * for (int i = 0; i < src.Length; i++) {
              
                int oldpixel = src[i];
                int newpixel = getClosestColor(oldpixel);

                dst[i] = newpixel;

                int qe = ColorUtil.sub(oldpixel, newpixel);

                addPix(src, x + 1, y, ColorUtil.mul(qe, 7 / 16f));
                addPix(src, x - 1, y + 1, ColorUtil.mul(qe, 3 / 16f));
                addPix(src, x, y + 1, ColorUtil.mul(qe, 5 / 16f));
                addPix(src, x + 1, y + 1, ColorUtil.mul(qe, 1 / 16f));
            }
            
            return toImage(dst, width, height);
             * */

            for (uint y = 0; y < sourceBitmap.Height; y++) {
                for (uint x = 0; x < sourceBitmap.Width; x++) {
                    RGB888 oldPixel = sourceBitmap[x, y];
                    byte paletteColor = FindClosestPaletteEntry(oldPixel, TargetImage.Palette);
                    RGB888 newPixel = new RGB888(TargetImage.Palette[paletteColor]);

                    TargetImage[x, y] = paletteColor;

                    //RGB888 qe = oldPixel.Subtract(newPixel);
                }
            }

        }

        private static void AddPixel(MemoryBitmap Bitmap, uint X, uint Y, RGB888 Color)
        {
            if (Bitmap == null)
                throw new ArgumentNullException("Bitmap");
            if (Color == null)
                throw new ArgumentNullException("Color");
            if(X >= Bitmap.Width)
                throw new ArgumentOutOfRangeException("X");
            if (Y >= Bitmap.Height)
                throw new ArgumentOutOfRangeException("Y");

            Bitmap[X, Y] = Bitmap[X, Y].Add(Color);
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

        

        /*

        public Image genImage()
        {
            for (int i = 0; i < src.Length; i++) {
                int x = i % width;
                int y = i / width;
                int oldpixel = src[i];
                int newpixel = getClosestColor(oldpixel);

                dst[i] = newpixel;

                int qe = ColorUtil.sub(oldpixel, newpixel);

                addPix(src, x + 1, y, ColorUtil.mul(qe, 7 / 16f));
                addPix(src, x - 1, y + 1, ColorUtil.mul(qe, 3 / 16f));
                addPix(src, x, y + 1, ColorUtil.mul(qe, 5 / 16f));
                addPix(src, x + 1, y + 1, ColorUtil.mul(qe, 1 / 16f));
            }
            
            return toImage(dst, width, height);
        }

        private int[] toPixelArray(Image img)
        {
            byte[] pixels = ((DataBufferByte)img.getRaster().getDataBuffer()).getData();
            int[] data = new int[img.getWidth() * img.getHeight()];
            bool hasAlpha = img.getAlphaRaster() != null;

            if (hasAlpha)
                for (int i = 0; i < data.length; i++)
                    data[i] = ColorUtil.asColor(pixels[i * 4 + 3] & 0xFF, pixels[i * 4 + 2] & 0xFF, pixels[i * 4 + 1] & 0xFF, 0);
            else
                for (int i = 0; i < pixels.length / 3; i++)
                    data[i] = ColorUtil.asColor(pixels[i * 3 + 2] & 0xFF, pixels[i * 3 + 1] & 0xFF, pixels[i * 3] & 0xFF, 0);

            return data;
        }

        private Image toImage(int[] pixels, int width, int height)
        {
            BufferedImage img = new BufferedImage(width, height, BufferedImage.TYPE_INT_RGB);
            int[] data = ((DataBufferInt)img.getRaster().getDataBuffer()).getData();
            System.arraycopy(pixels, 0, data, 0, pixels.length);
            return img;
        }
         * */
    }
}
