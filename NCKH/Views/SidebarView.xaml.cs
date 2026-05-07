using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NCKH.Components;

public partial class SidebarView : ContentView, INotifyPropertyChanged
{
    public static readonly BindableProperty ActivePageProperty =
        BindableProperty.Create(nameof(ActivePage), typeof(string), typeof(SidebarView), string.Empty);

    public string UserInitial => "A"; // Giả lập lấy chữ cái đầu

    public string ActivePage
    {
        get => (string)GetValue(ActivePageProperty);
        set => SetValue(ActivePageProperty, value);
    }

    private string _activeStatus = string.Empty;
    public string ActiveStatus
    {
        get => _activeStatus;
        set
        {
            _activeStatus = value ?? string.Empty;
            OnPropertyChanged();
            // Notify tất cả các IsActive* để XAML tự re-evaluate binding
            OnPropertyChanged(nameof(IsChoDuyetActive));
            OnPropertyChanged(nameof(IsDaDuyetActive));
            OnPropertyChanged(nameof(IsDaTraVeActive));
        }
    }

    // 3 computed bool để bind màu nền từng mục trong XAML
    public bool IsChoDuyetActive => ActiveStatus == "ChoDuyet";
    public bool IsDaDuyetActive => ActiveStatus == "DaDuyet";
    public bool IsDaTraVeActive => ActiveStatus == "DaTraVe";

    // --- LOGIC MENU TRUNG TÂM THƯ VIỆN ---
    private bool _isLibraryMenuOpen = false;
    public bool IsLibraryMenuOpen
    {
        get => _isLibraryMenuOpen;
        set
        {
            _isLibraryMenuOpen = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ArrowIcon));
        }
    }

    public string ArrowIcon => IsLibraryMenuOpen ? "▼" : "▶";

    // --- ĐIỀU KHIỂN RESPONSIVE ---
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
        UpdateLayout(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);
        DeviceDisplay.MainDisplayInfoChanged += (s, e) => UpdateLayout(e.DisplayInfo.Width / e.DisplayInfo.Density);
    }

    private void UpdateLayout(double width)
    {
        IsMobileMode = width < 768;
        IsSidebarVisible = !IsMobileMode;
    }

    private void OnToggleSidebar(object sender, EventArgs e) => IsSidebarVisible = !IsSidebarVisible;

    private void CloseSidebarIfMobile() { if (IsMobileMode) IsSidebarVisible = false; }

    // --- XỬ LÝ CLICK ---

    private void OnToggleLibraryMenu(object sender, EventArgs e) => IsLibraryMenuOpen = !IsLibraryMenuOpen;

    // QUAN TRỌNG: Dùng Relative Routing (không có //) để tránh lỗi Stack
    // 1. Hồ sơ chờ duyệt
    // 1. Hồ sơ chờ duyệt
    private async void OnHoSoChoDuyetTapped(object sender, EventArgs e)
    {
        CloseSidebarIfMobile();
        ActiveStatus = "ChoDuyet";
        await NavigateToXacNhanPage("ChoDuyet");
    }

    private async void OnHoSoDaDuyetTapped(object sender, EventArgs e)
    {
        CloseSidebarIfMobile();
        ActiveStatus = "DaDuyet";
        await NavigateToXacNhanPage("DaDuyet");
    }

    private async void OnHoSoDaTraVeTapped(object sender, EventArgs e)
    {
        CloseSidebarIfMobile();
        ActiveStatus = "DaTraVe";
        await NavigateToXacNhanPage("DaTraVe");
    }

    private async Task NavigateToXacNhanPage(string status)
    {
        await Shell.Current.GoToAsync($"TTTVXacNhanTaiLieuPage?status={status}");
        MessagingCenter.Send(this, "FilterChanged", status);
    }
    private async void OnThongBaoTapped(object sender, EventArgs e)
    {
        CloseSidebarIfMobile();
        await Shell.Current.GoToAsync("///ThongBaoPage");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool answer = await Shell.Current.DisplayAlert("Xác nhận", "Bạn muốn thoát hệ thống?", "Thoát", "Hủy");
        if (answer) await Shell.Current.GoToAsync("//LoginPage");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}