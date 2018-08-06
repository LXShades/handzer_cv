using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageAnalysis.Analysis.Highlighters
{
    /// <summary>
    /// A highlighter with a cross appearance, centred on a single point
    /// </summary>
    public class PointHighlighter : Highlighter
    {
        /// <summary>
        /// Size of the visible cross, in pixels
        /// </summary>
        public int CrossSize = 5;

        /// <summary>
        /// Position of this cross, in pixels
        /// </summary>
        public Point Position;

        /// <summary>
        /// Pen used for rendering the cross 
        /// </summary>
        public Pen Pen = new Pen(Color.Red, 2);

        public PointHighlighter(int x, int y)
        {
            Position = new Point(x, y);
        }

        public PointHighlighter(Point position)
        {
            Position = position;
        }

        public void Draw(ref Graphics graphics)
        {
            graphics.DrawLine(Pen, new Point(Position.X - CrossSize, Position.Y), new Point(Position.X + CrossSize, Position.Y));
            graphics.DrawLine(Pen, new Point(Position.X, Position.Y - CrossSize), new Point(Position.X, Position.Y + CrossSize));
        }
    }
}
