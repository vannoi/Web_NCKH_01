using NCKH.Views;

namespace NCKH;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("TaoHoSoPage", typeof(Views.TaoHoSoPage));

        

        // Trang Quản lý định mức (Trang có bảng STT, Mã, Tên định mức...)
        // Phải trỏ về đúng Class QuanLyDinhMucPage
        
        Routing.RegisterRoute("ThemDinhMucPage", typeof(ThemDinhMucPage));
        Routing.RegisterRoute("AttachmentsPage", typeof(Views.AttachmentsPage));
        // Trong AppShell.xaml.cs hoặc nơi bạn đăng ký route
        Routing.RegisterRoute("AttachmentsPage", typeof(NCKH.Views.AttachmentsPage));
    }
}
