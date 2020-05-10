using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using TPClient.Helpers;
using TPClient.Model;
using TPClient.Services;
using TPClient.View;

namespace TPClient.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel()
        {
            AutoStart = RegistryManager.Instance.AutoStart;
            DataSource = JokesManager.Instance.DataSource;
            Category = JokesManager.Instance.Category;
        }

        private ComboBoxItem _culture;

        public ComboBoxItem Culture
        {
            get => _culture;
            set
            {
                _culture = value;
                OnPropertyChanged();
            }
        }
        
        private bool _autoStart;

        public bool AutoStart
        {
            get => _autoStart;
            set
            {
                _autoStart = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> DataSources
        {
            get
            {
                return JokesManager.Instance.GetDataSources();
            }
        }

        private string _dataSource;

        public string DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value; 
                OnPropertyChanged();
                OnPropertyChanged("Categories");
            }
        }

        public IEnumerable<string> Categories
        {
            get
            {
                return JokesManager.Instance.GetCategories(_dataSource);
            }
        }

        private string _category;

        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Save settings command
        /// </summary>
        private RelayCommand _saveCommand;

        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(obj =>
                    {
                        var mainView = MainViewModel.Instance;

                        if (mainView == null)
                            return;

                        RegistryManager.Instance.AutoStart = AutoStart;

                        JokesManager.Instance.DataSource = DataSource;
                        JokesManager.Instance.Category = Category;

                        LocalizationManager.Instance.SwitchLanguage(Culture.Tag.ToString());

                        mainView.MessageQueue.Enqueue("Settings saved successfully");
                        mainView.Navigation.Navigate<JokesPage>();
                    });
                }

                return _saveCommand;
            }
        }

        /// <summary>
        /// Cancel command
        /// </summary>
        private RelayCommand _cancelCommand;

        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(obj =>
                    {
                        MainViewModel.Instance.Navigation.Navigate<JokesPage>();
                    });
                }

                return _cancelCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
