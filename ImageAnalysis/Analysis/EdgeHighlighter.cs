using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageAnalysis.Analysis
{
    /// <summary>
    /// A highlighter with a single coloured line
    /// </summary>
   public class EdgeHighlighter
    {
        /// <summary>
        /// Visual line attributes
        /// TODO proper way of documenting multiple self-explanatory properties
        /// </summary>
        public Point LineStart;
        public Point LineEnd;

        public Color LineColour = Color.Red;
       
        public int LineThickness = 1;

        /// <summary>
        /// Pen used for rendering the line
        /// </summary>
        private Pen pen = new Pen(Color.Red);

        public EdgeHighlighter(Point start, Point end)
        {
            LineStart = start;
            LineEnd = end;
        }

        public void Draw(ref Graphics formGraphics)
        {
            formGraphics.DrawLine(pen, LineStart, LineEnd);
        }
    }
}
