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

namespace ImageAnalysis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Add all filter types to the filter list combobox
            var types = from type in Assembly.GetExecutingAssembly().GetTypes()
                        where typeof(Filter).IsAssignableFrom(type) && type != typeof(Filter)
                        select type;
            ComboBox filterList = (ComboBox)Controls.Find("AddFilterList", true)[0];

            types.ToList().ForEach(type => filterList.Items.Add(((Filter)(Activator.CreateInstance(type)))?.FilterName));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Load an image
                Bitmap image1 = (Bitmap)Image.FromFile(@".\BLARGH.png", true);
                GraphicsUnit srslyBruh = GraphicsUnit.Pixel;

                // Apply filters to the image
                Filter[] filters = new Filter[] { new FilterBlackAndWhite() };
                
                foreach (Filter filter in filters)
                {
                    filter.Apply(ref image1);
                }
                
                // Create the image texture
                TextureBrush texture = new TextureBrush(image1)
                {
                    WrapMode = System.Drawing.Drawing2D.WrapMode.Tile
                };

                // Draw the texture
                Graphics formGraphics = this.CreateGraphics();

                formGraphics.DrawImage(image1, image1.GetBounds(ref srslyBruh));
                formGraphics.Dispose();
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("There was an error opening the bitmap. " +
                    "Please check the path.");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            switch (box?.SelectedIndex)
            {
                // ....
                case 0:
                {
                    break;
                }
            }
        }
    }
}
