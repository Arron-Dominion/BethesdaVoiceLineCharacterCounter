using BethesdaVoiceLineCharacterCounter.Application.Dtos.Input;
using BethesdaVoiceLineCharacterCounter.Application.Dtos.Output;
using BethesdaVoiceLineCharacterCounter.Application.Features;
using BethesdaVoiceLineCharacterCounter.Application.Tests.Fixtures;
using BethesdaVoiceLineCharacterCounter.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethesdaVoiceLineCharacterCounter.Application.Tests.Features
{
    [Trait("Application", "GetLineStatistics")]
    public class GetLineStatisticsTest : ApplicationTest
    {
        #region [ Constructors ]

        public GetLineStatisticsTest() 
            : base()
        { 
        
        }

        #endregion

        #region [ Tests ]

        [Theory]
        [InlineData(BethesdaGames.Fallout4)]
        [InlineData(BethesdaGames.Skyrim)]
        [InlineData(BethesdaGames.SkyrimSpecialEdition)]
        public void GetLineStatisticsFromInput_OneSectionSuccess(BethesdaGames game)
        {
            LineStatisticsDto expected = new LineStatisticsDto()
            {
                TotalCharacters = 4,
                TotalDialogueSections = 1
            };

            GetLineStatisticsDto input = new GetLineStatisticsDto()
            {
                BethesdaGame = game,
                InputText = "TEST"
            };

            GetLineStatistics feature = new GetLineStatistics();

            LineStatisticsDto result = feature.GetLineStatisticsFromInput(input);

            Assert.Equal(expected.TotalCharacters, result.TotalCharacters);
            Assert.Equal(expected.TotalDialogueSections, result.TotalDialogueSections);
        }

        [Theory]
        [InlineData(BethesdaGames.Fallout4, (long)1)]
        [InlineData(BethesdaGames.Skyrim, (long)2)]
        [InlineData(BethesdaGames.SkyrimSpecialEdition, (long)2)]
        public void GetLineStatisticsFromInput_DifferentSectionsForGame_Success(BethesdaGames game, long sections)
        {
            LineStatisticsDto expected = new LineStatisticsDto()
            {
                TotalCharacters = 150,
                TotalDialogueSections = sections
            };

            GetLineStatisticsDto input = new GetLineStatisticsDto()
            {
                BethesdaGame = game,
                InputText = "123450912897231879128930890128970128901289701238970123897031289712389123980129388912373891273891201239801892091290128912897321897129812891298181818211"
            };

            GetLineStatistics feature = new GetLineStatistics();

            LineStatisticsDto result = feature.GetLineStatisticsFromInput(input);

            Assert.Equal(input.InputText.Length, expected.TotalCharacters);
            Assert.Equal(expected.TotalCharacters, result.TotalCharacters);
            Assert.Equal(expected.TotalDialogueSections, result.TotalDialogueSections);
        }

        #endregion
    }
}
