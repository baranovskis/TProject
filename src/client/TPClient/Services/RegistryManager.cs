using Microsoft.Win32;
using TPClient.Helpers;
using System.Reflection;

namespace TPClient.Services
{
    /// <summary>
    /// Менеджер настроек приложения. 
    /// </summary>
    public class RegistryManager : Singleton<RegistryManager>
    {
        /// <summary>
        /// Ключь реестра приложения
        /// </summary>
        private RegistryKey _setupKey;

        /// <summary>
        /// Ключь реестра автозапуска.
        /// </summary>
        private RegistryKey _autoStartKey;

        /// <summary>
        /// Авто-запуск программы при старте Windows.
        /// </summary>
        public bool AutoStart
        {
            get => _autoStartKey.GetValue("ToiletProject") != null;
            set
            {
                if (value)
                {
                    _autoStartKey.SetValue("ToiletProject", Assembly.GetExecutingAssembly().Location);
                    return;
                }

                _autoStartKey.DeleteValue("ToiletProject", false);
            }
        }

        public RegistryManager()
        {
            InitRegistry();
        }

        /// <summary>
        /// Получить/установить значение.
        /// </summary>
        /// <param name="name">Название ключа</param>
        /// <returns></returns>
        public object this[string name]
        {
            get => _setupKey?.GetValue(name);
            set => _setupKey?.SetValue(name, value ?? string.Empty);
        }

        /// <summary>
        /// Загрузить/создать ключи деревьев риестра.
        /// </summary>
        private void InitRegistry()
        {
            while (true)
            {
                _setupKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\TProject\Setup", true);
                _autoStartKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                if (_setupKey != null)
                    return;

                var softwareKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);

                foreach (var key in new[] {"TProject", "Setup"})
                {
                    softwareKey = softwareKey?.CreateSubKey(key);
                }
            }
        }
    }
}
