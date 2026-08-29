namespace Plugin.Maui.DeviceOrientationPlus.Tests;

public sealed class ScreenOrientationTests
{
    [Theory]
    [InlineData(ScreenOrientation.Portrait, true, false)]
    [InlineData(ScreenOrientation.PortraitUpsideDown, true, false)]
    [InlineData(ScreenOrientation.PortraitSensor, true, false)]
    [InlineData(ScreenOrientation.Landscape, false, true)]
    [InlineData(ScreenOrientation.LandscapeLeft, false, true)]
    [InlineData(ScreenOrientation.LandscapeRight, false, true)]
    [InlineData(ScreenOrientation.LandscapeSensor, false, true)]
    [InlineData(ScreenOrientation.Unspecified, false, false)]
    public void Family_helpers(ScreenOrientation orientation, bool portrait, bool landscape)
    {
        Assert.Equal(portrait, orientation.IsPortrait());
        Assert.Equal(landscape, orientation.IsLandscape());
    }

    [Theory]
    [InlineData(ScreenOrientation.Landscape, ScreenOrientation.LandscapeLeft, true)]
    [InlineData(ScreenOrientation.Landscape, ScreenOrientation.LandscapeRight, true)]
    [InlineData(ScreenOrientation.LandscapeLeft, ScreenOrientation.Landscape, true)]
    [InlineData(ScreenOrientation.Portrait, ScreenOrientation.Portrait, true)]
    [InlineData(ScreenOrientation.Portrait, ScreenOrientation.Landscape, false)]
    [InlineData(ScreenOrientation.PortraitSensor, ScreenOrientation.PortraitUpsideDown, true)]
    [InlineData(ScreenOrientation.Unspecified, ScreenOrientation.Portrait, true)]
    public void Allows_matches_families(ScreenOrientation requested, ScreenOrientation current, bool allowed)
    {
        Assert.Equal(allowed, requested.Allows(current));
    }
}
