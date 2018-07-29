﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageAnalysis
{
    /// <summary>
    /// Filter for an image
    /// </summary>
    interface Filter
    {
        /// <summary>
        /// The human-readable name of this filter
        /// </summary>
        string FilterName { get; }

        /// <summary>
        /// Applies the filter to an image
        /// </summary>
        /// <param name="bitmap">The image to be filtered</param>
        void Apply(ref System.Drawing.Bitmap bitmap);
    }
}
