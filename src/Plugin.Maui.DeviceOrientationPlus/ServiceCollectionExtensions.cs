namespace Plugin.Maui.DeviceOrientationPlus;

/// <summary>
/// Registers DeviceOrientationPlus services without MAUI lifecycle hooks.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IDeviceOrientation"/> using the supplied options instance.
    /// </summary>
    public static IServiceCollection AddDeviceOrientationPlus(this IServiceCollection services, DeviceOrientationOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services.AddSingleton(options);
        services.TryAddSingleton<IDeviceOrientation>(sp =>
        {
            var resolved = sp.GetService<DeviceOrientationOptions>() ?? options;
            var instance = Orientation.Create(resolved, OrientationPlatform.Create());
            Orientation.SetCurrent(instance);
            return instance;
        });

        return services;
    }

    /// <summary>
    /// Adds <see cref="IDeviceOrientation"/> and applies <paramref name="configure"/> to a new options instance.
    /// </summary>
    public static IServiceCollection AddDeviceOrientationPlus(this IServiceCollection services, Action<DeviceOrientationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new DeviceOrientationOptions();
        configure?.Invoke(options);
        return services.AddDeviceOrientationPlus(options);
    }
}
