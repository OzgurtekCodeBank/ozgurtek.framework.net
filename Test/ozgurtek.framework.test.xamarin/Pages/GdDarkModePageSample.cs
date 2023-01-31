using System;
using System.Collections.ObjectModel;
using ozgurtek.framework.test.xamarin.Managers;
using ozgurtek.framework.ui.controls.xamarin.Pages;
using ozgurtek.framework.ui.controls.xamarin.Views;
using Xamarin.Forms;

namespace ozgurtek.framework.test.xamarin.Pages
{
    class GdDarkModePageSample : GdPage
    {
        private Switch _switchControl;
        private GdListView _listView;

        public GdDarkModePageSample()
        {
            StackLayout stackLayout = new StackLayout();

            _switchControl = new Switch();
            _switchControl.OnColor = Color.GhostWhite;
            _switchControl.ThumbColor = Color.DimGray;
            _switchControl.IsToggled = false;
            _switchControl.Toggled += IsDarkToggled;

            stackLayout.Children.Add(_switchControl);

            GdViewBox viewBox = new GdViewBox();

            GdLabel label = new GdLabel();
            label.Text = "Test Dark Label";
            viewBox.AddItem("test1", label);

            //select page
            GdLabel selectPage = new GdLabel();
            selectPage.Text = "Click to see select page samples...";
            viewBox.AddItem("Select Page", selectPage);
            selectPage.Gesture.Tapped += SelectPageGestureOnTapped;

            GdLabel tabbPage = new GdLabel();
            tabbPage.Text = "Click to see Tabb page samples...";
            viewBox.AddItem("Tabb Page", tabbPage);
            tabbPage.Gesture.Tapped += TabbedPageGestureOnTapped;

            GdTextEntry textEntry = new GdTextEntry();
            viewBox.AddItem("Text Entry", textEntry);

            GdDoubleEntry doubleEntry = new GdDoubleEntry();
            viewBox.AddItem("Double Entry", doubleEntry);

            GdIntegerEntry integerEntry = new GdIntegerEntry();
            viewBox.AddItem("Integer Entry", integerEntry);

            GdPicker picker = new GdPicker();
            picker.CreateItem("test1", 1);
            picker.CreateItem("test2", 2);
            picker.SelectedIndex = 0;
            viewBox.AddItem("Picker", picker);

            GdButton button = new GdButton();
            viewBox.AddItem("Button", button);


            stackLayout.Children.Add(viewBox);

            DialogContent.Content = stackLayout;

            SetDarkMode(LogicalChildren);
            this.ShowTopbar("DArkMode Test");
        }

        private void SetDarkMode(ReadOnlyCollection<Element> elements)
        {
            foreach (Element el in elements)
            {
                if (el is Layout inputView)
                {
                    inputView.SetAppThemeColor(Xamarin.Forms.Layout.BackgroundColorProperty, Color.White, Color.Black);
                }

                if (el is View layout)
                {
                    layout.SetAppThemeColor(View.BackgroundColorProperty, Color.White, Color.Black);
                }

                if (el is InputView input)
                {
                    input.SetAppThemeColor(InputView.TextColorProperty, Color.Black, Color.White);
                }

                if (el is Label label)
                {
                    label.SetAppThemeColor(Label.TextColorProperty, Color.Black, Color.White);
                }

                if (el is Frame frame)
                {
                    frame.SetAppThemeColor(Frame.BorderColorProperty, Color.Black, Color.White);
                }

                SetDarkMode(el.LogicalChildren);
            }
        }

        private void IsDarkToggled(object sender, ToggledEventArgs e)
        {
            Switch dark = (Switch) sender;

            if (dark.IsToggled)
                Application.Current.UserAppTheme = OSAppTheme.Dark;
            else
            {
                Application.Current.UserAppTheme = OSAppTheme.Light;
            }
        }

        private void SelectPageGestureOnTapped(object sender, EventArgs e)
        {
            GdSelectPageSample1 sample1 = new GdSelectPageSample1();
            SetDarkMode(sample1.LogicalChildren);
            sample1.ShowPage(null);
        }

        private void TabbedPageGestureOnTapped(object sender, EventArgs e)
        {
            // GdTabbedPageSample1 tabMenu = new GdTabbedPageSample1();
            // tabMenu.ShowPage(null);
        }
    }
}