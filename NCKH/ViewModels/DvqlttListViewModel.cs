using NCKH.Models;
using NCKH.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NCKH.ViewModels
{
    public class DvqlttViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService = new ApiService();
        private List<ApplicationModel> _tatCaHoSo = new List<ApplicationModel>();
        private ObservableCollection<ApplicationModel> _danhSachHoSo;
        private bool _isRefreshing;

        public ObservableCollection<ApplicationModel> DanhSachHoSo
        {
            get => _danhSachHoSo;
            set { _danhSachHoSo = value; OnPropertyChanged(); }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set { _isRefreshing = value; OnPropertyChanged(); }
        }

        // Thêm Command để bind vào RefreshView trong XAML
        public ICommand RefreshCommand { get; }

        public DvqlttViewModel()
        {
            DanhSachHoSo = new ObservableCollection<ApplicationModel>();
            RefreshCommand = new Command(async () => await RefreshDataAsync());
        }

        public async Task RefreshDataAsync()
        {
            IsRefreshing = true;
            try
            {
                // Gọi API với logic stepId đã chỉnh trong ApiService
                var response = await _apiService.GetApplicationsAsync();

                Console.WriteLine($"DEBUG: So luong ho so tu API: {response?.Count ?? 0}");

                _tatCaHoSo = response ?? new List<ApplicationModel>();

                // Xử lý logic phân tách minh chứng ngay khi nhận dữ liệu
                foreach (var app in _tatCaHoSo)
                {
                    PhanLoaiMinhChung(app);
                }

                MainThread.BeginInvokeOnMainThread(() => {
                    DanhSachHoSo = new ObservableCollection<ApplicationModel>(_tatCaHoSo);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải dữ liệu: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        // Logic phân tách minh chứng
        private void PhanLoaiMinhChung(ApplicationModel app)
        {
            if (app.applicationFiles == null) return;

            app.preAttachments = app.applicationFiles
                .Where(f => f.isInternal == false) // File từ bước trước là public
                .ToList();

            app.myApplications = app.applicationFiles
                .Where(f => f.isInternal == true)  // File của đơn vị là internal
                .ToList();
        }

        public void FilterHoSo(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                DanhSachHoSo = new ObservableCollection<ApplicationModel>(_tatCaHoSo);
                return;
            }

            var ketQua = _tatCaHoSo.Where(x =>
                (x.title?.ToLower().Contains(tuKhoa.ToLower()) ?? false) ||
                (x.code?.ToLower().Contains(tuKhoa.ToLower()) ?? false)
            ).ToList();

            DanhSachHoSo = new ObservableCollection<ApplicationModel>(ketQua);
        }

        // Thêm phương thức xử lý cập nhật trạng thái
        public async Task UpdateAppStatus(ApplicationModel app, int newStatus, string note)
        {
            if (app?.stepDetailId == null) return;

            // Hiển thị trạng thái đang xử lý
            bool success = await _apiService.UpdateApplicationStatusAsync(
                app.id,
                app.stepDetailId.Value,
                newStatus,
                note);

            if (success)
            {
                // Thông báo thành công
                await Shell.Current.DisplayAlert("Thành công", "Dữ liệu đã được cập nhật.", "OK");
                await RefreshDataAsync();
            }
            else
            {
                // Thông báo lỗi nếu API trả về false
                await Shell.Current.DisplayAlert("Lỗi", "Không thể cập nhật hồ sơ. Vui lòng thử lại.", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}