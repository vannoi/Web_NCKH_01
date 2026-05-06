using System.Diagnostics;

namespace NCKH.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        try { InitializeComponent(); }
        catch (Exception ex) { Debug.WriteLine("LỖI XAML: " + ex.Message); }
    }

    private void OnGotoDashboard(object sender, EventArgs e) { }

    private async void OnGoToHoSoList(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//HoSoListPage");

    private async void OnGoToDinhMuc(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//QuanLyDinhMucPage");

    private async void OnGoToKiemTra(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//KiemTraQuaTrinhPage");

    private async void OnGoToThongBao(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ThongBaoPage");
}
