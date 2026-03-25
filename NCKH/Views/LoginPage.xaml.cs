namespace NCKH.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage() => InitializeComponent();

    private async void OnDangNhapClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Lỗi", "Vui lòng nhập đầy đủ thông tin", "OK");
            return;
        }

        await Navigation.PushAsync(new DvqlttListPage());
    }
}