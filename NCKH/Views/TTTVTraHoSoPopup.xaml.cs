using NCKH.Models;

namespace NCKH.Views
{
    public partial class TTTVTraHoSoPopup : ContentPage
    {
        public event Action<TTTVHoSoItem, string> OnReturnSubmitted;

        private readonly TTTVHoSoItem _item;

        public TTTVTraHoSoPopup(TTTVHoSoItem item)
        {
            InitializeComponent();
            _item = item;

            LabelMaHoSo.Text = $"Mã hồ sơ: {item.MaHoSoGoc}";
            LabelTenGiangVien.Text = $"Giảng viên: {item.TenGiangVien}";
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            string lyDo = LyDoEditor.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(lyDo))
            {
                await DisplayAlert("Lỗi", "Vui lòng nhập lý do trả hồ sơ!", "OK");
                LyDoEditor.Focus();
                return;
            }

            OnReturnSubmitted?.Invoke(_item, lyDo);
            await DisplayAlert("Thành công", "Đã trả hồ sơ về thành công.", "OK");
            await Navigation.PopModalAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnOverlayTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
        private async void OnReturnClicked(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ CommandParameter của MenuFlyoutItem
            var menuItem = sender as MenuFlyoutItem;
            var item = menuItem?.CommandParameter as TTTVHoSoItem;

            if (item != null)
            {
                // Phải có từ khóa 'new' và truyền tham số vào constructor
                var popupPage = new NCKH.Views.TTTVTraHoSoPopup(item);

                // Sử dụng PushModalAsync để hiển thị popup
                await Navigation.PushModalAsync(popupPage);
            }
        }
    }
}
