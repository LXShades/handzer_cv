using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ImageAnalysis.Analysis.Highlighters;
using ImageAnalysis.Images;
using System.Numerics;

namespace ImageAnalysis.Analysis
{
    /// <summary>
    /// Tries to find tunnels in the image using a grid, marking points of interest
    /// </summary>
    class AnalyserTunnelFinder : Images.Filters.Filter
    {
        public override string FilterName { get { return "TunnelFind"; } }

        public override void Apply(ref Bitmap bitmap, out Highlighter[] highlightersOut)
        {
            // Make a copy of the bitmap filtered with the Difference filter
            Bitmap differenceBitmap = new Bitmap(bitmap);
            var filter = new Images.Filters.FilterDifference();
            Highlighter[] temp;

            filter.Apply(ref differenceBitmap, out temp);

            // Analyse gridlines
            ImageReader reader = new ImageReader(ref bitmap);
            ImageReader differenceReader = new ImageReader(ref differenceBitmap);
            List<Highlighter> highlighters = new List<Highlighter>();

            int numHorizontalDivisions = bitmap.Width / gridInterval, numVerticalDivisions = bitmap.Height / gridInterval;
            List<EdgePoint>[] horizontalEdges = new List<EdgePoint>[numVerticalDivisions];
            List<EdgePoint>[] verticalEdges = new List<EdgePoint>[numHorizontalDivisions];

            // Verticals
            for (int tileX = 0; tileX < bitmap.Width / gridInterval * gridInterval; tileX += gridInterval)
            {
                FindEdgesOnLine(ref reader, ref differenceReader, out verticalEdges[tileX / gridInterval], new Point(tileX, 0), 0, 1);
            }

            // Horizontals
            for (int tileY = 0; tileY < bitmap.Height / gridInterval * gridInterval; tileY += gridInterval)
            {
                FindEdgesOnLine(ref reader, ref differenceReader, out horizontalEdges[tileY / gridInterval], new Point(0, tileY), 0, 1);
            }

            // Debug: highlight the edges
            foreach (List<EdgePoint> edgeList in verticalEdges)
            {
                foreach (EdgePoint edge in edgeList) // ow the edge
                {
                    // ow the edge
                    highlighters.Add(new PointHighlighter(edge.X, edge.Y));
                    highlighters.Add(new PointHighlighter(edge.SegmentCentre.X, edge.SegmentCentre.Y)
                    {
                        Pen = new Pen(Color.Orange)
                    });

                    // add a line too
                    if (edge.LastPoint != null)
                    {
                        highlighters.Add(new EdgeHighlighter(new Point(edge.LastPoint.X, edge.LastPoint.Y), new Point(edge.X, edge.Y))
                        {
                            Pen = new Pen(edge.AverageSegmentColour, 2.0f)
                        });
                    }
                }
            }

            // Connect the edge points
            ConnectEdgePoints(verticalEdges, highlighters);

            // Add a grid highlighter representing the grid we checked
            GridHighlighter grid = new GridHighlighter(new Rectangle(0, 0, bitmap.Width, bitmap.Height), gridInterval);
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
        private int gridInterval = 32;

        private void FindEdgesOnLine(ref ImageReader reader, ref ImageReader diffReader, out List<EdgePoint> edgePoints, Point start, int xStep, int yStep)
        {
            edgePoints = new List<EdgePoint>();

            unsafe
            {
                // Determine the popularity of brightnesses (we'll grab the pixels brighter than a percentile)
                int x = start.X, y = start.Y;
                int[] counts = new int[256];
                int numPixelsAlongLine = 0;

                Array.Clear(counts, 0, 256);

                while (x >= 0 && y >= 0 && x < diffReader.Width && y < diffReader.Height)
                {
                    ++counts[diffReader.PixelRows[y][x] & 0xFF];
                    ++numPixelsAlongLine;

                    x += xStep;
                    y += yStep;
                }

                // Considering the averages, determine the brightness threshold to qualify a pixel
                int expectedCount = 90 * numPixelsAlongLine / 100;
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

                while (x >= 0 && y >= 0 && x < diffReader.Width && y < diffReader.Height)
                {
                    // Register the pixel if it's just gone above the threshold
                    bool isAboveThreshold = ((diffReader.PixelRows[y][x] & 0xFF) > brightnessThreshold);

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

        private void ConnectEdgePoints(IEnumerable<EdgePoint>[] gridLines, List<Highlighter> highlighters)
        {
            // We want to join lines of similar edge lengths together. If we're coming from the tips of the fingers, we're fine increasing the width over time
            // If we're coming from the opposite, we're fine decreasing the width
            // Basically, we want to track whether the width increases or decreases as we go along
            List<EdgePoint> finalEdgeList = new List<EdgePoint>();

            for (int i = Input.LastClickPosition.X / gridInterval; i < gridLines.Count(); ++i)
            {
                EdgePoint mostSimilarPoint = null;
                SimilarityPack highestSimilarity = new SimilarityPack();

                foreach (EdgePoint p in gridLines[i])
                {
                    SimilarityPack similarity;

                    if (finalEdgeList.Count == 0)
                    {
                        similarity = new SimilarityPack();
                        similarity.Distance = 1.0f / ((p.X - Input.LastClickPosition.X) * (p.X - Input.LastClickPosition.X) + (p.Y - Input.LastClickPosition.Y) * (p.Y - Input.LastClickPosition.Y));
                    }
                    else if (finalEdgeList.Count == 1)
                    {
                        similarity = p.CalculateSimilarity(finalEdgeList[0], null);
                    }
                    else
                    {
                        similarity = p.CalculateSimilarity(finalEdgeList[finalEdgeList.Count - 1], finalEdgeList[finalEdgeList.Count - 2]);
                    }

                    if (similarity.OverallSimilarity >= highestSimilarity.OverallSimilarity)
                    {
                        mostSimilarPoint = p;
                        highestSimilarity = similarity;
                    }
                }

                finalEdgeList.Add(mostSimilarPoint);
                DebugInfo += highestSimilarity.Info + "\r\n";
            }

            for (int i = 1; i < finalEdgeList.Count; ++i)
            {
                highlighters.Add(new EdgeHighlighter(new Point(finalEdgeList[i - 1].X, finalEdgeList[i - 1].Y), new Point(finalEdgeList[i].X, finalEdgeList[i].Y))
                {
                    Pen = new Pen(Color.Green, 1.0f)
                });
            }
        }

        /// <summary>
        /// A point on an edge, reported by some functions, with some extra information
        /// </summary>
        private class EdgePoint
        {
            public int X;
            public int Y;

            public Point Position
            {
                get
                {
                    return new Point(X, Y);
                }
                set
                {
                    X = value.X;
                    Y = value.Y;
                }
            }

            public Color ColourUnderPoint;

            public EdgePoint LastPoint { get; }

            // This group of vars is only valid when LastPoint is valid
            public Color AverageSegmentColour;
            public Point SegmentCentre;

            public unsafe EdgePoint(int x, int y, ref ImageReader reader, ref EdgePoint lastPoint)
            {
                // Set vars
                X = x;
                Y = y;

                ColourUnderPoint = Color.FromArgb((int)reader.PixelRows[y][x]);

                LastPoint = lastPoint;

                SegmentCentre = new Point(x, y);
                AverageSegmentColour = ColourUnderPoint;

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
                        AverageSegmentColour = Color.FromArgb((int)r / diffSize, (int)g / diffSize, (int)b / diffSize);
                        SegmentCentre = new Point(x + xDiff / 2, y + yDiff / 2);
                    }
                }
            }

            /// <summary>
            /// Calculates the similarity between this point and either 1 or 2 previous points representing a line segment
            /// This function decides how appropriately this point would fit into a path joining 'point'
            /// Comparison factors include: thickness of this valley compared to the last, valley colour, relative directions of the valleys
            /// </summary>
            /// <param name="point">Point to compare with</param>
            /// <param name="previousPoint">Point prior to 'point', if applicable</param>
            /// <returns>A floating-point similarity value between 0 and 1 where 1 is virtually identical</returns>
            public SimilarityPack CalculateSimilarity(EdgePoint point, EdgePoint previousPoint = null)
            {
                SimilarityPack similarity = new SimilarityPack()
                {
                    Colour = ImageMath.PixelDifference(ColourUnderPoint, point.ColourUnderPoint),
                    Direction = 1.0f,
                    Length = 1.0f,
                    Width = 1.0f,
                    Distance = 1.0f,
                };

                if (previousPoint != null)
                {
                    Vector2 previousVector = new Vector2(point.X - previousPoint.X, point.Y - previousPoint.Y);
                    Vector2 currentVector = new Vector2(X - point.X, Y - point.Y);

                    if (previousVector.LengthSquared() > 0 && currentVector.LengthSquared() > 0)
                    {
                        // Direction similarity: Use the dot product of the normalised segment vectors
                        similarity.Direction = Vector2.Dot(previousVector / previousVector.Length(), currentVector / currentVector.Length());

                        // Length similarity: Use the smaller length divided by the bigger length.....?
                        similarity.Length = previousVector.Length() / currentVector.Length();

                        if (similarity.Length > 1.0f)
                        {
                            similarity.Length = 1.0f / similarity.Length; // swap the division operands lazily
                        }
                    }
                }

                if (point.LastPoint != null && LastPoint != null)
                {
                    // Width similarity: Use the smaller width divided by the bigger width?
                    similarity.Width = Vector2.Distance(new Vector2(X, Y), new Vector2(LastPoint.X, LastPoint.Y)) / 
                                      Vector2.Distance(new Vector2(point.X, point.Y), new Vector2(point.LastPoint.X, point.LastPoint.Y));

                    if (similarity.Width > 1.0f)
                    {
                        similarity.Width = 1.0f / similarity.Width;
                    }
                }

                if (previousPoint == null)
                {
                    float distanceToPoint = Vector2.Distance(new Vector2(X, Y), new Vector2(point.X, point.Y));
                    similarity.Distance = distanceToPoint > 0 ? 1.0f / distanceToPoint : 1.0f;
                }

                return similarity;
            }
        }

        public class SimilarityPack
        {
            public float OverallSimilarity
            {
                get
                {
                    return Colour + Direction + Length + Width + Distance;
                }
            }

            public string Info
            {
                get
                {
                    return String.Format("Clr: {0:0.##} Dir: {1:0.##} Len: {2:0.##} Wid: {3:0.##} Dist: {4:0.##}", Colour, Direction, Length, Width, Distance);
                }
            }

            public float Colour = 0.0f;
            public float Direction = 0.0f;
            public float Length = 0.0f;
            public float Width = 0.0f;
            public float Distance = 0.0f;
        }
    }
}
