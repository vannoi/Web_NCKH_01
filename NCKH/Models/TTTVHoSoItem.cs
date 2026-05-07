using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NCKH.Models
{
    public class TTTVHoSoItem : INotifyPropertyChanged
    {
        public int STT { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string MaHoSoGoc { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        // Hiển thị Title thay cho tên giảng viên nếu API không có trường này
        public string TenGiangVien => string.IsNullOrEmpty(Title) ? "Giảng viên" : Title;

        [JsonPropertyName("preAttachments")]
        public List<AttachmentWrapper> PreAttachments { get; set; } = new();

        private List<AttachmentWrapper> _myApplications = new();
        [JsonPropertyName("myApplications")]
        public List<AttachmentWrapper> MyApplications
        {
            get => _myApplications;
            set { _myApplications = value; OnPropertyChanged(); UpdateFileCounts(); }
        }

        // Logic tính toán số lượng file
        public int SoFileDatCoc => PreAttachments?.Count ?? 0;
        public int SoFileMyAttachment => MyApplications?.Count ?? 0;
        public int TongSoTaiLieu => SoFileDatCoc + SoFileMyAttachment;

        public bool HasMyAttachments => SoFileMyAttachment > 0;
        public bool IsCanViewFiles => TrangThai == TTTVTrangThai.ChoXuLy;
        public bool IsCanProcess => TrangThai == TTTVTrangThai.ChoXuLy;
        public bool IsAlreadyProcessed => !IsCanProcess;
        public string PickerTitle => IsCanProcess ? "Xử lý" : TrangThaiText;

        private int _status;
        [JsonPropertyName("status")]
        public int Status
        {
            get => _status;
            set
            {
                _status = value;
                TrangThai = value switch
                {
                    1 => TTTVTrangThai.ChoXuLy,
                    2 => TTTVTrangThai.ConfirmedOnRequestForm,
                    3 => TTTVTrangThai.ReturnedOnRequestForm,
                    0 => TTTVTrangThai.Draft,
                    _ => TTTVTrangThai.ChoXuLy
                };
            }
        }

        private TTTVTrangThai _trangThai;
        [JsonIgnore]
        public TTTVTrangThai TrangThai
        {
            get => _trangThai;
            set { _trangThai = value; OnPropertyChanged(); NotifyUI(); }
        }

        // Cập nhật giao diện khi dữ liệu thay đổi
        public void NotifyUI()
        {
            OnPropertyChanged(nameof(TrangThaiText));
            OnPropertyChanged(nameof(TrangThaiColor));
            OnPropertyChanged(nameof(TrangThaiBgColor));
            OnPropertyChanged(nameof(IsCanProcess));
            OnPropertyChanged(nameof(PickerTitle));
            OnPropertyChanged(nameof(IsCanViewFiles));
            OnPropertyChanged(nameof(IsAlreadyProcessed));
            UpdateFileCounts();
        }

        public void UpdateFileCounts()
        {
            OnPropertyChanged(nameof(SoFileDatCoc));
            OnPropertyChanged(nameof(SoFileMyAttachment));
            OnPropertyChanged(nameof(TongSoTaiLieu));
            OnPropertyChanged(nameof(DanhSachTepDinhKem));
            OnPropertyChanged(nameof(DanhSachThemTep));
        }

        public string TrangThaiText => TrangThai switch
        {
            TTTVTrangThai.ChoXuLy => "Chờ xử lý",
            TTTVTrangThai.ConfirmedOnRequestForm => "Đã xác nhận",
            TTTVTrangThai.ReturnedOnRequestForm => "Đã trả về",
            TTTVTrangThai.Draft => "Bản nháp",
            _ => "Chưa xác định"
        };

        public Color TrangThaiColor => TrangThai switch
        {
            TTTVTrangThai.ChoXuLy => Color.FromArgb("#D97706"),
            TTTVTrangThai.ConfirmedOnRequestForm => Color.FromArgb("#2563EB"),
            TTTVTrangThai.ReturnedOnRequestForm => Color.FromArgb("#DC2626"),
            _ => Color.FromArgb("#64748B")
        };

        public Color TrangThaiBgColor => TrangThai switch
        {
            TTTVTrangThai.ChoXuLy => Color.FromArgb("#FEF3C7"),
            TTTVTrangThai.ConfirmedOnRequestForm => Color.FromArgb("#DBEAFE"),
            TTTVTrangThai.ReturnedOnRequestForm => Color.FromArgb("#FEE2E2"),
            _ => Color.FromArgb("#F1F5F9")
        };

        public List<string> DanhSachTepDinhKem => PreAttachments?.Select(x => x.FileDetail.Name).ToList() ?? new();
        public List<string> DanhSachThemTep => MyApplications?.Select(x => x.FileDetail.Name).ToList() ?? new();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class AttachmentWrapper
    {
        [JsonPropertyName("file")]
        public FileDetail FileDetail { get; set; } = new();
    }

    public class FileDetail
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("path")] public string Path { get; set; } = string.Empty;
    }

    public enum TTTVTrangThai
    {
        Draft = 0,
        ChoXuLy = 1,
        ConfirmedOnRequestForm = 2,
        ReturnedOnRequestForm = 3,
        ForwardedToKHCN = 4,
        PreliminaryReview = 5,
        Returned = 6,
        ChoDuyet = 7
    }
}