using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BethesdaVoiceLineCharacterCounter.ViewModels;
using BethesdaVoiceLineCharacterCounter.Views;

namespace BethesdaVoiceLineCharacterCounter;

/// <summary>
/// Represents the Avalonia application and configures its desktop window.
/// </summary>
public partial class App : Avalonia.Application
{
    #region [ Methods ]

    /// <summary>
    /// Loads the application resources declared in Avalonia XAML.
    /// </summary>
    /// <returns>No value is returned.</returns>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Creates the main window after Avalonia completes framework initialization.
    /// </summary>
    /// <returns>No value is returned.</returns>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new VoiceLineCounterViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    #endregion
}