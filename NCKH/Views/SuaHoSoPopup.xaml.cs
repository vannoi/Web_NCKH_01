using NCKH.Models;
using NCKH.Services;
using Microsoft.Maui.Storage;

namespace NCKH.Views
{
    public partial class SuaHoSoPopup : ContentPage
    {
        private HoSo _hoSo;
        private FileResult _selectedFile;
        private readonly ApiService _apiService = new();

        public event Action<HoSo>? OnHoSoUpdated;

        public SuaHoSoPopup(HoSo hoSo)
        {
            InitializeComponent();
            _hoSo = hoSo;

            // Đổ dữ liệu cũ vào form
            EditTieuDe.Text = hoSo.TieuDe;
            EditMoTa.Text = hoSo.MoTa;
        }

        // Chọn tệp PDF mới
        private async void OnChonFileTapped(object sender, EventArgs e)
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
                System.Diagnostics.Debug.WriteLine($"FilePicker error: {ex.Message}");
            }
        }

        private void OnXoaFileClicked(object sender, EventArgs e)
        {
            _selectedFile = null;
            SelectedFileLayout.IsVisible = false;
        }

        private async void OnHuySuaClicked(object sender, EventArgs e)
            => await Navigation.PopModalAsync();

        // Lưu thay đổi — gọi API Update
        private async void OnLuuSuaClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EditTieuDe.Text))
            {
                await DisplayAlert("Lỗi", "Vui lòng nhập tiêu đề hồ sơ!", "OK");
                return;
            }

            // Chỉ cập nhật title và mô tả, KHÔNG đổi status
            _hoSo.TieuDe = EditTieuDe.Text.Trim();
            _hoSo.MoTa = EditMoTa.Text?.Trim() ?? "";
            // Giữ _hoSo.Step = 0 (backend chỉ cho sửa hồ sơ ở trạng thái Bản nháp)

            bool success = await _apiService.UpdateHoSoAsync(_hoSo.Id, _hoSo);

            if (success)
            {
                OnHoSoUpdated?.Invoke(_hoSo);
                await DisplayAlert("Thành công", "Đã cập nhật hồ sơ!", "OK");
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Lỗi", "Không thể cập nhật. Vui lòng thử lại!", "OK");
            }
        }
    }
}
