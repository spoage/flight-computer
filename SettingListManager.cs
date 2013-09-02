using System.Collections.Generic;
using KSP.IO;

namespace FlightComputer
{
    public abstract class SettingListManager
    {
        public string SettingsFile;

        protected bool WriteNeeded;
        protected LogManager Logger;

        public abstract List<string> GetLinesToWrite();
        public abstract void AddItem(string itemText = null);

        public void LoadFromFile()
        {
            this.Logger.Log("Loading settings list from: " + this.SettingsFile);
            if (File.Exists<FlightComputer>(this.SettingsFile))
            {
                string[] settingListLines = File.ReadAllLines<FlightComputer>(this.SettingsFile);

                foreach (string line in settingListLines)
                {
                    string currentLine = line.Trim();

                    // Skip all this jazz if the line is completely empty.
                    if (currentLine.Length == 0)
                    {
                        continue;
                    }

                    this.AddItem(currentLine);
                }
            }
            else
            {
                this.Logger.Error("Unable to find specified settings list file.");
            }
        }

        public void SaveToFile()
        {
            if (this.WriteNeeded)
            {
                this.Logger.Log("Saving settings list file.");
                if (File.Exists<FlightComputer>(this.SettingsFile))
                {
                    TextWriter file = File.CreateText<FlightComputer>(this.SettingsFile);

                    List<string> linesToWrite = this.GetLinesToWrite();
                    foreach (string line in linesToWrite)
                    {
                        file.WriteLine(line);
                    }

                    file.Close();

                    this.WriteNeeded = false;
                }
                else
                {
                    this.Logger.Error("No setting list available to write to file.");
                }
            }
        }
    }
}
