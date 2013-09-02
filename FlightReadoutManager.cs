using System.Collections.Generic;
using UnityEngine;

namespace FlightComputer
{
    public class FlightReadoutManager : SettingListManager
    {
        public static bool Debug = true;

        public FlightComputer Computer;
        public List<FlightReadout> Readouts = new List<FlightReadout>();

        public FlightReadoutManager(FlightComputer computer, string filename = null)
        {
            this.Logger = new LogManager("ReadoutManager", filename);
            this.SettingsFile = filename;
            this.Computer = computer;
            this.LoadFromFile();
        }

        ~FlightReadoutManager()
        {
            this.SaveToFile();
            this.Readouts.Clear();
        }

        public override List<string> GetLinesToWrite()
        {
            List<string> lines = new List<string>();
            foreach (FlightReadout readout in this.Readouts)
            {
                lines.Add(readout.Settings.SettingsFile);
            }

            return lines;
        }

        public override void AddItem(string readoutFileName = null)
        {
            this.Logger.Log("Adding readout to list.");

            if (readoutFileName == null)
            {
                readoutFileName = StaticRandom.Next() + ".cfg";
            }

            bool listContainsReadout = false;
            foreach (FlightReadout readout in this.Readouts)
            {
                if (readoutFileName == readout.Settings.SettingsFile)
                {
                    listContainsReadout = true;
                }
            }

            if (!listContainsReadout)
            {
                FlightReadout newReadout = new FlightReadout(this.Computer, readoutFileName);
                this.Readouts.Add(newReadout);
            }
        }

        public void RemoveItem(string readoutFileName)
        {
            foreach (FlightReadout readout in this.Readouts)
            {
                if (readoutFileName == readout.Settings.SettingsFile)
                {
                    this.Readouts.Remove(readout);
                    return;
                }
            }
        }

        public void RemoveItem(FlightReadout readout)
        {
            this.Logger.Log("Removing readout from list.");

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
                            this.RemoveItem(readout);
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Add New Readout"))
                    {
                        this.AddItem();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
