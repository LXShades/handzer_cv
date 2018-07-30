using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System;

namespace ImageAnalysis.Images.Filters
{
    public enum DifferenceType
    {
        AverageDifference,
        MaxDifference,
        MinDifference,
    };

    public class FilterDifference : Filter
    {
        /// <summary>
        /// Square radius of pixels to be compared around each pixel
        /// </summary>
        public int PixelRadius = 1;
        
        /// <summary>
        /// The type of difference to be assigned to each pixel
        /// </summary>
        public DifferenceType DifferenceType = DifferenceType.MaxDifference;

        public string FilterName { get { return "Difference"; } }

        public void Apply(ref System.Drawing.Bitmap bitmap)
        {
            Bitmap outputBitmap = new Bitmap(bitmap);

            BitmapData imageData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData outputImageData = outputBitmap.LockBits(new Rectangle(0, 0, outputBitmap.Width, outputBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                // Welcome back to unsafe-land, home of C++ and myself
                uint* pixels = (uint*)imageData.Scan0.ToPointer();
                uint* outputPixels = (uint*)outputImageData.Scan0.ToPointer();

                for (int baseY = 0; baseY < bitmap.Height - PixelRadius; ++baseY)
                {
                    uint* pixelRow = &pixels[baseY * imageData.Stride / 4];
                    byte* pixelBs = (byte*)pixelRow, pixelGs = (byte*)&pixelRow[1], pixelRs = (byte*)&pixelRow[2];

                    for (int baseX = 0; baseX < bitmap.Width - PixelRadius; ++baseX)
                    {
                        byte curR = pixelRs[baseX << 2], curG = pixelGs[baseX << 2], curB = pixelBs[baseX << 2];
                        int rMin = 255, gMin = 255, bMin = 255;
                        int rMax = 0, gMax = 0, bMax = 0;

                        // Search the radius around this pixel and track the difference
                        for (int y = 0; y <= PixelRadius; ++y)
                        {
                            byte* subBs = &pixelBs[(y * imageData.Stride) + (baseX << 2)], subGs = &subBs[1], subRs = &subBs[2];

                            for (int x = 0; x <= PixelRadius; ++x)
                            {
                                // Compare red
                                // TODO is there a cleaner way to do this?
                                if (subRs[x << 2] < rMin)
                                    rMin = subRs[x << 2];
                                if (subGs[x << 2] < gMin)
                                    gMin = subGs[x << 2];
                                if (subBs[x << 2] < bMin)
                                    bMin = subBs[x << 2];
                                if (subRs[x << 2] > rMax)
                                    rMax = subRs[x << 2];
                                if (subGs[x << 2] > gMax)
                                    gMax = subGs[x << 2];
                                if (subBs[x << 2] > bMax)
                                    bMax = subBs[x << 2];
                            }
                        }

                        // Write the difference to the output
                        outputPixels[baseY * outputImageData.Stride / 4 + baseX] = 0xFF000000 | (uint)((rMax - rMin) << 16 | (gMax - gMin) << 8 | (bMax - bMin));
                        // or the maximum difference of all colour channels..
                        byte maxDiff = ImageMath.Max((byte)(rMax - rMin), (byte)(gMax - gMin), (byte)(bMax - bMin));
                        outputPixels[baseY * outputImageData.Stride / 4 + baseX] = 0xFF000000 | (uint)(maxDiff << 16 | maxDiff << 8 | maxDiff);
                    }
                }
            }

            // Release resources
            bitmap.UnlockBits(imageData);
            outputBitmap.UnlockBits(outputImageData);

            // Copy output data to input data
            bitmap = new Bitmap(outputBitmap); // TODO there is a better way to do this...right?
        }
    }
}
