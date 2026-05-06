using NCKH.Models;

namespace NCKH.Views;

public partial class ThemDinhMucPage : ContentPage
{
    private string _loai = "NCKH"; // Mặc định chọn NCKH

    // Event để báo về cho trang gọi khi lưu thành công
    public event Action<DinhMuc>? OnDinhMucDuocThem;

    public ThemDinhMucPage()
    {
        InitializeComponent();
    }

    // ── Loại giờ ──────────────────────────────────────────
    private void OnLoaiNCKH(object sender, TappedEventArgs e) => SetLoai("NCKH");
    private void OnLoaiGD(object sender, TappedEventArgs e) => SetLoai("Giảng dạy");
    private void OnLoaiPV(object sender, TappedEventArgs e) => SetLoai("Phục vụ cộng đồng");

    private void SetLoai(string loai)
    {
        _loai = loai;

        // Reset cả 3 badge
        ResetBadge(BadgeNCKH);
        ResetBadge(BadgeGD);
        ResetBadge(BadgePV);

        // Highlight badge được chọn
        switch (loai)
        {
            case "NCKH":
                HighlightBadge(BadgeNCKH, "#EEF2FF", "#818CF8", "#4338CA");
                break;
            case "Giảng dạy":
                HighlightBadge(BadgeGD, "#F0FDF4", "#10B981", "#065F46");
                break;
            case "Phục vụ cộng đồng":
                HighlightBadge(BadgePV, "#FFF7ED", "#F59E0B", "#92400E");
                break;
        }
    }

    private static void ResetBadge(Border badge)
    {
        badge.BackgroundColor = Color.FromArgb("#F8FAFF");
        badge.Stroke = new SolidColorBrush(Color.FromArgb("#E2E8F0"));
        ((Label)badge.Content).TextColor = Color.FromArgb("#64748B");
    }

    private static void HighlightBadge(Border badge, string bg, string stroke, string text)
    {
        badge.BackgroundColor = Color.FromArgb(bg);
        badge.Stroke = new SolidColorBrush(Color.FromArgb(stroke));
        ((Label)badge.Content).TextColor = Color.FromArgb(text);
    }

    // ── Nút Hủy / Quay lại ────────────────────────────────
    private async void OnQuayLaiClicked(object sender, EventArgs e)
        => await Navigation.PopAsync();

    // ── Nút Lưu ───────────────────────────────────────────
    private async void OnLuuClicked(object sender, EventArgs e)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(EntryMa.Text) ||
            string.IsNullOrWhiteSpace(EntryTen.Text) ||
            string.IsNullOrWhiteSpace(EntrySoGio.Text))
        {
            await DisplayAlert("Thiếu thông tin",
                "Vui lòng điền đầy đủ Mã, Tên và Số giờ chuẩn!", "OK");
            return;
        }

        if (!int.TryParse(EntrySoGio.Text, out int soGio) || soGio <= 0)
        {
            await DisplayAlert("Số giờ không hợp lệ",
                "Số giờ chuẩn phải là số nguyên dương!", "OK");
            return;
        }

        if (PickerTrangThai.SelectedIndex < 0)
        {
            await DisplayAlert("Thiếu thông tin",
                "Vui lòng chọn trạng thái!", "OK");
            return;
        }

        // Tạo object mới
        var dinhMucMoi = new DinhMuc
        {
            MaDinhMuc = EntryMa.Text.Trim().ToUpper(),
            TenDinhMuc = EntryTen.Text.Trim(),
            GioChuan = soGio,
            LoaiGio = _loai,
            TrangThai = PickerTrangThai.SelectedItem!.ToString()!
        };

        // Báo về cho trang cha
        OnDinhMucDuocThem?.Invoke(dinhMucMoi);

        await DisplayAlert("Thành công", "Đã thêm định mức mới!", "OK");
        await Navigation.PopAsync();
    }
}