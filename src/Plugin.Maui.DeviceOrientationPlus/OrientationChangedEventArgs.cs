namespace Plugin.Maui.DeviceOrientationPlus;

/// <summary>
/// Raised when the device orientation or the active lock changes.
/// </summary>
public sealed class OrientationChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrientationChangedEventArgs"/> class.
    /// </summary>
    public OrientationChangedEventArgs(
        ScreenOrientation previous,
        ScreenOrientation current,
        ScreenOrientation locked,
        bool isLocked,
        double width,
        double height)
    {
        Previous = previous;
        Current = current;
        Locked = locked;
        IsLocked = isLocked;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets the orientation before this change.
    /// </summary>
    public ScreenOrientation Previous { get; }

    /// <summary>
    /// Gets the orientation now on screen.
    /// </summary>
    public ScreenOrientation Current { get; }

    /// <summary>
    /// Gets the active lock, or <see cref="ScreenOrientation.Unspecified"/> when unlocked.
    /// </summary>
    public ScreenOrientation Locked { get; }

    /// <summary>
    /// Gets a value indicating whether a lock is currently applied.
    /// </summary>
    public bool IsLocked { get; }

    /// <summary>
    /// Gets the display width in device-independent pixels.
    /// </summary>
    public double Width { get; }

    /// <summary>
    /// Gets the display height in device-independent pixels.
    /// </summary>
    public double Height { get; }

    /// <summary>
    /// Gets a value indicating whether the current orientation is portrait-family.
    /// </summary>
    public bool IsPortrait => Current.IsPortrait();

    /// <summary>
    /// Gets a value indicating whether the current orientation is landscape-family.
    /// </summary>
    public bool IsLandscape => Current.IsLandscape();
}
