using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageAnalysis.Analysis.Highlighters
{
    /// <summary>
    /// A highlighter that generates a uniform grid of a given size and interval
    /// </summary>
    public class GridHighlighter : Highlighter
    {
        /// <summary>
        ///  Size of the gaps in the grid, in pixels
        /// </summary>
        public int GridIntervalSize = 10;

        /// <summary>
        ///  Boundaries that the grid will stay within
        /// </summary>
        public Rectangle GridBounds = new Rectangle(0, 0, 0, 0);

        /// <summary>
        /// Pen used for rendering the line
        /// </summary>
        public Pen Pen = new Pen(Color.Red, 3);

        public GridHighlighter(Rectangle bounds, int gridInterval)
        {
            GridBounds = bounds;
            GridIntervalSize = gridInterval;
        }

        public void Draw(ref Graphics formGraphics)
        {
            Rectangle drawArea = GridBounds;

            // Cut down the draw area so that it fits in with the grid interval
            drawArea.X = (drawArea.X + GridIntervalSize - 1) / GridIntervalSize * GridIntervalSize;
            drawArea.Y = (drawArea.Y + GridIntervalSize - 1) / GridIntervalSize * GridIntervalSize;
            drawArea.Width = drawArea.Width / GridIntervalSize * GridIntervalSize;
            drawArea.Height = drawArea.Height / GridIntervalSize * GridIntervalSize;

            // Draw the horizontal gridlines
            for (int y = drawArea.Top; y < drawArea.Bottom; y += GridIntervalSize)
            {
                formGraphics.DrawLine(Pen, drawArea.Left, y, drawArea.Right, y);
            }

            // Draw the verticals
            for (int x = drawArea.Left; x < drawArea.Right; x += GridIntervalSize)
            {
                formGraphics.DrawLine(Pen, x, drawArea.Top, x, drawArea.Bottom);
            }
        }
    }
}
