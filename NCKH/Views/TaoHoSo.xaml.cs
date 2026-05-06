using Microsoft.Maui.Storage;
using NCKH.Models;
using NCKH.Services;
using NCKH.ViewModels;

namespace NCKH.Views;

public partial class TaoHoSoPage : ContentPage
{
    private FileResult _selectedFile;
    private readonly ApiService _apiService = new();

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
        catch { }
    }

    private void OnRemoveFileClicked(object sender, EventArgs e)
    {
        _selectedFile = null;
        SelectedFileLayout.IsVisible = false;
    }

    // GỬI HỒ SƠ → status = 1 (Đang xét duyệt)
    private async void OnNopHoSoClicked(object sender, EventArgs e)
    {
        if (!Validate(requireFile: true)) return;

        var stream = await _selectedFile.OpenReadAsync();
        var hosoMoi = new HoSo
        {
            TieuDe = TieuDeEntry.Text.Trim(),
            MoTa = MoTaEditor.Text.Trim(),
            Step = 1,   // ← teacher luôn nộp với status = 1
            NgayNop = DateTime.Now,
            FileStream = stream,
            FileName = _selectedFile.FileName
        };

        bool success = await _apiService.AddHoSoAsync(hosoMoi);
        if (success)
        {
            await HoSoListViewModel.Instance.LoadDuLieuAsync();
            await DisplayAlert("Thành công", "Đã gửi hồ sơ mới!", "OK");
            await ClosePage();
        }
        else
            await DisplayAlert("Lỗi", "Không thể gửi hồ sơ. Vui lòng thử lại!", "OK");
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