using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageAnalysis
{
    static class Input
    {
        public static Point LastClickPosition = new Point(0, 0);

        /// <summary>
        /// Called when the mouse is clicked
        /// </summary>
        /// <param name="clickX">X position of the mouse on click</param>
        /// <param name="clickY">Y position of the mouse on click</param>
        public static void OnClick(int clickX, int clickY)
        {
            LastClickPosition.X = clickX;
            LastClickPosition.Y = clickY;
        }
    }
}
