using Plugin.Maui.DeviceOrientationPlus;

namespace Plugin.Maui.DeviceOrientationPlus.Sample;

public partial class VideoPage : ContentPage
{
    public VideoPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Orientation.SetAsync(ScreenOrientation.Landscape);
        StatusLabel.Text = $"current={Orientation.Current}  locked={Orientation.Locked}";
    }

    protected override void OnDisappearing()
    {
        Orientation.Unlock();
        base.OnDisappearing();
    }
}
