using System.Diagnostics;

namespace GMADB.Services;

/// <summary>
/// Service for interacting with ADB (Android Debug Bridge).
/// Runs ADB commands via cmd.exe and captures output in real-time.
/// </summary>
public sealed class AdbService
{
    /// <summary>
    /// Raised when a line of output is received from an ADB command.
    /// </summary>
    public event EventHandler<string>? OutputReceived;

    /// <summary>
    /// Raised when an error line is received from an ADB command.
    /// </summary>
    public event EventHandler<string>? ErrorReceived;

    /// <summary>
    /// Runs "adb devices" and returns the output.
    /// </summary>
    public async Task<string> GetDevicesAsync(CancellationToken cancellationToken = default)
    {
        return await RunAdbCommandAsync("devices", cancellationToken);
    }

    /// <summary>
    /// Runs "adb sideload" with the specified file path.
    /// </summary>
    public async Task<string> SideloadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return await RunAdbCommandAsync($"sideload \"{filePath}\"", cancellationToken);
    }

    /// <summary>
    /// Runs a custom ADB command and returns the output.
    /// </summary>
    public async Task<string> RunCustomCommandAsync(string command, CancellationToken cancellationToken = default)
    {
        return await RunAdbCommandAsync(command, cancellationToken);
    }

    /// <summary>
    /// Runs an ADB command via cmd.exe, streaming output in real-time.
    /// </summary>
    private async Task<string> RunAdbCommandAsync(string arguments, CancellationToken cancellationToken = default)
    {
        var process = new Process();
        var startInfo = new ProcessStartInfo
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = "cmd.exe",
            Arguments = $"/c adb {arguments}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        process.StartInfo = startInfo;
        process.Start();

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        // Read output and error streams asynchronously for real-time streaming
        var outputTask = Task.Run(async () =>
        {
            while (!process.StandardOutput.EndOfStream)
            {
                var line = await process.StandardOutput.ReadLineAsync(cancellationToken);
                if (line is not null)
                {
                    outputBuilder.AppendLine(line);
                    OutputReceived?.Invoke(this, line);
                }
            }
        }, cancellationToken);

        var errorTask = Task.Run(async () =>
        {
            while (!process.StandardError.EndOfStream)
            {
                var line = await process.StandardError.ReadLineAsync(cancellationToken);
                if (line is not null)
                {
                    errorBuilder.AppendLine(line);
                    ErrorReceived?.Invoke(this, line);
                }
            }
        }, cancellationToken);

        await Task.WhenAll(outputTask, errorTask);
        await process.WaitForExitAsync(cancellationToken);

        process.Close();

        var result = outputBuilder.ToString();
        var error = errorBuilder.ToString();

        if (!string.IsNullOrEmpty(error))
        {
            result += error;
        }

        return result;
    }
}
