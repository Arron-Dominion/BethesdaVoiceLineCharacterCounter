using BethesdaVoiceLineCharacterCounter.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethesdaVoiceLineCharacterCounter.Application.Utilities
{
    /// <summary>
    /// This class will handle various logic for Character Limit
    /// </summary>
    internal static class CharacterLimitHandler
    {
        /// <summary>
        /// This method will fetch the appropriate character limit for the Bethesda Game.
        /// NOTE: Will default to Skyrim's if needed.
        /// </summary>
        /// <param name="game">The passed in BethesdaGames enum for the game.</param>
        /// <returns>A valid Character Limit.</returns>
        public static long FetchCharacterLimit(BethesdaGames game)
        {
            long limit = 0;

            switch (game)
            {
                case BethesdaGames.Skyrim:
                case BethesdaGames.SkyrimSpecialEdition:
                    limit = 149;
                    break;
                case BethesdaGames.Fallout4:
                    limit = 150;
                    break;
                default:
                    limit = 149;
                    break;
            }

            return limit;
        }
    }
}
