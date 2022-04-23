using System;
using Xamarin.Forms;

namespace ozgurtek.framework.ui.map.skiasharp.Touch
{
    public class GdTouchActionEventArgs : EventArgs
    {
        public GdTouchActionEventArgs(long id, GdTouchActionType type, Point location, bool isInContact)
        {
            Id = id;
            Type = type;
            Location = location;
            IsInContact = isInContact;
        }

        public long Id { private set; get; }

        public GdTouchActionType Type { private set; get; }

        public Point Location { private set; get; }

        public bool IsInContact { private set; get; }
    }
}