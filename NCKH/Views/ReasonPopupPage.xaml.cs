using NCKH.Models;
using System.Collections.ObjectModel;

namespace NCKH.Views;

public partial class ReasonPopupPage : ContentPage
{
    // Cập nhật sự kiện để trả về cả lý do và danh sách tệp đính kèm
    public event Action<string, List<FileAttachment>> OnReasonSubmitted;

    public ObservableCollection<FileAttachment> SelectedFiles { get; set; } = new();

    public ReasonPopupPage(string tenHoSo)
    {
        InitializeComponent();
        TenHoSoLabel.Text = $"Hồ sơ: {tenHoSo}";
        BindingContext = this;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    // Xử lý chọn File từ thiết bị
    private async void OnAddFileClicked(object sender, EventArgs e)
    {
        try
        {
            var results = await FilePicker.Default.PickMultipleAsync(new PickOptions
            {
                PickerTitle = "Chọn tệp đính kèm minh chứng"
            });

            if (results != null)
            {
                foreach (var result in results)
                {
                    // Chuyển đổi từ FileResult sang FileAttachment
                    SelectedFiles.Add(new FileAttachment
                    {
                        id = Guid.NewGuid(),
                        fileName = result.FileName,
                        fileUrl = result.FullPath, // Lưu tạm path để upload sau
                        isInternal = true
                    });
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", "Không thể chọn tệp: " + ex.Message, "OK");
        }
    }

    // Xóa file khỏi danh sách chọn
    private void OnRemoveFileClicked(object sender, EventArgs e)
    {
        var file = (sender as Button)?.CommandParameter as FileAttachment;
        if (file != null)
        {
            SelectedFiles.Remove(file);
        }
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        string reason = ReasonEditor.Text;
        if (string.IsNullOrWhiteSpace(reason))
        {
            await DisplayAlert("Chú ý", "Bạn phải nhập lý do trả hồ sơ.", "OK");
            return;
        }

        // Gửi dữ liệu về trang danh sách thông qua Action
        OnReasonSubmitted?.Invoke(reason, SelectedFiles.ToList());

        await Navigation.PopModalAsync(); // Đóng Popup
    }
}