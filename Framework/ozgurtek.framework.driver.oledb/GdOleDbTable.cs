using System;
using System.Data;
using System.Data.OleDb;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common.Data;
using ozgurtek.framework.core.Data;

namespace ozgurtek.framework.driver.oledb
{
    public class GdOleDbTable : GdAbstractDbTable
    {
        public GdOleDbTable(GdAbstractDbDataSource dataSource, string name, IGdFilter filter) 
            : base(dataSource, name, filter)
        {
        }

        public override bool CanEditRow
        {
            get { return false; }
        }

        public override bool CanEditField
        {
            get { return false; }
        }

        public override bool CanSupportTransaction
        {
            get { return false; }
        }

        public override IGdTable Clone()
        {
            return new GdOleDbTable(DataSource, Name, _filter);
        }

        protected override GdDataType? GetFieldType(DataRow row)
        {
            return null;
        }

        protected override long ExecuteDelete(long id, IDbCommand command)
        {
            throw new NotSupportedException();
        }

        protected override void ExecuteTruncate(IDbCommand command)
        {
            throw new NotSupportedException();
        }

        protected override GdRowBuffer CreateRowBuffer()
        {
            return new GdOleDbRowBuffer();
        }

        protected override IDbDataParameter CreateParameter(IGdParamater parameter)
        {
            OleDbParameter result = new OleDbParameter();
            result.ParameterName = parameter.Name;

            if (DbConvert.IsDbNull(parameter.Value))
            {
                result.Value = DBNull.Value;
            }
            else if (parameter.Value is Geometry geometry)
            {
                result.OleDbType = OleDbType.Binary;
                result.Value = DbConvert.ToGeometry(geometry);
            }
            else
            {
                result.Value = parameter.Value;
            }

            //force set datatype
            if (parameter.DataType.HasValue)
            {
                switch (parameter.DataType)
                {
                    case GdDataType.Blob:
                    case GdDataType.Geometry:
                        result.OleDbType = OleDbType.Binary;
                        break;
                    case GdDataType.Boolean:
                        result.OleDbType = OleDbType.Boolean;
                        break;
                    case GdDataType.Date:
                        result.OleDbType = OleDbType.Date;
                        break;
                    case GdDataType.Integer:
                        result.OleDbType = OleDbType.Integer;
                        break;
                    case GdDataType.Real:
                        result.OleDbType = OleDbType.Double;
                        break;
                    case GdDataType.String:
                        result.OleDbType = OleDbType.VarChar;
                        break;
                }
            }

            return result;
        }

        protected override void EndAppend(IGdFilter filter)
        {
            string columnFilter = "*";
            if (!string.IsNullOrEmpty(ColumnFilter))
                columnFilter = ColumnFilter;

            string orderBy = "";
            if (!string.IsNullOrEmpty(OrderBy))
                orderBy = " ORDER BY " + OrderBy;

            //todo: offset desteklenmiyor çözüm bulmak lazım.

            string limit = "";
            if (_limit >= 0)
                limit = " TOP " + Limit;

            filter.Text = $"SELECT {limit} {columnFilter} FROM ({filter.Text}) A {orderBy}";
        }

        protected override int ExecuteDeleteField(IGdField field, IDbCommand command)
        {
            throw new NotSupportedException();
        }

        protected override int ExecuteCreateField(IGdField field, IDbCommand command)
        {
            throw new NotSupportedException();
        }

        protected override long ExecuteUpdate(IGdRowBuffer row, IDbCommand command)
        {
            throw new NotSupportedException();
        }

        protected override long ExecuteInsert(IGdRowBuffer row, IDbCommand command)
        {
            throw new NotSupportedException();
        }

        public GdOleDbDataSource DataSource
        {
            get { return (GdOleDbDataSource)_dataSource; }
        }
    }
}
