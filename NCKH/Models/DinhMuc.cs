using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCKH.Models
{
    public class DinhMuc
    {
        public int STT { get; set; }
        public string MaDinhMuc { get; set; }
        public string TenDinhMuc { get; set; }
        public string LoaiGio { get; set; }
        public double GioChuan { get; set; }
        public string TrangThai { get; set; }

        // Logic để lấy màu sắc tương ứng với trạng thái
        public Color TrangThaiColor => TrangThai == "Đang áp dụng" ? Colors.Green : Colors.Gray;
    }
}