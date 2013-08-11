using System;
using System.Collections.Generic;
using KSP.IO;

namespace FlightComputer
{
    public class SettingsManager
    {
        public static bool Debug = true;

        public string SettingsFile;

        private LogManager _logger;
        private bool _writeNeeded;
        private SortedDictionary<string, string> _values = new SortedDictionary<string, string>();

        public SettingsManager(string filename = "settings.cfg")
        {
            this._logger = new LogManager("Settings", filename);

            this.SettingsFile = filename;
            this.LoadFromFile();
        }

        ~SettingsManager()
        {
            this.SaveToFile();
        }

        public void LoadFromFile(bool clearBeforeReload = true)
        {
            this._logger.Log("Loading settings from: " + this.SettingsFile);
            if (File.Exists<SettingsManager>(this.SettingsFile))
            {
                if (clearBeforeReload)
                {
                    this._values.Clear();
                }

                string[] settingLines = File.ReadAllLines<SettingsManager>(this.SettingsFile);

                foreach (string settingLine in settingLines)
                {
                    string currentLine = settingLine.Trim();

                    // Skip all this jazz if the line is completely empty.
                    // Allow for comments inside the config file.
                    if (currentLine.Length == 0 || currentLine.StartsWith("//") || currentLine.StartsWith("#"))
                    {
                        continue;
                    }

                    string[] line = currentLine.Split(new char[] {'='}, 2);
                    string settingName = line[0].Trim();
                    string settingValue = line[1].Trim();

                    if (!this._values.ContainsKey(settingName))
                    {
                        this._values.Add(settingName, settingValue);
                    }
                    else
                    {
                        this._values[settingName] = settingValue;
                    }
                }
            }
            else
            {
                this._logger.Error("Unable to find specified settings file.");
            }
        }

        public void SaveToFile()
        {
            if (this._writeNeeded)
            {
                this._logger.Log("Saving settings file.");
                if (this._values.Count > 0 && File.Exists<SettingsManager>(this.SettingsFile))
                {
                    TextWriter file = File.CreateText<SettingsManager>(this.SettingsFile);

                    foreach (KeyValuePair<string, string> setting in this._values)
                    {
                        file.WriteLine(setting.Key + "=" + setting.Value);
                    }

                    file.Close();

                    this._writeNeeded = false;
                }
                else
                {
                    this._logger.Error("No settings available to write to file.");
                }
            }
        }

        public T Get<T>(string key)
        {
            if (this._values.ContainsKey(key))
            {
                return (T)Convert.ChangeType(this._values[key], typeof(T));
            }

            return (T)Convert.ChangeType(null, typeof(T));
        }

        // Provide a mechanism for defaulting the value, should it not exist.
        public T Get<T>(string key, T value)
        {
            if (this._values.ContainsKey(key))
            {
                return (T)Convert.ChangeType(this._values[key], typeof(T));
            }

            this._logger.Notice("No value for '" + key + "' found. Defaulting to: " + (string)Convert.ChangeType(value, typeof(string)));
            this.Set(key, value);
            return (T)Convert.ChangeType(this._values[key], typeof(T));
        }

        public void Set<T>(string key, T value)
        {
            if (this._values.ContainsKey(key))
            {
                string stringValue = (string)Convert.ChangeType(value, typeof(string));

                // Check to make sure the value actually did change before moving forward.
                if (this._values[key] != stringValue)
                {
                    this._values[key] = stringValue;
                    this._writeNeeded = true;
                }
            }
            else
            {
                this._values.Add(key, (string)Convert.ChangeType(value, typeof(string)));
                this._writeNeeded = true;
            }
        }

        public SortedDictionary<string, string>.Enumerator GetEnumerator()
        {
            return this._values.GetEnumerator();
        }

        public override string ToString()
        {
            string itemString = "";
            foreach (var item in this._values)
            {
                itemString = itemString + String.Format("{0}='{1}' ", item.Key, item.Value);
            }

            return itemString;
        }
    }
}
