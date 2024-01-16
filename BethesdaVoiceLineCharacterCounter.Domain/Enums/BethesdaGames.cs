using System.ComponentModel;

namespace BethesdaVoiceLineCharacterCounter.Domain.Enums
{
    /// <summary>
    /// Enum for the supported Bethesda Games.
    /// NOTE: New releases as there are more moddable Bethesda games.
    /// </summary>
    public enum BethesdaGames
    {
        [Description("Fallout 4")]
        Fallout4,
        [Description("Skyrim")]
        Skyrim,
        [Description("Skyrim Special Edition")]
        SkyrimSpecialEdition
    }
}
