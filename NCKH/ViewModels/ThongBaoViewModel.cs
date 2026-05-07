using NCKH.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NCKH.ViewModels
{
    public class ThongBaoViewModel : INotifyPropertyChanged
    {
        private List<NotificationModel> _allNotifications;
        private string _selectedFilter = "Tất cả";
        private ObservableCollection<NotificationModel> _displayNotifications;

        public string SelectedFilter
        {
            get => _selectedFilter;
            set { _selectedFilter = value; OnPropertyChanged(); UpdateTabColors(); }
        }

        public ObservableCollection<NotificationModel> DisplayNotifications
        {
            get => _displayNotifications;
            set { _displayNotifications = value; OnPropertyChanged(); }
        }

        // --- Tab Colors Binding (Giữ nguyên logic của Hiền) ---
        public Color TabAllBg => SelectedFilter == "Tất cả" ? Color.FromArgb("#5A67D8") : Colors.White;
        public Color TabAllText => SelectedFilter == "Tất cả" ? Colors.White : Color.FromArgb("#64748B");
        public Color TabUnreadBg => SelectedFilter == "Chưa đọc" ? Color.FromArgb("#5A67D8") : Colors.White;
        public Color TabUnreadText => SelectedFilter == "Chưa đọc" ? Colors.White : Color.FromArgb("#64748B");
        // ... Tương tự cho TabHoSo và TabHeThong

        public ICommand FilterCommand { get; }
        public ICommand ItemTappedCommand { get; }

        public ThongBaoViewModel()
        {
            FilterCommand = new Command<string>(ExecuteFilterCommand);
            ItemTappedCommand = new Command<NotificationModel>(OnItemTapped);
            LoadData();
        }

        private void LoadData()
        {
            _allNotifications = new List<NotificationModel>
            {
                 new NotificationModel { Title = "Hồ sơ được duyệt", Description = "Hồ sơ Bài báo Scopus Q1 của bạn đã được thông qua.", Time = "5 phút trước", Icon = "📋", Type = "HoSo", IsUnread = true, IconBgColor = Color.FromArgb("#EEF2FF") },
                 new NotificationModel { Title = "Hồ sơ cần bổ sung", Description = "Cần bổ sung minh chứng PDF cho đề tài Cấp Cơ sở.", Time = "2 giờ trước", Icon = "⚠️", Type = "HoSo", IsUnread = true, IconBgColor = Color.FromArgb("#FFF7ED") },
                 new NotificationModel { Title = "Giải ngân thành công", Description = "Kinh phí đợt 1 cho đề tài đã được chuyển vào tài khoản.", Time = "1 ngày trước", Icon = "💰", Type = "HoSo", IsUnread = true, IconBgColor = Color.FromArgb("#DCFCE7") },
                 new NotificationModel { Title = "Nhắc nhở nộp báo cáo", Description = "Hạn chót nộp báo cáo tiến độ quý 3 là ngày 15/11.", Time = "3 ngày trước", Icon = "⏰", Type = "HoSo", IsUnread = false, IconBgColor = Color.FromArgb("#FEF9C3") },
                 new NotificationModel { Title = "Kết quả phản biện", Description = "Đã có kết quả đánh giá Hội đồng vòng 1, vui lòng xem chi tiết.", Time = "5 ngày trước", Icon = "📝", Type = "HoSo", IsUnread = false, IconBgColor = Color.FromArgb("#F3E8FF") },

   
                  new NotificationModel { Title = "Quy định mới về NCKH", Description = "Cập nhật quy chuẩn trích dẫn tài liệu tham khảo năm học mới.", Time = "10 phút trước", Icon = "📚", Type = "HeThong", IsUnread = true, IconBgColor = Color.FromArgb("#E0E7FF") },
                  new NotificationModel { Title = "Cảnh báo bảo mật", Description = "Phát hiện đăng nhập lạ. Vui lòng đổi mật khẩu nếu không phải bạn.", Time = "4 giờ trước", Icon = "🔒", Type = "HeThong", IsUnread = true, IconBgColor = Color.FromArgb("#FFE4E6") },
                  new NotificationModel { Title = "Cập nhật hệ thống", Description = "Hệ thống Quản lý NCKH đã được nâng cấp lên phiên bản v2.1.", Time = "1 tuần trước", Icon = "🔧", Type = "HeThong", IsUnread = false, IconBgColor = Color.FromArgb("#F1F5F9") },
                  new NotificationModel { Title = "Bảo trì định kỳ", Description = "Hệ thống sẽ tạm ngưng từ 00:00 - 04:00 Chủ Nhật tuần này.", Time = "2 tuần trước", Icon = "⚙️", Type = "HeThong", IsUnread = false, IconBgColor = Color.FromArgb("#FEE2E2") },
                  new NotificationModel { Title = "Khảo sát người dùng", Description = "Mời bạn tham gia khảo sát để cải thiện trải nghiệm hệ thống.", Time = "1 tháng trước", Icon = "✨", Type = "HeThong", IsUnread = false, IconBgColor = Color.FromArgb("#FEF08A") }
            };
            DisplayNotifications = new ObservableCollection<NotificationModel>(_allNotifications);
        }

        private void ExecuteFilterCommand(string filterType)
        {
            SelectedFilter = filterType;
            var filtered = _allNotifications.AsEnumerable();

            if (filterType == "Chưa đọc") filtered = _allNotifications.Where(x => x.IsUnread);
            else if (filterType == "Hồ sơ") filtered = _allNotifications.Where(x => x.Type == "HoSo");
            else if (filterType == "Hệ thống") filtered = _allNotifications.Where(x => x.Type == "HeThong");

            DisplayNotifications = new ObservableCollection<NotificationModel>(filtered);
        }

        private async void OnItemTapped(NotificationModel notification)
        {
            if (notification == null) return;

            // 1. Đánh dấu đã đọc -> UI tự động mờ và mất chấm xanh nhờ NotifyPropertyChanged
            notification.IsUnread = false;

            // 2. Nếu đang ở tab "Chưa đọc", xóa nó khỏi danh sách hiển thị ngay
            if (SelectedFilter == "Chưa đọc")
            {
                DisplayNotifications.Remove(notification);
            }

            await App.Current.MainPage.DisplayAlert("Thông báo", $"Nội dung: {notification.Description}", "Đóng");
        }

        private void UpdateTabColors()
        {
            OnPropertyChanged(nameof(TabAllBg)); OnPropertyChanged(nameof(TabAllText));
            OnPropertyChanged(nameof(TabUnreadBg)); OnPropertyChanged(nameof(TabUnreadText));
            // ... OnPropertyChanged cho các tab còn lại
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}