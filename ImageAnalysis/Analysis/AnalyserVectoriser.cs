using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ImageAnalysis.Analysis.Highlighters;

namespace ImageAnalysis.Analysis
{
    /// <summary>
    /// Tries to vectorise the image, returning highlighters representing the vectors discovered
    /// </summary>
    class AnalyserVectoriser : Images.Filters.Filter
    {
        public override string FilterName { get { return "Vectorize"; } }

        public override void Apply(ref Bitmap bitmap, out Highlighter[] highlightersOut)
        {
            List<Highlighter> highlighters = new List<Highlighter>();

            // Process each tile
            Images.ImageReader reader = new Images.ImageReader(ref bitmap);

            for (int tileX = 0; tileX < bitmap.Width / vectorGridInterval * vectorGridInterval; tileX += vectorGridInterval)
            {
                for (int tileY = 0; tileY < bitmap.Height / vectorGridInterval * vectorGridInterval; tileY += vectorGridInterval)
                {
                    AnalyseTile(
                        ref reader,
                        new Rectangle(tileX, tileY, Math.Min(tileX + vectorGridInterval, bitmap.Width) - tileX, Math.Min(tileY + vectorGridInterval, bitmap.Height) - tileY), 
                        ref highlighters);
                }
            }

            // Add a grid highlighter representing the grid we checked
            /*highlighters[1] = new GridHighlighter(new Rectangle(0, 0, bitmap.Width, bitmap.Height), vectorGridInterval);
            (highlighters[1] as GridHighlighter).Pen.Width = 1;
            (highlighters[1] as GridHighlighter).Pen.Color = Color.Gray;*/

            highlightersOut = highlighters.ToArray();

            // Release resources
            reader.Dispose();
        }

        /// <summary>
        /// The gap between grid lines, in pixels, per possible vector
        /// </summary>
        private int vectorGridInterval = 32;

        private void AnalyseTile(ref Images.ImageReader reader, Rectangle tile, ref List<Highlighter> highlighters)
        {
            unsafe
            {
                // Find the brightest pixel
                int highestBrightness = 0;
                int brightestX = -1, brightestY = -1;

                for (int y = tile.Y; y < tile.Bottom; ++y)
                {
                    for (int x = tile.X; x < tile.Right; ++x)
                    {
                        if (Images.ImageMath.PixelBrightness(reader.PixelRows[y][x]) >= highestBrightness)
                        {
                            brightestX = x;
                            brightestY = y;
                            highestBrightness = Images.ImageMath.PixelBrightness(reader.PixelRows[y][x]);
                        }
                    }
                }

                // Add a point highlighter on the brightest pixel
                highlighters.Add(new PointHighlighter(brightestX, brightestY));

                // Follow a path along the edge
                // For each pixel:
                // Check neighbouring pixels, making sure they're not already checked
                // Follow the path of the brightest pixel

                // ACTUALLY... the dots could just be joined, between the brightest pixels in each tile
                for (int y = tile.Y; y < tile.Bottom; ++y)
                {
                    for (int x = tile.X; x < tile.Right; ++x)
                    {

                    }
                }
            }
        }
    }
}
