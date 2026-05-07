using NCKH.ViewModels;
using NCKH.Models;
using NCKH.Constants;

namespace NCKH.Views;

public partial class DvqlttListPage : ContentPage
{
    public DvqlttListPage()
    {
        InitializeComponent();
        BindingContext = new DvqlttViewModel();
    }

    private async void OnViewPreAttachmentsClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var app = button?.BindingContext as ApplicationModel;

        if (app != null)
        {
            // Truyền ID, Tên hồ sơ, Danh sách preAttachments, isPrivate = false
            var page = new AttachmentsPage(app.id, app.title, app.preAttachments, false);
            await Navigation.PushModalAsync(page);
        }
    }

    private async void OnViewMyApplicationsClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var app = button?.BindingContext as ApplicationModel;

        if (app != null)
        {
            // Truyền ID, Tên hồ sơ, Danh sách myApplications, isPrivate = true
            var page = new AttachmentsPage(app.id, app.title, app.myApplications, true);

            // Đăng ký sự kiện khi đóng Modal để load lại dữ liệu nếu có thay đổi file
            page.Disappearing += async (s, args) =>
            {
                if (BindingContext is DvqlttViewModel vm)
                {
                    await vm.RefreshDataAsync(); // Tải lại danh sách để cập nhật số lượng file
                }
            };

            await Navigation.PushModalAsync(page);
        }
    }

    private async void OnActionChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;
        var applicationModel = picker?.BindingContext as ApplicationModel;
        if (applicationModel == null || picker.SelectedIndex == -1) return;

        string selectedAction = picker.SelectedItem.ToString();

        if (selectedAction.Contains("Trả hồ sơ"))
        {
            var reasonPage = new ReasonPopupPage(applicationModel.title);

            // Đăng ký sự kiện khi người dùng nhấn "Xác nhận" trên Popup
            reasonPage.OnReasonSubmitted += async (lyDo, fileDinhKem) =>
            {
                if (BindingContext is DvqlttViewModel vm)
                {
                    // 1. Nếu có file minh chứng mới, upload trước
                    // 2. Gọi Update với Status = 3 Trả về
                    await vm.UpdateAppStatus(applicationModel, 3, lyDo);
                }
            };
            await Navigation.PushModalAsync(reasonPage);
        }
        else if (selectedAction.Contains("Chuyển đến"))
        {
            bool confirm = await DisplayAlert("Xác nhận", $"Chuyển hồ sơ đến {applicationModel.NextStepUnit}?", "Đồng ý", "Hủy");
            if (confirm && BindingContext is DvqlttViewModel vm)
            {
                // Gửi Status = 2 Chuyển tiếp
                await vm.UpdateAppStatus(applicationModel, 2, $"Đơn vị đã duyệt và chuyển đi.");
            }
        }

        // Reset picker về trạng thái chưa chọn để lần sau bấm lại vẫn ăn
        picker.SelectedIndex = -1;
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is DvqlttViewModel viewModel)
            viewModel.FilterHoSo(e.NewTextValue);
    }

    private void OnSearchClicked(object sender, EventArgs e)
    {
        if (BindingContext is DvqlttViewModel viewModel)
            viewModel.FilterHoSo(SearchEntry.Text);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DvqlttViewModel viewModel)
        {
            await viewModel.RefreshDataAsync(); // Tự động cập nhật dữ liệu mới nhất
        }
    }
}