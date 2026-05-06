using NCKH.ViewModels; // Đảm bảo namespace này khớp với file ThongBaoViewModel của bạn

namespace NCKH.Views;

public partial class ThongBaoPage : ContentPage
{
    public ThongBaoPage()
    {
        InitializeComponent();
        this.BindingContext = new NCKH.ViewModels.ThongBaoViewModel();
    }
    private async void OnMenuClicked(object sender, EventArgs e)
    {
        await SidebarControl.TranslateTo(0, 0, 250, Easing.CubicOut);
    }
}