using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MaterialDesignThemes.Wpf;
using TPClient.Helpers;
using TPClient.Services;
using TPClient.Utilities;
using TPClient.View;

namespace TPClient.ViewModel
{
    public class MainViewModel : Singleton<MainViewModel>, INotifyPropertyChanged
    {
        private const int MESSAGE_TIME = 2000;

        public MainViewModel()
        {
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(MESSAGE_TIME));
        }

        private NavigationService _navigation;
        public NavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
            }
        }

        private SnackbarMessageQueue _messageQueue;
        public SnackbarMessageQueue MessageQueue
        {
            get => _messageQueue;
            set
            {
                _messageQueue = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastUpdate { get; private set; }

        private bool _status;

        public bool Status
        {
            get => _status;
            set
            {
                _status = value;
                LastUpdate = DateTime.Now;

                OnPropertyChanged();
                OnPropertyChanged("StatusText");
                OnPropertyChanged("StatusIcon");
            }
        }

        public string StatusText => _status
            ? LocalizationManager.Instance.GetKeyValue("Closed")
            : LocalizationManager.Instance.GetKeyValue("Open");

        public PackIconKind StatusIcon => _status
            ? PackIconKind.Lock
            : PackIconKind.LockOpen;


        private bool _notification;

        public bool Notification
        {
            get => _notification;
            set
            {
                _notification = value;
                OnPropertyChanged();
            }
        }

        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }
      
        /// <summary>
        /// Комманда сворачивания формы.
        /// </summary>
        private RelayCommand _minimizeCommand;

        public RelayCommand MinimizeCommand
        {
            get
            {
                if (_minimizeCommand == null)
                {
                    _minimizeCommand = new RelayCommand(obj =>
                    {
                        IsVisible = false;
                    });
                }

                return _minimizeCommand;
            }
        }


        /// <summary>
        /// Комманда сворачивания формы.
        /// </summary>
        private RelayCommand _settingsCommand;

        public RelayCommand SettingsCommand
        {
            get
            {
                if (_settingsCommand == null)
                {
                    _settingsCommand = new RelayCommand(obj =>
                    {
                        Navigation.Navigate<SettingsPage>();
                    });
                }

                return _settingsCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
