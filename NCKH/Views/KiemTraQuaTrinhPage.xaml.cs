using Microsoft.Maui.Controls;
using NCKH.Models;
using NCKH.ViewModels;

namespace NCKH.Views
{
    public partial class KiemTraQuaTrinhPage : ContentPage
    {
        public KiemTraQuaTrinhPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await HoSoListViewModel.Instance.LoadDuLieuAsync();
            TienTrinhCollectionView.ItemsSource = HoSoListViewModel.DanhSachHoSo;
        }

        // Expand/Collapse — nhận HoSo object thay vì string tên Grid
        private void OnExpandTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is HoSo hoso)
                hoso.IsExpanded = !hoso.IsExpanded;
        }

        private async void OnMenuClicked(object sender, EventArgs e)
            => await SidebarControl.TranslateTo(0, 0, 250, Easing.CubicOut);

        private async void OnCloseMenuClicked(object sender, EventArgs e)
            => await SidebarControl.TranslateTo(-300, 0, 250, Easing.CubicIn);
    }
}