using System.Collections.Generic;
using UnityEngine;

namespace FlightComputer
{
    public class FlightReadout
    {
        public FlightComputer Computer;
        public SettingsManager Settings = null;
        public List<FlightReadoutIndicator> Indicators = new List<FlightReadoutIndicator>();
        
        private bool _showSettings;
        private int _readoutWindowId;
        private int _settingsWindowId;
        private Rect _readoutWindowPosition;
        private Rect _settingsWindowPosition;
        private LogManager _logger;

        public FlightReadout(FlightComputer computer, string readoutConfigFile)
        {
            this.Computer = computer;

            this.Settings = new SettingsManager(readoutConfigFile);

            this._logger = new LogManager("Readouts", this.GetReadoutName());
            this._logger.Log("Config loaded and logger initialized.");

            this._logger.Log("Initializing readout window position.");
            this._readoutWindowId = StaticRandom.Next();
            this._readoutWindowPosition = new Rect(
                this.Settings.Get("POSITION_X", 50),
                this.Settings.Get("POSITION_Y", 50),
                0,
                0
            );
            this._logger.Log("Initialized readout window. Window ID: " + this._readoutWindowId);

            // Set up the window position for the settings window.
            this._settingsWindowId = StaticRandom.Next();
            this._settingsWindowPosition = new Rect(
                this.Settings.Get("SETTINGS_POSITION_X", 200),
                this.Settings.Get("SETTINGS_POSITION_Y", 200),
                170,
                70
            );
            this._logger.Log("Initialized readout settings window. Window ID: " + this._settingsWindowId);

            this._logger.Log("Building list of indicators for readout.");
            foreach (KeyValuePair<string, string> setting in this.Settings)
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
            this.Settings.SaveToFile();
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
            this.Settings.Set("ACTIVE", true);
        }

        public void HideReadout()
        {
            this._logger.Log("Readout hidden.");

            RenderingManager.RemoveFromPostDrawQueue(3, this.DrawGUI);
            this.Settings.Set("ACTIVE", false);
        }

        public bool IsActive()
        {
            bool activityState = this.Settings.Get("ACTIVE", false);

            this._logger.Log("Readout is active: " + activityState);
            return activityState;
        }

        public string GetReadoutName()
        {
            return this.Settings.Get("READOUT_NAME", "Flight Readout " + this._readoutWindowId);
        }

        public void SetReadoutName(string newReadoutName)
        {
            this.Settings.Set("READOUT_NAME", newReadoutName);
        }

        private void DrawGUI()
        {
            if (this.Computer.vessel != null && this.Computer.vessel == FlightGlobals.ActiveVessel)
            {
                GUI.skin = HighLogic.Skin;

                this._readoutWindowPosition = GUILayout.Window(
                    this._readoutWindowId,
                    this._readoutWindowPosition,
                    this.DrawReadout,
                    this.GetReadoutName(),
                    GUILayout.MinWidth(100),
                    GUILayout.ExpandWidth(true)
                );

                // Set the values for where this window is being rendered.
                this.Settings.Set("POSITION_X", (int)this._readoutWindowPosition.x);
                this.Settings.Set("POSITION_Y", (int)this._readoutWindowPosition.y);
                this.Settings.Set("WIDTH", (int)this._readoutWindowPosition.width);
                this.Settings.Set("HEIGHT", (int)this._readoutWindowPosition.height);

                if (this._showSettings)
                {
                    this._settingsWindowPosition = GUILayout.Window(
                        this._settingsWindowId,
                        this._settingsWindowPosition,
                        this.DrawReadoutSettings,
                        "Readout Settings - " + this.GetReadoutName()
                    );

                    // Set the values for where this window is being rendered.
                    this.Settings.Set("SETTINGS_POSITION_X", (int)this._settingsWindowPosition.x);
                    this.Settings.Set("SETTINGS_POSITION_Y", (int)this._settingsWindowPosition.y);
                }
            }
        }

        private void DrawReadout(int windowId)
        {
            GUIStyle readoutSettingsButtonStyle = new GUIStyle(GUI.skin.button);
            readoutSettingsButtonStyle.padding = new RectOffset(1, 1, 1, 1);

            if (GUI.Button(new Rect(this._readoutWindowPosition.width - 26, 6, 20, 20), FlightComputer.Textures["SETTINGS"], readoutSettingsButtonStyle))
            {
                this._showSettings = !this._showSettings;
            }

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

        private void DrawReadoutSettings(int windowId)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("blah blah");
            GUILayout.EndHorizontal();
        }
    }
}
