#if IOS
using Foundation;
using UIKit;

namespace Plugin.Maui.DeviceOrientationPlus;

sealed class IosOrientationPlatform : IOrientationPlatform
{
    static UIInterfaceOrientationMask currentMask = UIInterfaceOrientationMask.AllButUpsideDown;
    NSObject? observer;
    bool started;

    public event EventHandler<ScreenOrientation>? OrientationChanged;

    public bool IsSupported => true;

    internal static bool AllowPortraitUpsideDown { get; set; }

    /// <summary>
    /// Mask returned from AppDelegate <c>application:supportedInterfaceOrientationsForWindow:</c>.
    /// </summary>
    public static UIInterfaceOrientationMask SupportedInterfaceOrientations => currentMask;

    public ScreenOrientation GetCurrent()
    {
        var interfaceOrientation = ReadInterfaceOrientation();
        return interfaceOrientation switch
        {
            UIInterfaceOrientation.Portrait => ScreenOrientation.Portrait,
            UIInterfaceOrientation.PortraitUpsideDown => ScreenOrientation.PortraitUpsideDown,
            UIInterfaceOrientation.LandscapeLeft => ScreenOrientation.LandscapeLeft,
            UIInterfaceOrientation.LandscapeRight => ScreenOrientation.LandscapeRight,
            _ => FromDeviceDisplay()
        };
    }

    public void Apply(ScreenOrientation orientation)
    {
        currentMask = ToMask(orientation);
        RequestGeometry(currentMask, orientation);
    }

    public void Start()
    {
        if (started)
            return;

        started = true;
        UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
        observer = UIDevice.Notifications.ObserveOrientationDidChange((_, _) =>
            OrientationChanged?.Invoke(this, GetCurrent()));
    }

    public void Stop()
    {
        observer?.Dispose();
        observer = null;
        if (started)
            UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();
        started = false;
    }

    static UIInterfaceOrientationMask ToMask(ScreenOrientation orientation)
    {
        var allowUpsideDown = AllowPortraitUpsideDown;

        return orientation switch
        {
            ScreenOrientation.Unspecified => allowUpsideDown
                ? UIInterfaceOrientationMask.All
                : UIInterfaceOrientationMask.AllButUpsideDown,
            ScreenOrientation.Portrait => UIInterfaceOrientationMask.Portrait,
            ScreenOrientation.PortraitUpsideDown => UIInterfaceOrientationMask.PortraitUpsideDown,
            ScreenOrientation.PortraitSensor => allowUpsideDown
                ? UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown
                : UIInterfaceOrientationMask.Portrait,
            ScreenOrientation.Landscape => UIInterfaceOrientationMask.Landscape,
            ScreenOrientation.LandscapeLeft => UIInterfaceOrientationMask.LandscapeLeft,
            ScreenOrientation.LandscapeRight => UIInterfaceOrientationMask.LandscapeRight,
            ScreenOrientation.LandscapeSensor => UIInterfaceOrientationMask.Landscape,
            _ => UIInterfaceOrientationMask.AllButUpsideDown
        };
    }

    static void RequestGeometry(UIInterfaceOrientationMask mask, ScreenOrientation orientation)
    {
        foreach (var scene in UIApplication.SharedApplication.ConnectedScenes)
        {
            if (scene is not UIWindowScene windowScene)
                continue;

            var root = windowScene.KeyWindow?.RootViewController
                ?? windowScene.Windows.FirstOrDefault()?.RootViewController;

            if (OperatingSystem.IsIOSVersionAtLeast(16))
            {
                windowScene.RequestGeometryUpdate(
                    new UIWindowSceneGeometryPreferencesIOS(mask),
                    _ => { });
                root?.SetNeedsUpdateOfSupportedInterfaceOrientations();
                continue;
            }

            RequestLegacyRotation(orientation);
        }

        if (!OperatingSystem.IsIOSVersionAtLeast(16)
            && UIApplication.SharedApplication.ConnectedScenes.Count == 0)
        {
            RequestLegacyRotation(orientation);
        }
    }

    static void RequestLegacyRotation(ScreenOrientation orientation)
    {
        var deviceOrientation = orientation switch
        {
            ScreenOrientation.Portrait or ScreenOrientation.PortraitSensor => UIDeviceOrientation.Portrait,
            ScreenOrientation.PortraitUpsideDown => UIDeviceOrientation.PortraitUpsideDown,
            ScreenOrientation.Landscape or ScreenOrientation.LandscapeSensor or ScreenOrientation.LandscapeLeft
                => UIDeviceOrientation.LandscapeRight,
            ScreenOrientation.LandscapeRight => UIDeviceOrientation.LandscapeLeft,
            _ => (UIDeviceOrientation?)null
        };

        if (deviceOrientation is { } value)
        {
#pragma warning disable CA1422
            UIDevice.CurrentDevice.SetValueForKey(
                NSNumber.FromNInt((nint)value),
                new NSString("orientation"));
            UIViewController.AttemptRotationToDeviceOrientation();
#pragma warning restore CA1422
        }
    }

    static UIInterfaceOrientation ReadInterfaceOrientation()
    {
        foreach (var scene in UIApplication.SharedApplication.ConnectedScenes)
        {
            if (scene is not UIWindowScene windowScene)
                continue;

            if (OperatingSystem.IsIOSVersionAtLeast(16))
                return windowScene.EffectiveGeometry.InterfaceOrientation;

#pragma warning disable CA1422
            return windowScene.InterfaceOrientation;
#pragma warning restore CA1422
        }

#pragma warning disable CA1422
        return UIApplication.SharedApplication.StatusBarOrientation;
#pragma warning restore CA1422
    }

    static ScreenOrientation FromDeviceDisplay() =>
        DeviceDisplay.MainDisplayInfo.Orientation switch
        {
            DisplayOrientation.Portrait => ScreenOrientation.Portrait,
            DisplayOrientation.Landscape => ScreenOrientation.Landscape,
            _ => ScreenOrientation.Unspecified
        };
}
#endif
