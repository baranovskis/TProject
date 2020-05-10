using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TPClient.Helpers;
using TPClient.Services;

namespace TPClient.ViewModel
{
    public class JokesViewModel : INotifyPropertyChanged
    {
        public JokesViewModel()
        {
            Joke = JokesManager.Instance.GetJoke();
        }

        private string _joke;

        public string Joke
        {
            get => _joke;
            set
            {
                _joke = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Last update info command.
        /// </summary>
        private RelayCommand _infoCommand;

        public RelayCommand InfoCommand
        {
            get
            {
                if (_infoCommand == null)
                {
                    _infoCommand = new RelayCommand(obj =>
                    {
                        var mainView = MainViewModel.Instance;

                        if (mainView == null)
                            return;

                        mainView.MessageQueue.Enqueue("Последнее обновление: " + mainView.LastUpdate.ToString("dd.MM.yyyy H:mm:ss"));
                    });
                }

                return _infoCommand;
            }
        }

        /// <summary>
        /// Get new joke command.
        /// </summary>
        private RelayCommand _refreshCommand;

        public RelayCommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(obj =>
                    {
                        Joke = JokesManager.Instance.GetJoke();
                    });
                }
                ;

                return _refreshCommand;
            }
        }

        /// <summary>
        /// Комманда копирования шутки.
        /// </summary>
        private RelayCommand _copyCommand;

        public RelayCommand CopyCommand
        {
            get
            {
                if (_copyCommand == null)
                {
                    _copyCommand = new RelayCommand(obj =>
                    {
                        if (!string.IsNullOrEmpty(Joke))
                        {
                            Clipboard.SetText(Joke);
                        }
                    });
                }

                return _copyCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
