namespace Plugin.Maui.DeviceOrientationPlus;

/// <summary>
/// Screen orientation lock, unlock, change events, and per-page set.
/// </summary>
public interface IDeviceOrientation
{
    /// <summary>
    /// Gets the orientation now on screen.
    /// </summary>
    ScreenOrientation Current { get; }

    /// <summary>
    /// Gets the active lock, or <see cref="ScreenOrientation.Unspecified"/> when unlocked.
    /// </summary>
    ScreenOrientation Locked { get; }

    /// <summary>
    /// Gets a value indicating whether a lock is currently applied.
    /// </summary>
    bool IsLocked { get; }

    /// <summary>
    /// Gets a value indicating whether native lock/unlock is available.
    /// </summary>
    bool IsSupported { get; }

    /// <summary>
    /// Gets a value indicating whether the current orientation is portrait-family.
    /// </summary>
    bool IsPortrait { get; }

    /// <summary>
    /// Gets a value indicating whether the current orientation is landscape-family.
    /// </summary>
    bool IsLandscape { get; }

    /// <summary>
    /// Gets the live options. Mutate through <see cref="Configure"/>.
    /// </summary>
    DeviceOrientationOptions Options { get; }

    /// <summary>
    /// Raised when the device rotates or the lock changes the on-screen orientation.
    /// </summary>
    event EventHandler<OrientationChangedEventArgs>? Changed;

    /// <summary>
    /// Locks the display to <paramref name="orientation"/>. Nested locks stack;
    /// call <see cref="Unlock"/> to restore the previous lock.
    /// </summary>
    void Lock(ScreenOrientation orientation);

    /// <summary>
    /// Releases the most recent lock. When the stack is empty the sensor / user may rotate again.
    /// </summary>
    void Unlock();

    /// <summary>
    /// Locks to <paramref name="orientation"/> and waits until the display matches,
    /// or until <see cref="DeviceOrientationOptions.SetTimeout"/>.
    /// </summary>
    /// <returns><c>true</c> when the current orientation satisfies the request.</returns>
    Task<bool> SetAsync(ScreenOrientation orientation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Locks to <paramref name="orientation"/> and returns a scope that unlocks on dispose.
    /// </summary>
    Task<IAsyncDisposable> ScopeAsync(ScreenOrientation orientation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies <paramref name="orientation"/> while <paramref name="page"/> is visible,
    /// then unlocks when the page disappears.
    /// </summary>
    IDisposable Bind(Page page, ScreenOrientation orientation);

    /// <summary>
    /// Updates options on this instance.
    /// </summary>
    void Configure(Action<DeviceOrientationOptions> configure);

    /// <summary>
    /// Returns a point-in-time view of orientation and lock.
    /// </summary>
    OrientationSnapshot GetSnapshot();

    /// <summary>
    /// Starts platform listeners. Called by <c>UseDeviceOrientationPlus</c>; safe to call more than once.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops platform listeners and unhooks display events.
    /// </summary>
    void Stop();
}
