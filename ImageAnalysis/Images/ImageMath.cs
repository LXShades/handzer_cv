using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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

        /// <summary>
        /// Returns the difference between two pixel colours
        /// </summary>
        /// <returns>The differnece between two pixel colours in the range 0 to 1</returns>
        public static float PixelDifference(byte aR, byte aG, byte aB, byte bR, byte bG, byte bB)
        {
            return (float)Math.Sqrt((aR - bR) * (aR - bR) + (aG - bG) * (aG - bG) + (aB - bB) * (aB - bB)) / 441.7f;
        }

        public static float PixelDifference(Color a, Color b)
        {
            return (float)Math.Sqrt((a.R - b.R) * (a.R - b.R) + (a.G - b.G) * (a.G - b.G) + (a.B - b.B) * (a.B - b.B)) / 441.7f;
        }
    }
}
