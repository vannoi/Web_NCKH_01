using NCKH.ViewModels;

namespace NCKH.Views
{
    // 1. Tên class PHẢI khớp hoàn toàn với x:Class="NCKH.Views.QuanLyPheDuyetPage" trong file XAML
    public partial class QuanLyPheDuyetPage : ContentPage
    {
        public QuanLyPheDuyetPage()
        {
            try
            {
                InitializeComponent();

                // 2. Gán BindingContext để giao diện nhận dữ liệu từ ViewModel
                BindingContext = new QuanLyNghienCuuViewModel();
            }
            catch (Exception ex)
            {
                // Hỗ trợ debug nếu XAML có lỗi cấu trúc
                System.Diagnostics.Debug.WriteLine($"XAML Error: {ex.Message}");
            }
        }

 

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Có thể làm mới dữ liệu tại đây nếu cần
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}