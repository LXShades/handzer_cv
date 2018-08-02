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
        public string FilterName { get { return "Vectorise"; } }

        public void Apply(ref Bitmap bmp, out Highlighters.Highlighter[] highlighters)
        {
            highlighters = new Highlighters.Highlighter[2];

            // Return a single line highlighter
            highlighters[0] = new EdgeHighlighter(new Point(50, 50), new Point(100, 100));
            highlighters[1] = new GridHighlighter(new Rectangle(0, 0, bmp.Width, bmp.Height), 32);
            (highlighters[1] as GridHighlighter).Pen.Width = 1;
            (highlighters[1] as GridHighlighter).Pen.Color = Color.Black;
        }
    }
}
