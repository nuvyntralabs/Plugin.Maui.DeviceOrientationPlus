using Microsoft.Extensions.Logging;
using Plugin.Maui.DeviceOrientationPlus;

namespace Plugin.Maui.DeviceOrientationPlus.Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.Services.AddSingleton<MainPage>();

        builder
            .UseMauiApp<App>()
            .UseDeviceOrientationPlus();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
