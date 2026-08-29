using Microsoft.Maui.LifecycleEvents;

namespace Plugin.Maui.DeviceOrientationPlus;

/// <summary>
/// MAUI host registration for DeviceOrientationPlus.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Registers <see cref="IDeviceOrientation"/> and starts orientation listeners.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.UseDeviceOrientationPlus();
    /// </code>
    /// </example>
    public static MauiAppBuilder UseDeviceOrientationPlus(this MauiAppBuilder builder, Action<DeviceOrientationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddDeviceOrientationPlus(configure);
        builder.Services.AddTransient<IMauiInitializeService, DeviceOrientationInitializer>();

        builder.ConfigureLifecycleEvents(events =>
        {
#if ANDROID
            events.AddAndroid(android =>
            {
                android.OnPostCreate((activity, _) => Orientation.CurrentInstance.Start());
                android.OnResume(_ => Orientation.CurrentInstance.Start());
            });
#elif IOS
            events.AddiOS(ios =>
            {
                ios.OnActivated(_ => Orientation.CurrentInstance.Start());
            });
#endif
        });

        return builder;
    }
}
