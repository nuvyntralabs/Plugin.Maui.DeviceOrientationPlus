namespace Plugin.Maui.DeviceOrientationPlus;

/// <summary>
/// Point-in-time view of device orientation and the active lock.
/// </summary>
public sealed class OrientationSnapshot
{
    /// <summary>
    /// Gets the orientation now on screen.
    /// </summary>
    public ScreenOrientation Current { get; init; }

    /// <summary>
    /// Gets the active lock, or <see cref="ScreenOrientation.Unspecified"/> when unlocked.
    /// </summary>
    public ScreenOrientation Locked { get; init; }

    /// <summary>
    /// Gets a value indicating whether a lock is currently applied.
    /// </summary>
    public bool IsLocked { get; init; }

    /// <summary>
    /// Gets a value indicating whether native lock/unlock is available.
    /// </summary>
    public bool IsSupported { get; init; }

    /// <summary>
    /// Gets the display width in device-independent pixels.
    /// </summary>
    public double Width { get; init; }

    /// <summary>
    /// Gets the display height in device-independent pixels.
    /// </summary>
    public double Height { get; init; }

    /// <summary>
    /// Gets the display density.
    /// </summary>
    public double Density { get; init; }

    /// <summary>
    /// Gets a value indicating whether the current orientation is portrait-family.
    /// </summary>
    public bool IsPortrait => Current.IsPortrait();

    /// <summary>
    /// Gets a value indicating whether the current orientation is landscape-family.
    /// </summary>
    public bool IsLandscape => Current.IsLandscape();
}
