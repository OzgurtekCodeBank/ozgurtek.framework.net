using ozgurtek.framework.common.Util;
using ozgurtek.framework.core.Data;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Gridding
{
    public class GdGrid : StackLayout
    {
        private bool _paging;
        private IGdTable _table;
        private long _totalRowCount;
        private readonly Grid _grid;
        private double _rowHeight = 30;
        private Button _addRowButton;
        private bool _showHeader = true;
        private long _rowPerRowCountPerPage = 50;

        private readonly StackLayout _navLayout;
        public EventHandler<GdAbstractGridCell> Rendering;
        private readonly List<GdGridColumnDefination> _columnDefinations = new List<GdGridColumnDefination>();

        private long _currentPage;

        public GdGrid()
        {
            _grid = new Grid();
            _grid.RowSpacing = 0;
            _grid.ColumnSpacing = 0;

            _navLayout = CreateNavButtons();

            ScrollView scrollView = new ScrollView();
            scrollView.Content = _grid;

            Children.Add(scrollView);
            Children.Add(_navLayout);
        }

        public IGdTable Table
        {
            get { return _table; }
            set
            {
                _table = value;

                Rendering = null;

                _grid.ColumnDefinitions.Clear();
                _grid.RowDefinitions.Clear();
                _grid.Children.Clear();

                _columnDefinations.Clear();
                foreach (IGdField field in _table.Schema.Fields)
                {
                    GdGridColumnDefination defination = new GdGridColumnDefination();
                    defination.Type = GdGridColumnType.Table;
                    defination.Caption = field.FieldName;
                    _columnDefinations.Add(defination);
                }
            }
        }

        public void Render()
        {
            _totalRowCount = 0;
            _currentPage = 0;

            if (ShowHeader)
                FillHeader();

            FillTable();
        }

        public bool Paging
        {
            get { return _paging; }
            set
            {
                _paging = value;
                _navLayout.IsVisible = value;
            }
        }

        public List<GdGridColumnDefination> ColumnDefinations
        {
            get { return _columnDefinations; }
        }

        public long RowCountPerPage
        {
            get => _rowPerRowCountPerPage;
            set => _rowPerRowCountPerPage = value;
        }

        public bool ShowHeader
        {
            get => _showHeader;
            set => _showHeader = value;
        }

        public double RowHeight
        {
            get => _rowHeight;
            set => _rowHeight = value;
        }

        private void FillHeader()
        {
            int i = -1;
            foreach (GdGridColumnDefination defination in ColumnDefinations)
            {
                GdLabelGridCell gridCell = new GdLabelGridCell(null, null, 0, 0);
                gridCell.Label.FontAttributes = FontAttributes.Bold;
                gridCell.BackgroundColor = Color.WhiteSmoke;
                gridCell.Text = defination.Caption;
                _grid.Children.Add(gridCell, ++i, 0);
            }
        }

        private void FillTable()
        {
            int rownum = -1;
            long start = _currentPage * _rowPerRowCountPerPage;
            long end = start + _rowPerRowCountPerPage;
            foreach (IGdRow row in _table.Rows)
            {
                rownum++;
                if (rownum < start)
                    continue;

                if (rownum >= end)
                    break;

                _totalRowCount++;
                int colnum = -1;
                foreach (GdGridColumnDefination defination in ColumnDefinations)
                {
                    colnum++;
                    string value = string.Empty;
                    GdAbstractGridCell result;

                    int rnum = rownum;
                    if (ShowHeader)
                        rnum += 1;

                    if (defination.Type == GdGridColumnType.Table)
                    {
                        if (!row.IsNull(defination.Field))
                            value = row.GetAsString(defination.Field);

                        GdLabelGridCell gridCell = new GdLabelGridCell(_table, row, colnum, rnum);
                        gridCell.Text = value;
                        result = gridCell;
                    }
                    else if (defination.Type == GdGridColumnType.Custom)
                    {
                        GdCustomGridCell cell = new GdCustomGridCell(_table, row, colnum, rnum);
                        result = cell;
                    }
                    else if (defination.Type == GdGridColumnType.Calculated)
                    {
                        GdLabelGridCell gridCell = new GdLabelGridCell(_table, row, colnum, rnum);
                        GdLabelFormatBuilder formatBuilderBuilder = new GdLabelFormatBuilder(_table);
                        string resolveFormat = formatBuilderBuilder.ResolveFormat(row, defination.Field);
                        gridCell.Text = resolveFormat;
                        result = gridCell;
                    }
                    else
                        throw new Exception("undefined grid cell type");

                    OnRendering(result);

                    if (!result.Cancel)
                        _grid.Children.Add(result, colnum, rnum);
                }

                _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(_rowHeight) });
            }

            _addRowButton.Text = _totalRowCount.ToString();
        }

        protected void OnRendering(GdAbstractGridCell cell)
        {
            if (Rendering != null)
                Rendering(this, cell);
        }

        private StackLayout CreateNavButtons()
        {
            StackLayout navButtonsLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.End,
            };

            _addRowButton = new Button()
            {
                WidthRequest = 100,
                HeightRequest = 30
            };

            _addRowButton.Clicked += (sender, args) =>
            {
                _currentPage++;
                FillTable();
            };
            navButtonsLayout.Children.Add(_addRowButton);
            navButtonsLayout.IsVisible = Paging;

            return navButtonsLayout;
        }
    }
}