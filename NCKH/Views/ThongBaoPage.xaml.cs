using NCKH.ViewModels;

namespace NCKH.Views;

public partial class ThongBaoPage : ContentPage
{
    public ThongBaoPage()
    {
        InitializeComponent();
        BindingContext = new ThongBaoViewModel();
    }

    private async void OnMenuClicked(object sender, EventArgs e)
    {
        SidebarControl.IsVisible = true;

        await SidebarControl.TranslateTo(0, 0, 250, Easing.CubicOut);
    }

    private async void OnCloseMobileSidebar(object sender, EventArgs e)
    {
        await SidebarControl.TranslateTo(-280, 0, 250, Easing.CubicIn);

        if (DeviceInfo.Current.Idiom == DeviceIdiom.Phone)
        {
            SidebarControl.IsVisible = false;
        }
    }

    private bool _isMobileSidebarOpen;
    public bool IsMobileSidebarOpen
    {
        get => _isMobileSidebarOpen;
        set
        {
            _isMobileSidebarOpen = value;
            OnPropertyChanged();
        }
    }
}