using Microsoft.Maui.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NCKH.Models
{
    public class HoSo : INotifyPropertyChanged
    {
        // --- FIELD CÓ TRONG API ---
        [JsonIgnore]
        private int _stt;
        public int STT
        {
            get => _stt;
            set { _stt = value; OnPropertyChanged(); }
        }

        public string Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("title")]
        public string TieuDe { get; set; }

        [JsonProperty("description")]
        public string MoTa { get; set; }  

        [JsonIgnore]
        public Stream FileStream { get; set; } 

        [JsonIgnore]
        public string FileName { get; set; }

        private int _step;
        [JsonProperty("status")]
        public int Step
        {
            get => _step;
            set
            {
                _step = value;
                OnPropertyChanged();

                // Cập nhật các thuộc tính danh sách & Badge
                OnPropertyChanged(nameof(ProgressValue));
                OnPropertyChanged(nameof(ProgressBarColor));
                OnPropertyChanged(nameof(BuocHienTai));
                OnPropertyChanged(nameof(TrangThai));
                OnPropertyChanged(nameof(TrangThaiVaPhanTram));
                OnPropertyChanged(nameof(TrangThaiColor));
                OnPropertyChanged(nameof(TrangThaiBg));
                OnPropertyChanged(nameof(BadgeBg));
                OnPropertyChanged(nameof(BadgeText));
                OnPropertyChanged(nameof(BadgeTextColor));

                // Cập nhật logic màn Kiểm tra tiến trình
                OnPropertyChanged(nameof(Step0Done));
                OnPropertyChanged(nameof(Step1Done));
                OnPropertyChanged(nameof(Step2Done));
                OnPropertyChanged(nameof(Step3Done));

                OnPropertyChanged(nameof(Step0NotDone));
                OnPropertyChanged(nameof(Step1NotDone));
                OnPropertyChanged(nameof(Step2NotDone));
                OnPropertyChanged(nameof(Step3NotDone));

                OnPropertyChanged(nameof(Step0Current));
                OnPropertyChanged(nameof(Step1Current));
                OnPropertyChanged(nameof(Step2Current));
                OnPropertyChanged(nameof(Step3Current));

                OnPropertyChanged(nameof(Step0Text));
                OnPropertyChanged(nameof(Step1Text));
                OnPropertyChanged(nameof(Step2Text));
                OnPropertyChanged(nameof(Step3Text));

                OnPropertyChanged(nameof(Step0Bg));
                OnPropertyChanged(nameof(Step1Bg));
                OnPropertyChanged(nameof(Step2Bg));
                OnPropertyChanged(nameof(Step3Bg));

                OnPropertyChanged(nameof(Line1Color));
                OnPropertyChanged(nameof(Line2Color));
                OnPropertyChanged(nameof(Line3Color));
            }
        }
        [JsonProperty("myApplications")]
        public List<ApplicationFileItem> MyApplications { get; set; } = new();

        [JsonProperty("preAttachments")]
        public List<ApplicationFileItem> PreAttachments { get; set; } = new();

        [JsonProperty("stepDetailId")]
        public string StepDetailId { get; set; }

        [JsonProperty("applicationFiles")]
        public List<FileAttachment> ApplicationFiles { get; set; } = new();

        // --- FIELD KHÔNG CÓ TRONG API ---
        [JsonIgnore]
        public DateTime NgayNop { get; set; }

        [JsonIgnore]
        public string TenGiangVien { get; set; }

        private bool _isExpanded;
        [JsonIgnore]
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        // =================================================================
        // CÁC THUỘC TÍNH HIỂN THỊ TRẠNG THÁI (ĐÃ GỘP ĐỦ TẤT CẢ)
        // =================================================================

        [JsonIgnore]
        public string TrangThai => Step switch
        {
            0 => "Bản nháp",
            1 => "Đang xét duyệt",
            2 => "Chờ duyệt (PĐT)",
            3 => "Đã hoàn tất",
            _ => "Chưa xác định"
        };

        [JsonIgnore]
        public string TrangThaiVaPhanTram => Step switch
        {
            0 => "Bản nháp (25%)",
            1 => "Đang xét duyệt (50%)",
            2 => "Chờ duyệt (PĐT) (75%)",
            3 => "Đã hoàn tất (100%)",
            _ => "Chưa xác định"
        };

        [JsonIgnore]
        public string BuocHienTai => Step switch
        {
            0 => "Nháp",
            1 => "Xét duyệt",
            2 => "Phòng Đào tạo",
            3 => "Hoàn tất",
            _ => "Chưa xác định"
        };

        // --- MÀU BADGE ---
        [JsonIgnore]
        public Color BadgeBg => Step switch
        {
            0 => Color.FromArgb("#F1F5F9"),
            1 => Color.FromArgb("#FEF3C7"),
            2 => Color.FromArgb("#F5F3FF"),
            3 => Color.FromArgb("#DCFCE7"),
            _ => Colors.Transparent
        };

        [JsonIgnore]
        public Color BadgeText => Step switch
        {
            0 => Color.FromArgb("#64748B"),
            1 => Color.FromArgb("#D97706"),
            2 => Color.FromArgb("#7C3AED"),
            3 => Color.FromArgb("#16A34A"),
            _ => Colors.Black
        };

        [JsonIgnore] public Color BadgeTextColor => BadgeText;
        [JsonIgnore] public Color TrangThaiColor => BadgeText;
        [JsonIgnore] public Color TrangThaiBg => BadgeBg;

        // --- PROGRESS BAR ---
        [JsonIgnore]
        public double ProgressValue => (Step + 1) / 4.0;

        [JsonIgnore]
        public Color ProgressBarColor => Step switch
        {
            0 => Color.FromArgb("#94A3B8"),
            1 => Color.FromArgb("#D97706"),
            2 => Color.FromArgb("#7C3AED"),
            3 => Color.FromArgb("#16A34A"),
            _ => Colors.Black
        };

        // --- LOGIC CHO MÀN HÌNH KIỂM TRA QUÁ TRÌNH ---

        // 1. Kiểm tra đã qua bước đó chưa
        [JsonIgnore] public bool Step0Done => Step >= 0;
        [JsonIgnore] public bool Step1Done => Step >= 1;
        [JsonIgnore] public bool Step2Done => Step >= 2;
        [JsonIgnore] public bool Step3Done => Step >= 3;

        // 2. Nghịch đảo của Done (Dùng để ẩn hiện dấu tròn rỗng)
        [JsonIgnore] public bool Step0NotDone => !Step0Done;
        [JsonIgnore] public bool Step1NotDone => !Step1Done;
        [JsonIgnore] public bool Step2NotDone => !Step2Done;
        [JsonIgnore] public bool Step3NotDone => !Step3Done;

        // 3. Kiểm tra có đang ĐÚNG ở bước đó không (Để highlight)
        [JsonIgnore] public bool Step0Current => Step == 0;
        [JsonIgnore] public bool Step1Current => Step == 1;
        [JsonIgnore] public bool Step2Current => Step == 2;
        [JsonIgnore] public bool Step3Current => Step == 3;

        // 4. Màu chữ của từng bước
        [JsonIgnore] public Color Step0Text => Step >= 0 ? Color.FromArgb("#4F8EF7") : Color.FromArgb("#94A3B8");
        [JsonIgnore] public Color Step1Text => Step >= 1 ? Color.FromArgb("#4F8EF7") : Color.FromArgb("#94A3B8");
        [JsonIgnore] public Color Step2Text => Step >= 2 ? Color.FromArgb("#4F8EF7") : Color.FromArgb("#94A3B8");
        [JsonIgnore] public Color Step3Text => Step >= 3 ? Color.FromArgb("#16A34A") : Color.FromArgb("#94A3B8");

        // 5. Màu nền nhạt (Highlight) của bước đang chọn
        [JsonIgnore] public Color Step0Bg => Step == 0 ? Color.FromArgb("#EFF6FF") : Colors.Transparent;
        [JsonIgnore] public Color Step1Bg => Step == 1 ? Color.FromArgb("#FFFBEB") : Colors.Transparent;
        [JsonIgnore] public Color Step2Bg => Step == 2 ? Color.FromArgb("#F5F3FF") : Colors.Transparent;
        [JsonIgnore] public Color Step3Bg => Step == 3 ? Color.FromArgb("#F0FDF4") : Colors.Transparent;

        // 6. Màu đường nối
        [JsonIgnore] public Color Line1Color => Step >= 1 ? Color.FromArgb("#4F8EF7") : Color.FromArgb("#E2E8F0");
        [JsonIgnore] public Color Line2Color => Step >= 2 ? Color.FromArgb("#4F8EF7") : Color.FromArgb("#E2E8F0");
        [JsonIgnore] public Color Line3Color => Step >= 3 ? Color.FromArgb("#16A34A") : Color.FromArgb("#E2E8F0");

        // --- INotifyPropertyChanged ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}