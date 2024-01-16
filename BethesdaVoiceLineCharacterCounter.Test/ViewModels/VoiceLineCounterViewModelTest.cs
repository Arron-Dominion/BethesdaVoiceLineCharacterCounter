using BethesdaVoiceLineCharacterCounter.Application.Dtos.Output;
using BethesdaVoiceLineCharacterCounter.Domain.Enums;
using BethesdaVoiceLineCharacterCounter.Domain.Models;
using BethesdaVoiceLineCharacterCounter.Test.Fixtures;
using BethesdaVoiceLineCharacterCounter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethesdaVoiceLineCharacterCounter.Test.ViewModels
{
    [Trait("ViewModels", "VoiceLineCounterViewModel")]
    public class VoiceLineCounterViewModelTest : PresentationTest
    {
        #region [ Constructors ]

        public VoiceLineCounterViewModelTest() 
            : base()
        { 
        
        }

        #endregion

        #region [ Tests ]

        [Fact]
        public void VoiceLineCounterViewModel_Constructor_Success()
        {
            List<BethesdaGame> expectedGames = new List<BethesdaGame>()
            {
                new BethesdaGame() 
                {
                    GameName = "Fallout 4",
                    GameType = BethesdaGames.Fallout4
                },
                new BethesdaGame()
                {
                    GameName = "Skyrim",
                    GameType = BethesdaGames.Skyrim
                },
                new BethesdaGame()
                {
                    GameName = "Skyrim Special Edition",
                    GameType = BethesdaGames.SkyrimSpecialEdition
                }
            };

            VoiceLineCounterViewModel viewModel = new VoiceLineCounterViewModel();

            for (int i = 0; i < expectedGames.Count; i++)
            {
                Assert.Equal(expectedGames[i].GameName, viewModel.BethesdaGames[i].GameName);
                Assert.Equal(expectedGames[i].GameType, viewModel.BethesdaGames[i].GameType);
            }

            Assert.NotNull(viewModel.RunCommand);
        }

        [Theory]
        [InlineData("Fallout 4", BethesdaGames.Fallout4)]
        [InlineData("Skyrim", BethesdaGames.Skyrim)]
        [InlineData("Skyrim Special Edition", BethesdaGames.SkyrimSpecialEdition)]
        public void VoiceLineCounterViewModel_RunCanExecute_False(string gameName, BethesdaGames gameEnum)
        {
            bool expected = false;

            VoiceLineCounterViewModel viewModel = new VoiceLineCounterViewModel();

            viewModel.SelectedBethesdaGame = viewModel.BethesdaGames.Where(x => x.GameName.Equals(gameName) && x.GameType.Equals(gameEnum)).FirstOrDefault();

            Assert.Equal(expected, viewModel.RunCommand.CanExecute(null));
        }

        [Theory]
        [InlineData("Fallout 4", BethesdaGames.Fallout4)]
        [InlineData("Skyrim", BethesdaGames.Skyrim)]
        [InlineData("Skyrim Special Edition", BethesdaGames.SkyrimSpecialEdition)]
        public void VoiceLineCounterViewModel_RunCanExecute_True(string gameName, BethesdaGames gameEnum)
        {
            bool expected = true;

            VoiceLineCounterViewModel viewModel = new VoiceLineCounterViewModel();

            viewModel.SelectedBethesdaGame = viewModel.BethesdaGames.Where(x => x.GameName.Equals(gameName) && x.GameType.Equals(gameEnum)).FirstOrDefault();
            viewModel.InputText = "TEST!";

            Assert.Equal(expected, viewModel.RunCommand.CanExecute(null));
        }

        [Theory]
        [InlineData("Fallout 4", BethesdaGames.Fallout4)]
        [InlineData("Skyrim", BethesdaGames.Skyrim)]
        [InlineData("Skyrim Special Edition", BethesdaGames.SkyrimSpecialEdition)]
        public void VoiceLineCounterViewModel_RunExecute_OneSection(string gameName, BethesdaGames gameEnum)
        {
            LineStatisticsDto expected = new LineStatisticsDto()
            {
                TotalCharacters = 5,
                TotalDialogueSections = 1
            };

            VoiceLineCounterViewModel viewModel = new VoiceLineCounterViewModel();

            viewModel.SelectedBethesdaGame = viewModel.BethesdaGames.Where(x => x.GameName.Equals(gameName) && x.GameType.Equals(gameEnum)).FirstOrDefault();
            viewModel.InputText = "TEST!";

            viewModel.RunCommand.Execute(null);

            Assert.Equal(expected.TotalCharacters, viewModel.LineStatistics.TotalCharacters);
            Assert.Equal(expected.TotalDialogueSections, viewModel.LineStatistics.TotalDialogueSections);
        }

        [Theory]
        [InlineData("Fallout 4", BethesdaGames.Fallout4, (long)1)]
        [InlineData("Skyrim", BethesdaGames.Skyrim, (long)2)]
        [InlineData("Skyrim Special Edition", BethesdaGames.SkyrimSpecialEdition, (long)2)]
        public void VoiceLineCounterViewModel_RunExecute_DifferentSections(string gameName, BethesdaGames gameEnum, long sections)
        {
            LineStatisticsDto expected = new LineStatisticsDto()
            {
                TotalCharacters = 150,
                TotalDialogueSections = sections
            };

            VoiceLineCounterViewModel viewModel = new VoiceLineCounterViewModel();

            viewModel.SelectedBethesdaGame = viewModel.BethesdaGames.Where(x => x.GameName.Equals(gameName) && x.GameType.Equals(gameEnum)).FirstOrDefault();
            viewModel.InputText = "123450912897231879128930890128970128901289701238970123897031289712389123980129388912373891273891201239801892091290128912897321897129812891298181818211";

            viewModel.RunCommand.Execute(null);

            Assert.Equal(expected.TotalCharacters, viewModel.LineStatistics.TotalCharacters);
            Assert.Equal(expected.TotalDialogueSections, viewModel.LineStatistics.TotalDialogueSections);
        }

        #endregion
    }
}
