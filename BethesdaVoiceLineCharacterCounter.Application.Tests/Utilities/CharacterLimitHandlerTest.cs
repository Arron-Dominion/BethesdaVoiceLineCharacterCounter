using BethesdaVoiceLineCharacterCounter.Application.Tests.Fixtures;
using BethesdaVoiceLineCharacterCounter.Application.Utilities;
using BethesdaVoiceLineCharacterCounter.Domain.Enums;

namespace BethesdaVoiceLineCharacterCounter.Application.Tests.Utilities
{
    [Trait("Utilities", "CharacterLimitHandler")]
    public class CharacterLimitHandlerTest : ApplicationTest
    {
        #region [ Constructors ]

        public CharacterLimitHandlerTest() 
            : base()
        {

        }

        #endregion

        #region [ Tests ]

        [Theory]
        [InlineData(BethesdaGames.Fallout4, (long)150)]
        [InlineData(BethesdaGames.Skyrim, (long)149)]
        [InlineData(BethesdaGames.SkyrimSpecialEdition, (long)149)]
        public void FetchCharacterLimit_Success(BethesdaGames game, long expected)
        {
            long results = CharacterLimitHandler.FetchCharacterLimit(game);

            Assert.Equal(expected, results);
        }

        #endregion
    }
}
