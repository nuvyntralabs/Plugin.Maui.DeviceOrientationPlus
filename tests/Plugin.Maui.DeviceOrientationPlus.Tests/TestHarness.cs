namespace Plugin.Maui.DeviceOrientationPlus.Tests;

static class Harness
{
    public static (DeviceOrientationImplementation Manager, NetOrientationPlatform Platform) Create(
        Action<DeviceOrientationOptions>? configure = null)
    {
        Orientation.Reset();

        var options = new DeviceOrientationOptions
        {
            SetTimeout = TimeSpan.FromMilliseconds(50)
        };
        configure?.Invoke(options);

        var platform = new NetOrientationPlatform();
        var manager = Orientation.Create(options, platform);
        Orientation.SetDefault(manager);
        manager.Start();
        return (manager, platform);
    }
}
