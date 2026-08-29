#if IOS
using UIKit;

namespace Plugin.Maui.DeviceOrientationPlus;

public static partial class Orientation
{
    /// <summary>
    /// iOS mask to return from AppDelegate
    /// <c>application:supportedInterfaceOrientationsForWindow:</c>.
    /// Required for lock / unlock to take effect.
    /// </summary>
    public static UIInterfaceOrientationMask SupportedInterfaceOrientations =>
        IosOrientationPlatform.SupportedInterfaceOrientations;
}
#endif
