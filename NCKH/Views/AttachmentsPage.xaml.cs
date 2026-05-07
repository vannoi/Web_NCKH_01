using NCKH.Models;
using NCKH.Services;
using System;
using System.Collections.ObjectModel;

namespace NCKH.Views;

public partial class AttachmentsPage : ContentPage
{
    public bool IsPrivateMode { get; set; }
    public ObservableCollection<FileAttachment> Attachments { get; set; } = new();

    // Thêm biến để lưu ApplicationId
    private Guid _applicationId;
    private readonly ApiService _apiService = new ApiService();

    public AttachmentsPage(Guid applicationId, string tenHoSo, List<FileAttachment> filesHienThi, bool isPrivate = false)
    {
        InitializeComponent();
        _applicationId = applicationId; // Lưu lại ID hồ sơ
        this.IsPrivateMode = isPrivate;

        if (LabelTenHoSo != null)
        {
            LabelTenHoSo.Text = isPrivate ? $"Tệp riêng: {tenHoSo}" : $"Minh chứng: {tenHoSo}";
        }

        Attachments.Clear();
        if (filesHienThi != null)
        {
            foreach (var file in filesHienThi)
            {
                Attachments.Add(file);
            }
        }

        // Gán BindingContext để các thuộc tính IsPrivateMode hoạt động trong XAML
        BindingContext = this;
    }

    private async void OnDownloadSingleTapped(object sender, EventArgs e)
    {
        var file = (sender as Button)?.CommandParameter as FileAttachment;
        // Sử dụng thuộc tính 'fileUrl'
        if (file != null && !string.IsNullOrEmpty(file.fileUrl))
        {
            await Launcher.Default.OpenAsync(new Uri(file.fileUrl));
        }
        else
        {
            await DisplayAlert("Lỗi", "Không tìm thấy đường dẫn tải tệp.", "OK");
        }
    }

    private async void OnUploadFileTapped(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                // Gọi API Upload thực tế
                bool success = await _apiService.UploadAttachmentAsync(_applicationId, result);

                if (success)
                {
                    await DisplayAlert("Thành công", "Đã tải file lên hệ thống.", "OK");
                    Attachments.Add(new FileAttachment { fileName = result.FileName, isInternal = true });
                }
            }
        }
        catch (Exception ex) { /* Handle error */ }
    }

    private async void OnDeleteFileTapped(object sender, EventArgs e)
    {
        var file = (sender as Button)?.CommandParameter as FileAttachment;
        if (file != null)
        {
            bool confirm = await DisplayAlert("Xác nhận", $"Xóa file {file.fileName}?", "Xóa", "Hủy");
            if (confirm)
            {
                // Gọi API Delete thực tế
                bool success = await _apiService.DeleteAttachmentAsync(_applicationId, file.id);
                if (success)
                {
                    Attachments.Remove(file);
                }
                else
                {
                    await DisplayAlert("Lỗi", "Không thể xóa file trên server.", "OK");
                }
            }
        }
    }

    private async void OnDownloadAllTapped(object sender, EventArgs e)
    {
        if (Attachments.Count == 0)
        {
            await DisplayAlert("Thông báo", "Không có tệp nào để tải xuống.", "OK");
            return;
        }

        await DisplayAlert("Tải tất cả", $"Đang chuẩn bị tải {Attachments.Count} tệp...", "Bắt đầu");
    }

    private async void OnBackTapped(object sender, EventArgs e) => await Navigation.PopModalAsync();
}