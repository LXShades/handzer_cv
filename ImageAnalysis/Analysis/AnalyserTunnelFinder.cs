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
    /// Tries to find tunnels in the image using a grid, marking points of interest
    /// </summary>
    class AnalyserTunnelFinder : Images.Filters.Filter
    {
        public string FilterName { get { return "TunnelFind"; } }

        public void Apply(ref Bitmap bitmap, out Highlighter[] highlightersOut)
        {
            Images.ImageReader reader = new Images.ImageReader(ref bitmap);
            List<Highlighter> highlighters = new List<Highlighter>();

            // Analyse gridlines
            int numHorizontalDivisions = bitmap.Width / vectorGridInterval, numVerticalDivisions = bitmap.Height / vectorGridInterval;
            List<EdgePoint>[] horizontalEdges = new List<EdgePoint>[numVerticalDivisions];
            List<EdgePoint>[] verticalEdges = new List<EdgePoint>[numHorizontalDivisions];

            // Verticals
            for (int tileX = 0; tileX < bitmap.Width / vectorGridInterval * vectorGridInterval; tileX += vectorGridInterval)
            {
                FindEdgesOnLine(ref reader, out verticalEdges[tileX / vectorGridInterval], new Point(tileX, 0), 0, 1);
            }

            // Horizontals
            for (int tileY = 0; tileY < bitmap.Height / vectorGridInterval * vectorGridInterval; tileY += vectorGridInterval)
            {
                FindEdgesOnLine(ref reader, out horizontalEdges[tileY / vectorGridInterval], new Point(0, tileY), 0, 1);
            }

            // Debug: highlight the edges
            foreach (List<EdgePoint> edgeList in verticalEdges)
            {
                foreach (EdgePoint edge in edgeList) // ow the edge
                {
                    // ow the edge
                    highlighters.Add(new PointHighlighter(edge.X, edge.Y));
                    var h = new PointHighlighter(edge.CentrePoint.X, edge.CentrePoint.Y);
                    h.Pen.Color = Color.Orange;
                    highlighters.Add(h);
                }
            }

            // 

            // Add a grid highlighter representing the grid we checked
            GridHighlighter grid = new GridHighlighter(new Rectangle(0, 0, bitmap.Width, bitmap.Height), vectorGridInterval);
            grid.Pen.Width = 1;
            grid.Pen.Color = Color.Gray;

            //highlighters.Add(grid);

            // Complete the highlighter array
            highlightersOut = highlighters.ToArray();

            // Release resources
            reader.Dispose();
        }

        /// <summary>
        /// The gap between grid lines, in pixels, per possible vector
        /// </summary>
        private int vectorGridInterval = 32;

        private void FindEdgesOnLine(ref Images.ImageReader reader, out List<EdgePoint> edgePoints, Point start, int xStep, int yStep)
        {
            edgePoints = new List<EdgePoint>();

            unsafe
            {
                // Determine the popularity of brightnesses (we'll grab the pixels brighter than a percentile)
                int x = start.X, y = start.Y;
                int[] counts = new int[256];
                int numPixelsAlongLine = 0;

                Array.Clear(counts, 0, 256);

                while (x >= 0 && y >= 0 && x < reader.Width && y < reader.Height)
                {
                    ++counts[reader.PixelRows[y][x] & 0xFF];
                    ++numPixelsAlongLine;

                    x += xStep;
                    y += yStep;
                }

                // Considering the averages, determine the brightness threshold to qualify a pixel
                int expectedCount = 95 * numPixelsAlongLine / 100;
                int brightnessThreshold = 0;

                for (int i = 0, currentCount = 0; i < 256; ++i)
                {
                    if (currentCount >= expectedCount)
                    {
                        brightnessThreshold = i;
                        break;
                    }

                    currentCount += counts[i];
                }

                // Step along again, this time marking pixels that exceed the brightness threshold
                bool wasAboveThreshold = false;
                EdgePoint lastEdgePoint = null;

                x = start.X;
                y = start.Y;

                while (x >= 0 && y >= 0 && x < reader.Width && y < reader.Height)
                {
                    // Register the pixel if it's just gone above the threshold
                    bool isAboveThreshold = ((reader.PixelRows[y][x] & 0xFF) > brightnessThreshold);

                    if (isAboveThreshold && !wasAboveThreshold)
                    {
                        lastEdgePoint = new EdgePoint(x, y, ref reader, ref lastEdgePoint);
                        edgePoints.Add(lastEdgePoint);
                    }

                    // Advance
                    wasAboveThreshold = isAboveThreshold;
                    x += xStep;
                    y += yStep;
                }
            }
        }

        /// <summary>
        /// A point on an edge, reported by some functions, with some extra information
        /// </summary>
        private class EdgePoint
        {
            public int X;
            public int Y;

            public EdgePoint LastPoint { get; }

            public Color AverageColourFromLast;
            public Point CentrePoint;

            public unsafe EdgePoint(int x, int y, ref Images.ImageReader reader, ref EdgePoint lastPoint)
            {
                // Set vars
                X = x;
                Y = y;

                LastPoint = lastPoint;

                CentrePoint = new Point(x, y);
                AverageColourFromLast = Color.FromArgb((int)reader.PixelRows[y][x]);

                // Interpolate data from previous point if the point is available
                if (lastPoint != null)
                {
                    int xDiff = lastPoint.X - x, yDiff = lastPoint.Y - y;
                    int diffSize = (int)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);

                    if (diffSize > 0)
                    {
                        // Backtrace along the line between this point and the last
                        uint r = 0, g = 0, b = 0;
                        for (int curX = x, curY = y, iteration = 0; 
                            iteration < diffSize; 
                            ++iteration, curX = x + (iteration * xDiff) / diffSize, curY = y + (iteration * yDiff) / diffSize)
                        {
                            b += reader.PixelRows[curY][curX] & 0xFF;
                            g += (reader.PixelRows[curY][curX] >>  8) & 0xFF;
                            r += (reader.PixelRows[curY][curX] >> 16) & 0xFF;
                        }

                        // Determine the average colour
                        AverageColourFromLast = Color.FromArgb((int)r / diffSize, (int)g / diffSize, (int)b / diffSize);
                        CentrePoint = new Point(x + xDiff / 2, y + yDiff / 2);
                    }
                }
            }
        }
    }
}
