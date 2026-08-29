using Avalonia;

namespace BethesdaVoiceLineCharacterCounter;

/// <summary>
/// Provides the application entry point and Avalonia configuration.
/// </summary>
internal static class Program
{
    #region [ Methods ]

    /// <summary>
    /// Starts the application with the classic desktop lifetime.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    /// <returns>No value is returned.</returns>
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    /// <summary>
    /// Creates and configures the Avalonia application builder.
    /// </summary>
    /// <returns>A configured Avalonia application builder.</returns>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

    #endregion
}