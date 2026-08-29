namespace Plugin.Maui.DeviceOrientationPlus;

/// <summary>
/// Helpers for <see cref="ScreenOrientation"/>.
/// </summary>
public static class ScreenOrientationExtensions
{
    /// <summary>
    /// Returns <c>true</c> when the value is a portrait family member.
    /// </summary>
    public static bool IsPortrait(this ScreenOrientation orientation) =>
        orientation is ScreenOrientation.Portrait
            or ScreenOrientation.PortraitUpsideDown
            or ScreenOrientation.PortraitSensor;

    /// <summary>
    /// Returns <c>true</c> when the value is a landscape family member.
    /// </summary>
    public static bool IsLandscape(this ScreenOrientation orientation) =>
        orientation is ScreenOrientation.Landscape
            or ScreenOrientation.LandscapeLeft
            or ScreenOrientation.LandscapeRight
            or ScreenOrientation.LandscapeSensor;

    /// <summary>
    /// Returns <c>true</c> when <paramref name="current"/> satisfies the lock in <paramref name="requested"/>.
    /// </summary>
    public static bool Allows(this ScreenOrientation requested, ScreenOrientation current)
    {
        if (requested is ScreenOrientation.Unspecified || current is ScreenOrientation.Unspecified)
            return requested is ScreenOrientation.Unspecified;

        return requested switch
        {
            ScreenOrientation.Portrait => current is ScreenOrientation.Portrait,
            ScreenOrientation.PortraitUpsideDown => current is ScreenOrientation.PortraitUpsideDown,
            ScreenOrientation.PortraitSensor => current.IsPortrait(),
            ScreenOrientation.Landscape => current.IsLandscape() || current is ScreenOrientation.Landscape,
            ScreenOrientation.LandscapeLeft => current is ScreenOrientation.LandscapeLeft or ScreenOrientation.Landscape,
            ScreenOrientation.LandscapeRight => current is ScreenOrientation.LandscapeRight or ScreenOrientation.Landscape,
            ScreenOrientation.LandscapeSensor => current.IsLandscape(),
            _ => true
        };
    }
}
