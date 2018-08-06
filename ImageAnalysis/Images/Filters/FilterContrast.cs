using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using ImageAnalysis.Analysis.Highlighters;

namespace ImageAnalysis.Images.Filters
{
    public class FilterContrast : Filter
    {
        public override string FilterName { get { return "Contrast"; } }

        public override void Apply(ref Bitmap bitmap, out Highlighter[] highlighters)
        {
            // Decide the factor to scale the colours
            int desiredBrightness = 0;
            int[] samples = GetBrightnessSamples(bitmap);

            desiredBrightness = GetPercentileBrightness(90, samples);

            // Scan through all the pixels of the bitmap
            BitmapData imageData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                // Welcome back to unsafe-land, home of C++ and myself
                uint* pixels = (uint*)imageData.Scan0.ToPointer();

                for (uint* pixel = (uint*)imageData.Scan0.ToPointer(), lastPixel = &pixel[imageData.Height * imageData.Stride / 4];
                           pixel < lastPixel;
                           ++pixel)
                {
                    if ((*pixel & 0xFF) >= desiredBrightness || (*pixel >> 8 & 0xFF) >= desiredBrightness || (*pixel >> 16 & 0xFF) >= desiredBrightness)
                    {
                        *pixel = 0xFFFFFFFF;
                    }
                    else
                    {
                        *pixel = 0xFF000000;
                    }
                }
            }

            // Release resources
            bitmap.UnlockBits(imageData);

            // No highlighters to return
            highlighters = null;
        }

        /// <summary>
        /// Returns the brightness of each pixel, storing their quantity of incidence into a 256-item array.
        /// Each index holds the number of pixels whose brightness was the index.
        /// Useful for quickly calculating median, mode, etc
        /// </summary>
        /// <param name="bitmap">The input bitmap</param>
        /// <returns>The array of quantities of indicence per brightness level, in pixels</returns>
        private int[] GetBrightnessSamples(Bitmap bitmap)
        {
            // Begin with an array of 0 for each brightness value
            int[] outputArray = new int[256];

            Array.Clear(outputArray, 0, 256);

            // Scan through all the pixels of the bitmap
            BitmapData imageData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                // For each pixel of 'brightness', increment outputArray[brightness]
                for (uint* pixel = (uint*)imageData.Scan0.ToPointer(), lastPixel = &pixel[imageData.Height * imageData.Stride / 4]; 
                           pixel < lastPixel;
                           ++pixel)
                {
                    int brightness = ImageMath.Max((byte)(*pixel >> 16), (byte)(*pixel >> 8), (byte)*pixel);

                    ++outputArray[brightness];
                }
            }

            bitmap.UnlockBits(imageData);

            return outputArray;
        }

        /// <summary>
        /// Bad terminology aside (TODO learn stats), this returns the brightness of a pixel at a certain percentile
        /// </summary>
        /// <param name="percentile">Value between 0-100 representing the ffff</param>
        /// <param name="samples">A 256-item array containing the total number of pixels of each brightness index</param>
        /// <returns>The brightness of the pixels at this percentile</returns>
        private int GetPercentileBrightness(int percentile, int[] samples)
        {
            int listLength = 256 > samples.Length ? samples.Length : 256;

            // Count the total incidences first
            int numTotalPixels = 0;

            for (int i = 0; i < listLength; ++i)
            {
                numTotalPixels += samples[i];
            }

            // Find the brightness at this percentile
            int expectedNum = numTotalPixels * percentile / 100;
            int currentNum = 0;

            for (int i = 0; i < listLength; ++i)
            {
                currentNum += samples[i];

                if (currentNum >= expectedNum)
                {
                    return i;
                }
            }

            // Error??
            return 0;
        }
    }
}