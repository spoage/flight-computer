using System.Collections.Generic;
using UnityEngine;

namespace FlightComputer
{
    public class FlightReadout
    {
        public FlightComputer Computer;
        public SettingsManager ReadoutSettings = null;
        public List<FlightReadoutIndicator> Indicators = new List<FlightReadoutIndicator>();

        private int _windowId;
        private Rect _windowPosition;
        private LogManager _logger;

        public FlightReadout(FlightComputer computer, string readoutConfigFile)
        {
            this.Computer = computer;

            this.ReadoutSettings = new SettingsManager(readoutConfigFile);

            this._logger = new LogManager("Readouts", this.GetReadoutName());
            this._logger.Log("Config loaded and logger initialized.");

            this._logger.Log("Initializing readout window position.");
            this._windowId = StaticRandom.Next();
            this._windowPosition = new Rect(
                this.ReadoutSettings.Get("POSITION_X", 50),
                this.ReadoutSettings.Get("POSITION_Y", 50),
                this.ReadoutSettings.Get("WIDTH", 250),
                this.ReadoutSettings.Get("HEIGHT", 100)
            );
            this._logger.Log("Initialized readout window position. Window ID: " + this._windowId);

            this._logger.Log("Building list of indicators for readout.");
            foreach (KeyValuePair<string, string> setting in this.ReadoutSettings)
            {
                if (setting.Key.StartsWith("INDICATOR_"))
                {
                    this._logger.Log("Attempting to load readout indicator. Type: " + setting.Value);
                    FlightReadoutIndicator indicator = FlightReadoutIndicator.Factory(this, setting.Key);
                    if (indicator != null)
                    {
                        this.Indicators.Add(indicator);
                        this._logger.Log("Successfully loaded readout indicator. Type: " + setting.Value);
                    }
                    else
                    {
                        this._logger.Error("Unable to load readout indicator. Type: " + setting.Value);
                    }
                }
            }
            this._logger.Log("Indicators initialized and added.");

            if (this.IsActive())
            {
                this.DisplayReadout();
            }
        }

        public void DestroyReadout()
        {
            this._logger.Log("Destroying readout.");

            this.Indicators.Clear();
            RenderingManager.RemoveFromPostDrawQueue(3, this.DrawGUI);

            // Flush the settings out that might have changed.
            this.ReadoutSettings.SaveToFile();
        }

        public void ToggleReadout()
        {
            if (this.IsActive())
            {
                this.HideReadout();
            }
            else
            {
                this.DisplayReadout();
            }
        }

        public void DisplayReadout()
        {
            this._logger.Log("Readout displayed.");

            RenderingManager.AddToPostDrawQueue(3, this.DrawGUI);
            this.ReadoutSettings.Set("ACTIVE", true);
        }

        public void HideReadout()
        {
            this._logger.Log("Readout hidden.");

            RenderingManager.RemoveFromPostDrawQueue(3, this.DrawGUI);
            this.ReadoutSettings.Set("ACTIVE", false);
        }

        public bool IsActive()
        {
            bool activityState = this.ReadoutSettings.Get("ACTIVE", false);

            this._logger.Log("Readout is active: " + activityState);
            return activityState;
        }

        public string GetReadoutName()
        {
            return this.ReadoutSettings.Get("READOUT_NAME", "Flight Readout " + this._windowId);
        }

        public void SetReadoutName(string newReadoutName)
        {
            this.ReadoutSettings.Set("READOUT_NAME", newReadoutName);
        }

        private void WindowGUI(int windowId)
        {
            GUILayout.BeginHorizontal(GUI.skin.textArea);
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
            GUILayout.EndHorizontal();

            if (!this.Computer.Settings.Get("LOCKED_POSITION", false))
            {
                GUI.DragWindow();
            }
        }

        private void DrawGUI()
        {
            if (this.Computer.vessel != null && this.Computer.vessel == FlightGlobals.ActiveVessel)
            {
                GUI.skin = HighLogic.Skin;

                this._windowPosition = GUILayout.Window(
                    this._windowId,
                    this._windowPosition,
                    this.WindowGUI,
                    this.GetReadoutName(),
                    GUILayout.MinWidth(100),
                    GUILayout.ExpandWidth(true)
                );

                // Set the values for where this window is being rendered.
                this.ReadoutSettings.Set("POSITION_X", (int)this._windowPosition.x);
                this.ReadoutSettings.Set("POSITION_Y", (int)this._windowPosition.y);
                this.ReadoutSettings.Set("WIDTH", (int)this._windowPosition.width);
                this.ReadoutSettings.Set("HEIGHT", (int)this._windowPosition.height);
            }
        }
    }
}
