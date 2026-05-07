using NCKH.Services;

namespace NCKH.Views;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService = new();

    public LoginPage() => InitializeComponent();

    // NÚT MẮT — ẨN/HIỆN MẬT KHẨU
    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        EyeButton.Text = PasswordEntry.IsPassword ? "👁" : "🙈";
    }

    // NÚT ĐĂNG NHẬP
    private async void OnDangNhapClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Lỗi", "Vui lòng nhập đầy đủ thông tin", "OK");
            return;
        }

        bool success = await _apiService.LoginAsync(
            EmailEntry.Text.Trim(),
            PasswordEntry.Text);

        if (success)
            await Shell.Current.GoToAsync("//DashboardPage");
        else
            await DisplayAlert("Đăng nhập thất bại",
                "Email hoặc mật khẩu không đúng!", "OK");
    }

    // QUÊN MẬT KHẨU
    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Quên mật khẩu",
            "Vui lòng liên hệ phòng CNTT để được cấp lại mật khẩu.", "Đã hiểu");
    }

    protected override void OnSizeAllocated(double width, double height)
        => base.OnSizeAllocated(width, height);
}