namespace BethesdaVoiceLineCharacterCounter.Application.Dtos.Output
{
    /// <summary>
    /// This class will store the output statistics for the input voice line.
    /// </summary>
    public class LineStatisticsDto
    {
        #region [ Properties ]

        /// <summary>
        /// This represents the total characters in the input block.
        /// </summary>
        public long TotalCharacters { get; set; }

        /// <summary>
        /// This represents the total dialogue sections.
        /// </summary>
        public long TotalDialogueSections { get; set; }

        #endregion
    }
}
