using NCKH.Services;
using System.Collections.ObjectModel;
using System.Windows.Input; // Đảm bảo có thư viện này cho ICommand
using NCKH.Models;
using NCKH.ViewModels;

namespace NCKH.ViewModels
{
    public class TTTVXacNhanTaiLieuViewModel : BaseViewModel
    {
        private readonly TTTVApiService _apiService = new TTTVApiService();
        private List<TTTVHoSoItem> _allData = new();

        public ObservableCollection<TTTVHoSoItem> DanhSachHoSo { get; set; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ExecuteSearch();
            }
        }

        // Khai báo thuộc tính (Property)[cite: 23]
        public ICommand SearchCommand { get; set; }

        public TTTVXacNhanTaiLieuViewModel()
        {
            // Gán giá trị phải nằm trong Constructor[cite: 21, 23]
            SearchCommand = new Command(ExecuteSearch);

            // Gọi hàm load dữ liệu
            _ = LoadApiData();
        }

        private async Task LoadApiData()
        {
            var data = await _apiService.GetTTTVHoSoAsync();
            _allData = data;

            MainThread.BeginInvokeOnMainThread(() => {
                DanhSachHoSo.Clear();
                for (int i = 0; i < _allData.Count; i++)
                {
                    _allData[i].STT = i + 1; // Gán STT bắt đầu từ 1
                    DanhSachHoSo.Add(_allData[i]);
                }
            });
        }

        private void ExecuteSearch()
        {
            var keyword = SearchText?.Trim().ToLower() ?? "";

            var filtered = string.IsNullOrEmpty(keyword)
                ? _allData
                : _allData.Where(h =>
                    h.MaHoSoGoc.ToLower().Contains(keyword) ||
                    h.TenGiangVien.ToLower().Contains(keyword)).ToList();

            MainThread.BeginInvokeOnMainThread(() => {
                DanhSachHoSo.Clear();
                int index = 1;
                foreach (var item in filtered)
                {
                    item.STT = index++; // Cập nhật lại STT cho danh sách kết quả
                    DanhSachHoSo.Add(item);
                }
            });
        }
        public void ApplyStatusFilter(string filter)
        {
            if (_allData == null || !_allData.Any()) return;

            IEnumerable<TTTVHoSoItem> filtered;

            if (string.IsNullOrEmpty(filter))
            {
                filtered = _allData;
            }
            else
            {
                TTTVTrangThai? targetStatus = filter switch
                {
                    "ChoDuyet" => TTTVTrangThai.ChoXuLy,
                    "DaDuyet" => TTTVTrangThai.ConfirmedOnRequestForm,
                    "DaTraVe" => TTTVTrangThai.ReturnedOnRequestForm,
                    _ => null
                };

                filtered = targetStatus.HasValue
                    ? _allData.Where(x => x.TrangThai == targetStatus.Value)
                    : _allData;
            }

            // Dùng MainThread để cập nhật UI mượt mà
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DanhSachHoSo.Clear();
                foreach (var item in filtered)
                {
                    DanhSachHoSo.Add(item);
                }
            });
        }
        public void ApplySearch(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Nếu ô tìm kiếm trống, hiển thị lại toàn bộ danh sách đã lọc theo status
                DanhSachHoSo = new ObservableCollection<TTTVHoSoItem>(_allData);
            }
            else
            {
                // Lọc danh sách theo mã hồ sơ (MaHoSoGoc) hoặc Tên giảng viên
                var filtered = _allData.Where(x =>
                    x.MaHoSoGoc.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    x.TenGiangVien.Contains(searchText, StringComparison.OrdinalIgnoreCase));

                DanhSachHoSo = new ObservableCollection<TTTVHoSoItem>(filtered);
            }
        }
    }
} 