using System.Collections.Generic;
using System.Windows.Forms;
using ozgurtek.framework.driver.gdal;

namespace ozgurtek.framework.test.winforms
{
    public partial class TestForm : Form
    {
        private string _source = @"C:\Users\eniso\Desktop\work\testdata\shp\mahalle.shp";
        private string _wfsSource = @"WFS:http://185.122.200.110:8080/geoserver/dhmi/wfs?service=WFS";


        public TestForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, System.EventArgs e)
        {
            //GdOgrDataSource dataSource = GdOgrDataSource.Open(_wfsSource);
            //IEnumerable<GdOgrTable> gdOgrTables = dataSource.GetTable();
            //foreach (GdOgrTable gdOgrTable in gdOgrTables)
            //{
                
            //}
        }
    }
}