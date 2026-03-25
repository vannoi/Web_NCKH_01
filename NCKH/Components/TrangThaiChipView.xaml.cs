namespace NCKH.Components;

public partial class TrangThaiChipView : ContentView
{
    public static readonly BindableProperty TrangThaiProperty =
        BindableProperty.Create(nameof(TrangThai), typeof(string), typeof(TrangThaiChipView), "",
            propertyChanged: OnTrangThaiChanged);

    public string TrangThai
    {
        get => (string)GetValue(TrangThaiProperty);
        set => SetValue(TrangThaiProperty, value);
    }

    public TrangThaiChipView() => InitializeComponent();

    static void OnTrangThaiChanged(BindableObject bindable, object oldVal, object newVal)
    {
        var view = (TrangThaiChipView)bindable;
        view.UpdateChip();
    }

    void UpdateChip()
    {
        (ChipBorder.BackgroundColor, ChipLabel.TextColor, ChipLabel.Text) = TrangThai switch
        {
            "Draft" => (Color.FromArgb("#F1EFE8"), Color.FromArgb("#5F5E5A"), "Nháp"),
            "Submitted" => (Color.FromArgb("#E6F1FB"), Color.FromArgb("#185FA5"), "Đã nộp"),
            "Reviewing" => (Color.FromArgb("#FAEEDA"), Color.FromArgb("#854F0B"), "Đang xét"),
            "Approved" => (Color.FromArgb("#EAF3DE"), Color.FromArgb("#3B6D11"), "Đã duyệt"),
            "Rejected" => (Color.FromArgb("#FCEBEB"), Color.FromArgb("#A32D2D"), "Từ chối"),
            _ => (Color.FromArgb("#F1EFE8"), Color.FromArgb("#5F5E5A"), TrangThai)
        };
    }
}