using BethesdaVoiceLineCharacterCounter.Application.Utilities;
using BethesdaVoiceLineCharacterCounter.Domain.Enums;
using BethesdaVoiceLineCharacterCounter.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethesdaVoiceLineCharacterCounter.Application.Features
{
    /// <summary>
    /// This class will manage fetching the Bethesda Games.
    /// </summary>
    public class GetBethesdaGames
    {
        /// <summary>
        /// This method will generate a list of BethesdaGame for use as needed.
        /// </summary>
        /// <returns>A valid list ordered list of bethesda games.</returns>
        public List<BethesdaGame> GenerateBethesdaGames()
        {
            List<BethesdaGame> gameList = new List<BethesdaGame>();

            foreach(BethesdaGames bethesdaGame in Enum.GetValues(typeof(BethesdaGames)))
            {
                gameList.Add(new BethesdaGame()
                {
                    GameName = BethesdaGamesEnumExtensions.GetGameEnumDescription(bethesdaGame),
                    GameType = bethesdaGame
                });
            }

            return gameList;
        }
    }
}
