using System.Collections.Generic;
using UnityEngine;

namespace FlightComputer
{
    public class ReadoutIndicatorManager : SettingListManager
    {
        public static bool Debug = true;

        public FlightReadout Readout;
        public List<FlightReadoutIndicator> Indicators = new List<FlightReadoutIndicator>();

        public ReadoutIndicatorManager(FlightReadout readout, string filename = null)
        {
            this.Logger = new LogManager("IndicatorManager", filename);
            this.SettingsFile = filename;
            this.Readout = readout;
            this.LoadFromFile();
        }

        ~ReadoutIndicatorManager()
        {
            this.DestroyList();
        }

        public void DestroyList()
        {
            this.SaveToFile();
            this.Indicators.Clear();
        }

        public override List<string> GetLinesToWrite()
        {
            List<string> lines = new List<string>();
            foreach (FlightReadoutIndicator indicator in this.Indicators)
            {
                lines.Add((string)indicator.GetType().GetField("TypeIdentifier").GetValue(null));
            }
            return lines;
        }

        public override void AddItem(string indicatorType = null)
        {
            this.Logger.Log("Adding indicator to list: " + indicatorType);

            FlightReadoutIndicator indicator = FlightReadoutIndicator.Factory(this.Readout, indicatorType);
            if (indicator != null)
            {
                if (!this.Indicators.Contains(indicator))
                {
                    this.Indicators.Add(indicator);
                }
            }
            else
            {
                this.Logger.Error("Unable to find indicator for identifier: " + indicatorType);
            }
        }

        public void RemoveItem(string indicatorType = null)
        {
            foreach (FlightReadoutIndicator indicator in this.Indicators)
            {
                this.RemoveItem((string)indicator.GetType().GetField("TypeIdentifier").GetValue(null));
            };
        }

        public void RemoveItem(FlightReadoutIndicator readout)
        {
            this.Logger.Log("Removing indicator from list.");

            this.Indicators.Remove(readout);
        }

        public void GetIndicatorDisplay()
        {
            GUILayout.BeginVertical();
            {
                foreach (FlightReadoutIndicator indicator in this.Indicators)
                {
                    indicator.Render();
                }
            }
            GUILayout.EndVertical();
        }
    }
}
