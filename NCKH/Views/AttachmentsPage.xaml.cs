using NCKH.Models;
using System.Collections.ObjectModel;

namespace NCKH.Views;

// Nhận tham số truyền từ trang danh sách (QueryProperty)
[QueryProperty(nameof(HoSoId), "hoSoId")]
[QueryProperty(nameof(TenHoSo), "tenHoSo")]
public partial class AttachmentsPage : ContentPage
{
    public AttachmentsPage(HoSo hoso)
    {
        InitializeComponent();

        LoadMockData();

        BindingContext = this;
    }

    // Danh sách tệp đính kèm - Sử dụng ObservableCollection để tự động cập nhật UI khi thêm/xóa
    public ObservableCollection<FileAttachment> Attachments { get; set; } = new();

    private string _hoSoId = string.Empty;
    public string HoSoId
    {
        get => _hoSoId;
        set => _hoSoId = value;
    }

    private string _tenHoSo = string.Empty;
    public string TenHoSo
    {
        get => _tenHoSo;
        set
        {
            _tenHoSo = value;
            // Cập nhật nhãn hiển thị tên hồ sơ trên Header
            if (LabelTenHoSo != null)
                LabelTenHoSo.Text = $"Hồ sơ: {value}";
        }
    }

    public AttachmentsPage(string tenHoSo)
    {
        InitializeComponent();

        // Khởi tạo dữ liệu ảo (Mock Data)
        LoadMockData();

        // Gán BindingContext để CollectionView trong XAML nhận được dữ liệu
        BindingContext = this;
        TenHoSo = tenHoSo;
    }

    // Tạo dữ liệu ảo để hiển thị lên giao diện
    private void LoadMockData()
    {
        Attachments.Clear();
        Attachments.Add(new FileAttachment { FileName = "Thuyet_minh_de_tai_NCKH_2024.pdf", FileSize = "2.4 MB" });
        Attachments.Add(new FileAttachment { FileName = "Bang_ke_kinh_phi_du_kien.xlsx", FileSize = "1.1 MB" });
        Attachments.Add(new FileAttachment { FileName = "Bien_ban_hop_hoi_dong.docx", FileSize = "850 KB" });
        Attachments.Add(new FileAttachment { FileName = "Hinh_anh_minh_chung_thuc_te.jpg", FileSize = "5.7 MB" });
    }

    // Xử lý khi nhấn nút quay lại: Đóng Popup
    private async void OnBackTapped(object sender, EventArgs e)
    {

        await Navigation.PopModalAsync();
    }

    // Xử lý tải xuống một tệp đơn lẻ
    private async void OnDownloadSingleTapped(object sender, EventArgs e)
    {
        var button = sender as Button;
        var file = button?.CommandParameter as FileAttachment;

        if (file != null)
        {
            // Hiển thị thông báo giả lập quá trình tải
            await DisplayAlert("Tải xuống", $"Đang tải tệp: {file.FileName}\nVui lòng chờ trong giây lát...", "OK");
        }
    }


    // Xử lý xóa tệp khỏi danh sách
    private async void OnDeleteFileTapped(object sender, EventArgs e)
    {
        var button = sender as Button;
        var file = button?.CommandParameter as FileAttachment;

        if (file != null)
        {
            // Hỏi xác nhận trước khi xóa
            bool confirm = await DisplayAlert("Xác nhận xóa",
                $"Bạn có chắc chắn muốn gỡ bỏ tệp '{file.FileName}' không?",
                "Xóa tệp", "Hủy");

            if (confirm)
            {
                // Xóa khỏi danh sách, CollectionView sẽ tự động biến mất hàng đó
                Attachments.Remove(file);
            }
        }
    }


    // Xử lý tải tất cả tệp Nút ở Footer
    private async void OnDownloadAllTapped(object sender, EventArgs e)
    {
        if (Attachments.Count == 0)
        {
            await DisplayAlert("Thông báo", "Không có tệp nào để tải xuống.", "OK");
            return;
        }

        await DisplayAlert("Tải tất cả",
            $"Đang nén và chuẩn bị tải xuống {Attachments.Count} tệp tin...",
            "Bắt đầu");
    }


    // Tải file lên
    private async void OnUploadFileTapped(object sender, EventArgs e)
    {
        try
        {
            // Cấu hình các loại file cho phép (tùy chọn)
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                { DevicePlatform.iOS, new[] { "public.item" } },
                { DevicePlatform.Android, new[] { "application/*", "image/*", "text/*" } },
                { DevicePlatform.WinUI, new[] { ".jpg", ".png", ".pdf", ".docx", ".xlsx" } },
                { DevicePlatform.Tizen, new[] { "*/*" } },
                { DevicePlatform.macOS, new[] { "public.item" } },
                });

            PickOptions options = new()
            {
                PickerTitle = "Chọn tệp đính kèm",
                FileTypes = customFileType,
            };

            // Mở trình chọn file của hệ thống
            var result = await FilePicker.Default.PickAsync(options);

            if (result != null)
            {
                // 1-Lấy thông tin file
                var fileName = result.FileName;
                var filePath = result.FullPath;

                // 2-Tính toán dung lượng file (tùy chọn để hiển thị)
                var fileInfo = new FileInfo(filePath);
                long sizeInBytes = fileInfo.Length;
                string sizeDisplay = sizeInBytes < 1024 * 1024
                                     ? $"{sizeInBytes / 1024} KB"
                                     : $"{(double)sizeInBytes / (1024 * 1024):F2} MB";

                // 3-Hiển thị thông báo xác nhận hoặc xử lý upload lên Server
                bool confirm = await DisplayAlert("Xác nhận", $"Bạn muốn tải lên: {fileName} ({sizeDisplay})?", "Tải lên", "Hủy");

                if (confirm)
                {
                    // Giả lập thêm vào danh sách hiển thị ngay lập tức
                    // Sẽ gọi API Upload ở đây
                    Attachments.Add(new FileAttachment
                    {
                        FileName = fileName,
                        FileSize = sizeDisplay,
                    });

                    await DisplayAlert("Thành công", "Đã thêm tệp mới vào hồ sơ.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", "Không thể chọn tệp: " + ex.Message, "OK");
        }
    }
}

/// Model đại diện cho một tệp đính kèm
public class FileAttachment
{
    public string FileName { get; set; } = string.Empty;
    public string FileSize { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}