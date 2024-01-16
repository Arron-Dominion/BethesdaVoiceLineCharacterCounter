using BethesdaVoiceLineCharacterCounter.Application.Features;
using BethesdaVoiceLineCharacterCounter.Application.Tests.Fixtures;
using BethesdaVoiceLineCharacterCounter.Domain.Enums;
using BethesdaVoiceLineCharacterCounter.Domain.Models;

namespace BethesdaVoiceLineCharacterCounter.Application.Tests.Features
{
    [Trait("Application", "GetBethesdaGames")]
    public class GetBethesdaGamesTest : ApplicationTest
    {
        #region [ Constructors ]

        public GetBethesdaGamesTest()
            : base()
        {

        }

        #endregion

        #region [ Tests ]

        [Fact]
        public void GenerateBethesdaGames_Success()
        {
            List<BethesdaGame> expected = new List<BethesdaGame>()
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

            GetBethesdaGames feature = new GetBethesdaGames();

            List<BethesdaGame> result = feature.GenerateBethesdaGames();

            Assert.Equal(expected.Count, result.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].GameName, result[i].GameName);
                Assert.Equal(expected[i].GameType, result[i].GameType);
            }
        }

        #endregion
    }
}
