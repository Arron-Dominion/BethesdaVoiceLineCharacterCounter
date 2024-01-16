using BethesdaVoiceLineCharacterCounter.Application.Tests.Fixtures;
using BethesdaVoiceLineCharacterCounter.Application.Utilities;
using BethesdaVoiceLineCharacterCounter.Domain.Enums;

namespace BethesdaVoiceLineCharacterCounter.Application.Tests.Utilities
{
    [Trait("Utilities", "BethesdaGamesEnumExtensions")]
    public class BethesdaGamesEnumExtensionsTest : ApplicationTest
    {
        #region [ Constructors ]

        public BethesdaGamesEnumExtensionsTest() 
            : base()
        { 

        }

        #endregion

        #region [ Tests ]

        [Theory]
        [InlineData(BethesdaGames.Fallout4, "Fallout 4")]
        [InlineData(BethesdaGames.Skyrim, "Skyrim")]
        [InlineData(BethesdaGames.SkyrimSpecialEdition, "Skyrim Special Edition")]
        public void GetGameEnumDescription_Success(BethesdaGames game, string expected)
        {
            string result = BethesdaGamesEnumExtensions.GetGameEnumDescription(game);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Fallout 4", BethesdaGames.Fallout4)]
        [InlineData("Skyrim", BethesdaGames.Skyrim)]
        [InlineData("Skyrim Special Edition", BethesdaGames.SkyrimSpecialEdition)]
        public void GetGameEnum_Success(string description, BethesdaGames expected)
        {
            BethesdaGames result = BethesdaGamesEnumExtensions.GetGameEnum(description);

            Assert.Equal(expected, result);
        }

        #endregion
    }
}
