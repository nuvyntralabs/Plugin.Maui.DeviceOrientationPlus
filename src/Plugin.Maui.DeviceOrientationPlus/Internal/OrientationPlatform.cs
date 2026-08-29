namespace Plugin.Maui.DeviceOrientationPlus;

static class OrientationPlatform
{
    public static IOrientationPlatform Create() =>
#if ANDROID
        new AndroidOrientationPlatform();
#elif IOS
        new IosOrientationPlatform();
#else
        new NetOrientationPlatform();
#endif
}
