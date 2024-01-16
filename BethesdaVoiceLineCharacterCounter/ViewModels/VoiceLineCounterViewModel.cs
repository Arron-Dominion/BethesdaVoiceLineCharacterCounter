using BethesdaVoiceLineCharacterCounter.Application.Dtos.Input;
using BethesdaVoiceLineCharacterCounter.Application.Dtos.Output;
using BethesdaVoiceLineCharacterCounter.Application.Features;
using BethesdaVoiceLineCharacterCounter.Domain.Models;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BethesdaVoiceLineCharacterCounter.ViewModels
{
    internal class VoiceLineCounterViewModel : INotifyPropertyChanged
    {
        #region [ Events ]

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region [ Variables ]

        private BethesdaGame _selectedGame = new BethesdaGame();
        private List<BethesdaGame> _bethesdaGames = new List<BethesdaGame>();
        private string _inputText = "";
        private LineStatisticsDto _lineStatisticsDto = new LineStatisticsDto() 
                                                       { 
                                                            TotalCharacters = 0, 
                                                            TotalDialogueSections = 1 
                                                        };

        #endregion

        #region [ Properties ]

        public List<BethesdaGame> BethesdaGames
        {
            get => _bethesdaGames;
            set
            {
                _bethesdaGames = value;
                OnPropertyChanged();
            }
        }

        public BethesdaGame SelectedBethesdaGame
        {
            get => _selectedGame;
            set
            {
                _selectedGame = value;
                OnPropertyChanged(nameof(SelectedBethesdaGame));
                RunCommand.ChangeCanExecute();
            }
        }

        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
                RunCommand.ChangeCanExecute();
            }
        }

        public LineStatisticsDto LineStatistics
        {
            get => _lineStatisticsDto;
            set
            {
                _lineStatisticsDto = value;
                OnPropertyChanged();
            }
        }

        public Command RunCommand { private set; get; }

        #endregion

        #region [ Constructors ]

        public VoiceLineCounterViewModel() 
        {
            GetBethesdaGames getGames = new GetBethesdaGames();
            BethesdaGames = getGames.GenerateBethesdaGames();

            RunCommand = new Command(
                execute: RunExecute,
                canExecute: RunCanExecute);
        }

        #endregion

        #region [ Command Methods ]

        private void RunExecute()
        {
            GetLineStatistics getStatisics = new GetLineStatistics();
            GetLineStatisticsDto getStatisticsDto = new GetLineStatisticsDto()
            {
                BethesdaGame = SelectedBethesdaGame.GameType,
                InputText = InputText
            };

            LineStatistics = getStatisics.GetLineStatisticsFromInput(getStatisticsDto);
        }

        private bool RunCanExecute()
        {
            bool canRun = InputText.Length > 0 && SelectedBethesdaGame != null;
            return canRun;
        }

        #endregion

        #region [ Events ]

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }
}
