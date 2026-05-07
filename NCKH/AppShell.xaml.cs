using NCKH.Views;
namespace NCKH;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Đăng ký Route cho trang xác nhận tài liệu
        Routing.RegisterRoute("TTTVXacNhanTaiLieuPage", typeof(NCKH.Views.TTTVXacNhanTaiLieuPage));
    }
}
