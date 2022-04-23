using NetTopologySuite.Geometries;
using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ozgurtek.framework.ui.map.skiasharp.Touch;
using Xamarin.Forms;
using Point = Xamarin.Forms.Point;

namespace ozgurtek.framework.ui.map.skiasharp
{
    public class GdSkMap : AbsoluteLayout, IGdMap
    {
        private GdSkMapInternal _map;
        private ObservableCollection<IGdAdornment> _adornments;

        public GdSkMap()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _adornments = new ObservableCollection<IGdAdornment>();
            _adornments.CollectionChanged += Adornments_CollectionChanged;

            _map = new GdSkMapInternal();
            _map.Viewport.Changed += ViewPortChanged;

            SetLayoutBounds(_map, new Rectangle(-1, -1, 1, 1));
            SetLayoutFlags(_map, AbsoluteLayoutFlags.SizeProportional);
            Children.Add(_map);
            
            GdTouchEffect effect = new GdTouchEffect();
            effect.TouchAction += _map.OnTouchEffectAction;
            effect.Capture = true;
            Effects.Add(effect);
        }

        public IGdViewport Viewport
        {
            get { return _map.Viewport; }
            set { _map.Viewport = value; }
        }

        public IGdLayerCollection LayerCollection
        {
            get { return _map.LayerCollection; }
            set { _map.LayerCollection = value; }
        }

        public void Render(bool layer = true, bool marker = true)
        {
            _map.Render(layer, marker);
        }

        public GdColor BackColor
        {
            get { return _map.BackColor; }
            set { _map.BackColor = value; }
        }

        public void AbortRender()
        {
            _map.AbortRender();
        }

        public IGdMapController Controller
        {
            get { return _map.Controller; }
            set { _map.Controller = value; }
        }

        public IList<IGdMarker> Markers
        {
            get { return _map.Markers; }
            set { _map.Markers = value; }
        }

        public ObservableCollection<IGdAdornment> Adornments
        {
            get { return _adornments; }
        }

        public bool AllowPanController
        {
            get => _map.AllowPanController;
            set => _map.AllowPanController = value;
        }

        public bool AllowPinchController
        {
            get => _map.AllowPinchController;
            set => _map.AllowPinchController = value;
        }

        public bool AllowMouseWheelController
        {
            get => _map.AllowMouseWheelController;
            set => _map.AllowMouseWheelController = value;
        }

        public bool AllowDoubleTappedController
        {
            get => _map.AllowDoubleTappedController;
            set => _map.AllowDoubleTappedController = value;
        }
        
        private void Adornments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshAdorment();
        }

        private void ViewPortChanged(object sender, EventArgs e)
        {
            RefreshAdorment();
        }

        private void RefreshAdorment()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                //clear adornments
                List<View> adornmentsToDelete = Children.Where(c => c.ClassId == "adornment").ToList();
                foreach (View child in adornmentsToDelete)
                {
                    Children.Remove(child);
                }

                if (!_adornments.Any())
                    return;

                IGdViewport viewport = _map.Viewport;
                foreach (IGdAdornment adornment in _adornments)
                {
                    //dont show on screen
                    if (!viewport.World.Intersects(adornment.Point.Coordinate))
                        continue;

                    Coordinate pixel = viewport.WorldtoView(adornment.Point.Coordinate);
                    SetLayoutBounds(adornment.View, new Rectangle(new Point(pixel.X, pixel.Y), new Size(-1, -1)));
                    SetLayoutFlags(adornment.View, AbsoluteLayoutFlags.None);
                    adornment.View.ClassId = "adornment";

                    Children.Add(adornment.View);
                }
            });
        }
    }
}