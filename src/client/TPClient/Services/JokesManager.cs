using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using TPClient.APIs;
using TPClient.Helpers;

namespace TPClient.Services
{
    public class JokesManager : Singleton<JokesManager>
    {
        public string DataSource
        {
            get
            {
                var dataSource = RegistryManager.Instance["DataSource"] as string;
                return dataSource ?? "Chuck Norris Facts";
            }
            set
            {
                RegistryManager.Instance["DataSource"] = value;
            }
        }

        public string Category
        {
            get
            {
                var category = RegistryManager.Instance["Category"] as string;
                return category ?? string.Empty;
            }
            set
            {
                RegistryManager.Instance["Category"] = value;
            }
        }

        private Dictionary<string, Type> _dataSources;

        public JokesManager()
        {
            _dataSources = new Dictionary<string, Type>();

            var dataSources = AppDomain
                   .CurrentDomain
                   .GetAssemblies()
                   .SelectMany(assembly => assembly.GetTypes())
                   .Where(type => typeof(IJokeAPI).IsAssignableFrom(type) && type != typeof(IJokeAPI));

            foreach (var dataSource in dataSources)
            {
                var classInstance = Activator.CreateInstance(dataSource, null) as IJokeAPI;

                if (classInstance == null)
                    continue;

                _dataSources.Add(classInstance.Name, dataSource);
            }
        }

        /// <summary>
        /// Get list of available datasources.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetDataSources()
        {
            foreach (var dataSouce in _dataSources)
            {
                yield return dataSouce.Key;
            }
        }

        /// <summary>
        /// Get list of available categories;
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<string> GetCategories(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            foreach (var dataSource in _dataSources)
            {
                if (!dataSource.Key.Equals(source))
                    continue;

                var classInstance = Activator.CreateInstance(dataSource.Value, null) as IJokeAPI;

                if (classInstance == null)
                    continue;

                return classInstance.Categories;
            }

            return null;
        }

        /// <summary>
        /// Get random joke.
        /// </summary>
        /// <returns></returns>
        public string GetJoke()
        {
            foreach (var dataSource in _dataSources)
            {
                if (!dataSource.Key.Equals(DataSource))
                    continue;

                var classInstance = Activator.CreateInstance(dataSource.Value, null) as IJokeAPI;

                if (classInstance == null)
                    continue;

                var method = typeof(IJokeAPI).GetMethod("GetJoke");
                return method.Invoke(classInstance, null) as string;
            }

            return string.Empty;
        }
    }
}
