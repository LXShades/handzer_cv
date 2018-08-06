using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageAnalysis.Analysis.Highlighters
{
    /// <summary>
    /// A highlighter with a single coloured line
    /// </summary>
   public class EdgeHighlighter : Highlighter
    {
        /// <summary>
        /// Line start and end coordinates
        /// </summary>
        public Point LineStart;
        public Point LineEnd;

        /// <summary>
        /// Pen used for rendering the line
        /// </summary>
        public Pen Pen = new Pen(Color.Red, 3);

        public EdgeHighlighter(Point start, Point end)
        {
            LineStart = start;
            LineEnd = end;
        }

        public void Draw(ref Graphics formGraphics)
        {
            formGraphics.DrawLine(Pen, LineStart, LineEnd);
        }
    }
}
