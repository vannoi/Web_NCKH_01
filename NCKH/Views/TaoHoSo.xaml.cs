namespace NCKH.Views;

public partial class TaoHoSoPage : ContentPage
{
    public TaoHoSoPage() => InitializeComponent();

    // Xử lý nút Nộp hồ sơ
    private async void OnNopHoSoClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TieuDeEntry.Text))
        {
            await DisplayAlert("Lỗi", "Vui lòng nhập tiêu đề hồ sơ", "OK");
            return;
        }
        // TODO: gọi API nộp hồ sơ
        await DisplayAlert("Thành công", "Hồ sơ đã được nộp!", "OK");
        await Navigation.PopAsync();
    }

    // Xử lý nút Lưu nháp
    private async void OnLuuNhapClicked(object sender, EventArgs e)
    {
        // TODO: gọi API lưu nháp
        await DisplayAlert("Đã lưu", "Hồ sơ đã được lưu nháp!", "OK");
        await Navigation.PopAsync();
    }
}