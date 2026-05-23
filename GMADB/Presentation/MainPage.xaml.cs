using Windows.Storage.Pickers;

namespace GMADB.Presentation;

public sealed partial class MainPage : Page
{
    private readonly AdbService _adbService;
    private string? _selectedFilePath;

    public MainPage()
    {
        InitializeComponent();
        _adbService = new AdbService();
        _adbService.OutputReceived += OnOutputReceived;
        _adbService.ErrorReceived += OnErrorReceived;

        // Run ADB devices on startup
        _ = InitializeAsync();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ((App)Application.Current).MainWindow?.SetTitleBar(AppTitleBar);
    }

    private async Task InitializeAsync()
    {
        await RunAdbCommandAsync("devices", "ADB devices");
    }

    private void OnOutputReceived(object? sender, string line)
    {
        // Marshal to UI thread
        DispatcherQueue.TryEnqueue(() =>
        {
            OutputTextBox.Text += line + Environment.NewLine;
        });
    }

    private void OnErrorReceived(object? sender, string line)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            OutputTextBox.Text += "[ERROR] " + line + Environment.NewLine;
        });
    }

    private async void AdbDevicesButton_Click(object sender, RoutedEventArgs e)
    {
        await RunAdbCommandAsync("devices", "ADB devices");
    }

    private async void RunCustomCommandButton_Click(object sender, RoutedEventArgs e)
    {
        var command = CustomCommandTextBox.Text?.Trim();
        if (string.IsNullOrEmpty(command))
            return;

        AppendOutput($"\n=================Running: adb {command}===================");
        await _adbService.RunCustomCommandAsync(command);
        CustomCommandTextBox.Text = string.Empty;
    }

    private async void PickZipFileButton_Click(object sender, RoutedEventArgs e)
    {
        var senderButton = sender as Button;
        if (senderButton == null) return;

        senderButton.IsEnabled = false;

        try
        {
            var openPicker = new FileOpenPicker();

            var app = (App)Application.Current;
            var window = app.MainWindow;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".zip");
            openPicker.FileTypeFilter.Add(".img");
            openPicker.FileTypeFilter.Add("*");

            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                _selectedFilePath = file.Path;
                FilePathTextBox.Text = file.Name;
                FlashButton.IsEnabled = true;
            }
            else
            {
                FilePathTextBox.Text = "";
                _selectedFilePath = null;
            }
        }
        finally
        {
            senderButton.IsEnabled = true;
        }
    }

    private async void FlashButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedFilePath))
        {
            AppendOutput("\n[ERROR] No file selected. Please select a ROM file first.");
            return;
        }

        FlashButton.IsEnabled = false;
        SelectRomButton.IsEnabled = false;

        try
        {
            AppendOutput($"\n=================Starting Flash: {_selectedFilePath}===================");
            await _adbService.SideloadAsync(_selectedFilePath);
            AppendOutput("=================Flash Complete===================");
        }
        finally
        {
            FlashButton.IsEnabled = true;
            SelectRomButton.IsEnabled = true;
        }
    }

    private async Task RunAdbCommandAsync(string arguments, string label)
    {
        AppendOutput($"\n================={label}===================");
        await _adbService.RunCustomCommandAsync(arguments);
    }

    private void AppendOutput(string text)
    {
        OutputTextBox.Text += text + Environment.NewLine;
    }
}
