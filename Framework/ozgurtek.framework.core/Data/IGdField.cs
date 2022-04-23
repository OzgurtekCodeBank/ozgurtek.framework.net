namespace ozgurtek.framework.core.Data
{
    public interface IGdField
    {
        string FieldName { get; set; }

        GdDataType FieldType { get; set; }

        bool PrimaryKey { get; set; }

        bool NotNull { get; set; }

        string DefaultVal { get; set; }

        int Srid { get; set; }

        GdGeometryType? GeometryType { get; set; }

        IGdKeyValueSet Domain { get; }
    }
}
