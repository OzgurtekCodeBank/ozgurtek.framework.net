using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ozgurtek.framework.ui.controls.xamarin.Helper
{
    [ContentProperty(nameof(Path))]
    public class FileImageExtension : IMarkupExtension<FileImageSource>
    {
        public string Path { get; set; }

        public FileImageSource ProvideValue(IServiceProvider serviceProvider) => Convert(Path);

        public static FileImageSource Convert(string path)
        {
            if (path == null) throw new InvalidOperationException($"Cannot convert null to {typeof(ImageSource)}");

            if (Device.RuntimePlatform == Device.UWP)
            {
                path = System.IO.Path.Combine("Assets/", path);
            }
            return (FileImageSource)ImageSource.FromFile(path);
        }

        public static ImageSource ConvertIS(string path)
        {
            if (path == null) throw new InvalidOperationException($"Cannot convert null to {typeof(ImageSource)}");

            if (Device.RuntimePlatform == Device.UWP)
            {
                path = System.IO.Path.Combine("Assets/", path);
            }
            return ImageSource.FromFile(path);
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}
