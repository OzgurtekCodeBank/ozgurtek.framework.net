using ozgurtek.framework.core.Data;
using ozgurtek.framework.core.Style;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Rectangle = Xamarin.Forms.Shapes.Rectangle;

namespace ozgurtek.framework.ui.controls.xamarin.Views.Style
{
    public class GdStylePreviewView : StackLayout
    {
        public GdStylePreviewView()
        {
            WidthRequest = 40;
            HeightRequest = 10;
        }

        //todo: enis-> ali bunu faydası nedir tartışalım
        public static readonly BindableProperty LayerStyleProperty = BindableProperty.Create(
            nameof(LayerStyle),
            typeof(IGdStyle),
            typeof(GdStylePreviewView),
            null,
            propertyChanged: LayerStylePropertyChanged);

        public IGdStyle LayerStyle
        {
            get { return (IGdStyle)GetValue(LayerStyleProperty); }
            set { SetValue(LayerStyleProperty, value); }
        }

        public static void LayerStylePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            GdStylePreviewView preview = (GdStylePreviewView)bindable;
            IGdStyle style = (IGdStyle)newvalue;

            if (style is IGdPolygonStyle polygonStyle)
            {
                DrawPolygon(polygonStyle, preview);
                return;
            }

            if (style is IGdLineStyle lineStyle)
            {
                DrawLine(lineStyle, preview);
                return;
            }

            if (style is IGdPointStyle pointStyle)
            {
                DrawPoint(pointStyle, preview);
            }
        }

        private static void DrawPolygon(IGdPolygonStyle polygonStyle, GdStylePreviewView preview)
        {
            Rectangle rectangle = new Rectangle();

            IGdFill fill = polygonStyle.Fill;
            if (fill != null && fill.Color.A > 0)
            {
                GdColor color = polygonStyle.Fill.Color;
                Color rgba = Color.FromRgba(color.R, color.G, color.B, color.A);
                rectangle.Fill = new SolidColorBrush(rgba);
            }

            IGdStroke stroke = polygonStyle.Stroke;
            if (stroke != null && stroke.Color.A > 0 && stroke.Width > 0)
            {
                rectangle.StrokeThickness = polygonStyle.Stroke.Width;
                GdColor color = polygonStyle.Stroke.Color;
                Color rgba = Color.FromRgba(color.R, color.G, color.B, color.A);
                rectangle.Stroke = new SolidColorBrush(rgba);
            }

            rectangle.HorizontalOptions = LayoutOptions.FillAndExpand;
            rectangle.VerticalOptions = LayoutOptions.FillAndExpand;
            preview.Children.Add(rectangle);
        }

        private static void DrawLine(IGdLineStyle lineStyle, GdStylePreviewView preview)
        {
            IGdStroke stroke = lineStyle.Stroke;
            if (stroke != null && stroke.Color.A > 0 && stroke.Width > 0)
            {
                Line line = new Line
                {
                    X1 = 0,
                    X2 = preview.WidthRequest,
                    Y1 = preview.HeightRequest, //todo: enis -> ali beklediğim gibi değil..
                    Y2 = preview.HeightRequest,
                };

                line.StrokeThickness = stroke.Width;
                GdColor color = stroke.Color;
                Color rgba = Color.FromRgba(color.R, color.G, color.B, color.A);
                line.Stroke = new SolidColorBrush(rgba);

                preview.Children.Add(line);
            }
        }

        private static void DrawPoint(IGdPointStyle pointStyle, GdStylePreviewView preview)
        {
            double w = preview.WidthRequest;
            double h = preview.HeightRequest;
            double s = w > h ? h : w;

            List<Shape> shapes = new List<Shape>();
            if (pointStyle.PointStleType == GdPointStyleType.Square)
            {
                Shape shape = new Rectangle();
                shape.HorizontalOptions = LayoutOptions.Center; //todo: enis -> ali beklediğim gibi değil..
                shape.VerticalOptions = LayoutOptions.Center;
                shape.WidthRequest = s;
                shape.HeightRequest = s;
                shapes.Add(shape);
            }
            else if (pointStyle.PointStleType == GdPointStyleType.Circle)
            {
                var shape = new Ellipse();
                shape.HorizontalOptions = LayoutOptions.Center; //todo: enis -> ali beklediğim gibi değil..
                shape.VerticalOptions = LayoutOptions.Center;
                shape.WidthRequest = s;
                shape.HeightRequest = s;
                shapes.Add(shape);
            }

            foreach (Shape shape in shapes)
            {
                IGdFill fill = pointStyle.Fill;
                if (fill != null && fill.Color.A > 0)
                {
                    GdColor color = fill.Color;
                    Color rgba = Color.FromRgba(color.R, color.G, color.B, color.A);
                    shape.Fill = new SolidColorBrush(rgba);
                }

                IGdStroke stroke = pointStyle.Stroke;
                if (stroke != null && stroke.Color.A > 0 && stroke.Width > 0)
                {
                    shape.StrokeThickness = stroke.Width;
                    GdColor color = stroke.Color;
                    Color rgba = Color.FromRgba(color.R, color.G, color.B, color.A);
                    shape.Stroke = new SolidColorBrush(rgba);
                }

                preview.Children.Add(shape);
            }
        }
    }
}
