namespace NCKH.Components;

public partial class RoleChipView : ContentView
{
    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(RoleChipView), "",
            propertyChanged: OnPropsChanged);

    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(RoleChipView), false,
            propertyChanged: OnPropsChanged);

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public RoleChipView() => InitializeComponent();

    static void OnPropsChanged(BindableObject bindable, object oldVal, object newVal)
    {
        var view = (RoleChipView)bindable;
        view.ChipLabel.Text = view.LabelText;

        if (view.IsSelected)
        {
            view.ChipBorder.BackgroundColor = Color.FromArgb("#E6F1FB");
            view.ChipBorder.Stroke = Color.FromArgb("#185FA5");
            view.ChipLabel.TextColor = Color.FromArgb("#185FA5");
            view.ChipLabel.FontAttributes = FontAttributes.Bold;
        }
        else
        {
            view.ChipBorder.BackgroundColor = Colors.Transparent;
            view.ChipBorder.Stroke = Color.FromArgb("#CCCCCC");
            view.ChipLabel.TextColor = Color.FromArgb("#888888");
            view.ChipLabel.FontAttributes = FontAttributes.None;
        }
    }

    // Trong file RoleChipView.xaml.cs
    public event EventHandler Tapped;

    private void OnStepTapped(object sender, TappedEventArgs e)
    {
        // Bắn sự kiện ra ngoài khi người dùng click
        Tapped?.Invoke(this, EventArgs.Empty);
    }

}