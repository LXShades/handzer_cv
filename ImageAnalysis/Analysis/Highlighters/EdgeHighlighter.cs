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
        /// Visual line attributes
        /// TODO proper way of documenting multiple self-explanatory properties
        /// </summary>
        public Point LineStart;
        public Point LineEnd;

        public Color LineColour
        {
            get
            {
                return LineColour;
            }
            set
            {
                pen = new Pen(LineColour, (float)LineThickness);
            }
        }
        private Color lineColour = Color.Red;
       
        public int LineThickness {
            get
            {
                return lineThickness;
            }
            set
            {
                pen = new Pen(LineColour, (float)LineThickness);
            }
        }
        private int lineThickness = 3;

        /// <summary>
        /// Pen used for rendering the line
        /// </summary>
        private Pen pen = new Pen(Color.Red, 3);

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
