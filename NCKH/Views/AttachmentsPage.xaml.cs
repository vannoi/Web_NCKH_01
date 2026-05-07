using NCKH.Models;
using NCKH.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NCKH.Views;

public partial class AttachmentsPage : ContentPage, INotifyPropertyChanged
{
    private HoSo _hoSo;
    private readonly ApiService _apiService = new();

    public ObservableCollection<FileAttachment> Attachments { get; set; } = new();

    public AttachmentsPage(HoSo hoso)
    {
        InitializeComponent();
        _hoSo = hoso;
        BindingContext = this;
        LabelTenHoSo.Text = $"Hồ sơ: {hoso.TieuDe}";
        LoadFilesFromHoSo(hoso);

        System.Diagnostics.Debug.WriteLine($"[ATTACH] TieuDe: {hoso.TieuDe}");
        System.Diagnostics.Debug.WriteLine($"[ATTACH] MyApplications count: {hoso.MyApplications?.Count ?? 0}");
        System.Diagnostics.Debug.WriteLine($"[ATTACH] ApplicationFiles count: {hoso.ApplicationFiles?.Count ?? 0}");

        foreach (var item in hoso.MyApplications ?? new())
        {
            System.Diagnostics.Debug.WriteLine($"[ATTACH] item null? {item == null}");
            System.Diagnostics.Debug.WriteLine($"[ATTACH] item.FileId: {item?.FileId}");
            System.Diagnostics.Debug.WriteLine($"[ATTACH] item.File null? {item?.File == null}");
            System.Diagnostics.Debug.WriteLine($"[ATTACH] item.File.Name: {item?.File?.Name}");
        }
    }

    private void LoadFilesFromHoSo(HoSo hoso)
    {
        Attachments.Clear();
        foreach (var item in hoso.MyApplications ?? new())
        {
            if (item.File == null) continue;
            Attachments.Add(new FileAttachment
            {
                Id = item.File.Id,
                FileName = item.File.Name,
                FileSize = item.File.FileSizeDisplay,
            });
        }
        System.Diagnostics.Debug.WriteLine($"[LOAD] Attachments.Count sau load: {Attachments.Count}");
    }

    private async Task ReloadFilesAsync()
    {
        var list = await _apiService.GetHoSoListAsync();
        var updated = list.FirstOrDefault(h => h.Id == _hoSo.Id);
        if (updated != null)
        {
            _hoSo = updated;
            LoadFilesFromHoSo(updated);
        }
    }

    private async void OnBackTapped(object sender, EventArgs e)
        => await Navigation.PopModalAsync();

    private async void OnUploadFileTapped(object sender, EventArgs e)
    {
        // Chặn nếu không phải Draft
        if (_hoSo.Step != 0)
        {
            await DisplayAlert("Không thể upload",
                $"Hồ sơ đang ở '{_hoSo.TrangThai}'.\nChỉ upload được khi ở trạng thái Bản nháp!", "OK");
            return;
        }

        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Chọn tệp đính kèm",
                FileTypes = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.Android, new[] { "application/*", "image/*" } },
                        { DevicePlatform.iOS, new[] { "public.item" } },
                        { DevicePlatform.WinUI, new[] { ".pdf", ".docx", ".jpg", ".png" } },
                    })
            });

            if (result == null) return;

            // Kiểm tra kích thước file — giới hạn 10MB
            var fileInfo = new FileInfo(result.FullPath);
            if (fileInfo.Length > 10L * 1024 * 1024)
            {
                await DisplayAlert("File quá lớn",
                    $"File '{result.FileName}' có dung lượng {fileInfo.Length / (1024.0 * 1024.0):F1} MB.\n" +
                    $"Vui lòng chọn file nhỏ hơn 10 MB!", "OK");
                return;
            }

            bool confirm = await DisplayAlert("Xác nhận",
                $"Tải lên file: {result.FileName}?", "Tải lên", "Hủy");
            if (!confirm) return;

            bool ok = await _apiService.UploadFileAsync(_hoSo.Id, result);

            if (ok)
            {
                await DisplayAlert("Thành công", "Đã upload file!", "OK");
                await ReloadFilesAsync();
            }
            else
            {
                await DisplayAlert("Không có quyền",
                    "Tài khoản không có quyền upload file cho hồ sơ này.\nKiểm tra lại role hoặc trạng thái hồ sơ!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", $"Không thể chọn tệp: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteFileTapped(object sender, EventArgs e)
    {
        var file = (sender as Button)?.CommandParameter as FileAttachment;
        if (file == null) return;

        bool confirm = await DisplayAlert("Xác nhận xóa",
            $"Bạn có chắc muốn xóa '{file.FileName}'?", "Xóa", "Hủy");
        if (!confirm) return;

        bool ok = await _apiService.DeleteFileAsync(_hoSo.Id, file.Id);

        if (ok)
        {
            Attachments.Remove(file);
            await ReloadFilesAsync();
        }
        else
            await DisplayAlert("Lỗi", "Không thể xóa file! Vui lòng thử lại.", "OK");
    }

    private async void OnDownloadSingleTapped(object sender, EventArgs e)
    {
        var file = (sender as Button)?.CommandParameter as FileAttachment;
        if (file != null)
            await DisplayAlert("Tải xuống", $"Đang tải: {file.FileName}", "OK");
    }

    private async void OnDownloadAllTapped(object sender, EventArgs e)
    {
        if (Attachments.Count == 0)
        {
            await DisplayAlert("Thông báo", "Không có tệp nào!", "OK");
            return;
        }
        await DisplayAlert("Tải tất cả",
            $"Đang chuẩn bị {Attachments.Count} tệp...", "OK");
    }

    public new event PropertyChangedEventHandler PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
