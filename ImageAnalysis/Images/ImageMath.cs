using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysis.Images
{
    class ImageMath
    {
        /// <summary>
        /// Returns the maximum value of the three byte colour parameters
        /// </summary>
        /// <returns>The maximum value of the parameters</returns>
        public static byte Max(byte r, byte g, byte b)
        {
            return (r > g ? (r > b ? r : b) : (g > b ? g : b));
        }

        /// <summary>
        /// Returns the brightness of a pixel
        /// </summary>
        /// <param name="pixel">The pixel colour in XRGB or ARGB</param>
        /// <returns></returns>
        public static byte PixelBrightness(uint pixel)
        {
            return Max((byte)(pixel >> 16), (byte)(pixel >> 8), (byte)pixel);
        }
    }
}
