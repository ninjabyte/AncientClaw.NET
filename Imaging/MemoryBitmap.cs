using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Claw.Imaging.Colorspaces;

namespace Claw.Imaging
{
    public class MemoryBitmap : IDisposable
    {
        public Bitmap BitmapImage { get; private set; }
        public uint Height { get { return (uint)BitmapImage.Height; } }
        public uint Width { get { return (uint)BitmapImage.Width; } }
        private BitmapData BmpData { get; set; }
        private byte[] PixelData;

        public MemoryBitmap(System.Drawing.Image SourceImage)
        {
            this.BitmapImage = new Bitmap(SourceImage);
        }

        public MemoryBitmap(uint Width, uint Height)
        {
            this.BitmapImage = new Bitmap((int)Width, (int)Height, PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// Required to edit the image.
        /// </summary>
        public void Lock()
        {
            BmpData = BitmapImage.LockBits(new Rectangle(0, 0, BitmapImage.Width, BitmapImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            PixelData = new byte[Math.Abs(BmpData.Stride) * BmpData.Height];
            System.Runtime.InteropServices.Marshal.Copy(BmpData.Scan0, PixelData, 0, PixelData.Length);
        }

        /// <summary>
        /// Required to use the image after editing.
        /// </summary>
        public void Unlock()
        {
            System.Runtime.InteropServices.Marshal.Copy(PixelData, 0, BmpData.Scan0, PixelData.Length);
            BitmapImage.UnlockBits(BmpData);
            BmpData = null;
            PixelData = null;
        }

        public byte BytePerPixel { get { return (byte)(Math.Abs(BmpData.Stride) / BmpData.Width); } }

        public uint Length { get { return (uint)(Width * Height * BytePerPixel); } }

        public RGB888 this[uint Pointer]
        {
            get
            {
                return new RGB888(PixelData[Pointer + 0], PixelData[Pointer + 1], PixelData[Pointer + 2], PixelData[Pointer + 3] > 127 ? false : true);
            }

            set
            {
                PixelData[Pointer + 0] = value.R;
                PixelData[Pointer + 1] = value.G;
                PixelData[Pointer + 2] = value.B;
                PixelData[Pointer + 3] = (byte)(value.Transparent ? 0 : 0xFF);
            }
        }

        public RGB888 this[uint Column, uint Row]
        {
            get
            {
                return new RGB888(GetByte(Column, Row, 0), GetByte(Column, Row, 1), GetByte(Column, Row, 2), GetByte(Column, Row, 3) > 127 ? false : true);
            }

            set
            {
                SetByte(Column, Row, 0, value.R);
                SetByte(Column, Row, 1, value.G);
                SetByte(Column, Row, 2, value.B);
                SetByte(Column, Row, 3, (byte)(value.Transparent ? 0 : 0xFF));
            }
        }

        private byte GetByte(uint Column, uint Row, byte Offset)
        {
            return PixelData[Row * Math.Abs(BmpData.Stride) + (Column * 4) + Offset];
        }

        private void SetByte(uint Column, uint Row, byte Offset, byte Value)
        {
            PixelData[Row * Math.Abs(BmpData.Stride) + (Column * 4) + Offset] = Value;
        }

        public void Dispose()
        {
            if(BmpData != null && PixelData != null && BitmapImage != null)
            Unlock();

            BmpData = null;
            PixelData = null;
            BitmapImage = null;
        }
    }
}
