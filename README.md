# GMADB

**GUI Minimal ADB** — a native Windows app for interacting with Android Debug Bridge, built with Uno Platform & WinUI 3.

GMADB simplifies flashing custom ROMs and running ADB commands through a clean, modern interface with real-time output streaming.

## Features

- **ADB Devices** — List all connected devices with a single click. Runs automatically on start-up.
- **Custom Commands** — Execute any ADB command directly from the app without opening a terminal.
- **ROM Flashing** — Select a `.zip` or `.img` file and flash it via `adb sideload` with one button.
- **Real-time Output** — All ADB output streams live to the console panel as it happens.
- **Mica Backdrop** — Native Windows 11 Mica material with an extended title bar.

## How to Use

1. **List Devices** — Click *ADB devices* (runs automatically on launch) to see connected devices.
2. **Run a Custom Command** — Type any ADB command (e.g. `shell pm list packages`) and click *Run*.
3. **Flash a ROM** —
   - Click *Select ROM* and pick your `.zip` or `.img` file.
   - Click *Flash* to start `adb sideload`. **Do not close the window** until the flash completes.

## Prerequisites

- **Windows 10 1809+** or **Windows 11**
- **.NET 9 SDK**
- **ADB** installed and available on your system `PATH`
- **USB debugging** enabled on your Android device

## Build & Run

```shell
git clone https://github.com/yadev64/GMADB.git
cd GMADB
dotnet run --project GMADB/GMADB.csproj --framework net9.0-windows10.0.26100
```

## Tech Stack

| Component | Technology |
|-----------|------------|
| UI Framework | Uno Platform / WinUI 3 |
| Runtime | .NET 9 |
| Language | C# / XAML |
| ADB Integration | `cmd.exe` process with real-time streaming |
| Window Chrome | Mica backdrop + extended title bar |

## Contributing

Contributions are welcome! See [Contribute on GitHub](https://github.com/yadev64/GMADB).

## License

This project is open source. See the [repository](https://github.com/yadev64/GMADB) for details.
