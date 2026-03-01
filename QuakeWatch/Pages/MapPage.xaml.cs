using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;

namespace QuakeWatch.Pages;

public partial class MapPage : ContentPage {
    private bool _mapReady;

    public MapPage() {
        InitializeComponent();

        MapWebView.Source = "map.html";
        MapWebView.Navigated += (_, __) => _mapReady = true;
    }

    private async void OnMyLocationClicked(object sender, EventArgs e) {
        try {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted) {
                await DisplayAlert("Location", "Permission not granted.", "OK");
                return;
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            Location? location = await Geolocation.Default.GetLocationAsync(request)
                               ?? await Geolocation.Default.GetLastKnownLocationAsync();

            if (location is null) {
                await DisplayAlert("Location", "Could not get location.", "OK");
                return;
            }

            if (!_mapReady) {
                await DisplayAlert("Map", "Map not ready yet.", "OK");
                return;
            }

            var inv = System.Globalization.CultureInfo.InvariantCulture;
            string js = $"window.setUserLocation({location.Latitude.ToString(inv)}, {location.Longitude.ToString(inv)});";
            await MapWebView.EvaluateJavaScriptAsync(js);
        }
        catch (Exception ex) {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}