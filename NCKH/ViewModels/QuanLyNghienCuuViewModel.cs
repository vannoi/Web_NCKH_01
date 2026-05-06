using System.Collections.ObjectModel;
using System.Windows.Input;
using NCKH.Models;

namespace NCKH.ViewModels
{
    public class QuanLyNghienCuuViewModel : BaseViewModel
    {
        // Danh sách hồ sơ sẽ hiển thị lên CollectionView
        private ObservableCollection<NghienCuuKhoaHocModel> _danhSachHoSo;
        public ObservableCollection<NghienCuuKhoaHocModel> DanhSachHoSo
        {
            get => _danhSachHoSo;
            set => SetProperty(ref _danhSachHoSo, value);
        }

        // Biến kiểm tra quyền để ẩn/hiện nút Duyệt
        private bool _laGiangVien;
        public bool LaGiangVien
        {
            get => _laGiangVien;
            set => SetProperty(ref _laGiangVien, value);
        }

        // Lệnh xử lý khi nhấn nút Phê Duyệt
        public ICommand DuyetHoSoCommand { get; }

        public QuanLyNghienCuuViewModel()
        {
            // 1. Khởi tạo danh sách hồ sơ
            DanhSachHoSo = new ObservableCollection<NghienCuuKhoaHocModel>();

            // 2. Mặc định cho phép hiển thị nút (Bạn có thể lấy từ UserSession sau)
            LaGiangVien = true;

            // 3. Khởi tạo lệnh Duyệt
            DuyetHoSoCommand = new Command<NghienCuuKhoaHocModel>(OnDuyetHoSo);

            // 4. Đổ dữ liệu mẫu vào
            LoadDataMau();
        }

        private void LoadDataMau()
        {
            // Làm sạch danh sách trước khi thêm để tránh trùng lặp
            DanhSachHoSo.Clear();

            DanhSachHoSo.Add(new NghienCuuKhoaHocModel
            {
                TenHoSo = "Nghiên cứu ứng dụng AI trong chẩn đoán hình ảnh y tế",
                GioDatDuoc = 120,
                TrangThai = "Chờ duyệt"
            });

            DanhSachHoSo.Add(new NghienCuuKhoaHocModel
            {
                TenHoSo = "Biên soạn giáo trình Lập trình di động .NET MAUI nâng cao",
                GioDatDuoc = 80,
                TrangThai = "Đã duyệt"
            });

            DanhSachHoSo.Add(new NghienCuuKhoaHocModel
            {
                TenHoSo = "Bài báo: Giải pháp bảo mật cho hệ thống IoT công nghiệp",
                GioDatDuoc = 40,
                TrangThai = "Chỉnh sửa"
            });

            DanhSachHoSo.Add(new NghienCuuKhoaHocModel
            {
                TenHoSo = "Đề tài cấp Bộ: Xây dựng hệ thống Smart City tại Đồng Nai",
                GioDatDuoc = 300,
                TrangThai = "Chờ duyệt"
            });

            DanhSachHoSo.Add(new NghienCuuKhoaHocModel
            {
                TenHoSo = "Hướng dẫn sinh viên đạt giải NCKH cấp Trường 2023",
                GioDatDuoc = 20,
                TrangThai = "Đã duyệt"
            });

            DanhSachHoSo.Add(new NghienCuuKhoaHocModel
            {
                TenHoSo = "Xây dựng phần mềm quản lý giảng viên và giờ dạy tích hợp",
                GioDatDuoc = 150,
                TrangThai = "Chờ duyệt"
            });

            DanhSachHoSo.Add(new NghienCuuKhoaHocModel
            {
                TenHoSo = "Bài báo Q1: Deep Learning for Natural Language Processing",
                GioDatDuoc = 200,
                TrangThai = "Đã duyệt"
            });

            DanhSachHoSo.Add(new NghienCuuKhoaHocModel
            {
                TenHoSo = "Nghiên cứu vật liệu Nano trong xử lý nước thải công nghiệp",
                GioDatDuoc = 180,
                TrangThai = "Chỉnh sửa"
            });
        }

        private async void OnDuyetHoSo(NghienCuuKhoaHocModel hoSo)
        {
            if (hoSo == null) return;

            // Hiển thị thông báo xác nhận (Option)
            bool confirm = await Shell.Current.DisplayAlert("Xác nhận",
                $"Bạn có chắc muốn phê duyệt đề tài: {hoSo.TenHoSo}?", "Đồng ý", "Hủy");

            if (confirm)
            {
                // Cập nhật trạng thái
                hoSo.TrangThai = "Đã duyệt";

                // Sau này bạn sẽ gọi API ở đây:
                // await _apiService.ApproveHoSoAsync(hoSo.Id);

                await Shell.Current.DisplayAlert("Thành công", "Hồ sơ đã được phê duyệt!", "OK");

                // Refresh lại danh sách nếu cần thiết, hoặc UI sẽ tự cập nhật 
                // nếu class NghienCuuKhoaHocModel có NotifyPropertyChanged
            }
        }
    }
}