using System.Runtime.CompilerServices;

namespace Plugin.Maui.DeviceOrientationPlus;

/// <content>
/// Per-page attached property: <c>Orientation.Preferred</c>.
/// </content>
public static partial class Orientation
{
    static readonly ConditionalWeakTable<Page, PageOrientationBinding> pageBindings = [];

    /// <summary>
    /// Gets the per-page preferred orientation. Applied on appearing and unlocked on disappearing.
    /// </summary>
    public static readonly BindableProperty PreferredProperty = BindableProperty.CreateAttached(
        "Preferred",
        typeof(ScreenOrientation),
        typeof(Orientation),
        ScreenOrientation.Unspecified,
        propertyChanged: OnPreferredChanged);

    /// <summary>
    /// Gets the preferred orientation for <paramref name="page"/>.
    /// </summary>
    public static ScreenOrientation GetPreferred(BindableObject page) =>
        (ScreenOrientation)page.GetValue(PreferredProperty);

    /// <summary>
    /// Sets the preferred orientation for <paramref name="page"/>.
    /// </summary>
    public static void SetPreferred(BindableObject page, ScreenOrientation value) =>
        page.SetValue(PreferredProperty, value);

    static void OnPreferredChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Page page)
            return;

        if (pageBindings.TryGetValue(page, out var existing))
        {
            existing.Detach();
            pageBindings.Remove(page);
        }

        if (newValue is not ScreenOrientation preferred || preferred is ScreenOrientation.Unspecified)
            return;

        var binding = new PageOrientationBinding(page, preferred);
        pageBindings.Add(page, binding);
        binding.Attach();
    }

    sealed class PageOrientationBinding
    {
        readonly Page page;
        bool applied;

        public PageOrientationBinding(Page page, ScreenOrientation preferred)
        {
            this.page = page;
            Preferred = preferred;
        }

        public ScreenOrientation Preferred { get; }

        public void Attach()
        {
            page.Appearing += OnAppearing;
            page.Disappearing += OnDisappearing;
        }

        public void Detach()
        {
            page.Appearing -= OnAppearing;
            page.Disappearing -= OnDisappearing;
            Release();
        }

        async void OnAppearing(object? sender, EventArgs e)
        {
            applied = true;
            await Shared.SetAsync(Preferred);
        }

        void OnDisappearing(object? sender, EventArgs e) => Release();

        void Release()
        {
            if (!applied)
                return;

            applied = false;
            Shared.Unlock();
        }
    }
}
