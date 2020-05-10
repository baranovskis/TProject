using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using TPClient.Helpers;

namespace TPClient.Services
{
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        public string Localization
        {
            get
            {
                var dataSource = RegistryManager.Instance["Localization"] as string;
                return dataSource ?? "en-US";
            }
            set
            {
                RegistryManager.Instance["Localization"] = value;
            }
        }

        /// <summary>  
        /// Set language based on previously save language setting,  
        /// otherwise set to OS lanaguage  
        /// </summary>  
        /// <param name="element"></param>  
        public void SetDefaultLanguage()
        {
            SetLanguageResourceDictionary(GetLocXAMLFilePath(Localization));
        }

        /// <summary>  
        /// Dynamically load a Localization ResourceDictionary from a file  
        /// </summary>  
        public void SwitchLanguage(string inFiveCharLang)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(inFiveCharLang);
            SetLanguageResourceDictionary(GetLocXAMLFilePath(inFiveCharLang));

            // Save new culture info to registry
            RegistryManager.Instance["Localization"] = inFiveCharLang;
        }

        /// <summary>  
        /// Returns the path to the ResourceDictionary file based on the language character string.  
        /// </summary>  
        /// <param name="inFiveCharLang"></param>  
        /// <returns></returns>  
        public string GetLocXAMLFilePath(string inFiveCharLang)
        {
            string locXamlFile = inFiveCharLang + ".xaml";
            string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(directory, "i18N", locXamlFile);
        }

        /// <summary>  
        /// Sets or replaces the ResourceDictionary by dynamically loading  
        /// a Localization ResourceDictionary from the file path passed in.  
        /// </summary>  
        /// <param name="inFile"></param>  
        private void SetLanguageResourceDictionary(string inFile)
        {
            if (File.Exists(inFile))
            {
                // Read in ResourceDictionary File  
                var languageDictionary = new ResourceDictionary
                {
                    Source = new Uri(inFile)
                };

                // Remove any previous Localization dictionaries loaded  
                int langDictId = -1;

                for (int i = 0; i < Application.Current.Resources.MergedDictionaries.Count; i++)
                {
                    var md = Application.Current.Resources.MergedDictionaries[i];

                    // Make sure your Localization ResourceDictionarys have the ResourceDictionaryName  
                    // key and that it is set to a value starting with "Loc-".  
                    if (md.Contains("ResourceDictionaryName"))
                    {
                        if (md["ResourceDictionaryName"].ToString().StartsWith("Loc-"))
                        {
                            langDictId = i;
                            break;
                        }
                    }
                }

                if (langDictId == -1)
                {
                    // Add in newly loaded Resource Dictionary  
                    Application.Current.Resources.MergedDictionaries.Add(languageDictionary);
                }
                else
                {
                    // Replace the current langage dictionary with the new one  
                    Application.Current.Resources.MergedDictionaries[langDictId] = languageDictionary;
                }
            }
        }

        public string GetKeyValue(string key)
        {
            for (int i = 0; i < Application.Current.Resources.MergedDictionaries.Count; i++)
            {
                var md = Application.Current.Resources.MergedDictionaries[i];

                // Make sure your Localization ResourceDictionarys have the ResourceDictionaryName  
                // key and that it is set to a value starting with "Loc-".  
                if (md.Contains("ResourceDictionaryName"))
                {
                    if (md["ResourceDictionaryName"].ToString().StartsWith("Loc-"))
                    {
                        if (md.Contains(key))
                        {
                            return md[key].ToString();
                        }
                    }
                }
            }

            return key;
        }
    }
}
