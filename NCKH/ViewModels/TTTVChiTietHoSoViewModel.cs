using NCKH.Models;
using System.Windows.Input;

namespace NCKH.ViewModels
{
    public class TTTVChiTietHoSoViewModel : BaseViewModel
    {
        private TTTVHoSoItem _hoSoDetail;
        public TTTVHoSoItem HoSoDetail
        {
            get => _hoSoDetail;
            set { _hoSoDetail = value; OnPropertyChanged(); }
        }

        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }

        public TTTVChiTietHoSoViewModel()
        {
            ApproveCommand = new Command(OnApprove);
            RejectCommand = new Command(OnReject);
        }

        private async void OnApprove()
        {
            bool confirm = await Shell.Current.DisplayAlert("Xác nhận", "Phê duyệt hồ sơ này?", "Đồng ý", "Hủy");
            if (confirm) { /* Logic cập nhật trạng thái */ }
        }

        private void OnReject() { /* Logic mở popup trả hồ sơ */ }
    }
}