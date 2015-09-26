using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Claw.Imaging
{
    public class ColorConverter
    {
        int[] pal, src, dst;
        int width, height;

        /* public ColorConverter(String ifile)
         {
             BufferedImage palImg = ImageIO.read(ColorConverter.class.getResource("/assets/palette.png"));
             BufferedImage srcImg = ImageIO.read(new File(ifile));
		
             width = srcImg.getWidth();
             height = srcImg.getHeight();
		
             pal = toPixelArray(palImg);
             src = toPixelArray(srcImg);
             dst = new int[src.length]; // the alpha component of the dst image is used to store the color's index on the palette.
         }*/

        public ColorConverter(System.Drawing.Image Image)
        {
            MemoryBitmap mbmp = new MemoryBitmap(Image);
            mbmp.Lock();
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

        private void addPix(int[] img, int x, int y, int c)
        {
            if (x >= width || y >= height)
                return;
            img[y * width + x] = ColorUtil.add(img[y * width + x], c);
        }

        private int getClosestColor(int color)
        {
            int closestDst = int.MaxValue, col = 0, dst;

            for (int i = 0; i < pal.Length; i++)
                if ((dst = ColorUtil.getDifferenceSq(color, pal[i])) < closestDst) {
                    closestDst = dst;
                    col = i;
                }

            return ColorUtil.setAlpha(pal[col], col);
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
