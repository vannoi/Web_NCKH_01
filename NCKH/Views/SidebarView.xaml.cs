using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
namespace NCKH.Components;

public partial class SidebarView : ContentView, INotifyPropertyChanged
{
    public static readonly BindableProperty ActivePageProperty =
        BindableProperty.Create(nameof(ActivePage), typeof(string), typeof(SidebarView), string.Empty);
    public string UserInitial => "Nguyễn Văn A".Split(' ').Last().Substring(0, 1).ToUpper();
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // Hiển thị thông báo xác nhận nhanh
        bool answer = await Shell.Current.DisplayAlert("Xác nhận", "Bạn muốn thoát hệ thống?", "Thoát", "Hủy");

        if (answer)
        {
            // Về trang đăng nhập (Thay "LoginPage" bằng tên Route trang Login của bạn)
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
    public string ActivePage
    {
        get => (string)GetValue(ActivePageProperty);
        set => SetValue(ActivePageProperty, value);
    }

    private bool _isHoSoOpen = false;
    public bool IsHoSoOpen
    {
        get => _isHoSoOpen;
        set { _isHoSoOpen = value; OnPropertyChanged(); OnPropertyChanged(nameof(ArrowIcon)); }
    }
    public string ArrowIcon => IsHoSoOpen ? "▼" : "▶";

    private bool _isMenuVisible = false;
    public bool IsMenuVisible
    {
        get => _isMenuVisible;
        set { _isMenuVisible = value; OnPropertyChanged(); }
    }

    // --- THÊM BIẾN ĐIỀU KHIỂN MOBILE ---
    private bool _isSidebarVisible = true;
    public bool IsSidebarVisible
    {
        get => _isSidebarVisible;
        set { _isSidebarVisible = value; OnPropertyChanged(); }
    }

    private bool _isMobileMode = false;
    public bool IsMobileMode
    {
        get => _isMobileMode;
        set { _isMobileMode = value; OnPropertyChanged(); }
    }

    public SidebarView()
    {
        InitializeComponent();
        this.BindingContext = this;

        // Lắng nghe sự kiện thay đổi kích thước màn hình
        DeviceDisplay.MainDisplayInfoChanged += OnDisplayInfoChanged;

        // Kiểm tra kích thước ngay khi khởi tạo
        UpdateLayout(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);
    }

    private void OnDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        UpdateLayout(e.DisplayInfo.Width / e.DisplayInfo.Density);
    }

    private void UpdateLayout(double width)
    {
        // Nếu màn hình nhỏ hơn 768px thì coi là Mobile
        if (width < 768)
        {
            IsMobileMode = true;
            IsSidebarVisible = false; // Mặc định ẩn menu trên mobile
        }
        else
        {
            IsMobileMode = false;
            IsSidebarVisible = true; // Luôn hiện menu trên Web
        }
    }

    // Hàm để nút Hamburger gọi (đóng/mở menu trên mobile)
    private void OnToggleSidebar(object sender, EventArgs e)
    {
        if (IsMobileMode)
        {
            IsSidebarVisible = !IsSidebarVisible;
        }
    }

    // Khi bấm vào một Menu Item, nếu đang ở mobile thì tự đóng Sidebar lại
    private void CloseSidebarIfMobile()
    {
        if (IsMobileMode)
        {
            IsSidebarVisible = false;
        }
    }

    private void OnToggleHoSoMenu(object sender, EventArgs e) => IsHoSoOpen = !IsHoSoOpen;

    private async void OnDashboardTapped(object sender, EventArgs e)
    {
        CloseSidebarIfMobile();
        await Shell.Current.GoToAsync("//DashboardPage");
    }

    private async void OnDanhSachHoSoTapped(object sender, EventArgs e)
    {
        CloseSidebarIfMobile();
        await Shell.Current.GoToAsync("//HoSoListPage");
    }

    private async void OnKiemTraTapped(object sender, EventArgs e)
    {
        CloseSidebarIfMobile();
        await Shell.Current.GoToAsync("//KiemTraQuaTrinhPage");
    }

    

    private async void OnThongBaoTapped(object sender, EventArgs e)
    {
        CloseSidebarIfMobile();
        await Shell.Current.GoToAsync("//ThongBaoPage");
    }

    private void OnUserMenuTapped(object sender, EventArgs e) => IsMenuVisible = !IsMenuVisible;

    private async void OnLogoutTapped(object sender, EventArgs e) => await Shell.Current.GoToAsync("//LoginPage");

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}