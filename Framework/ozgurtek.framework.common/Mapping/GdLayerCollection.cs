using ozgurtek.framework.core.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ozgurtek.framework.common.Mapping
{
    public class GdLayerCollection : IGdLayerCollection
    {
        private IGdLayer _activeLayer;
        private readonly List<IGdLayer> _layerList = new List<IGdLayer>();

        #region Implementation of IGdLayerCollection

        public int Count
        {
            get { return _layerList.Count; }
        }

        public IGdLayer this[int index]
        {
            get { return _layerList[index]; }
        }

        public void Add(IGdLayer layer)
        {
            _layerList.Add(layer);
            LayerAdded?.Invoke(this, new core.Mapping.GdLayerEventArgs(layer));
        }

        public void AddRange(IEnumerable<IGdLayer> layers)
        {
            List<IGdLayer> temp = new List<IGdLayer>(layers);
            _layerList.AddRange(temp);
            LayerAdded?.Invoke(this, new core.Mapping.GdLayerEventArgs(temp));
        }

        public void Clear()
        {
            _layerList.Clear();
            if (Reset != null)
                Reset(this, null);

            ActiveLayer = null;
        }

        public bool Contains(IGdLayer layer)
        {
            return _layerList.Contains(layer);
        }

        public int IndexOf(IGdLayer layer)
        {
            return _layerList.IndexOf(layer);
        }

        public void Insert(int index, IGdLayer layer)
        {
            _layerList.Insert(index, layer);
            LayerAdded?.Invoke(this, new core.Mapping.GdLayerEventArgs(layer));
        }

        public bool Remove(IGdLayer layer)
        {
            int index = IndexOf(layer);
            if (index < 0)
                return false;

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            IGdLayer layer = _layerList[index];
            _layerList.RemoveAt(index);
            LayerRemoved?.Invoke(this, new core.Mapping.GdLayerEventArgs(layer));

            if (!layer.Equals(_activeLayer))
                return;

            int activeIndex = index;
            if (activeIndex > _layerList.Count - 1)
                activeIndex--;

            ActiveLayer = (activeIndex >= 0) ? _layerList[activeIndex] : null;
        }

        public IGdLayer[] ToArray()
        {
            return _layerList.ToArray();
        }

        public IGdLayer ActiveLayer
        {
            get
            {
                return _activeLayer;
            }
            set
            {
                if (value != null && value.Equals(_activeLayer))
                    return;

                _activeLayer = value;
                if (ActiveLayerChanged != null)
                    ActiveLayerChanged(this, EventArgs.Empty);
            }
        }

        public IGdLayer[] FindLayer(string name)
        {
            List<IGdLayer> result = new List<IGdLayer>();
            foreach (IGdLayer layer in _layerList)
            {
                if (string.Compare(layer.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    result.Add(layer);
            }
            return result.ToArray();
        }

        public event EventHandler<core.Mapping.GdLayerEventArgs> LayerAdded;
        public event EventHandler<core.Mapping.GdLayerEventArgs> LayerRemoved;
        public event EventHandler Reset;
        public event EventHandler ActiveLayerChanged;

        #endregion        

        #region Implementation of IEnumerable

        public IEnumerator<IGdLayer> GetEnumerator()
        {
            return _layerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
