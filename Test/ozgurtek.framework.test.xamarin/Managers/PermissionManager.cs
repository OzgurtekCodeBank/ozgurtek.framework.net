using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ozgurtek.framework.test.xamarin.Managers
{
    public class PermissionManager
    {
        public async Task<bool> CheckForLocationPermission()
        {
            var status = await CheckAndRequestPermissionAsync(
                new Permissions.LocationWhenInUse(),
                "Konum erişimi açık değil");
            return status == PermissionStatus.Granted;
        }

        public async Task<bool> CheckForCamPermission()
        {
            var status = await CheckAndRequestPermissionAsync(
                new Permissions.Camera(),
                "Kamera izni açık değil");
            return status == PermissionStatus.Granted;
        }

        public async Task<bool> CheckForStoreReadPermission()
        {
            var status = await CheckAndRequestPermissionAsync(
                new Permissions.StorageRead(),
               "Dosya yazma silme erişimi açık değil");
            return status == PermissionStatus.Granted;
        }

        public async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission, string message)
            where T : Permissions.BasePermission
        {
            var status = await permission.CheckStatusAsync();

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Uyarı",
                    "IOS izni",
                    "Tamam");
                return status;
            }

            if (status != PermissionStatus.Granted)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Uyarı",
                    message,
                    "Tamam");

                status = await permission.RequestAsync();
            }

            return status;
        }
    }
}
