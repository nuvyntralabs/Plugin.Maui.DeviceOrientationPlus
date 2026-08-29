namespace Plugin.Maui.DeviceOrientationPlus.Tests;

public sealed class OrientationTests
{
    [Fact]
    public void Lock_sets_locked_and_current()
    {
        var (manager, _) = Harness.Create();

        manager.Lock(ScreenOrientation.Landscape);

        Assert.True(manager.IsLocked);
        Assert.Equal(ScreenOrientation.Landscape, manager.Locked);
        Assert.Equal(ScreenOrientation.Landscape, manager.Current);
        Assert.True(manager.IsLandscape);
    }

    [Fact]
    public void Unlock_clears_the_lock()
    {
        var (manager, _) = Harness.Create();
        manager.Lock(ScreenOrientation.Portrait);

        manager.Unlock();

        Assert.False(manager.IsLocked);
        Assert.Equal(ScreenOrientation.Unspecified, manager.Locked);
        Assert.Equal(ScreenOrientation.Portrait, manager.Current);
    }

    [Fact]
    public void Nested_lock_restores_previous()
    {
        var (manager, _) = Harness.Create();
        manager.Lock(ScreenOrientation.Portrait);
        manager.Lock(ScreenOrientation.Landscape);

        Assert.Equal(ScreenOrientation.Landscape, manager.Locked);

        manager.Unlock();

        Assert.Equal(ScreenOrientation.Portrait, manager.Locked);
        Assert.True(manager.IsLocked);

        manager.Unlock();

        Assert.False(manager.IsLocked);
    }

    [Fact]
    public void Unlock_when_unlocked_is_a_no_op()
    {
        var (manager, _) = Harness.Create();

        manager.Unlock();

        Assert.False(manager.IsLocked);
        Assert.Equal(ScreenOrientation.Portrait, manager.Current);
    }

    [Fact]
    public void Changed_fires_on_lock_and_platform_rotate()
    {
        var (manager, platform) = Harness.Create();
        var seen = new List<ScreenOrientation>();
        manager.Changed += (_, e) => seen.Add(e.Current);

        manager.Lock(ScreenOrientation.Landscape);
        platform.Simulate(ScreenOrientation.LandscapeLeft);

        Assert.Contains(ScreenOrientation.Landscape, seen);
        Assert.Contains(ScreenOrientation.LandscapeLeft, seen);
    }

    [Fact]
    public async Task SetAsync_completes_when_already_matching()
    {
        var (manager, _) = Harness.Create();
        manager.Lock(ScreenOrientation.Portrait);
        manager.Unlock();

        var matched = await manager.SetAsync(ScreenOrientation.Portrait);

        Assert.True(matched);
        Assert.Equal(ScreenOrientation.Portrait, manager.Locked);
    }

    [Fact]
    public async Task SetAsync_waits_for_platform_match()
    {
        var (manager, platform) = Harness.Create(options =>
            options.SetTimeout = TimeSpan.FromSeconds(2));

        var set = manager.SetAsync(ScreenOrientation.LandscapeLeft);
        platform.Simulate(ScreenOrientation.LandscapeLeft);

        Assert.True(await set);
        Assert.Equal(ScreenOrientation.LandscapeLeft, manager.Current);
    }

    [Fact]
    public async Task ScopeAsync_unlocks_on_dispose()
    {
        var (manager, _) = Harness.Create();

        await using (await manager.ScopeAsync(ScreenOrientation.Landscape))
        {
            Assert.True(manager.IsLocked);
            Assert.Equal(ScreenOrientation.Landscape, manager.Locked);
        }

        Assert.False(manager.IsLocked);
    }

    [Fact]
    public void Snapshot_includes_lock_state()
    {
        var (manager, _) = Harness.Create();
        manager.Lock(ScreenOrientation.LandscapeSensor);

        var snapshot = manager.GetSnapshot();

        Assert.True(snapshot.IsLocked);
        Assert.Equal(ScreenOrientation.Landscape, snapshot.Current);
        Assert.Equal(ScreenOrientation.LandscapeSensor, snapshot.Locked);
        Assert.True(snapshot.IsSupported);
        Assert.True(snapshot.IsLandscape);
    }

    [Fact]
    public void Static_api_lock_unlock_and_snapshot()
    {
        Harness.Create();

        Orientation.Lock(ScreenOrientation.Landscape);
        Assert.True(Orientation.IsLocked);
        Assert.Equal(ScreenOrientation.Landscape, Orientation.Current);
        Assert.True(Orientation.GetSnapshot().IsLocked);

        Orientation.Unlock();
        Assert.False(Orientation.IsLocked);
        Assert.Equal(ScreenOrientation.Unspecified, Orientation.Locked);
    }

    [Fact]
    public void Configure_updates_timeout()
    {
        var (manager, _) = Harness.Create();

        manager.Configure(options => options.SetTimeout = TimeSpan.FromSeconds(5));

        Assert.Equal(TimeSpan.FromSeconds(5), manager.Options.SetTimeout);
    }

    [Fact]
    public void Options_defaults()
    {
        var options = new DeviceOrientationOptions();

        Assert.Equal(TimeSpan.FromSeconds(3), options.SetTimeout);
        Assert.False(options.AllowPortraitUpsideDown);
    }
}
