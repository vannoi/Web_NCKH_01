namespace NCKH.Views;

public partial class HoSoListPage : ContentPage
{
    public HoSoListPage() => InitializeComponent();

    // Điều hướng tới form tạo hồ sơ
    private async void OnTaoHoSoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TaoHoSoPage());
    }

    private async void OnDvqlttMenuClicked(object sender, EventArgs e)
    {
        // Thêm hàm này vào file HoSoListPage.xaml.cs và gọi nó từ Button ĐVQLTT trong XAML
        await Navigation.PushAsync(new DvqlttListPage());
    }
}