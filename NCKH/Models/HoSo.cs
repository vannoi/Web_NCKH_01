namespace NCKH.Models;

public class HoSo
{
    public int Id { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string MoTa { get; set; } = string.Empty;
    public int SoLuongTepChung { get; set; }
    public string TenGiangVien { get; set; } = string.Empty;
    public int SoLuongTepRieng { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string BuocHienTai { get; set; } = string.Empty;
    public DateTime NgayNop { get; set; }
}