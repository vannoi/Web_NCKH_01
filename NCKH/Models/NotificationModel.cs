using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics; // Đảm bảo gọi thư viện chứa đối tượng Color

namespace NCKH.Models
{
    public class NotificationModel : INotifyPropertyChanged
    {
        private bool _isUnread;
        public string Title { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        public Color IconBgColor { get; set; }

        public bool IsUnread
        {
            get => _isUnread;
            set
            {
                _isUnread = value;
                OnPropertyChanged();
                // Bỏ OnPropertyChanged cho màu/mờ vì giá trị đã cố định.
                // Chỉ cần báo cho UI cập nhật lại việc ẩn/hiện dấu chấm xanh.
                OnPropertyChanged(nameof(ShowDotIfRead));
            }
        }

        // Logic hiển thị dấu chấm xanh (chỉ hiện khi chưa đọc)
        public bool ShowDotIfRead => IsUnread;

        // --- ĐÃ BỎ THUỘC TÍNH MỜ ---
        // Các thuộc tính dưới đây luôn trả về giá trị đậm/rõ nét cho cả bài đã đọc và chưa đọc
        public double RowOpacity => 1.0;
        public Color TitleColor => Color.FromArgb("#1E293B");
        public Color DescColor => Color.FromArgb("#64748B");

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}