using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlightComputer
{
    public class FlightComputer : PartModule
    {
        public static bool Debug = true;
        public const int ControllerWindowHeight = 30;
        public enum SIUnitType { Speed, Distance, Pressure, Density, Force, Mass };
        public static Dictionary<string, Texture> Textures;

        public SettingsManager Settings;
        public FlightReadoutManager ReadoutManager;
        public VesselComputer VesselComputer;

        private bool _showSettings;
        private bool _panelCollapsed;
        private int _controllerWindowId;
        private int _settingsWindowId;
        private LogManager _logger;
        private Rect _controllerWindowPosition;
        private Rect _settingsWindowPosition;

        /////////////////////////
        // Build the base GUI. //
        /////////////////////////

        private void DrawGUI()
        {
            if (this.vessel != null && this.vessel == FlightGlobals.ActiveVessel)
            {
                GUI.skin = HighLogic.Skin;

                this._controllerWindowPosition = GUILayout.Window(
                    this._controllerWindowId,
                    this._controllerWindowPosition,
                    this.DrawControllerGUI,
                    this._panelCollapsed ? "KFC" : "Manage Flight Readouts",
                    GUILayout.MinWidth(FlightComputer.ControllerWindowHeight + 10),
                    GUILayout.ExpandWidth(true)
                );

                // Set the values for where this window is being rendered.
                this.Settings.Set("POSITION_X", (int)this._controllerWindowPosition.x);
                this.Settings.Set("POSITION_Y", (int)this._controllerWindowPosition.y);

                if (this._showSettings)
                {
                    this._settingsWindowPosition = GUILayout.Window(
                        this._settingsWindowId,
                        this._settingsWindowPosition,
                        this.DrawSettingsGUI,
                        "KFC - Global Settings"
                    );

                    // Set the values for where this window is being rendered.
                    this.Settings.Set("SETTINGS_POSITION_X", (int)this._settingsWindowPosition.x);
                    this.Settings.Set("SETTINGS_POSITION_Y", (int)this._settingsWindowPosition.y);
                }
            }
        }

        ///////////////////////////////
        // Build the controller GUI. //
        ///////////////////////////////

        private void LoadGUIAssets()
        {
            FlightComputer.Textures = new Dictionary<string, Texture>();

            Texture2D texture1 = new Texture2D(128, 128);
            texture1.LoadImage(KSP.IO.File.ReadAllBytes<FlightReadout>("gear.png"));
            FlightComputer.Textures.Add("SETTINGS", texture1);

            Texture2D texture2 = new Texture2D(128, 128);
            texture2.LoadImage(KSP.IO.File.ReadAllBytes<FlightReadout>("arrow_left.png"));
            FlightComputer.Textures.Add("PANEL_COLLAPSE_LEFT", texture2);

            Texture2D texture3 = new Texture2D(128, 128);
            texture3.LoadImage(KSP.IO.File.ReadAllBytes<FlightReadout>("arrow_right.png"));
            FlightComputer.Textures.Add("PANEL_COLLAPSE_RIGHT", texture3);
        }

        private void DrawControllerGUI(int windowId)
        {
            GUILayout.BeginHorizontal();

            this.DrawCollapseButton();
            if (!this._panelCollapsed)
            {
                this.DrawSettingsButton();
                this.ReadoutManager.DrawReadoutButtons();
            }

            GUILayout.EndHorizontal();

            if (!this.Settings.Get("LOCKED_POSITION", false))
            {
                GUI.DragWindow();
            }
        }

        private void DrawCollapseButton()
        {
            string collapseTextureName = this._panelCollapsed ? "PANEL_COLLAPSE_RIGHT" : "PANEL_COLLAPSE_LEFT";

            if (GUILayout.Button(FlightComputer.Textures[collapseTextureName], GUILayout.MaxWidth(ControllerWindowHeight), GUILayout.MaxHeight(ControllerWindowHeight)))
            {
                this._panelCollapsed = !this._panelCollapsed;

                // We set the window width to zero so that it has to recalculate the width of the window on
                // every frame, and resize accordingly.
                this._controllerWindowPosition.width = 0;
            }
        }

        private void DrawSettingsButton()
        {
            if (GUILayout.Button(FlightComputer.Textures["SETTINGS"], GUILayout.MaxWidth(ControllerWindowHeight), GUILayout.MaxHeight(ControllerWindowHeight)))
            {
                this._showSettings = !this._showSettings;
            }
        }

        /////////////////////////////
        // Build the settings GUI. //
        /////////////////////////////

        private void DrawSettingsGUI(int windowId)
        {
            GUILayout.BeginHorizontal();
            {
                this.Settings.Set("LOCKED_POSITION", GUILayout.Toggle(this.Settings.Get("LOCKED_POSITION", false), "Lock Panels"));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUI.skin.textArea);
            {
                this.ReadoutManager.DrawReadoutManagementGUI();
            }
            GUILayout.EndHorizontal();

            // We always allow the settings window to be dragged.
            GUI.DragWindow();
        }

        //////////////////////////////////////////////
        // The main application logic for the part. //
        //////////////////////////////////////////////

        ~FlightComputer()
        {
            UnityEngine.Debug.Log("Destroying flight computer instance.");

            this.CleanUp();
        }

        public override void OnStart(StartState state)
        {
            if (state != StartState.Editor && state != StartState.None)
            {
                // We use this to unify flight data calculations.
                this.VesselComputer = new VesselComputer(this.vessel);

                // Set up the central logging system.
                this._logger = new LogManager("Main");
                this._logger.Log("Initialized data log manager.");

                // Start loading up the rest of the plugin.
                this._logger.Log("Starting flight computer.");
                this.Settings = new SettingsManager();

                // Load the GUI assets.
                this.LoadGUIAssets();

                // Set up the debug flag from the config. We pass this along to everything.
                FlightComputer.Debug = this.Settings.Get("DEBUG", false);

                // Set up the window position for the controller panel.
                this._controllerWindowId = StaticRandom.Next();
                this._controllerWindowPosition = new Rect(
                    this.Settings.Get("POSITION_X", 200),
                    this.Settings.Get("POSITION_Y", 200),
                    0,
                    0
                );
                this._logger.Log("Initialized flight readout controller window. Window ID: " + this._controllerWindowId);

                // Set up the window position for the settings window.
                this._settingsWindowId = StaticRandom.Next();
                this._settingsWindowPosition = new Rect(
                    this.Settings.Get("SETTINGS_POSITION_X", 200),
                    this.Settings.Get("SETTINGS_POSITION_Y", 200),
                    170,
                    70
                );
                this._logger.Log("Initialized settings window. Window ID: " + this._settingsWindowId);

                // Initialize the flight readouts
                this._logger.Log("Building list of flight readouts.");
                this.ReadoutManager = new FlightReadoutManager(this);

                RenderingManager.AddToPostDrawQueue(3, this.DrawGUI);
            }
        }

        public override void OnSave(ConfigNode node)
        {
            UnityEngine.Debug.Log("KFC OnSave() invoked.");
        }

        public override void OnLoad(ConfigNode node)
        {
            UnityEngine.Debug.Log("KFC OnLoad() invoked.");
        }

        public override void OnInactive()
        {
            this._logger.Log("Part destroyed and/or inactive.");

            this.CleanUp();
        }

        private void CleanUp()
        {
            UnityEngine.Debug.Log("Cleaning up flight computer.");

            // Flush the settings out that might have changed.
            this.Settings.SaveToFile();
        }

        //////////////////////////////////
        // Library stuff all down here. //
        //////////////////////////////////

        // Standard utility method to format data in certain ways.
        public static string FormatSI(double number, SIUnitType type)
        {
            // Return a spacing string if number is 0;
            if ((int)number == 0)
            {
                return "-----";
            }

            // Assign memory for storing the notations.
            string[] notation = { "" };

            // Select the SIUnitType used and populate the notation array.
            switch (type)
            {
                case FlightComputer.SIUnitType.Distance:
                    notation = new string[] { "mm", "m", "km", "Mm", "Gm", "Tm", "Pm", "Em", "Zm", "Ym" };
                    number *= 1000;
                    break;
                case FlightComputer.SIUnitType.Speed:
                    notation = new string[] { "mm/s", "m/s", "km/s", "Mm/s", "Gm/s", "Tm/s", "Pm/s", "Em/s", "Zm/s", "Ym/s" };
                    number *= 1000;
                    break;
                case FlightComputer.SIUnitType.Pressure:
                    notation = new string[] { "Pa", "kPa", "MPa", "GPa", "TPa", "PPa", "EPa", "ZPa", "YPa" };
                    number *= 1000;
                    break;
                case FlightComputer.SIUnitType.Density:
                    notation = new string[] { "mg/m³", "g/m³", "kg/m³", "Mg/m³", "Gg/m³", "Tg/m³", "Pg/m³", "Eg/m³", "Zg/m³", "Yg/m³" };
                    number *= 1000000;
                    break;
                case FlightComputer.SIUnitType.Force:
                    notation = new string[] { "N", "kN", "MN", "GN", "TN", "PT", "EN", "ZN", "YN" };
                    number *= 1000;
                    break;
                case FlightComputer.SIUnitType.Mass:
                    notation = new string[] { "g", "kg", "Mg", "Gg", "Tg", "Pg", "Eg", "Zg", "Yg" };
                    number *= 1000;
                    break;
            }

            // Loop through the notations until the smallest usable one is found.
            int notationIndex;
            for (notationIndex = 0; notationIndex < notation.Length; notationIndex++)
            {
                if (Math.Abs(number) <= 1000)
                {
                    break;
                }

                number = number / 1000;
            }

            // Return a string of the concatinated number and selected notation.
            return number.ToString("0.000") + notation[notationIndex];
        }

        // Format a time string in a specific way.
        public static string FormatTime(double seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);

            int years = 0;
            if (time.Days > 365)
            {
                years = time.Days / 365;
                time = time.Subtract(TimeSpan.FromDays(years * 365));
            }

            string timeOutput;
            if (years > 0)
            {
                timeOutput = string.Format("{0:D}:{1:g}", years, time);
            }
            else
            {
                timeOutput = string.Format("{0:g}", time);
            }

            // This forces it to be truncated to only one decimal place.
            int timeOutputDecimalPlace = timeOutput.LastIndexOf('.');
            if (timeOutputDecimalPlace > -1)
            {
                timeOutput = timeOutput.Substring(0, timeOutputDecimalPlace + 2);
            }

            return timeOutput;
        }
    }
}