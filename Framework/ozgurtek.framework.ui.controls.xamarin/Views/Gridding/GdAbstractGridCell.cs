using ozgurtek.framework.core.Data;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Gridding
{
    public abstract class GdAbstractGridCell : Frame
    {
        private readonly IGdTable _table;
        private readonly IGdRow _row;
        private readonly int _rowNum;
        private readonly int _columnNum;
        private bool _cancel;

        protected GdAbstractGridCell(IGdTable table, IGdRow row, int rowNum, int columnNum)
        {
            _table = table;
            _row = row;
            _rowNum = rowNum;
            _columnNum = columnNum;
        }

        public IGdRow Row
        {
            get { return _row; }
        }

        public IGdTable Table
        {
            get { return _table; }
        }

        public int RowNum
        {
            get { return _rowNum; }
        }

        public int ColumnNum
        {
            get { return _columnNum; }
        }

        public bool Cancel
        {
            get { return _cancel; }
        }

        public void CancelRenderCell()
        {
            _cancel = true;
        }
    }
}
