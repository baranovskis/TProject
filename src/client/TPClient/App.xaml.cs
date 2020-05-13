using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using TPClient.Model.Api;
using TPClient.Services;
using TPClient.Utilities;
using TPClient.View;
using TPClient.ViewModel;

namespace TPClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private const string AppName = "Toilet Project v3.0.0";

        private MQTTManager _mqttManager;
        private NotifyIcon _notifyIcon;
        private Thread _backgroundThread;
        private MainWindow _mainWindow;

        /// <summary>
        /// Create forms and setup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += FatalExceptionHandler;

            Log.Instance.Info("**********************");
            Log.Instance.Info("*** Toilet Project ***");
            Log.Instance.Info("*********** v3.0.0 ***");
            Log.Instance.Info("**********************");

            LocalizationManager.Instance.SetDefaultLanguage();
            
            _mainWindow = new MainWindow();
            MainWindow = _mainWindow;

            // Set position
            var workingArea = SystemParameters.WorkArea;
            _mainWindow.Left = workingArea.Right - _mainWindow.Width - 5;
            _mainWindow.Top = workingArea.Bottom - _mainWindow.Height - 5;

            _mqttManager = new MQTTManager(HandleSignal);

            _backgroundThread = new Thread(ThreadStartingPoint);
            _backgroundThread.SetApartmentState(ApartmentState.STA);
            _backgroundThread.IsBackground = true;
            _backgroundThread.Start();

            var contextMenu = new ContextMenu();

            var exitItem = new MenuItem
            {
                Index = 1,
                Text = LocalizationManager.Instance.GetKeyValue("Exit")
            };

            exitItem.Click += exitItem_Click;
            contextMenu.MenuItems.Add(exitItem);

            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Text = AppName,
                ContextMenu = contextMenu
            };

            _notifyIcon.DoubleClick += delegate
            {
                // Show window
                _mainWindow.Show();
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.ShowInTaskbar = false;
            };

            SetNotifyIcon(State.Off);
            Log.Instance.Info("Application initialized!");
        }

        /// <summary>
        /// Fatal exception handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void FatalExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            if (args != null)
            {
                var e = (Exception)args.ExceptionObject;
                Log.Instance.Fatal(e);
            }
            else
            {
                Log.Instance.Fatal("Unknown fatal error!");
            }

            System.Windows.Forms.MessageBox.Show(LocalizationManager.Instance.GetKeyValue("Error"), AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Register & unregister app from autorun.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void regItem_Click(object sender, EventArgs e)
        {
            var item = sender as MenuItem;

            if (item == null) 
                return;
            
            var newState = !RegistryManager.Instance.AutoStart;
            RegistryManager.Instance.AutoStart = newState;
            item.Checked = newState;
        }

        /// <summary>
        /// Close the form, which closes the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitItem_Click(object sender, EventArgs e)
        {
            if (_backgroundThread != null)
            {
                _backgroundThread.Abort();
            }

            if (_mqttManager != null)
            {
                _mqttManager.Dispose();
            }

            if (_mainWindow != null)
            {
                _mainWindow.Close();
            }

            Current.Shutdown();
        }

        /// <summary>
        /// Change notification icon.
        /// </summary>
        /// <param name="state"></param>
        private void SetNotifyIcon(State state)
        {
            var iconUri = new Uri("pack://application:,,,/Resources/icon_error.ico");

            switch (state)
            {
                case State.On:
                    _notifyIcon.Text = LocalizationManager.Instance.GetKeyValue("LightOn");
                    iconUri = new Uri("pack://application:,,,/Resources/icon_on.ico");
                    break;
                case State.Off:
                    _notifyIcon.Text = LocalizationManager.Instance.GetKeyValue("LightOff");

                    _mainWindow.Dispatcher.Invoke(() =>
                    {
                        var msg = LocalizationManager.Instance.GetKeyValue("LightNotify");
                        var viewModel = _mainWindow.DataContext as MainViewModel;

                        if (viewModel != null && viewModel.Notification)
                        {
                            viewModel.Notification = false;
                            viewModel.MessageQueue.Enqueue(msg);

                            _notifyIcon.ShowBalloonTip(30000, "TProject Notification", msg,
                                ToolTipIcon.None);
                        }
                    });

                    iconUri = new Uri("pack://application:,,,/Resources/icon_off.ico");
                    break;
            }

            var streamResourceInfo = GetResourceStream(iconUri);

            if (streamResourceInfo != null)
            {
                _notifyIcon.Icon = new Icon(streamResourceInfo.Stream);
            }
        }

        /// <summary>
        /// Start background thread.
        /// </summary>
        private void ThreadStartingPoint()
        {
            while (true)
            {
                try
                {
                    if (_mqttManager.IsListeningForSignals())
                    {
                        Thread.Sleep(30000);
                        continue;
                    }

                    var listening = _mqttManager.ListenSignalsTask();

                    if (listening)
                        continue;

                    _mainWindow.Dispatcher.Invoke(() =>
                    {
                        var msg = LocalizationManager.Instance.GetKeyValue("Reconnect");
                        var viewModel = _mainWindow.DataContext as MainViewModel;

                        if (viewModel != null)
                        {
                            viewModel.MessageQueue.Enqueue(msg);
                        }

                        _notifyIcon.ShowBalloonTip(15000, "TProject Notification", msg,
                            ToolTipIcon.Warning);
                    });

                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex);
                }
            }
        }

        /// <summary>
        /// Handle recived data.
        /// </summary>
        /// <param name="signal"></param>
        protected void HandleSignal(Signal signal)
        {
            //if (!_mqttManager.IsListeningForSignals())
            //    return;

            if (signal == null)
            {
                _mainWindow.Dispatcher.Invoke(() =>
                {
                    var viewModel = _mainWindow.DataContext as MainViewModel;

                    if (viewModel != null)
                    {
                        viewModel.MessageQueue.Enqueue(LocalizationManager.Instance.GetKeyValue("WrongFormat"));
                    }
                });

                return;
            }

            var state = signal.SensorStatus ? State.On : State.Off;

            Dispatcher.Invoke(() =>
            {
                var viewModel = _mainWindow.DataContext as MainViewModel;

                if (viewModel != null)
                {
                    viewModel.Status = signal.SensorStatus;
                }
            });

            SetNotifyIcon(state);
        }
    }
}
