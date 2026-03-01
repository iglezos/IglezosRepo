using QuakeWatch.Pages;

namespace QuakeWatch;

public partial class AppShell : Shell {
    public AppShell() {
        InitializeComponent();
        Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));
    }
}