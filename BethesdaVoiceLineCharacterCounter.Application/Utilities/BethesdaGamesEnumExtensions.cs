using BethesdaVoiceLineCharacterCounter.Domain.Enums;

namespace BethesdaVoiceLineCharacterCounter.Application.Utilities
{
    /// <summary>
    /// This class will provide functionality for the BethesdaGames enum.
    /// </summary>
    internal static class BethesdaGamesEnumExtensions
    {
        /// <summary>
        /// This method will get the Description from the Enum.
        /// </summary>
        /// <param name="game">The passed in enum</param>
        /// <returns>The appropriate description.</returns>
        public static string GetGameEnumDescription(BethesdaGames game)
        {
            string gameEnumDescription = string.Empty;

            gameEnumDescription = EnumExtensions.GetEnumDescription(game);

            return gameEnumDescription;
        }

        /// <summary>
        /// This method will get the BethesdaGames enum from the Description.
        /// NOTE: Defaults to Skyrim if can't find a valid one.
        /// </summary>
        /// <param name="description">The passed in description</param>
        /// <returns>The appropriate enum, defaulting to Skyrim if needed.</returns>
        public static BethesdaGames GetGameEnum(string description)
        {
            BethesdaGames gameEnum = BethesdaGames.Skyrim;

            foreach(BethesdaGames bethesdaGames in Enum.GetValues(typeof(BethesdaGames)))
            {
                if (description.Equals(GetGameEnumDescription(bethesdaGames))) 
                { 
                    gameEnum = bethesdaGames;
                    break;
                }
            }

            return gameEnum;
        }
    }
}
