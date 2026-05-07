using NCKH.Models;
using NCKH.Services;

namespace NCKH.Views
{
    public partial class XoaHoSoPopup : ContentPage
    {
        private HoSo _hoSo;
        private readonly ApiService _apiService = new();

        public event Action<HoSo>? OnXacNhanXoa;

        public XoaHoSoPopup(HoSo hoSo)
        {
            InitializeComponent();
            _hoSo = hoSo;
        }

        private async void OnHuyXoaClicked(object sender, EventArgs e)
            => await Navigation.PopModalAsync();

        // Xác nhận xóa — gọi API DELETE
        private async void OnXacNhanXoaClicked(object sender, EventArgs e)
        {
            bool success = await _apiService.DeleteHoSoAsync(_hoSo.Id);
            if (success)
            {
                OnXacNhanXoa?.Invoke(_hoSo); // ← Gọi event về HoSoListPage
                await Navigation.PopModalAsync();
            }
        }
    }
}
