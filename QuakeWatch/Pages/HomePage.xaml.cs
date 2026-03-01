namespace QuakeWatch.Pages;

public partial class HomePage : ContentPage {
    public HomePage() {
        InitializeComponent();
    }

    private async void OnOpenMapClicked(object sender, EventArgs e) {
        await Shell.Current.GoToAsync(nameof(MapPage));
    }
}