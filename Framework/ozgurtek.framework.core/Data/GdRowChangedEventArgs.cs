using System;

namespace ozgurtek.framework.core.Data
{
    public class GdRowChangedEventArgs : EventArgs
    {
        private readonly IGdTable _table;
        private readonly string _type;
        private readonly IGdRowBuffer _buffer;

        public GdRowChangedEventArgs(IGdTable table, string type, IGdRowBuffer buffer)
        {
            _table = table;
            _type = type;
            _buffer = buffer;
        }

        public IGdTable Table
        {
            get { return _table; }
        }

        public string Type
        {
            get { return _type; }
        }

        public IGdRowBuffer Buffer
        {
            get { return _buffer; }
        }
    }
}
