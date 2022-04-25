using System;
using System.Timers;
using NetTopologySuite.Geometries;
using ozgurtek.framework.common;
using ozgurtek.framework.common.Geodesy;
using ozgurtek.framework.core.Mapping;
using ozgurtek.framework.test.xamarin.Pages;
using ozgurtek.framework.ui.controls.xamarin.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ozgurtek.framework.test.xamarin.Managers
{
    public static class Extensions
    {
        public static async void ShowPage(this GdPage page, object initParam)
        {
            LoadingPopup loadingPopup = new LoadingPopup();
            await PopupNavigation.Instance.PushAsync(loadingPopup);
            
            try
            {
                if (!page.Tags.ContainsKey("InitParam"))
                    page.Tags.Add("InitParam", initParam);
                else
                    page.Tags["InitParam"] = initParam;

                await PopupNavigation.Instance.PushAsync(page, false);
            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.RemovePageAsync(loadingPopup);
            }
            finally
            {
                await PopupNavigation.Instance.RemovePageAsync(loadingPopup);
            }
        }

        public static void ClosePage(this GdPage page)
        {
            PopupNavigation.Instance.RemovePageAsync(page, false);
        }

        public static object GetInitParam(this GdPage page)
        {
            return page.Tags["InitParam"];
        }

        public static void Goto(this IGdMap map, Coordinate world, double meter = 500)
        {
            //create project epsg:3857
            Coordinate coordinate = GdProjection.Project(world, map.Viewport.Srid, 3857);

            Coordinate ll = new Coordinate(coordinate.X - meter / 2, coordinate.Y + meter / 2); //sol alt
            Coordinate lr = new Coordinate(coordinate.X + meter / 2, coordinate.Y + meter / 2); //sağ alt
            Coordinate ur = new Coordinate(coordinate.X + meter / 2, coordinate.Y - meter / 2); //sağ ust
            Coordinate ul = new Coordinate(coordinate.X - meter / 2, coordinate.Y - meter / 2); //sol üst

            GeometryFactory geometryFactory = GdFactoryFinder.Instance.GeometryServices.CreateGeometryFactory();
            Polygon polygon = geometryFactory.CreatePolygon(new[] {ul, ur, lr, ll, ul});
            polygon.SRID = 3857;

            Geometry geometry = GdProjection.Project(polygon, map.Viewport.Srid);
            map.Viewport.World = geometry.EnvelopeInternal;

            map.Render();
        }

        public static void ShowTopbar(this GdPage page, string caption)
        {
            page.DialogTopBar.IsVisible = true;
            page.DialogTopBar.Padding = 10;
            page.DialogLabel.Text = caption;
            page.DialogLabel.FontSize = 18;
            page.DialogLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            page.DialogLabel.HorizontalTextAlignment = TextAlignment.Center;
            page.DialogLabel.TextColor = Color.Black;

            page.TopBarLeftButton.IsVisible = true;
            page.TopBarLeftButton.HeightRequest = 40;
            page.TopBarLeftButton.Source = ImageSource.FromFile("double_left_blue.png");
            page.TopBarLeftButton.Gesture.Tapped += (o, args) => { PopupNavigation.Instance.PopAsync(); };
        }

        public static void Toast(this View view)
        {
            view.Opacity = 0;
            view.IsVisible = true;
            view.FadeTo(1, 300);
            Timer closeViewTimer = new Timer()
            {
                Interval = 1000,
                AutoReset = false,
                Enabled = true,
            };
            closeViewTimer.Elapsed += (sender, args) =>
            {
                MainThread.BeginInvokeOnMainThread(() => { view.IsVisible = false; });
            };
            closeViewTimer.Start();
        }
    }
}