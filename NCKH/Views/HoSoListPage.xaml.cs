using NCKH.Models;
using NCKH.ViewModels;

namespace NCKH.Views
{
    public partial class HoSoListPage : ContentPage
    {
        private HoSo _currentSelectedHoSo;

        public HoSoListPage()
        {
            InitializeComponent();
            BindingContext = HoSoListViewModel.Instance;
            HoSoCollectionView.ItemsSource = HoSoListViewModel.DanhSachHoSo;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await HoSoListViewModel.Instance.LoadDuLieuAsync();

            // Gán STT 1, 2, 3 sau khi load xong
            int i = 1;
            foreach (var item in HoSoListViewModel.DanhSachHoSo)
                item.STT = i++;
        }

        // TÌM KIẾM
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.Trim().ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(searchText))
            {
                HoSoCollectionView.ItemsSource = HoSoListViewModel.DanhSachHoSo;
                return;
            }

            var filtered = HoSoListViewModel.DanhSachHoSo.Where(h =>
              
                h.STT.ToString().Contains(searchText) ||
                // Tìm theo Tiêu đề
                (!string.IsNullOrEmpty(h.TieuDe) && h.TieuDe.ToLower().Contains(searchText)) ||
                // Tìm theo Trạng thái
                (!string.IsNullOrEmpty(h.TrangThai) && h.TrangThai.ToLower().Contains(searchText)) 
                
            ).ToList();

            HoSoCollectionView.ItemsSource = filtered;
        }
        // TẠO MỚI
        private async void OnTaoHoSoClicked(object sender, EventArgs e)
            => await Navigation.PushModalAsync(new TaoHoSoPage());

        // CHI TIẾT POPUP
        private void OnChiTietClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is HoSo hoso)
            {
                _currentSelectedHoSo = hoso;
                PopupTieuDe.Text = hoso.TieuDe ?? "";
                PopupTieuDeDetail.Text = hoso.TieuDe ?? "";
                PopupNgayNop.Text = hoso.NgayNop == default
                                          ? "Chưa có" : hoso.NgayNop.ToString("dd/MM/yyyy");
                PopupTrangThai.Text = hoso.TrangThai;
                PopupBuoc.Text = hoso.BuocHienTai;
                PopupMoTa.Text = string.IsNullOrEmpty(hoso.MoTa)
                                          ? "Không có mô tả." : hoso.MoTa;
                PopupOverlay.IsVisible = true;
            }
        }

        private void OnDongPopupClicked(object sender, EventArgs e)
        {
            PopupOverlay.IsVisible = false;
            _currentSelectedHoSo = null;
        }

        private void OnPopupOverlayTapped(object sender, TappedEventArgs e)
            => PopupOverlay.IsVisible = false;

        // XEM PDF
        private async void OnMinhChungPdfTapped(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is HoSo hoso)
                await Navigation.PushModalAsync(new AttachmentsPage(hoso));
        }

        
        private async void OnPopupXemPdfClicked(object sender, EventArgs e)
        {
            if (_currentSelectedHoSo != null)
            {
                await Navigation.PushModalAsync(new AttachmentsPage(_currentSelectedHoSo));
                PopupOverlay.IsVisible = false;
            }
        }

        // SỬA
        private async void OnSuaClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is HoSo hoso)
            {
                var popupSua = new SuaHoSoPopup(hoso);
                popupSua.OnHoSoUpdated += async (hoSoMoi) =>
                {
                    await HoSoListViewModel.Instance.LoadDuLieuAsync();

                    int i = 1;
                    foreach (var item in HoSoListViewModel.DanhSachHoSo)
                        item.STT = i++;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        HoSoCollectionView.ItemsSource = null;
                        HoSoCollectionView.ItemsSource = HoSoListViewModel.DanhSachHoSo;
                    });
                };
                await Navigation.PushModalAsync(popupSua);
            }
        }

        // XÓA
        private async void OnXoaClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is HoSo hoso)
            {
                var popupXoa = new XoaHoSoPopup(hoso);
                popupXoa.OnXacNhanXoa += async (hosoCanXoa) =>
                {
                   
                    HoSoListViewModel.DanhSachHoSo.Remove(hosoCanXoa);
                    int i = 1;
                    foreach (var item in HoSoListViewModel.DanhSachHoSo)
                        item.STT = i++;
                    await HoSoListViewModel.Instance.LoadDuLieuAsync();

                    i = 1;
                    foreach (var item in HoSoListViewModel.DanhSachHoSo)
                        item.STT = i++;
                };
                await Navigation.PushModalAsync(popupXoa);
            }
        }
        protected override void OnDisappearing()
            => base.OnDisappearing();
    }
}