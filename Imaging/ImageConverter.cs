using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;


namespace Claw.Imaging
{
    public class ImageConverter
    {
        private BackgroundWorker worker;
        private ConversionArgs currentState;
        private DateTime startTime;

        public bool IsBusy { get { return worker.IsBusy; } }

        public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e);
        public delegate void ProgressCompletedEventHandler(object sender, ProgressCompletedEventArgs e);

        public event ProgressChangedEventHandler ProgressChanged;
        public event ProgressCompletedEventHandler ProgressCompleted;

        public ImageConverter()
        {
            worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = false };
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs args)
        {
            var conversionArgs = (ConversionArgs)args.Argument;

            if (conversionArgs.TargetFormat == Image.PixelFormat.Monochrome1bit) {
                args.Result = ConvertToMonochrome1bpp(conversionArgs.SourceImage, worker);
            } else if (conversionArgs.TargetFormat == Image.PixelFormat.Grayscale4bit) {
                args.Result = ConvertToGrayscale4bpp(conversionArgs.SourceImage, worker);
            } else if (conversionArgs.TargetFormat == Image.PixelFormat.Palette8bit) {
                args.Result = ConvertToPalette8bpp(conversionArgs.SourceImage, conversionArgs.Palette, worker);
            } else if (conversionArgs.TargetFormat == Image.PixelFormat.RGB16bit) {
                args.Result = ConvertToRGB16bpp(conversionArgs.SourceImage, worker);
            } else
                args.Result = null;
        }

        private void worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs args)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, new ProgressChangedEventArgs((uint)args.ProgressPercentage, (uint)currentState.SourceImage.Height, DateTime.Now - startTime));
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            if (args.Result != null) {
                if (ProgressCompleted != null)
                    ProgressCompleted(this, new ProgressCompletedEventArgs((Image)args.Result, startTime - DateTime.Now));
            } else {
                if (ProgressCompleted != null)
                    ProgressCompleted(this, new ProgressCompletedEventArgs());
            }
        }

        public void ConvertToMonochrome1bppAsync(System.Drawing.Image Image, Palette Palette)
        {
            if (worker.IsBusy)
                throw new InvalidOperationException("This instance is busy converting!");
            if (Image == null)
                throw new ArgumentNullException("Image");

            var args = new ConversionArgs(Imaging.Image.PixelFormat.Monochrome1bit, Image);
            currentState = args;
            startTime = DateTime.Now;
            worker.RunWorkerAsync(args);
        }

        public void ConvertToGrayscale4bppAsync(System.Drawing.Image Image)
        {
            if (worker.IsBusy)
                throw new InvalidOperationException("This instance is busy converting!");
            if (Image == null)
                throw new ArgumentNullException("Image");

            var args = new ConversionArgs(Imaging.Image.PixelFormat.Grayscale4bit, Image);
            currentState = args;
            startTime = DateTime.Now;
            worker.RunWorkerAsync(args);
        }

        public void ConvertToPalette8bppAsync(System.Drawing.Image Image, Palette Palette)
        {
            if (worker.IsBusy)
                throw new InvalidOperationException("This instance is busy converting!");
            if (Image == null)
                throw new ArgumentNullException("Image");
            if (Palette == null)
                throw new ArgumentNullException("Palette");

            var args = new ConversionArgs(Image, Palette);
            currentState = args;
            startTime = DateTime.Now;
            worker.RunWorkerAsync(args);
        }

        public void ConvertToRGB16bppAsync(System.Drawing.Image Image)
        {
            if (worker.IsBusy)
                throw new InvalidOperationException("This instance is busy converting!");
            if (Image == null)
                throw new ArgumentNullException("Image");

            var args = new ConversionArgs(Imaging.Image.PixelFormat.RGB16bit, Image);
            currentState = args;
            startTime = DateTime.Now;
            worker.RunWorkerAsync(args);
        }

        private static Image ConvertToMonochrome1bpp(System.Drawing.Image Image, BackgroundWorker Worker)
        {
            // Allocate size for the image (1 byte per 8 pixel)
            var bytes = new byte[(Image.Width / 8) * Image.Height];
            // Create new bitmap
            var bmp = new Bitmap(Image);

            for (int y = 0; y < Image.Height; y++) {
                for (int x = 0; x < (Image.Width / 8); x++) {
                    byte pixelField = 0;

                    // Monochromize the image by averaging the luminance and loop through the 8 pixels per byte
                    for (int b = 0; b < 8; b++) {
                        Color pixelColor = bmp.GetPixel(x + b, y);
                        if (((pixelColor.R + pixelColor.G + pixelColor.B) / 2) > 127)
                            pixelField |= (byte)(1 << b);
                    }

                    // Copy the pixel field to the buffer
                    bytes[y * (Image.Height / 8) + x] = pixelField;

                    if (Worker != null)
                        Worker.ReportProgress(y * (Image.Height / 8) + x);
                }
            }

            // Return the final image
            return new Image(bytes, Imaging.Image.PixelFormat.Monochrome1bit, (uint)Image.Width, (uint)Image.Height);
        }

        public static Image ConvertToMonochrome1bpp(System.Drawing.Image Image)
        {
            return ConvertToMonochrome1bpp(Image, null);
        }

        private static Image ConvertToGrayscale4bpp(System.Drawing.Image Image, BackgroundWorker Worker)
        {
            return null;
        }

        public static Image ConvertToGrayscale4bpp(System.Drawing.Image Image)
        {
            return ConvertToGrayscale4bpp(Image, null);
        }

        private static Image ConvertToPalette8bpp(System.Drawing.Image Image, Palette Palette, BackgroundWorker Worker)
        {
            // Allocate memory for the target image
            var bytes = new byte[Image.Width * Image.Height];
            // Create a new bitmap copy of the image
            var bmp = new Bitmap(Image);

            // Loop through the pixels and find the closest approximation in the palette
            for (int y = 0; y < Image.Height; y++) {
                for (int x = 0; x < Image.Width; x++) {
                    bytes[y * Image.Width + x] = Palette.FindClosestEntry(bmp.GetPixel(x, y));
                }

                if (Worker != null)
                    Worker.ReportProgress(y);
            }

            // Clean up
            bmp.Dispose();

            // Create and return new image
            return new Image(bytes, Imaging.Image.PixelFormat.Palette8bit, (uint)Image.Width, (uint)Image.Height);
        }

        public static Image ConvertToPalette8bpp(System.Drawing.Image Image, Palette Palette)
        {
            return ConvertToPalette8bpp(Image, Palette, null);
        }

        private static Image ConvertToRGB16bpp(System.Drawing.Image Image, BackgroundWorker Worker)
        {
            return null;
        }

        public static Image ConvertToRGB16bpp(System.Drawing.Image Image)
        {
            return ConvertToRGB16bpp(Image, null);
        }

        private struct ConversionArgs
        {
            public Image.PixelFormat TargetFormat;
            public System.Drawing.Image SourceImage;
            public Palette Palette;

            public ConversionArgs(Image.PixelFormat TargetFormat, System.Drawing.Image SourceImage, Palette Palette)
            {
                this.TargetFormat = TargetFormat;
                this.SourceImage = SourceImage;
                this.Palette = Palette;
            }

            public ConversionArgs(Image.PixelFormat TargetFormat, System.Drawing.Image SourceImage)
            {
                this.TargetFormat = TargetFormat;
                this.SourceImage = SourceImage;
                this.Palette = null;
            }

            public ConversionArgs(System.Drawing.Image SourceImage, Palette Palette)
            {
                this.TargetFormat = Image.PixelFormat.Palette8bit;
                this.SourceImage = SourceImage;
                this.Palette = Palette;
            }
        }

        public class ProgressCompletedEventArgs : EventArgs
        {
            public Image Result { get; private set; }
            public bool Success { get; private set; }
            public TimeSpan TotalDuration { get; private set; }

            public ProgressCompletedEventArgs()
            {
                Success = false;
            }

            public ProgressCompletedEventArgs(Image Result, TimeSpan TotalDuration)
            {
                Success = true;
                this.Result = Result;
                this.TotalDuration = TotalDuration;
            }
        }

        public class ProgressChangedEventArgs : EventArgs
        {
            public uint ProcessedRows { get; private set; }
            public uint TotalRows { get; private set; }
            public TimeSpan Runtime { get; private set; }

            public ProgressChangedEventArgs(uint ProcessedRows, uint TotalRows, TimeSpan Runtime)
            {
                this.ProcessedRows = ProcessedRows;
                this.TotalRows = TotalRows;
                this.Runtime = Runtime;
            }
        }
    }
}
