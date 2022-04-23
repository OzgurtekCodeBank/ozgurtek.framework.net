using System;
using System.Collections.Generic;
using ozgurtek.framework.ui.controls.xamarin.Models;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Rg.Plugins.Popup.Pages;
using Xamarin.Essentials;
using Xamarin.Forms;
using DeviceInfo = Xamarin.Essentials.DeviceInfo;

namespace ozgurtek.framework.ui.controls.xamarin.Pages
{
    public abstract class GdPage : PopupPage
    {
        public StackLayout DialogTopBar;
        public GdImageButton TopBarLeftButton;
        public GdImageButton TopBarRightButton;
        public Label DialogLabel;
        public Frame DialogContent;
        public Button DialogAcceptButton;
        public Button DialogCancelButton;
        public Position DialogPosition { get; set; } = Position.Center;
        public GdPageSize WidthSize { get; set; }
        public GdPageSize HeightSize { get; set; }
        public bool FullScreenIfNoDesktop { get; set; } = false;
        public Dictionary<string, object> Tags = new Dictionary<string, object>();

        public enum Position
        {
            Left,
            Center,
            Right
        }

        protected GdPage()
        {
            //top bar
            DialogTopBar = new StackLayout
            {
                HeightRequest = 50,
                BackgroundColor = Color.White,
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, 0, 0, 0)
            };

            //top bar left button
            TopBarLeftButton = new GdImageButton
            {
                HeightRequest = 20,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = false,
                //Source = FileImageExtension.ConvertIS("double_left_blue.png")
            };
            DialogTopBar.Children.Add(TopBarLeftButton);

            //top bar caption
            DialogLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.Black,
                HorizontalTextAlignment = TextAlignment.Center
            };
            DialogTopBar.Children.Add(DialogLabel);

            //top bar right button
            TopBarRightButton = new GdImageButton
            {
                HeightRequest = 20,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = false
            };
            DialogTopBar.Children.Add(TopBarRightButton);

            //content
            DialogContent = new Frame
            {
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                CornerRadius = 0,
                Padding = 5
            };

            //main layout
            StackLayout mainLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                Spacing = 0
            };

            //dialog buttons
            StackLayout buttonsLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                IsVisible = true,
                Padding = 5
            };

            StackLayout innerLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.End,
            };

            DialogAcceptButton = new Button
            {
                Text = "Ok",
                IsVisible = false
            };

            DialogCancelButton = new Button
            {
                Text = "Cancel",
                IsVisible = false
            };

            innerLayout.Children.Add(DialogAcceptButton);
            innerLayout.Children.Add(DialogCancelButton);
            buttonsLayout.Children.Add(innerLayout);

            mainLayout.Children.Add(DialogTopBar);
            mainLayout.Children.Add(DialogContent);
            mainLayout.Children.Add(buttonsLayout);

            mainLayout.BindingContext = this;

            Content = mainLayout;
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (FullScreenIfNoDesktop && (DeviceInfo.Idiom != DeviceIdiom.Desktop))
            {
                WidthSize = null;
                HeightSize = null;
            }

            double scale = 1.0;
            double layoutWidth = width;
            double layoutHeight = height;

            if (WidthSize != null)
            {
                layoutWidth = WidthSize.GetCalculatedSize(layoutWidth) * scale;
            }

            if (HeightSize != null)
            {
                layoutHeight = HeightSize.GetCalculatedSize(layoutHeight) * scale;
                HasSystemPadding = false;
            }
            else
            {
                HasSystemPadding = true;
            }

            double xPos = 0, yPos = 0;
            switch (DialogPosition)
            {
                case Position.Center:
                    xPos = (width - layoutWidth) / 2;
                    yPos = (height - layoutHeight) / 2;
                    break;
                case Position.Left:
                    xPos = 0;
                    yPos = 0;
                    break;
                case Position.Right:
                    xPos = width - layoutWidth;
                    yPos = 0;
                    break;
            }

            base.LayoutChildren(x + xPos, y + yPos, layoutWidth, layoutHeight);
        }

        public string Caption
        {
            get => DialogLabel.Text;
            set => DialogLabel.Text = value;
        }
    }
}