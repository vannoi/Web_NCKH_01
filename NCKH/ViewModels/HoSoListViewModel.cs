using System.Collections.ObjectModel;
using NCKH.Models;

namespace NCKH.ViewModels;

public class HoSoListViewModel
{
    public ObservableCollection<HoSo> DanhSachHoSo { get; set; } = new();

    public HoSoListViewModel()
    {
        LoadDuLieuMau();
    }

    void LoadDuLieuMau()
    {
        DanhSachHoSo.Add(new HoSo
        {
            Id = 1,
            TieuDe = "Nghiên cứu ứng dụng AI trong giảng dạy",
            MoTa = "Phân tích khả năng ứng dụng AI trong trường học nhằm tối ưu hóa giáo trình.",
            TenGiangVien = "Nguyễn Văn A",
            SoLuongTepChung = 3,
            SoLuongTepRieng = 2,
            TrangThai = "Approved",
            BuocHienTai = "Phòng KHCN-HTQT",
            NgayNop = new DateTime(2026, 1, 12)
        });

        DanhSachHoSo.Add(new HoSo
        {
            Id = 2,
            TieuDe = "Phân tích dữ liệu lớn trong giáo dục",
            MoTa = "Sử dụng Hadoop và Spark để phân tích hành vi học tập của sinh viên.",
            TenGiangVien = "Trần Thị B",
            SoLuongTepChung = 5,
            SoLuongTepRieng = 1,
            TrangThai = "Reviewing",
            BuocHienTai = "Thư viện",
            NgayNop = new DateTime(2026, 2, 5)
        });

        DanhSachHoSo.Add(new HoSo
        {
            Id = 3,
            TieuDe = "Xây dựng hệ thống quản lý NCKH",
            MoTa = "Phần mềm quản lý quy trình nộp và duyệt hồ sơ nghiên cứu trực tuyến.",
            TenGiangVien = "Lê Hoàng C",
            SoLuongTepChung = 2,
            SoLuongTepRieng = 0,
            TrangThai = "Submitted",
            BuocHienTai = "Khoa/Viện",
            NgayNop = new DateTime(2026, 3, 10)
        });

        DanhSachHoSo.Add(new HoSo
        {
            Id = 4,
            TieuDe = "Ứng dụng IoT trong nông nghiệp",
            MoTa = "Hệ thống cảm biến đo độ ẩm đất và tự động tưới tiêu qua di động.",
            TenGiangVien = "Phạm Minh D",
            SoLuongTepChung = 1,
            SoLuongTepRieng = 0,
            TrangThai = "Draft",
            BuocHienTai = "Chưa nộp",
            NgayNop = new DateTime(2026, 3, 14)
        });
    }
}