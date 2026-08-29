namespace Plugin.Maui.DeviceOrientationPlus;

sealed class DeviceOrientationImplementation : IDeviceOrientation
{
    readonly IOrientationPlatform platform;
    readonly Stack<ScreenOrientation> lockStack = new();
    readonly List<PageBinding> bindings = [];
    ScreenOrientation lastCurrent;
    bool started;

    public DeviceOrientationImplementation(DeviceOrientationOptions options, IOrientationPlatform platform)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
        this.platform = platform ?? throw new ArgumentNullException(nameof(platform));
        lastCurrent = platform.GetCurrent();
        this.platform.OrientationChanged += OnPlatformOrientationChanged;
    }

    public ScreenOrientation Current => platform.GetCurrent();

    public ScreenOrientation Locked =>
        lockStack.Count > 0 ? lockStack.Peek() : ScreenOrientation.Unspecified;

    public bool IsLocked => Locked is not ScreenOrientation.Unspecified;

    public bool IsSupported => platform.IsSupported;

    public bool IsPortrait => Current.IsPortrait();

    public bool IsLandscape => Current.IsLandscape();

    public DeviceOrientationOptions Options { get; }

    public event EventHandler<OrientationChangedEventArgs>? Changed;

    internal IOrientationPlatform Platform => platform;

    public void Start()
    {
        if (started)
        {
            platform.Apply(Locked);
            return;
        }

        started = true;
        SyncPlatformOptions();
        platform.Start();
        DeviceDisplay.MainDisplayInfoChanged += OnDisplayInfoChanged;
        lastCurrent = platform.GetCurrent();
        platform.Apply(Locked);
    }

    public void Lock(ScreenOrientation orientation)
    {
        Start();
        lockStack.Push(orientation);
        platform.Apply(orientation);
        RaiseIfChanged(platform.GetCurrent(), force: true);
    }

    public void Unlock()
    {
        Start();

        if (lockStack.Count > 0)
            lockStack.Pop();

        platform.Apply(Locked);
        RaiseIfChanged(platform.GetCurrent(), force: true);
    }

    public async Task<bool> SetAsync(ScreenOrientation orientation, CancellationToken cancellationToken = default)
    {
        Start();

        if (orientation.Allows(Current) && Locked == orientation)
        {
            Lock(orientation);
            return true;
        }

        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnChanged(object? sender, OrientationChangedEventArgs e)
        {
            if (orientation.Allows(e.Current))
                tcs.TrySetResult(true);
        }

        Changed += OnChanged;
        try
        {
            Lock(orientation);

            if (orientation.Allows(Current))
                return true;

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (Options.SetTimeout > TimeSpan.Zero)
                timeout.CancelAfter(Options.SetTimeout);

            try
            {
                return await tcs.Task.WaitAsync(timeout.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return orientation.Allows(Current);
            }
        }
        finally
        {
            Changed -= OnChanged;
        }
    }

    public async Task<IAsyncDisposable> ScopeAsync(ScreenOrientation orientation, CancellationToken cancellationToken = default)
    {
        await SetAsync(orientation, cancellationToken).ConfigureAwait(false);
        return new OrientationScope(this);
    }

    public IDisposable Bind(Page page, ScreenOrientation orientation)
    {
        ArgumentNullException.ThrowIfNull(page);
        Start();

        var binding = new PageBinding(this, page, orientation);
        bindings.Add(binding);
        binding.Attach();
        return binding;
    }

    public void Configure(Action<DeviceOrientationOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(Options);
        SyncPlatformOptions();
    }

    void SyncPlatformOptions()
    {
#if IOS
        IosOrientationPlatform.AllowPortraitUpsideDown = Options.AllowPortraitUpsideDown;
#endif
    }

    public OrientationSnapshot GetSnapshot()
    {
        var info = TryGetDisplayInfo();
        return new OrientationSnapshot
        {
            Current = Current,
            Locked = Locked,
            IsLocked = IsLocked,
            IsSupported = IsSupported,
            Width = info.Width,
            Height = info.Height,
            Density = info.Density
        };
    }

    void OnPlatformOrientationChanged(object? sender, ScreenOrientation orientation) =>
        RaiseIfChanged(orientation);

    void OnDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e) =>
        RaiseIfChanged(platform.GetCurrent());

    void RaiseIfChanged(ScreenOrientation next, bool force = false)
    {
        var raise = () =>
        {
            var previous = lastCurrent;
            if (!force && previous == next)
                return;

            lastCurrent = next;
            var info = TryGetDisplayInfo();
            Changed?.Invoke(this, new OrientationChangedEventArgs(
                previous,
                next,
                Locked,
                IsLocked,
                info.Width,
                info.Height));
        };

#if ANDROID || IOS
        if (!MainThread.IsMainThread)
        {
            MainThread.BeginInvokeOnMainThread(raise);
            return;
        }
#endif
        raise();
    }

    static (double Width, double Height, double Density) TryGetDisplayInfo()
    {
        try
        {
            var info = DeviceDisplay.MainDisplayInfo;
            return (info.Width, info.Height, info.Density);
        }
        catch (Exception)
        {
            return (0, 0, 1);
        }
    }

    sealed class OrientationScope : IAsyncDisposable
    {
        DeviceOrientationImplementation? owner;

        public OrientationScope(DeviceOrientationImplementation owner) => this.owner = owner;

        public ValueTask DisposeAsync()
        {
            Interlocked.Exchange(ref owner, null)?.Unlock();
            return ValueTask.CompletedTask;
        }
    }

    sealed class PageBinding : IDisposable
    {
        readonly DeviceOrientationImplementation owner;
        readonly Page page;
        readonly ScreenOrientation orientation;
        bool applied;
        bool disposed;

        public PageBinding(DeviceOrientationImplementation owner, Page page, ScreenOrientation orientation)
        {
            this.owner = owner;
            this.page = page;
            this.orientation = orientation;
        }

        public void Attach()
        {
            page.Appearing += OnAppearing;
            page.Disappearing += OnDisappearing;
        }

        async void OnAppearing(object? sender, EventArgs e)
        {
            applied = true;
            await owner.SetAsync(orientation);
        }

        void OnDisappearing(object? sender, EventArgs e) => Release();

        void Release()
        {
            if (!applied)
                return;

            applied = false;
            owner.Unlock();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            page.Appearing -= OnAppearing;
            page.Disappearing -= OnDisappearing;
            Release();
            owner.bindings.Remove(this);
        }
    }
}
