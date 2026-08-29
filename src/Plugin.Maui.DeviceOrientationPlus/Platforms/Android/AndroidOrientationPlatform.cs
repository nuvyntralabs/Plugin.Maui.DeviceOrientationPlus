#if ANDROID
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using AndroidOrientation = Android.Content.PM.ScreenOrientation;

namespace Plugin.Maui.DeviceOrientationPlus;

sealed class AndroidOrientationPlatform : IOrientationPlatform, IDisposable
{
    OrientationCallback? callback;
    bool started;

    public event EventHandler<ScreenOrientation>? OrientationChanged;

    public bool IsSupported => true;

    public ScreenOrientation GetCurrent()
    {
        var activity = Platform.CurrentActivity;
        if (activity is null)
            return FromDeviceDisplay();

        var rotation = activity.Display?.Rotation
            ?? activity.WindowManager?.DefaultDisplay?.Rotation
            ?? SurfaceOrientation.Rotation0;

        var config = activity.Resources?.Configuration?.Orientation ?? Android.Content.Res.Orientation.Undefined;

        return MapRotation(rotation, config);
    }

    public void Apply(ScreenOrientation orientation)
    {
        var activity = Platform.CurrentActivity;
        if (activity is null)
            return;

        activity.RequestedOrientation = orientation switch
        {
            ScreenOrientation.Unspecified => AndroidOrientation.Unspecified,
            ScreenOrientation.Portrait => AndroidOrientation.Portrait,
            ScreenOrientation.PortraitUpsideDown => AndroidOrientation.ReversePortrait,
            ScreenOrientation.Landscape => AndroidOrientation.UserLandscape,
            ScreenOrientation.LandscapeLeft => AndroidOrientation.Landscape,
            ScreenOrientation.LandscapeRight => AndroidOrientation.ReverseLandscape,
            ScreenOrientation.PortraitSensor => AndroidOrientation.SensorPortrait,
            ScreenOrientation.LandscapeSensor => AndroidOrientation.SensorLandscape,
            _ => AndroidOrientation.Unspecified
        };
    }

    public void Start()
    {
        if (started)
            return;

        started = true;
        var activity = Platform.CurrentActivity;
        if (activity is null)
            return;

        callback = new OrientationCallback(this);
        activity.RegisterComponentCallbacks(callback);
    }

    public void Stop()
    {
        var activity = Platform.CurrentActivity;
        if (activity is not null && callback is not null)
            activity.UnregisterComponentCallbacks(callback);

        callback = null;
        started = false;
    }

    public void Dispose() => Stop();

    void Raise() => OrientationChanged?.Invoke(this, GetCurrent());

    static ScreenOrientation MapRotation(SurfaceOrientation rotation, Android.Content.Res.Orientation config)
    {
        if (config is Android.Content.Res.Orientation.Portrait)
        {
            return rotation is SurfaceOrientation.Rotation180
                ? ScreenOrientation.PortraitUpsideDown
                : ScreenOrientation.Portrait;
        }

        if (config is Android.Content.Res.Orientation.Landscape)
        {
            return rotation is SurfaceOrientation.Rotation270 or SurfaceOrientation.Rotation180
                ? ScreenOrientation.LandscapeRight
                : ScreenOrientation.LandscapeLeft;
        }

        return rotation switch
        {
            SurfaceOrientation.Rotation0 => ScreenOrientation.Portrait,
            SurfaceOrientation.Rotation90 => ScreenOrientation.LandscapeLeft,
            SurfaceOrientation.Rotation180 => ScreenOrientation.PortraitUpsideDown,
            SurfaceOrientation.Rotation270 => ScreenOrientation.LandscapeRight,
            _ => FromDeviceDisplay()
        };
    }

    static ScreenOrientation FromDeviceDisplay() =>
        DeviceDisplay.MainDisplayInfo.Orientation switch
        {
            DisplayOrientation.Portrait => ScreenOrientation.Portrait,
            DisplayOrientation.Landscape => ScreenOrientation.Landscape,
            _ => ScreenOrientation.Unspecified
        };

    sealed class OrientationCallback(AndroidOrientationPlatform owner) : Java.Lang.Object, IComponentCallbacks
    {
        public void OnConfigurationChanged(Configuration newConfig) => owner.Raise();

        public void OnLowMemory()
        {
        }
    }
}
#endif
