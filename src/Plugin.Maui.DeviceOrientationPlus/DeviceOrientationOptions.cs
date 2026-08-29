namespace Plugin.Maui.DeviceOrientationPlus;

/// <summary>
/// Defaults applied by <c>UseDeviceOrientationPlus</c> or <see cref="Orientation.Configure"/>.
/// </summary>
public sealed class DeviceOrientationOptions
{
    /// <summary>
    /// Gets or sets how long <see cref="IDeviceOrientation.SetAsync"/> waits for the
    /// display to match the requested orientation. Default is 3 seconds.
    /// </summary>
    public TimeSpan SetTimeout { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Gets or sets whether an unlocked iPhone may rest upside-down.
    /// Default is <c>false</c> (the usual Info.plist set).
    /// </summary>
    public bool AllowPortraitUpsideDown { get; set; }
}
