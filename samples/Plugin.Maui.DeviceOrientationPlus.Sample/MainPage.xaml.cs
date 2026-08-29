using Plugin.Maui.DeviceOrientationPlus;

namespace Plugin.Maui.DeviceOrientationPlus.Sample;

public partial class MainPage : ContentPage
{
    readonly IDeviceOrientation orientation;
    readonly List<string> log = [];

    public MainPage(IDeviceOrientation orientation)
    {
        InitializeComponent();
        this.orientation = orientation;
        this.orientation.Changed += OnChanged;
        Refresh();
    }

    void OnLockPortrait(object? sender, EventArgs e) => Orientation.Lock(ScreenOrientation.Portrait);

    void OnLockLandscape(object? sender, EventArgs e) => Orientation.Lock(ScreenOrientation.Landscape);

    void OnUnlock(object? sender, EventArgs e) => Orientation.Unlock();

    async void OnOpenVideo(object? sender, EventArgs e) =>
        await Navigation.PushAsync(new VideoPage());

    async void OnOpenScanner(object? sender, EventArgs e) =>
        await Navigation.PushAsync(new ScannerPage());

    void OnChanged(object? sender, OrientationChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            log.Insert(0, $"{e.Previous} → {e.Current}  locked={e.Locked}");
            if (log.Count > 8)
                log.RemoveAt(log.Count - 1);
            Refresh();
        });
    }

    void Refresh()
    {
        var snapshot = orientation.GetSnapshot();
        StatusLabel.Text =
            $"current={snapshot.Current}  locked={snapshot.Locked}  isLocked={snapshot.IsLocked}";
        DisplayLabel.Text =
            $"portrait={snapshot.IsPortrait}  landscape={snapshot.IsLandscape}  size={snapshot.Width:0}x{snapshot.Height:0}";
        LogLabel.Text = log.Count == 0 ? "Rotate the device or tap Lock." : string.Join(Environment.NewLine, log);
    }
}
