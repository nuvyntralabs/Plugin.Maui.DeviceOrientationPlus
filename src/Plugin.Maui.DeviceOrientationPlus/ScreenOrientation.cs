namespace Plugin.Maui.DeviceOrientationPlus;

/// <summary>
/// Requested or observed screen orientation.
/// </summary>
public enum ScreenOrientation
{
    /// <summary>
    /// No lock. The sensor / user may rotate freely.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Upright portrait.
    /// </summary>
    Portrait,

    /// <summary>
    /// Portrait rotated 180°.
    /// </summary>
    PortraitUpsideDown,

    /// <summary>
    /// Either landscape direction.
    /// </summary>
    Landscape,

    /// <summary>
    /// Landscape with the home indicator / button on the right.
    /// </summary>
    LandscapeLeft,

    /// <summary>
    /// Landscape with the home indicator / button on the left.
    /// </summary>
    LandscapeRight,

    /// <summary>
    /// Portrait or upside-down, following the sensor.
    /// </summary>
    PortraitSensor,

    /// <summary>
    /// Either landscape, following the sensor.
    /// </summary>
    LandscapeSensor
}
