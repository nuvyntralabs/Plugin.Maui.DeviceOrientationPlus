#if !ANDROID && !IOS
namespace Plugin.Maui.DeviceOrientationPlus;

sealed class NetOrientationPlatform : IOrientationPlatform
{
    ScreenOrientation current = ScreenOrientation.Portrait;

    public event EventHandler<ScreenOrientation>? OrientationChanged;

    public bool IsSupported => true;

    public ScreenOrientation GetCurrent() => current;

    public void Apply(ScreenOrientation orientation)
    {
        if (orientation is ScreenOrientation.Unspecified)
            return;

        current = orientation switch
        {
            ScreenOrientation.PortraitSensor => ScreenOrientation.Portrait,
            ScreenOrientation.LandscapeSensor => ScreenOrientation.Landscape,
            _ => orientation
        };

        OrientationChanged?.Invoke(this, current);
    }

    public void Start()
    {
    }

    public void Stop()
    {
    }

    public void Simulate(ScreenOrientation orientation)
    {
        current = orientation;
        OrientationChanged?.Invoke(this, current);
    }
}
#endif
