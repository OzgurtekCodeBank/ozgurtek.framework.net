using System;
using System.IO;
using System.Windows.Forms;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.postgres;

namespace ozgurtek.framework.converter.winforms
{
    public partial class AppSpesific : UserControl
    {
        private string _con =
            "User ID=postgres;Search Path=public;Password=AZe2IcyGt6yEewyZ;Host=185.122.200.110;Port=5432;Timeout=30;Database=dhmi_amdb;ApplicationName=JsonTiler;Pooling=false";

        public AppSpesific()
        {
            InitializeComponent();
        }

        private void TileBinaJsonButton_Click(object sender, EventArgs e)
        {
            try
            {
                TileBina();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show(exception.Message);
            }
        }

        private void TileBina()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult dialogResult = dialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
                return;

            GdPgDataSource dataSource = GdPgDataSource.Open(_con);

            string distinctSql = "select distinct teskilat_key from maks_yapi my";
            GdSqlFilter distinctFilter = new GdSqlFilter(distinctSql);
            GdPgTable distinctTable = dataSource.ExecuteSql("tes_key", distinctFilter);
            foreach (IGdRow row in distinctTable.Rows)
            {
                long teskilatKey = row.GetAsInteger("teskilat_key");

                string havalimaniSql = $"select * from maks_yapi where teskilat_key = {teskilatKey}";
                GdSqlFilter havalimaniFilter = new GdSqlFilter(havalimaniSql);
                GdPgTable havalimaniTable = dataSource.ExecuteSql("tes_key", havalimaniFilter);
                GdExtrudedModelExportEngine exportEngine = new GdExtrudedModelExportEngine();
                exportEngine.EpsgCode = 4326;
                exportEngine.DescFieldName = "kimlikno";
                exportEngine.FidFieldName = "objectid";
                exportEngine.GeomFieldName = "geom";
                exportEngine.ExtFieldName = "ext_field";
                exportEngine.SuppressBlankTile = true;
                exportEngine.TileSizeInMeter = 2000;

                string path = Path.Combine(dialog.SelectedPath, teskilatKey.ToString());
                Directory.CreateDirectory(path);
                exportEngine.OutputFolder = path;

                exportEngine.Export(havalimaniTable, new GdTrack());
            }
        }
    }
}
