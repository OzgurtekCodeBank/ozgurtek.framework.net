using ozgurtek.framework.common.Data;
using ozgurtek.framework.common.Data.Format;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.converter.winforms
{
    public static class Util
    {
        public const string GdId = "gd_id";
        public const string GdGeometry = "gd_geometry";
        public const string GdHeight = "gd_ext_height";
        public const string GdStyle = "gd_style";
        public const string GdTextureCoords = "gd_texture_coords";
        public const string GdDescription = "gd_description";

        public static GdMemoryTable PrepareNewMemTable()
        {
            GdMemoryTable memTable = new GdMemoryTable();
            memTable.CreateField(new GdField(GdId, GdDataType.String));
            memTable.CreateField(new GdField(GdGeometry, GdDataType.Geometry));
            memTable.CreateField(new GdField(GdHeight, GdDataType.Real));
            memTable.CreateField(new GdField(GdStyle, GdDataType.String));
            memTable.CreateField(new GdField(GdTextureCoords, GdDataType.String));
            memTable.CreateField(new GdField(GdDescription, GdDataType.Real));
            return memTable;
        }
    }
}
