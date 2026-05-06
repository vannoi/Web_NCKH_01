namespace NCKH.Models
{
    public class NghienCuuKhoaHocModel
    {
        public string TenHoSo { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Chờ duyệt";
        public double GioDatDuoc { get; set; }
        public string LoaiHinh { get; set; } = string.Empty;
    }
}