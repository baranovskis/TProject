using System;

namespace TPClient.Helpers
{
    public class Singleton<T> where T : new()
    {
        private static T _instance;
        private static readonly object _syncRoot = new object();

        public static T Instance
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = default(T) == null
                            ? Activator.CreateInstance<T>()
                            : default(T);
                    }
                }

                return _instance;
            }
        }
    }
}
