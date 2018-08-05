using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageAnalysis.Images
{
    /// <summary>
    /// Helper for reading an image's pixels without needing to manage locks/unlocks
    /// Don't use more than one of these at once on the same image
    /// TODO error handling
    /// TODO should it keep the image exclusively locked during use??
    /// </summary>
    unsafe class ImageReader: IDisposable
    {
        /// <summary>
        ///  Array of pixel row arrays
        /// </summary>
        public uint*[] PixelRows;

        /// <summary>
        /// Dimensions of the image
        /// </summary>
        public int Width { get; }
        public int Height { get; }

        /// <summary>
        /// Creates an image reader from the given bitmap
        /// </summary>
        /// <param name="_bitmap">The bitmap to be locked and pixels read</param>
        public ImageReader(ref Bitmap _bitmap)
        {
            // Setup vars
            bitmap = _bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;

            // Lock and copy the bitmap's pixels into PixelRows
            bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            uint* imagePixels = (uint*)bitmapData.Scan0.ToPointer();
            PixelRows = new uint*[bitmapData.Height];

            for (int y = 0; y < bitmapData.Height; ++y)
            {
                PixelRows[y] = &imagePixels[y * bitmapData.Stride / 4];
            }
        }

        private Bitmap bitmap;
        private BitmapData bitmapData;

        protected virtual void Dispose(bool disposing)
        {
            if (bitmapData != null)
            {
                // Unlock the bitmap
                bitmap.UnlockBits(bitmapData);
                bitmapData = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImageReader()
        {
            Dispose(false);
        }
    }
}
