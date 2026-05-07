using NCKH.Components;

using NCKH.Models;

using NCKH.ViewModels;

using System.Collections.ObjectModel;

namespace NCKH.Views;

[QueryProperty(nameof(StatusFilter), "status")]
public partial class TTTVXacNhanTaiLieuPage : ContentPage
{
    private readonly TTTVXacNhanTaiLieuViewModel _viewModel;

    public TTTVXacNhanTaiLieuPage()
    {
        InitializeComponent();
        _viewModel = new TTTVXacNhanTaiLieuViewModel();
        BindingContext = _viewModel;
    }

    // 1. Property nhận tham số từ Shell
    private string _statusFilter = string.Empty;
    public string StatusFilter
    {
        get => _statusFilter;
        set
        {
            _statusFilter = value ?? string.Empty;
            // QUAN TRỌNG: Gọi hàm lọc và tải lại dữ liệu ngay khi giá trị status thay đổi
            ApplyStatusFilter();
        }
    }
    private async void ApplyStatusFilter()
    {
        if (_viewModel != null)
        {
            // Đợi một chút để đảm bảo ViewModel đã Load xong dữ liệu từ API lần đầu
            // Hoặc kiểm tra nếu danh sách gốc đang trống thì gọi LoadApiData
            if (_viewModel.DanhSachHoSo.Count == 0)
            {
                // Bước này quan trọng nếu người dùng truy cập trực tiếp bằng link route
                await Task.Delay(100);
            }

            _viewModel.ApplyStatusFilter(_statusFilter);

            // Cập nhật tiêu đề trang tương ứng với trạng thái
            PageTitle = _statusFilter switch
            {
                "ChoDuyet" => "Hồ sơ chờ duyệt",
                "DaDuyet" => "Hồ sơ đã duyệt",
                "DaTraVe" => "Hồ sơ đã trả về",
                _ => "Xác nhận tài liệu"
            };
        }
    }
    private string _pageTitle = "Xác nhận tài liệu";
    public string PageTitle
    {
        get => _pageTitle;

        set { _pageTitle = value; OnPropertyChanged(); }
    }
   
    #region Sidebar & Navigation Logic

    private bool _isMobileSidebarOpen = false;

    public bool IsMobileSidebarOpen
    {
        get => _isMobileSidebarOpen;

        set { _isMobileSidebarOpen = value; OnPropertyChanged(); }
    }
    private void OnToggleMobileSidebar(object sender, EventArgs e) => IsMobileSidebarOpen = !IsMobileSidebarOpen;
    private void OnCloseMobileSidebar(object sender, EventArgs e) => IsMobileSidebarOpen = false;
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Mặc định tải hồ sơ "Chờ Duyệt" nếu chưa có tham số truyền vào[cite: 33]

        if (string.IsNullOrEmpty(StatusFilter))

            StatusFilter = "ChoDuyet";

    }
    #endregion

    // 2. Tìm kiếm: Cập nhật Text và kích hoạt lệnh trong ViewModel

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _viewModel.SearchText = e.NewTextValue;

        if (_viewModel.SearchCommand.CanExecute(null))
        {
            _viewModel.SearchCommand.Execute(null);
        }

    }
    // 3. Xem tệp đính kèm (Dùng chung cho cả Desktop và Mobile)[cite: 33]
    private async void OnXemTepDinhKem(object sender, EventArgs e)
    {
        var view = sender as BindableObject;
        if (view?.BindingContext is not TTTVHoSoItem item) return;
        var popup = new TTTVDanhSachTepPopup($"Tệp đính kèm — {item.MaHoSoGoc}", item.DanhSachTepDinhKem);
        await Navigation.PushModalAsync(popup);

    }

    // CỐ ĐỊNH LỖI: EventHandler "OnXemTepDinhKemMobile" missing[cite: 33]

    private async void OnXemTepDinhKemMobile(object sender, EventArgs e)
    {
        OnXemTepDinhKem(sender, e);

    }
    // 4. Xử lý Hành động: Phê duyệt hoặc Trả hồ sơ
    private async void OnActionChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;

        if (picker.SelectedIndex == -1 || picker.BindingContext is not TTTVHoSoItem item) return;
        int actionIndex = picker.SelectedIndex;

        picker.SelectedIndex = -1;
        if (actionIndex == 0) // Phê duyệt
        {
            bool confirm = await DisplayAlert("Xác nhận", $"Phê duyệt hồ sơ {item.MaHoSoGoc}?", "Đồng ý", "Hủy");
            if (confirm)
            {
                // Sau này bạn sẽ gọi: await _viewModel.ApproveHoSoAsync(item);[cite: 23]

                await DisplayAlert("Thành công", "Đã gửi yêu cầu phê duyệt.", "OK");

                _viewModel.ApplyStatusFilter(StatusFilter); // Refresh danh sách
            }
        }
        else if (actionIndex == 1) // Trả hồ sơ
        {
            var popupPage = new TTTVTraHoSoPopup(item);

            popupPage.OnReturnSubmitted += async (hoSo, lyDo) => {

                // Sau này bạn sẽ gọi: await _viewModel.ReturnHoSoAsync(hoSo, lyDo);[cite: 23]

                await DisplayAlert("Thông báo", "Đã trả về hồ sơ.", "OK");

                _viewModel.ApplyStatusFilter(StatusFilter); // Refresh danh sách
            };
            await Navigation.PushModalAsync(popupPage);
        }
    }

    // 5. Upload tệp: Chọn file và cập nhật trạng thái UI[cite: 33]

    private async void OnUploadTep(object sender, EventArgs e)

    {
        var view = sender as BindableObject;

        if (view?.BindingContext is not TTTVHoSoItem item) return;
        try
        {
            var result = await FilePicker.Default.PickMultipleAsync(PickOptions.Default);

            if (result == null) return;
            foreach (var file in result)

            {
                // Thực hiện upload thực tế lên server API tại đây[cite: 23]

                item.DanhSachThemTep.Add(file.FileName);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", "Không thể chọn tệp: " + ex.Message, "OK");
        }
    }
    // CỐ ĐỊNH LỖI: Thêm phương thức Mobile để khớp với XAML[cite: 33]
    private async void OnUploadTepMobile(object sender, EventArgs e)
    {
        OnUploadTep(sender, e);
    }
}