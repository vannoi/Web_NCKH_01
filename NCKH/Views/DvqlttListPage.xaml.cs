using NCKH.Models;
using NCKH.ViewModels;

namespace NCKH.Views;

public partial class DvqlttListPage : ContentPage
{
    public DvqlttListPage()
    {
        InitializeComponent();
    }

    // Xử lý khi ấn vào cột ATTACHMENT (Tệp chung)

    private async void OnViewAttachmentsClicked(object sender, EventArgs e)
    {
        var hoso = (HoSo)((Button)sender).BindingContext;

        string action = await DisplayActionSheet(
            $"📁 HỒ SƠ: {hoso.TieuDe.ToUpper()}",
            "Đóng",
            null,
            "📄 Xem chi tiết đề tài (.pdf)",
            "📊 Bảng dữ liệu khảo sát (.xlsx)",
            "🖼️ Ảnh minh họa thực tế (.png)",
            "--------------------------",
            "📥 Tải xuống tất cả (.zip)");

        if (action == "📥 Tải xuống tất cả (.zip)")
        {
            await DisplayAlert("Hệ thống", "Đang nén và tải tệp...", "OK");
        }
    }

    // Tệp cá nhân (My Attachments)
    private async void OnViewMyAttachmentsClicked(object sender, EventArgs e)
    {
        var hoso = (HoSo)((Button)sender).BindingContext;

        await DisplayActionSheet(
            $"📝 GHI CHÚ RIÊNG - ID: {hoso.Id}",
            "Đóng",
            null,
            "📄 Nhận xét của cán bộ trực tiếp.docx",
            "📄 Biên bản thẩm định sơ bộ.pdf");
    }

    // Form trả lại (Return Form)
    private async void OnReturnApplicationClicked(object sender, EventArgs e)
    {
        var hoso = (HoSo)((Button)sender).BindingContext;

        // Tinh chỉnh Prompt để nhìn chuyên nghiệp hơn
        string result = await DisplayPromptAsync(
            "RETURN APPLICATION FORM",
            $"Gửi phản hồi yêu cầu chỉnh sửa cho giảng viên: {hoso.TenGiangVien}",
            "GỬI PHẢN HỒI",
            "HỦY BỎ",
            "Ví dụ: Thiếu minh chứng bài báo quốc tế...",
            -1,
            Keyboard.Text);

        if (!string.IsNullOrWhiteSpace(result))
        {
            await DisplayAlert("Thành công", "Hồ sơ đã được chuyển về trạng thái 'Chờ chỉnh sửa'.", "Đóng");
        }
    }

    // Trong file DvqlttListPage.xaml.cs
    private async void OnQuanLyHoSoTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NCKH.Views.HoSoListPage());
    }   
}