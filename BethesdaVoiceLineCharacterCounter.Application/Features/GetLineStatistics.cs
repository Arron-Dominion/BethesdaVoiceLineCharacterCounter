using BethesdaVoiceLineCharacterCounter.Application.Dtos.Input;
using BethesdaVoiceLineCharacterCounter.Application.Dtos.Output;
using BethesdaVoiceLineCharacterCounter.Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethesdaVoiceLineCharacterCounter.Application.Features
{
    /// <summary>
    /// This feature will allow for Line Statistics operations
    /// </summary>
    public class GetLineStatistics
    {
        #region [ Methods ]

        /// <summary>
        /// This method will take Line Statistic Input information and return appropriate statistics.
        /// </summary>
        /// <param name="input">The input parameters for this feature.</param>
        /// <returns>Line Statistics with appropriate information.</returns>
        public LineStatisticsDto GetLineStatisticsFromInput(GetLineStatisticsDto input)
        {
            LineStatisticsDto lineStatistics = new LineStatisticsDto();

            long gameCharacterLimit = CharacterLimitHandler.FetchCharacterLimit(input.BethesdaGame);

            int currCharacters = 0;

            lineStatistics.TotalDialogueSections = 1;

            foreach(char character in input.InputText)
            {
                lineStatistics.TotalCharacters += 1;
                currCharacters += 1;

                if ((currCharacters % gameCharacterLimit).Equals(0))
                {
                    lineStatistics.TotalDialogueSections += 1;
                    currCharacters = 0;
                }
            }

            if (currCharacters == 0 && input.InputText.Length > 0)
            {
                //Means we were right at the character limit
                lineStatistics.TotalDialogueSections -= 1;
            }

            return lineStatistics;
        }

        #endregion
    }
}
