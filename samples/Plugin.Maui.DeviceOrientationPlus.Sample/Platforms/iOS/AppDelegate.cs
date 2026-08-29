using Foundation;
using Plugin.Maui.DeviceOrientationPlus;
using UIKit;

namespace Plugin.Maui.DeviceOrientationPlus.Sample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    [Export("application:supportedInterfaceOrientationsForWindow:")]
    public UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow? forWindow)
        => Orientation.SupportedInterfaceOrientations;
}
