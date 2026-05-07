using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NCKH.Models;
using NCKH.Services;

namespace NCKH.ViewModels;

public class HoSoListViewModel : INotifyPropertyChanged
{
    // ← Singleton để TaoHoSo có thể gọi LoadDuLieuAsync()
    public static HoSoListViewModel Instance { get; } = new HoSoListViewModel();

    private readonly ApiService _apiService = new();

    public static ObservableCollection<HoSo> DanhSachHoSo { get; set; } = new();

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    public ICommand LoadCommand { get; }

    public HoSoListViewModel()
    {
        LoadCommand = new Command(async () => await LoadDuLieuAsync());
        _ = LoadDuLieuAsync();
    }

    public async Task LoadDuLieuAsync()
    {
        try
        {
            IsLoading = true;
            var list = await _apiService.GetHoSoListAsync();
            DanhSachHoSo.Clear();
            foreach (var item in list)
            {
                if (item.NgayNop == default)
                    item.NgayNop = DateTime.Now;
                DanhSachHoSo.Add(item);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadDuLieuAsync Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}