using BethesdaVoiceLineCharacterCounter.Domain.Enums;
using BethesdaVoiceLineCharacterCounter.Domain.Models;
using BethesdaVoiceLineCharacterCounter.ViewModels;

namespace BethesdaVoiceLineCharacterCounter.Test.ViewModels;

/// <summary>
/// Verifies the state, command eligibility, and calculations of <see cref="VoiceLineCounterViewModel"/>.
/// </summary>
[Trait("ViewModels", "VoiceLineCounterViewModel")]
public class VoiceLineCounterViewModelTest
{
    #region [ Tests ]

    /// <summary>
    /// Verifies that construction loads supported games and initializes the default state.
    /// </summary>
    /// <returns>No value is returned.</returns>
    [Fact]
    public void Constructor_LoadsGamesAndInitializesState()
    {
        (string Name, BethesdaGames Type)[] expectedGames =
        {
            ("Fallout 4", BethesdaGames.Fallout4),
            ("Skyrim", BethesdaGames.Skyrim),
            ("Skyrim Special Edition", BethesdaGames.SkyrimSpecialEdition),
            ("Starfield", BethesdaGames.Starfield)
        };

        VoiceLineCounterViewModel viewModel = new();

        Assert.Collection(
            viewModel.BethesdaGames,
            game => AssertGame(expectedGames[0], game),
            game => AssertGame(expectedGames[1], game),
            game => AssertGame(expectedGames[2], game),
            game => AssertGame(expectedGames[3], game));
        Assert.Null(viewModel.SelectedBethesdaGame);
        Assert.Empty(viewModel.InputText);
        Assert.Equal(0, viewModel.LineStatistics.TotalCharacters);
        Assert.Equal(1, viewModel.LineStatistics.TotalDialogueSections);
        Assert.False(viewModel.RunCommand.CanExecute(null));
    }

    /// <summary>
    /// Verifies that selecting a game without entering text does not enable the run command.
    /// </summary>
    /// <param name="game">The selected game to test.</param>
    /// <returns>No value is returned.</returns>
    [Theory]
    [InlineData(BethesdaGames.Fallout4)]
    [InlineData(BethesdaGames.Skyrim)]
    [InlineData(BethesdaGames.SkyrimSpecialEdition)]
    [InlineData(BethesdaGames.Starfield)]
    public void RunCommand_CannotExecuteWithoutInput(BethesdaGames game)
    {
        VoiceLineCounterViewModel viewModel = CreateViewModel(game);

        Assert.False(viewModel.RunCommand.CanExecute(null));
    }

    /// <summary>
    /// Verifies that entering text without selecting a game does not enable the run command.
    /// </summary>
    /// <returns>No value is returned.</returns>
    [Fact]
    public void RunCommand_CannotExecuteWithInputOnly()
    {
        VoiceLineCounterViewModel viewModel = new() { InputText = "TEST!" };

        Assert.False(viewModel.RunCommand.CanExecute(null));
    }

    /// <summary>
    /// Verifies that selecting a game and entering text enables the run command.
    /// </summary>
    /// <param name="game">The selected game to test.</param>
    /// <returns>No value is returned.</returns>
    [Theory]
    [InlineData(BethesdaGames.Fallout4)]
    [InlineData(BethesdaGames.Skyrim)]
    [InlineData(BethesdaGames.SkyrimSpecialEdition)]
    [InlineData(BethesdaGames.Starfield)]
    public void RunCommand_CanExecuteWithGameAndInput(BethesdaGames game)
    {
        VoiceLineCounterViewModel viewModel = CreateViewModel(game);

        viewModel.InputText = "TEST!";

        Assert.True(viewModel.RunCommand.CanExecute(null));
    }

    /// <summary>
    /// Verifies that changing command inputs raises command eligibility notifications.
    /// </summary>
    /// <returns>No value is returned.</returns>
    [Fact]
    public void RunCommand_RequeriesWhenInputAndGameChange()
    {
        VoiceLineCounterViewModel viewModel = new();
        int changeCount = 0;
        viewModel.RunCommand.CanExecuteChanged += (_, _) => changeCount++;

        viewModel.InputText = "TEST!";
        viewModel.SelectedBethesdaGame = viewModel.BethesdaGames[0];

        Assert.Equal(2, changeCount);
    }

