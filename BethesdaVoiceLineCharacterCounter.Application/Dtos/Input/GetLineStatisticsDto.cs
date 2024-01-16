using BethesdaVoiceLineCharacterCounter.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethesdaVoiceLineCharacterCounter.Application.Dtos.Input
{
    /// <summary>
    /// This class will hold the necessary information to fetch Line Statistics
    /// </summary>
    public class GetLineStatisticsDto
    {
        #region [ Properties ]

        /// <summary>
        /// This will represent the current Bethesda Game to fetch statistics for.
        /// </summary>
        public BethesdaGames BethesdaGame { get; set; }

        /// <summary>
        /// This represents the user entered text.
        /// </summary>
        public string InputText { get; set; }

        #endregion
    }
}
