using BethesdaVoiceLineCharacterCounter.Domain.Enums;

namespace BethesdaVoiceLineCharacterCounter.Domain.Models
{
    /// <summary>
    /// This class will represent a single Bethesda Game.
    /// </summary>
    public class BethesdaGame
    {
        #region [ Properties ]

        /// <summary>
        /// This is the enum representing the particular game.
        /// </summary>
        public BethesdaGames GameType { get; set; }

        /// <summary>
        /// This is the string representation of the game.
        /// </summary>
        public string GameName { get; set; }

        #endregion
    }
}
