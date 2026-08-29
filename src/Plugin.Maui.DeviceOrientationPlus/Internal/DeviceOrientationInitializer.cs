namespace Plugin.Maui.DeviceOrientationPlus;

sealed class DeviceOrientationInitializer : IMauiInitializeService
{
    public void Initialize(IServiceProvider services)
    {
        var orientation = services.GetService<IDeviceOrientation>();
        if (orientation is null)
            return;

        Orientation.SetCurrent(orientation);
        orientation.Start();
    }
}
