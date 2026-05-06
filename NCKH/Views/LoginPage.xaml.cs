using NCKH.Services;
using Microsoft.Maui.Graphics; // Thư viện để dùng màu sắc

namespace NCKH.Views;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService = new();

    public LoginPage()
    {
        InitializeComponent();

        // FIX LỖI TRÙNG MÀU: Ép màu chữ hiển thị rõ ràng
        // Chỉnh màu đen cho chữ và màu xám đậm cho chữ gợi ý (Placeholder)
        EmailEntry.TextColor = Colors.Black;
        EmailEntry.PlaceholderColor = Colors.DarkSlateGray;
        EmailEntry.BackgroundColor = Colors.White; // Thêm nền trắng cho ô nhập nếu cần

        PasswordEntry.TextColor = Colors.Black;
        PasswordEntry.PlaceholderColor = Colors.DarkSlateGray;
        PasswordEntry.BackgroundColor = Colors.White;
    }

    // NÚT MẮT — ẨN/HIỆN MẬT KHẨU
    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        // Đổi Icon mắt
        if (sender is Button btn)
        {
            btn.Text = PasswordEntry.IsPassword ? "👁" : "🙈";
        }
    }

    // NÚT ĐĂNG NHẬP
    private async void OnDangNhapClicked(object sender, EventArgs e)
    {
        // 1. Kiểm tra đầu vào
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Lỗi", "Vui lòng nhập đầy đủ email và mật khẩu", "OK");
            return;
        }

        // 2. Hiệu ứng chờ (Vô hiệu hóa nút tránh bấm nhiều lần)
        var btn = (Button)sender;
        btn.IsEnabled = false;
        btn.Text = "Đang xác thực...";

        try
        {
            // 3. Gọi API (Sử dụng Trim để xóa khoảng trắng thừa)
            string email = EmailEntry.Text.Trim();
            string pass = PasswordEntry.Text;

            // Bạn có thể dùng bool success hoặc UserModel tùy theo ApiService của bạn
            var user = await _apiService.LoginAsync(email, pass);

            if (user != null)
            {
                // Đăng nhập thành công
                await Shell.Current.GoToAsync("//DashboardPage");
            }
            else
            {
                // Thất bại (Lỗi 401 hoặc sai thông tin)
                await DisplayAlert("Đăng nhập thất bại", "Tài khoản hoặc mật khẩu không chính xác!", "Thử lại");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi kết nối", $"Không thể kết nối đến máy chủ: {ex.Message}", "Đóng");
        }
        finally
        {
            // 4. Khôi phục trạng thái nút
            btn.IsEnabled = true;
            btn.Text = "ĐĂNG NHẬP";
        }
    }

    // QUÊN MẬT KHẨU
    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Quên mật khẩu", "Vui lòng liên hệ phòng CNTT để được cấp lại mật khẩu.", "Đã hiểu");
    }
}