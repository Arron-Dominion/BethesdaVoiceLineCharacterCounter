using BethesdaVoiceLineCharacterCounter.Application.Tests.Fixtures;
using BethesdaVoiceLineCharacterCounter.Application.Utilities;
using BethesdaVoiceLineCharacterCounter.Domain.Enums;

namespace BethesdaVoiceLineCharacterCounter.Application.Tests.Utilities
{
    [Trait("Utilities", "EnumExtensions")]
    public class EnumExtensionsTest : ApplicationTest
    {
        #region [ Constructors ]

        public EnumExtensionsTest()
            : base()
        { 

        }

        #endregion

        #region [ Tests ]

        [Theory]
        [InlineData(BethesdaGames.Fallout4, "Fallout 4")]
        [InlineData(BethesdaGames.Skyrim, "Skyrim")]
        [InlineData(BethesdaGames.SkyrimSpecialEdition, "Skyrim Special Edition")]
        public void GetEnumDescription_Success(BethesdaGames game, string expected)
        {
            string result = EnumExtensions.GetEnumDescription(game);

            Assert.Equal(expected, result);
        }

        #endregion
    }
}
