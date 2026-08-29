namespace Plugin.Maui.DeviceOrientationPlus;

interface IOrientationPlatform
{
    event EventHandler<ScreenOrientation>? OrientationChanged;

    bool IsSupported { get; }

    ScreenOrientation GetCurrent();

    void Apply(ScreenOrientation orientation);

    void Start();

    void Stop();
}
