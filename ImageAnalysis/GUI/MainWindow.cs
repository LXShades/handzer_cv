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
        
        // Test cam capture stuff

        // Event handlers
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Load an image
                Bitmap image = (Bitmap)Image.FromFile(@"..\..\BLARGH.png", true);

                // Actually, try capturing an image from the DroidCam Client
                // Get DroidCam Client window
                IntPtr hwndCaptureWindow = WinApi.User32.User32Methods.FindWindow(null, "DroidCam Client");

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

                    image = Image.FromHbitmap(bitmapTo);

                    DeleteObject(bitmapTo);
                    DeleteDC(hdcTo);
                    ReleaseDC(hwndCaptureWindow, hdcFrom);
                }

                GraphicsUnit srslyBruh = GraphicsUnit.Pixel;

                // Apply filters to the image
                foreach (FilterListItem filter in filterStackItems)
                {
                    Filter filterInstance = Activator.CreateInstance(filter.Type) as Filter;
                    
                    filterInstance.Apply(ref image);
                }
                
                // Draw the texture
                Graphics formGraphics = this.CreateGraphics();

                formGraphics.Clear(Color.SlateGray);
                formGraphics.DrawImage(image, image.GetBounds(ref srslyBruh));

                // Draw the highlighters (analysis stuff goes here?)
                Analysis.EdgeHighlighter testHighlighter = new Analysis.EdgeHighlighter(new Point(0, 0), new Point(50, 50));
                testHighlighter.Draw(ref formGraphics);

                formGraphics.Dispose();
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Windows.Forms.MessageBox.Show("There was an error opening the bitmap. " +
                    "Please check the path.");
            }
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
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

        public struct FilterListItem
        {
            public string Name { get; set; }
            public Type Type;
        }
    }
}
