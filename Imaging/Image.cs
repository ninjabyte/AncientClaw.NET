﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Claw.Imaging.Colorspaces;

namespace Claw.Imaging
{
    public class Image
    {
        public PixelFormat Format { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public byte[] Data { get; private set; } 

        public Image(byte[] Bytes, PixelFormat Format, uint Width, uint Height)
        {
            Data = Bytes;
            this.Format = Format;
            this.Width = Width;
            this.Height = Height;
        }

        public Image FromStream(Stream Stream)
        {
            return null;
        }

        public void ToStream(Stream Stream)
        {

        }

        public System.Drawing.Image ToImage(Palette Palette)
        {
            var bmp = new Bitmap((int)Width, (int)Height);

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    bmp.SetPixel(x, y, Palette.GetColorAt(Data[y * Width + x]));
                }
            }

            return bmp;
        }

        public enum PixelFormat : byte
        {
            Monochrome1bit,
            Grayscale4bit,
            Palette8bit,
            RGB16bit
        }
    }
}