    /// <summary>
    /// Verifies that a short voice line produces one dialogue section.
    /// </summary>
    /// <param name="game">The selected game to test.</param>
    /// <returns>No value is returned.</returns>
    [Theory]
    [InlineData(BethesdaGames.Fallout4)]
    [InlineData(BethesdaGames.Skyrim)]
    [InlineData(BethesdaGames.SkyrimSpecialEdition)]
    [InlineData(BethesdaGames.Starfield)]
    public void RunCommand_CalculatesOneSection(BethesdaGames game)
    {
        VoiceLineCounterViewModel viewModel = CreateViewModel(game);
        viewModel.InputText = "TEST!";

        viewModel.RunCommand.Execute(null);

        Assert.Equal(5, viewModel.LineStatistics.TotalCharacters);
        Assert.Equal(1, viewModel.LineStatistics.TotalDialogueSections);
    }

    /// <summary>
    /// Verifies that input exactly at a game's character limit remains in one dialogue section.
    /// </summary>
    /// <param name="game">The selected game to test.</param>
    /// <param name="characterLimit">The character limit for the selected game.</param>
    /// <returns>No value is returned.</returns>
    [Theory]
    [InlineData(BethesdaGames.Fallout4, 150)]
    [InlineData(BethesdaGames.Skyrim, 149)]
    [InlineData(BethesdaGames.SkyrimSpecialEdition, 149)]
    [InlineData(BethesdaGames.Starfield, 350)]
    public void RunCommand_UsesOneSectionAtExactLimit(BethesdaGames game, int characterLimit)
    {
        VoiceLineCounterViewModel viewModel = CreateViewModel(game);
        viewModel.InputText = new string('x', characterLimit);

        viewModel.RunCommand.Execute(null);

        Assert.Equal(characterLimit, viewModel.LineStatistics.TotalCharacters);
        Assert.Equal(1, viewModel.LineStatistics.TotalDialogueSections);
    }

    /// <summary>
    /// Verifies that input above a game's character limit requires two dialogue sections.
    /// </summary>
    /// <param name="game">The selected game to test.</param>
    /// <param name="characterLimit">The character limit for the selected game.</param>
    /// <returns>No value is returned.</returns>
    [Theory]
    [InlineData(BethesdaGames.Fallout4, 150)]
    [InlineData(BethesdaGames.Skyrim, 149)]
    [InlineData(BethesdaGames.SkyrimSpecialEdition, 149)]
    [InlineData(BethesdaGames.Starfield, 350)]
    public void RunCommand_UsesTwoSectionsAboveLimit(BethesdaGames game, int characterLimit)
    {
        VoiceLineCounterViewModel viewModel = CreateViewModel(game);
        viewModel.InputText = new string('x', characterLimit + 1);

        viewModel.RunCommand.Execute(null);

        Assert.Equal(characterLimit + 1, viewModel.LineStatistics.TotalCharacters);
        Assert.Equal(2, viewModel.LineStatistics.TotalDialogueSections);
    }

    /// <summary>
    /// Creates a view model with the requested game selected.
    /// </summary>
    /// <param name="game">The game to select.</param>
    /// <returns>A view model configured with the requested game.</returns>
    private static VoiceLineCounterViewModel CreateViewModel(BethesdaGames game)
    {
        VoiceLineCounterViewModel viewModel = new();
        viewModel.SelectedBethesdaGame = Assert.Single(
            viewModel.BethesdaGames,
            bethesdaGame => bethesdaGame.GameType == game);
        return viewModel;
    }

    /// <summary>
    /// Verifies a generated game against its expected name and type.
    /// </summary>
    /// <param name="expected">The expected game name and type.</param>
    /// <param name="actual">The generated game to verify.</param>
    /// <returns>No value is returned.</returns>
    private static void AssertGame(
        (string Name, BethesdaGames Type) expected,
        BethesdaGame actual)
    {
        Assert.Equal(expected.Name, actual.GameName);
        Assert.Equal(expected.Type, actual.GameType);
    }

    #endregion
}
