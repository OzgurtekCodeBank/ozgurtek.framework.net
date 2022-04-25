using System;
using System.Linq;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using mersin.ibs.mobile.UWP;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;


[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(TouchEffect), "TouchEffect")]
namespace mersin.ibs.mobile.UWP
{
    public class TouchEffect : PlatformEffect
    {
        FrameworkElement frameworkElement;
        ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchEffect effect;
        Action<Element, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionEventArgs> onTouchAction;

        protected override void OnAttached()
        {
            // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
            frameworkElement = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            effect = (ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchEffect)Element.Effects.
                        FirstOrDefault(e => e is ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchEffect);

            if (effect != null && frameworkElement != null)
            {
                // Save the method to call on touch events
                onTouchAction = effect.OnTouchAction;

                // Set event handlers on FrameworkElement
                frameworkElement.PointerEntered += OnPointerEntered;
                frameworkElement.PointerPressed += OnPointerPressed;
                frameworkElement.PointerMoved += OnPointerMoved;
                frameworkElement.PointerReleased += OnPointerReleased;
                frameworkElement.PointerExited += OnPointerExited;
                frameworkElement.PointerCanceled += OnPointerCancelled;
            }
        }

        protected override void OnDetached()
        {
            if (onTouchAction != null)
            {
                // Release event handlers on FrameworkElement
                frameworkElement.PointerEntered -= OnPointerEntered;
                frameworkElement.PointerPressed -= OnPointerPressed;
                frameworkElement.PointerMoved -= OnPointerMoved;
                frameworkElement.PointerReleased -= OnPointerReleased;
                frameworkElement.PointerExited -= OnPointerEntered;
                frameworkElement.PointerCanceled -= OnPointerCancelled;
            }
        }

        void OnPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Entered, args);
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Pressed, args);

            // Check setting of Capture property
            if (effect.Capture)
            {
                (sender as FrameworkElement).CapturePointer(args.Pointer);
            }
        }

        void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Moved, args);
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Released, args);
        }

        void OnPointerExited(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Exited, args);
        }

        void OnPointerCancelled(object sender, PointerRoutedEventArgs args)
        {
            CommonHandler(sender, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Cancelled, args);
        }

        void CommonHandler(object sender, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType touchActionType, PointerRoutedEventArgs args)
        {
            PointerPoint pointerPoint = args.GetCurrentPoint(sender as UIElement);
            Windows.Foundation.Point windowsPoint = pointerPoint.Position;

            onTouchAction(Element, new ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionEventArgs(args.Pointer.PointerId,
                                                            touchActionType,
                                                            new Point(windowsPoint.X, windowsPoint.Y),
                                                            args.Pointer.IsInContact));
        }
    }

}
