namespace NCKH.Views
{
    public partial class TTTVDanhSachTepPopup : ContentPage
    {
        public TTTVDanhSachTepPopup(string tieuDe, List<string> danhSachFile)
        {
            InitializeComponent();
            LabelTieuDe.Text = tieuDe;
            ListFile.ItemsSource = danhSachFile;
        }

        private async void OnDongClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnOverlayTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
