using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;

namespace ozgurtek.framework.common.Data
{
    public class GdRowBuffer : IGdRowBuffer, IGdRow
    {
        protected IGdTable _table;

        protected readonly Dictionary<string, IGdParamater> Row = new Dictionary<string, IGdParamater>(StringComparer.OrdinalIgnoreCase);

        public GdRowBuffer()
        {
        }

        public GdRowBuffer(IGdRow row)
        {
            CopyFrom(row);
        }

        public virtual void Put(string key, string value)
        {
            AddOrReplace(key, value);
        }

        public virtual void Put(string key, long value)
        {
            AddOrReplace(key, value);
        }

        public virtual void Put(string key, double value)
        {
            AddOrReplace(key, value);
        }

        public virtual void Put(string key, byte[] value)
        {
            AddOrReplace(key, value);
        }

        public virtual void Put(string key, Geometry value)
        {
            AddOrReplace(key, value);
        }

        public virtual void Put(string key, DateTime value)
        {
            AddOrReplace(key, value);
        }

        public virtual void Put(string key, bool value)
        {
            AddOrReplace(key, value);
        }

        public virtual void PutNull(string key)
        {
            AddOrReplace(key, null);
        }

        public virtual void Put(string key, object value, GdDataType dataType)
        {
            AddOrReplace(key, value, dataType);
        }

        public virtual int Size()
        {
            return Row.Count;
        }

        public virtual void Remove(string key)
        {
            Row.Remove(key);
        }

        public virtual void Clear()
        {
            Row.Clear();
        }

        public virtual bool ContainsKey(string key)
        {
            return Row.ContainsKey(key);
        }

        public virtual IGdTable Table
        {
            get { return _table; }
            set { _table = value; }
        }

        public virtual string GetAsString(string key)
        {
            return DbConvert.ToString(Row[key].Value);
        }

        public virtual long GetAsInteger(string key)
        {
            return DbConvert.ToInt64(Row[key].Value);
        }

        public virtual double GetAsReal(string key)
        {
            return DbConvert.ToDouble(Row[key].Value);
        }

        public virtual byte[] GetAsBlob(string key)
        {
            return DbConvert.ToBytes(Row[key].Value);
        }

        public virtual DateTime GetAsDate(string key)
        {
            return DbConvert.ToDateTime(Row[key].Value);
        }

        public virtual bool GetAsBoolean(string key)
        {
            return DbConvert.ToBoolean(Row[key].Value);
        }

        Geometry IGdRowBuffer.GetAsGeometry(string key)
        {
            return GetAsGeometry(key);
        }

        public virtual Geometry GetAsGeometry(string key)
        {
            return DbConvert.ToGeometry(Row[key].Value);
        }

        public virtual bool IsNull(string key)
        {
            return DbConvert.IsDbNull(Row[key].Value);
        }

        public IEnumerable<IGdParamater> Paramaters
        {
            get
            {
                return Row.Values;
            }
        }

        public virtual void Put(string key, bool? value)
        {
            if (value.HasValue)
                Put(key, value.Value);
            else
                PutNull(key);
        }

        public virtual void Put(string key, DateTime? value)
        {
            if (value.HasValue)
                Put(key, value.Value);
            else
                PutNull(key);
        }

        public virtual void Put(string key, long? value)
        {
            if (value.HasValue)
                Put(key, value.Value);
            else
                PutNull(key);
        }

        public virtual void Put(string key, double? value)
        {
            if (value.HasValue)
                Put(key, value.Value);
            else
                PutNull(key);
        }

        private void AddOrReplace(string key, object value, GdDataType? dataType = null)
        {
            if (Row.ContainsKey(key))
                Row[key] = new GdParameter(key, value, dataType);
            else
                Row.Add(key, new GdParameter(key, value, dataType));
        }

        //todo: enis bu kötü
        internal virtual void Put(string key, object value)
        {
            AddOrReplace(key, value);
        }

        //todo: enis bu kötü
        internal virtual IGdParamater Get(string key)
        {
            return Row[key];
        }

        private void CopyFrom(IGdRow row)
        {
            Table = row.Table;
            IGdSchema schema = row.Table.Schema;
            foreach (IGdField field in schema.Fields)
            {
                string fieldName = field.FieldName;
                if (row.IsNull(fieldName))
                {
                    PutNull(fieldName);
                    continue;
                }

                GdDataType dataType = field.FieldType;
                switch (dataType)
                {
                    case GdDataType.Boolean:
                        Put(fieldName, row.GetAsBoolean(fieldName));
                        break;
                    case GdDataType.Date:
                        Put(fieldName, row.GetAsDate(fieldName));
                        break;
                    case GdDataType.Integer:
                        Put(fieldName, row.GetAsInteger(fieldName));
                        break;
                    case GdDataType.Real:
                        Put(fieldName, row.GetAsReal(fieldName));
                        break;
                    case GdDataType.String:
                        Put(fieldName, row.GetAsString(fieldName));
                        break;
                    case GdDataType.Blob:
                        Put(fieldName, row.GetAsBlob(fieldName));
                        break;
                    case GdDataType.Geometry:
                        Put(fieldName, row.GetAsGeometry(field.FieldName));
                        break;
                }
            }
        }
    }
}