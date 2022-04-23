using Xamarin.Forms;

namespace ozgurtek.framework.ui.map.skiasharp.Touch
{
    public class GdTouchEffect : RoutingEffect
    {
        public event GdTouchActionEventHandler TouchAction;

        public GdTouchEffect() : base("XamarinDocs.TouchEffect")
        {
        }

        public bool Capture { set; get; }

        public void OnTouchAction(Element element, GdTouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }
}