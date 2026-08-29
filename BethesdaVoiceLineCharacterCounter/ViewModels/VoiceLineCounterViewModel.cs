using BethesdaVoiceLineCharacterCounter.Application.Dtos.Input;
using BethesdaVoiceLineCharacterCounter.Application.Dtos.Output;
using BethesdaVoiceLineCharacterCounter.Application.Features;
using BethesdaVoiceLineCharacterCounter.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BethesdaVoiceLineCharacterCounter.ViewModels;

/// <summary>
/// Manages game selection, voice-line input, command eligibility, and calculated statistics.
/// </summary>
public partial class VoiceLineCounterViewModel : ObservableObject
{
    #region [ Variables ]

    /// <summary>
    /// Stores the games available for selection.
    /// </summary>
    [ObservableProperty]
    private List<BethesdaGame> bethesdaGames;

    /// <summary>
    /// Stores the currently selected game, or <see langword="null"/> when no game is selected.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    private BethesdaGame? selectedBethesdaGame;

    /// <summary>
    /// Stores the voice-line text entered by the user.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    private string inputText = string.Empty;

    /// <summary>
    /// Stores the most recently calculated line statistics.
    /// </summary>
    [ObservableProperty]
    private LineStatisticsDto lineStatistics = new()
    {
        TotalCharacters = 0,
        TotalDialogueSections = 1
    };

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Initializes a new instance of the <see cref="VoiceLineCounterViewModel"/> class and loads the supported games.
    /// </summary>
    public VoiceLineCounterViewModel()
    {
        BethesdaGames = new GetBethesdaGames().GenerateBethesdaGames();
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Calculates character and dialogue-section totals for the current input and selected game.
    /// </summary>
    /// <returns>No value is returned.</returns>
    [RelayCommand(CanExecute = nameof(CanRun))]
    private void Run()
    {
        GetLineStatisticsDto input = new()
        {
            BethesdaGame = SelectedBethesdaGame!.GameType,
            InputText = InputText
        };

        LineStatistics = new GetLineStatistics().GetLineStatisticsFromInput(input);
    }

    /// <summary>
    /// Determines whether the statistics command has the input required to execute.
    /// </summary>
    /// <returns><see langword="true"/> when text and a selected game are available; otherwise, <see langword="false"/>.</returns>
    private bool CanRun() =>
        !string.IsNullOrEmpty(InputText) && SelectedBethesdaGame is not null;

    #endregion
}
