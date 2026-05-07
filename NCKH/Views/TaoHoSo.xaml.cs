
using Microsoft.Maui.Storage;
using NCKH.Models;
using NCKH.Services;
using NCKH.ViewModels;

namespace NCKH.Views;

public partial class TaoHoSoPage : ContentPage
{
    private FileResult _selectedFile;
    private readonly ApiService _apiService = new();
    private bool _isSubmitting = false;

    public TaoHoSoPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
        => await ClosePage();

    private async void OnUploadFileTapped(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Chọn tệp PDF",
                FileTypes = FilePickerFileType.Pdf
            });
            if (result != null)
            {
                _selectedFile = result;
                FileNameLabel.Text = result.FileName;
                SelectedFileLayout.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FilePicker] Error: {ex.Message}");
        }
    }

    private void OnRemoveFileClicked(object sender, EventArgs e)
    {
        _selectedFile = null;
        SelectedFileLayout.IsVisible = false;
    }

    private async void OnNopHoSoClicked(object sender, EventArgs e)
    {
        if (!Validate(requireFile: true)) return;
        if (_isSubmitting) return;
        _isSubmitting = true;

        try
        {
            // Kiểm tra file size TRƯỚC — giới hạn 10MB
            if (_selectedFile != null)
            {
                var fileInfo = new FileInfo(_selectedFile.FullPath);
                if (fileInfo.Length > 10L * 1024 * 1024) // 10MB
                {
                    await DisplayAlert("File quá lớn",
                        "Vui lòng chọn file nhỏ hơn 10MB!", "OK");
                    _isSubmitting = false;
                    return;
                }
            }

            var hosoMoi = new HoSo
            {
                TieuDe = TieuDeEntry.Text.Trim(),
                MoTa = MoTaEditor.Text.Trim(),
                Step = 0,
            };

            string newId = await _apiService.AddHoSoAsync(hosoMoi);
            if (string.IsNullOrEmpty(newId))
            {
                await DisplayAlert("Lỗi", "Không thể tạo hồ sơ!", "OK");
                return;
            }

            // Upload file nếu có
            if (_selectedFile != null)
            {
                bool uploaded = await _apiService.UploadFileAsync(newId, _selectedFile);
                if (!uploaded)
                    await DisplayAlert("Lưu ý",
                        "Hồ sơ đã tạo nhưng file chưa upload được.\nVui lòng thử file nhỏ hơn!", "OK");
            }

            await HoSoListViewModel.Instance.LoadDuLieuAsync();
            await DisplayAlert("Thành công", "Đã tạo hồ sơ mới!", "OK");
            await ClosePage();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", ex.Message, "OK");
        }
        finally
        {
            _isSubmitting = false;
        }
    }
    private bool Validate(bool requireFile)
    {
        if (string.IsNullOrWhiteSpace(TieuDeEntry.Text))
        {
            DisplayAlert("Lỗi", "Vui lòng nhập tiêu đề hồ sơ!", "OK");
            TieuDeEntry.Focus();
            return false;
        }
        if (string.IsNullOrWhiteSpace(MoTaEditor.Text) || MoTaEditor.Text.Trim().Length < 5)
        {
            DisplayAlert("Lỗi", "Mô tả quá ngắn (ít nhất 5 ký tự)!", "OK");
            MoTaEditor.Focus();
            return false;
        }
        if (requireFile && _selectedFile == null)
        {
            DisplayAlert("Lỗi", "Vui lòng đính kèm tập tin PDF!", "OK");
            return false;
        }
        return true;
    }
   
    private async Task ClosePage()
    {
        if (Navigation.ModalStack.Count > 0)
            await Navigation.PopModalAsync();
        else
            await Shell.Current.GoToAsync("..");
    }
}