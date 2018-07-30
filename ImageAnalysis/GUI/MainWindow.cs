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

namespace ImageAnalysis.GUI
{
    public partial class MainWindow : Form
    {
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
            ComboBox filterList = Controls.Find("AddFilterList", true)[0] as ComboBox;
            filterList.DataSource = filterListItems;
            filterList.DisplayMember = "Name";

            // Similarly, set the filter stack's data source to the filter stack items
            ListBox filterStack = Controls.Find("FilterStack", true)[0] as ListBox;
            filterStack.DataSource = filterStackItems;
            filterStack.DisplayMember = "Name";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Load an image
                Bitmap image1 = (Bitmap)Image.FromFile(@".\BLARGH.png", true);
                GraphicsUnit srslyBruh = GraphicsUnit.Pixel;

                // Apply filters to the image
                foreach (FilterListItem filter in filterStackItems)
                {
                    Filter filterInstance = Activator.CreateInstance(filter.Type) as Filter;
                    
                    filterInstance.Apply(ref image1);
                }
                
                // Create the image texture
                TextureBrush texture = new TextureBrush(image1)
                {
                    WrapMode = System.Drawing.Drawing2D.WrapMode.Tile
                };
                
                // Draw the texture
                Graphics formGraphics = this.CreateGraphics();

                formGraphics.Clear(Color.SlateGray);
                formGraphics.DrawImage(image1, image1.GetBounds(ref srslyBruh));
                formGraphics.Dispose();
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("There was an error opening the bitmap. " +
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

        public struct FilterListItem
        {
            public string Name { get; set; }
            public Type Type;
        }

        /// <summary>
        /// List of possible filters and their (TODO move type list to Filter.cs as static member?)
        /// </summary>
        private BindingList<FilterListItem> filterListItems = new BindingList<FilterListItem>();

        /// <summary>
        /// Items on the filter stack applied to the current image
        /// </summary>
        private BindingList<FilterListItem> filterStackItems = new BindingList<FilterListItem>();
    }
}
