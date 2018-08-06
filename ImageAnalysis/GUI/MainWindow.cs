using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using ImageAnalysis.Images.Filters;
using ImageAnalysis.Analysis.Highlighters;
using static WinApi.User32.User32Methods;
using static WinApi.Gdi32.Gdi32Methods;

namespace ImageAnalysis.GUI
{
    public partial class MainWindow : Form
    {
        /// <summary>
        /// List of possible filters and their (TODO move type list to Filter.cs as static member?)
        /// </summary>
        private BindingList<FilterListItem> filterListItems = new BindingList<FilterListItem>();

        /// <summary>
        /// Items on the filter stack applied to the current image
        /// </summary>
        private BindingList<FilterListItem> filterStackItems = new BindingList<FilterListItem>();

        public struct FilterListItem
        {
            public string Name { get; set; }
            public Type Type;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Add all filter types to the filter list
            var types = from type in Assembly.GetExecutingAssembly().GetTypes()
                        where typeof(Filter).IsAssignableFrom(type) && type != typeof(Filter)
                        select type;

            foreach (Type filterType in types.ToList())
            {
                filterListItems.Add(new FilterListItem()
                {
                    Name = (Activator.CreateInstance(filterType) as Filter).FilterName,
                    Type = filterType
                });
            }

            // Set the filter listbox's data source to that list
            AddFilterList.DataSource = filterListItems;
            AddFilterList.DisplayMember = "Name";

            // Similarly, set the filter stack's data source to the filter stack items
            FilterStack.DataSource = filterStackItems;
            FilterStack.DisplayMember = "Name";
        }

        // Event handlers
        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            try
            {
                // Load an image
                Bitmap image = (Bitmap)Image.FromFile(@"..\..\BLARGH.png", true);

                ApplyImageFilters(image);
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Windows.Forms.MessageBox.Show("There was an error opening the bitmap. " +
                    "Please check the path.");
            }
        }

        private void BtnCaptureCam_Click(object sender, EventArgs e)
        {
            // Get DroidCam video window
            IntPtr hwndCaptureWindow = WinApi.User32.User32Methods.FindWindow(null, "DroidCam Video");

            if (hwndCaptureWindow != null)
            {
                // Get dimensions of the capture window
                NetCoreEx.Geometry.Rectangle rect = new NetCoreEx.Geometry.Rectangle();

                GetWindowRect(hwndCaptureWindow, out rect);

                // Do fancy DC bitblit
                IntPtr hdcFrom = GetDC(hwndCaptureWindow);
                IntPtr hdcTo = CreateCompatibleDC(hdcFrom);
                IntPtr bitmapTo = CreateCompatibleBitmap(hdcFrom, rect.Width, rect.Height);

                SelectObject(hdcTo, bitmapTo);
                BitBlt(hdcTo, 0, 0, rect.Width, rect.Height, hdcFrom, 0, 0, WinApi.Gdi32.BitBltFlags.SRCCOPY);

                // Create the image
                Bitmap image = Image.FromHbitmap(bitmapTo);

                // Release resources
                DeleteObject(bitmapTo);
                DeleteDC(hdcTo);
                ReleaseDC(hwndCaptureWindow, hdcFrom);

                // Apply the image
                ApplyImageFilters(image);
            }
        }

        /// <summary>
        /// Applies the filter list to the given image
        /// TODO move and split this method
        /// </summary>
        /// <param name="image">The image to filter and display</param>
        private void ApplyImageFilters(Bitmap image)
        {
            GraphicsUnit pixelUnits = GraphicsUnit.Pixel;
            List<Highlighter> aggregateHighlighters = new List<Highlighter>();

            // Apply filters to the image
            foreach (FilterListItem filter in filterStackItems)
            {
                Filter filterInstance = Activator.CreateInstance(filter.Type) as Filter;
                Highlighter[] currentHighlighters;

                filterInstance.Apply(ref image, out currentHighlighters);

                if (currentHighlighters != null)
                {
                    // Add this filter's highlighters to the aggregate highlighter list
                    aggregateHighlighters.AddRange(currentHighlighters);
                }

                // Update the filter debug info box TODO move
                if (FilterStack.SelectedIndex >= 0 && filter.Equals(filterStackItems[FilterStack.SelectedIndex]))
                {
                    FilterInfoBox.Text = filterInstance.DebugInfo;
                }
            }

            // Draw it TODO move
            Graphics formGraphics = ImageBox.CreateGraphics();
            
            formGraphics.Clear(Color.SlateGray);
            formGraphics.DrawImage(image, image.GetBounds(ref pixelUnits));

            // Draw the highlighters (analysis stuff goes here?)
            foreach (Highlighter highlighter in aggregateHighlighters)
            {
                highlighter.Draw(ref formGraphics);
            }

            formGraphics.Flush();
            formGraphics.Dispose();
        }

        private void AddFilterList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // Add the selected filter item to the filter stack
            ComboBox box = sender as ComboBox;
            FilterListItem item = (FilterListItem)box.SelectedItem;
            
            box.Text = "Select a filter";
            filterStackItems.Add(item);
        }

        private void MoveFilterUp_Click(object sender, EventArgs e)
        {
            if (FilterStack.SelectedIndex >= 1 && FilterStack.SelectedIndex < filterStackItems.Count)
            {
                // Move the item up an index
                FilterListItem item = filterStackItems[FilterStack.SelectedIndex];
                int originalSelectedIndex = FilterStack.SelectedIndex; // preserve the selected index

                filterStackItems.RemoveAt(originalSelectedIndex);
                filterStackItems.Insert(originalSelectedIndex - 1, item);

                FilterStack.SelectedIndex = originalSelectedIndex - 1; // restore the selected index
            }
        }
        
        private void MoveFilterDown_Click(object sender, EventArgs e)
        {
            if (FilterStack.SelectedIndex >= 0 && FilterStack.SelectedIndex < filterStackItems.Count - 1)
            {
                // Move the item up an index
                FilterListItem item = filterStackItems[FilterStack.SelectedIndex];
                int originalSelectedIndex = FilterStack.SelectedIndex; // preserve the selected index

                filterStackItems.RemoveAt(originalSelectedIndex);
                filterStackItems.Insert(originalSelectedIndex + 1, item);

                FilterStack.SelectedIndex = originalSelectedIndex + 1; // restore the selected index
            }
        }

        private void DeleteFilter_Click(object sender, EventArgs e)
        {
            if (FilterStack.SelectedIndex >= 0 && FilterStack.SelectedIndex < filterStackItems.Count)
            {
                // Remove the item from the filter list
                filterStackItems.RemoveAt(FilterStack.SelectedIndex);
            }
        }

        private void ImageBox_MouseClick(object sender, MouseEventArgs e)
        {
            // Inform the input handler of the mouse click
            Input.OnClick(e.X, e.Y);

            // Reapply filters
            // TODO don't reload image entirely
            Bitmap image = (Bitmap)Image.FromFile(@"..\..\BLARGH.png", true);

            ApplyImageFilters(image);
        }
    }
}
