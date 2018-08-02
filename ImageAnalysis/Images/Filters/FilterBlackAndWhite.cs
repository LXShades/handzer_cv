using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ImageAnalysis.Analysis.Highlighters;

namespace ImageAnalysis.Images.Filters
{
    public class FilterBlackAndWhite : Filter
    {
        public string FilterName { get { return "Black and White"; } }

        public void Apply(ref Bitmap bitmap, out Highlighter[] highlighters)
        {
            // Scan through all the pixels of the bitmap
            System.Drawing.Imaging.BitmapData imageData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            unsafe
            {
                // Welcome back to unsafe-land, home of C++ and myself
                uint* pixels = (uint*)imageData.Scan0.ToPointer();

                for (int y = 0; y < bitmap.Height; ++y)
                {
                    uint* pixelRow = &pixels[y * imageData.Stride / 4];
                    byte* pixelRowB = (byte*)pixelRow, pixelRowG = &pixelRowB[1], pixelRowR = &pixelRowB[2]; // Split pixels into colour components

                    for (int x = 0; x < bitmap.Width; ++x)
                    {
                        // Use average of colours to get the black and white
                        byte average = (byte)((pixelRowR[x<<2] + pixelRowG[x << 2] + pixelRowB[x << 2]) / 3);
                        pixelRow[x] = (pixelRow[x] & 0xFF000000) | (uint)((average) | (average << 8) | (average << 16));
                    }
                }
            }

            // Release resources
            bitmap.UnlockBits(imageData);

            // No highlighters to return
            highlighters = null;
        }
    }
}
