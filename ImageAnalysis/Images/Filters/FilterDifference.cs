﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

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
                    byte* pixelRs = (byte*)pixelRow, pixelGs = (byte*)&pixelRow[1], pixelBs = (byte*)&pixelRow[2];

                    for (int baseX = 0; baseX < bitmap.Width - PixelRadius; ++baseX)
                    {
                        byte curR = pixelRs[baseX << 2], curG = pixelGs[baseX << 2], curB = pixelBs[baseX << 2];
                        int rDiff = 0, gDiff = 0, bDiff = 0;

                        // Search the radius around this pixel and track the difference
                        for (int y = 0; y < PixelRadius; ++y)
                        {
                            byte* subRs = (byte*)&pixelRs[y * imageData.Stride], subGs = &subRs[1], subBs = &subGs[1];

                            for (int x = 0; x < PixelRadius; ++x)
                            {
                                // Compare red
                                // TODO is there a cleaner way to do this?
                                rDiff = curR - subRs[x << 2] > rDiff ? curR - subRs[x << 2] : rDiff;
                                rDiff = subRs[x << 2] - curR > rDiff ? subRs[x << 2] - curR : rDiff;
                                gDiff = curG - subGs[x << 2] > gDiff ? curG - subGs[x << 2] : gDiff;
                                gDiff = subGs[x << 2] - curG > gDiff ? subGs[x << 2] - curG : gDiff;
                                bDiff = curB - subBs[x << 2] > bDiff ? curB - subBs[x << 2] : bDiff;
                                bDiff = subBs[x << 2] - curB > bDiff ? subBs[x << 2] - curB : bDiff;
                            }
                        }

                        // Write the difference to the output
                        outputPixels[baseY * outputImageData.Stride / 4 + baseX] = 0xFF000000 | (uint)(rDiff << 16 | gDiff << 8 | bDiff);
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