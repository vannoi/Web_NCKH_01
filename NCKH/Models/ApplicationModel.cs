using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NCKH.Models
{
    public class ApplicationResponse
    {
        public List<ApplicationModel> items { get; set; } = new List<ApplicationModel>();
        public int totalCount { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
    }

    public class ApplicationModel : INotifyPropertyChanged
    {
        // --- Các thuộc tính gốc từ API ---
        public Guid id { get; set; }
        public string? code { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public int? status { get; set; }
        public Guid? stepDetailId { get; set; }

        // --- Danh sách file đính kèm ---
        public List<FileAttachment> applicationFiles { get; set; } = new List<FileAttachment>();
        public List<FileAttachment> myApplications { get; set; } = new List<FileAttachment>();
        public List<FileAttachment> preAttachments { get; set; } = new List<FileAttachment>();

        // --- Thông tin bổ sung hiển thị trên UI ---
        private string? _teacherName = "Đang cập nhật...";
        public string? teacherName
        {
            get => _teacherName;
            set { _teacherName = value; OnPropertyChanged(); OnPropertyChanged(nameof(TeacherDisplay)); }
        }
        public string TeacherDisplay => string.IsNullOrEmpty(teacherName) || teacherName == "Đang cập nhật..."
            ? "Chưa có thông tin"
            : teacherName;

        // Logic xử lý quy trình
        // Đơn vị tiếp theo trong quy trình (Dùng để hiển thị trong Picker duyệt)
        public string NextStepUnit => "Trung tâm Thư viện";

        // Đơn vị nhận lại nếu bị trả hồ sơ
        public string ReturnStepUnit => "Giảng viên";


        // Danh sách các hành động để Bind vào Picker trong DvqlttListPage.xaml
        public List<string> AvailableActions => new List<string>
        {
            $"Chuyển đến {NextStepUnit}",
            $"Trả hồ sơ cho {ReturnStepUnit}"
        };

        // Hiển thị trạng thái bằng tiếng Việt dựa trên mã status
        public string StatusDisplay => status switch
        {
            0 => "Chờ xử lý",
            1 => "Đang kiểm tra",
            2 => "Đã duyệt & Chuyển tiếp",
            3 => $"Đã trả về {ReturnStepUnit}",
            _ => "Chưa xác định"
        };

        // Logic cập nhật giao diện
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}