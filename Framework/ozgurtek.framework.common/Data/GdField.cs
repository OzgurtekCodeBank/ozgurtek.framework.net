using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.common.Data
{
    public class GdField : IGdField
    {
        private string _fieldName;
        private GdDataType _fieldType;
        private bool _primaryKey;
        private bool _notNull;
        private string _defaultVal;//todo: bu object olacak
        private int _srid;
        private GdGeometryType? _geometryType;
        private IGdKeyValueSet _domain;

        public GdField()
        {
        }

        public GdField(string fieldName, GdDataType fieldType)
        {
            _fieldName = fieldName;
            _fieldType = fieldType;
        }

        public virtual string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        public virtual GdDataType FieldType
        {
            get { return _fieldType; }
            set { _fieldType = value; }
        }

        public virtual bool PrimaryKey
        {
            get { return _primaryKey; }
            set { _primaryKey = value; }
        }

        public virtual bool NotNull
        {
            get { return _notNull; }
            set { _notNull = value; }
        }

        public virtual string DefaultVal
        {
            get { return _defaultVal; }
            set { _defaultVal = value; }
        }

        public virtual GdGeometryType? GeometryType
        {
            get { return _geometryType; }
            set { _geometryType = value; }
        }

        public virtual IGdKeyValueSet Domain
        {
            get => _domain;
            set => _domain = value;
        }

        public virtual int Srid
        {
            get => _srid;
            set => _srid = value;
        }

        public override string ToString()
        {
            return FieldName;
        }
    }
}
