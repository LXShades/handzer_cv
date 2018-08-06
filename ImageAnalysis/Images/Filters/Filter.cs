using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ImageAnalysis.Analysis.Highlighters;

namespace ImageAnalysis.Images.Filters
{
    /// <summary>
    /// Filter for an image
    /// </summary>
    public abstract class Filter
    {
        /// <summary>
        /// The human-readable name of this filter
        /// </summary>
        public virtual string FilterName { get; }

        /// <summary>
        /// Custom text to be written in the filter debug info box
        /// </summary>
        public string DebugInfo;

        /// <summary>
        /// Applies the filter to an image
        /// </summary>
        /// <param name="bitmap">The image to be filtered</param>
        public abstract void Apply(ref Bitmap bitmap, out Highlighter[] highlightersOut);
    }
}
