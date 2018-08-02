using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysis.Analysis.Highlighters
{
    public interface Highlighter
    {
        /// <summary>
        /// Renders this highlighter
        /// </summary>
        /// <param name="formGraphics">The Graphics surface to render to</param>
        void Draw(ref System.Drawing.Graphics formGraphics);
    }
}
