namespace Plugin.Maui.DeviceOrientationPlus;

/// <summary>
/// Static entry point for screen orientation lock, unlock, and change events.
/// </summary>
/// <example>
/// <code>
/// using static Plugin.Maui.DeviceOrientationPlus.ScreenOrientation;
///
/// Orientation.Lock(Portrait);
/// Orientation.Unlock();
/// Orientation.Changed += (_, e) => { /* e.Current */ };
///
/// await Orientation.SetAsync(ScreenOrientation.Landscape);
/// </code>
/// </example>
public static partial class Orientation
{
    static IDeviceOrientation? current;

    /// <summary>
    /// Gets the shared instance. Created on first use when <c>UseDeviceOrientationPlus</c> was not called.
    /// </summary>
    public static IDeviceOrientation Shared
    {
        get
        {
            if (current is null)
                SetDefault(Create());
            return current!;
        }
    }

    /// <summary>
    /// Gets the orientation now on screen.
    /// </summary>
    public static ScreenOrientation Current => Shared.Current;

    /// <summary>
    /// Gets the active lock, or <see cref="ScreenOrientation.Unspecified"/> when unlocked.
    /// </summary>
    public static ScreenOrientation Locked => Shared.Locked;

    /// <summary>
    /// Gets a value indicating whether a lock is currently applied.
    /// </summary>
    public static bool IsLocked => Shared.IsLocked;

    /// <summary>
    /// Gets a value indicating whether native lock/unlock is available.
    /// </summary>
    public static bool IsSupported => Shared.IsSupported;

    /// <summary>
    /// Gets a value indicating whether the current orientation is portrait-family.
    /// </summary>
    public static bool IsPortrait => Shared.IsPortrait;

    /// <summary>
    /// Gets a value indicating whether the current orientation is landscape-family.
    /// </summary>
    public static bool IsLandscape => Shared.IsLandscape;

    /// <summary>
    /// Raised when the device rotates or the lock changes the on-screen orientation.
    /// </summary>
    public static event EventHandler<OrientationChangedEventArgs>? Changed
    {
        add => Shared.Changed += value;
        remove => Shared.Changed -= value;
    }

    /// <summary>
    /// Locks the display to <paramref name="orientation"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// Orientation.Lock(ScreenOrientation.Portrait);
    /// </code>
    /// </example>
    public static void Lock(ScreenOrientation orientation) => Shared.Lock(orientation);

    /// <summary>
    /// Releases the most recent lock.
    /// </summary>
    /// <example>
    /// <code>
    /// Orientation.Unlock();
    /// </code>
    /// </example>
    public static void Unlock() => Shared.Unlock();

    /// <summary>
    /// Locks to <paramref name="orientation"/> and waits until the display matches.
    /// </summary>
    /// <example>
    /// <code>
    /// protected override async void OnAppearing()
    /// {
    ///     await Orientation.SetAsync(ScreenOrientation.Landscape);
    /// }
    /// </code>
    /// </example>
    public static Task<bool> SetAsync(ScreenOrientation orientation, CancellationToken cancellationToken = default) =>
        Shared.SetAsync(orientation, cancellationToken);

    /// <summary>
    /// Locks to <paramref name="orientation"/> and unlocks when the returned scope is disposed.
    /// </summary>
    public static Task<IAsyncDisposable> ScopeAsync(ScreenOrientation orientation, CancellationToken cancellationToken = default) =>
        Shared.ScopeAsync(orientation, cancellationToken);

    /// <summary>
    /// Applies <paramref name="orientation"/> while <paramref name="page"/> is visible.
    /// </summary>
    public static IDisposable Bind(Page page, ScreenOrientation orientation) =>
        Shared.Bind(page, orientation);

    /// <summary>
    /// Updates options on the shared instance, creating one if needed.
    /// </summary>
    public static void Configure(Action<DeviceOrientationOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        if (current is not null)
        {
            current.Configure(configure);
            return;
        }

        var options = new DeviceOrientationOptions();
        configure(options);
        SetDefault(Create(options));
    }

    /// <summary>
    /// Returns a point-in-time view of orientation and lock.
    /// </summary>
    public static OrientationSnapshot GetSnapshot() => Shared.GetSnapshot();

    /// <summary>
    /// Creates a manager that uses the platform orientation APIs.
    /// </summary>
    public static IDeviceOrientation Create(DeviceOrientationOptions? options = null)
    {
        var instance = Create(options ?? new DeviceOrientationOptions(), OrientationPlatform.Create());
        SetDefault(instance);
        return instance;
    }

    /// <summary>
    /// Replaces the shared instance. Intended for tests and custom implementations.
    /// </summary>
    public static void SetDefault(IDeviceOrientation implementation) =>
        current = implementation ?? throw new ArgumentNullException(nameof(implementation));

    internal static IDeviceOrientation CurrentInstance => Shared;

    internal static DeviceOrientationImplementation Create(
        DeviceOrientationOptions options,
        IOrientationPlatform platform) =>
        new(options, platform);

    internal static void SetCurrent(IDeviceOrientation? instance) => current = instance;

    internal static void Reset() => current = null;
}
