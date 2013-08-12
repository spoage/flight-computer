using System.Collections.Generic;
using KSP.IO;
using UnityEngine;

namespace FlightComputer
{
    public class FlightReadoutManager
    {
        public static bool Debug = true;

        public FlightComputer Computer;
        public List<FlightReadout> Readouts = new List<FlightReadout>();

        private bool _writeNeeded;
        private string _readoutListFile;
        private LogManager _logger;

        public FlightReadoutManager(FlightComputer computer, string filename = "readouts.cfg")
        {
            this.Computer = computer;
            this._logger = new LogManager("ReadoutManager", filename);

            this._readoutListFile = filename;
            this.LoadFromFile();
        }

        ~FlightReadoutManager()
        {
            this.SaveToFile();

            this.Readouts.ForEach(readout => readout.DestroyReadout());
            this.Readouts.Clear();
        }

        public void LoadFromFile(bool clearBeforeReload = true)
        {
            this._logger.Log("Loading readout list from: " + this._readoutListFile);
            if (File.Exists<FlightReadoutManager>(this._readoutListFile))
            {
                if (clearBeforeReload)
                {
                    this.Readouts.Clear();
                }

                string[] readoutListLines = File.ReadAllLines<FlightReadoutManager>(this._readoutListFile);

                foreach (string line in readoutListLines)
                {
                    string currentLine = line.Trim();

                    // Skip all this jazz if the line is completely empty.
                    if (currentLine.Length == 0)
                    {
                        continue;
                    }

                    bool listContainsReadout = false;
                    foreach (FlightReadout readout in this.Readouts)
                    {
                        if (currentLine == readout.Settings.SettingsFile)
                        {
                            listContainsReadout = true;
                        }
                    }

                    if (!listContainsReadout)
                    {
                        this.AddReadout(currentLine);
                    }
                }
            }
            else
            {
                this._logger.Error("Unable to find specified readout list file.");
            }
        }

        public void SaveToFile()
        {
            if (this._writeNeeded)
            {
                this._logger.Log("Saving readout list file.");
                if (this.Readouts.Count > 0 && File.Exists<FlightReadoutManager>(this._readoutListFile))
                {
                    TextWriter file = File.CreateText<FlightReadoutManager>(this._readoutListFile);

                    foreach (FlightReadout readout in this.Readouts)
                    {
                        file.WriteLine(readout.Settings.SettingsFile);
                    }

                    file.Close();

                    this._writeNeeded = false;
                }
                else
                {
                    this._logger.Error("No readout list available to write to file.");
                }
            }
        }

        public void AddReadout(string readoutFileName = null)
        {
            this._logger.Log("Adding readout to list.");

            if (readoutFileName == null)
            {
                readoutFileName = StaticRandom.Next() + ".cfg";
            }

            FlightReadout newReadout = new FlightReadout(this.Computer, readoutFileName);
            this.Readouts.Add(newReadout);
        }

        public void RemoveReadout(FlightReadout readout)
        {
            this._logger.Log("Removing readout from list.");

            readout.DestroyReadout();
            this.Readouts.Remove(readout);
        }

        public void DrawReadoutButtons()
        {
            foreach (FlightReadout readout in this.Readouts)
            {
                if (GUILayout.Button(readout.GetReadoutName()))
                {
                    readout.ToggleReadout();
                }
            }
        }

        public void DrawReadoutManagementGUI()
        {
            GUILayout.BeginVertical();
            {
                foreach (FlightReadout readout in this.Readouts)
                {
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    {
                        readout.SetReadoutName(GUILayout.TextField(readout.GetReadoutName(), 50));

                        if (GUILayout.Button("Remove"))
                        {
                            this.RemoveReadout(readout);
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Add New Readout"))
                    {
                        this.AddReadout();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
