using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using ozgurtek.framework.test.xamarin.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("XamarinDocs")]
[assembly: ExportEffect(typeof(GdTouchEffect), "TouchEffect")]
namespace ozgurtek.framework.test.xamarin.iOS
{
    public class GdTouchEffect : PlatformEffect
    {
        UIView view;
        GdTouchRecognizer touchRecognizer;

        protected override void OnAttached()
        {
            // Get the iOS UIView corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchEffect effect = (ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchEffect)Element.Effects.FirstOrDefault(e => e is ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchEffect);

            if (effect != null && view != null)
            {
                // Create a TouchRecognizer for this UIView
                touchRecognizer = new GdTouchRecognizer(Element, view, effect);
                view.AddGestureRecognizer(touchRecognizer);
            }
        }

        protected override void OnDetached()
        {
            if (touchRecognizer != null)
            {
                // Clean up the TouchRecognizer object
                touchRecognizer.Detach();

                // Remove the TouchRecognizer from the UIView
                view.RemoveGestureRecognizer(touchRecognizer);
            }
        }
    }

    class GdTouchRecognizer : UIGestureRecognizer
    {
        Element element;        // Forms element for firing events
        UIView view;            // iOS UIView 
        ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchEffect touchEffect;
        bool capture;

        static Dictionary<UIView, GdTouchRecognizer> viewDictionary =
            new Dictionary<UIView, GdTouchRecognizer>();

        static Dictionary<long, GdTouchRecognizer> idToTouchDictionary =
            new Dictionary<long, GdTouchRecognizer>();

        public GdTouchRecognizer(Element element, UIView view, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchEffect touchEffect)
        {
            this.element = element;
            this.view = view;
            this.touchEffect = touchEffect;

            viewDictionary.Add(view, this);
        }

        public void Detach()
        {
            viewDictionary.Remove(view);
        }

        // touches = touches of interest; evt = all touches of type UITouch
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();
                FireEvent(this, id, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Pressed, touch, true);

                if (!idToTouchDictionary.ContainsKey(id))
                {
                    idToTouchDictionary.Add(id, this);
                }
            }

            // Save the setting of the Capture property
            capture = touchEffect.Capture;
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Moved, touch, true);
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (idToTouchDictionary[id] != null)
                    {
                        FireEvent(idToTouchDictionary[id], id, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Moved, touch, true);
                    }
                }
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Released, touch, false);
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (idToTouchDictionary[id] != null)
                    {
                        FireEvent(idToTouchDictionary[id], id, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Released, touch, false);
                    }
                }
                idToTouchDictionary.Remove(id);
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Cancelled, touch, false);
                }
                else if (idToTouchDictionary[id] != null)
                {
                    FireEvent(idToTouchDictionary[id], id, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Cancelled, touch, false);
                }
                idToTouchDictionary.Remove(id);
            }
        }

        void CheckForBoundaryHop(UITouch touch)
        {
            long id = touch.Handle.ToInt64();

            // TODO: Might require converting to a List for multiple hits
            GdTouchRecognizer recognizerHit = null;

            foreach (UIView view in viewDictionary.Keys)
            {
                CGPoint location = touch.LocationInView(view);

                if (new CGRect(new CGPoint(), view.Frame.Size).Contains(location))
                {
                    recognizerHit = viewDictionary[view];
                }
            }
            if (recognizerHit != idToTouchDictionary[id])
            {
                if (idToTouchDictionary[id] != null)
                {
                    FireEvent(idToTouchDictionary[id], id, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Exited, touch, true);
                }
                if (recognizerHit != null)
                {
                    FireEvent(recognizerHit, id, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType.Entered, touch, true);
                }
                idToTouchDictionary[id] = recognizerHit;
            }
        }

        void FireEvent(GdTouchRecognizer recognizer, long id, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionType actionType, UITouch touch, bool isInContact)
        {
            // Convert touch location to Xamarin.Forms Point value
            CGPoint cgPoint = touch.LocationInView(recognizer.View);
            Point xfPoint = new Point(cgPoint.X, cgPoint.Y);

            // Get the method to call for firing events
            Action<Element, ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionEventArgs> onTouchAction = recognizer.touchEffect.OnTouchAction;

            // Call that method
            onTouchAction(recognizer.element,
                new ozgurtek.framework.ui.map.skiasharp.Touch.GdTouchActionEventArgs(id, actionType, xfPoint, isInContact));
        }
    }

}