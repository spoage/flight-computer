using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace FlightComputer
{
    public class FlightComputer : PartModule
    {
        public static bool Debug = true;
        public const int ControllerWindowHeight = 30;
        public enum SIUnitType { Speed, Distance, Pressure, Density, Force, Mass };

        public SettingsManager Settings;

        private LogManager _logger;
        private Rect _controllerWindowPosition;
        private Rect _settingsWindowPosition;
        private Dictionary<string, Texture> _textures;
        private bool _showSettings;
        private bool _panelCollapsed;

        private int _controllerWindowId;
        private int _settingsWindowId;
        private List<FlightReadout> _readoutPanels = new List<FlightReadout>();

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
                        "KFC - Global Settings",
                        GUILayout.MinWidth(100),
                        GUILayout.ExpandWidth(true)
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
            this._textures = new Dictionary<string, Texture>();

            Texture2D texture1 = new Texture2D(128, 128);
            texture1.LoadImage(KSP.IO.File.ReadAllBytes<FlightReadout>("gear.png"));
            this._textures.Add("SETTINGS", texture1);

            Texture2D texture2 = new Texture2D(128, 128);
            texture2.LoadImage(KSP.IO.File.ReadAllBytes<FlightReadout>("arrow_left.png"));
            this._textures.Add("PANEL_COLLAPSE_LEFT", texture2);

            Texture2D texture3 = new Texture2D(128, 128);
            texture3.LoadImage(KSP.IO.File.ReadAllBytes<FlightReadout>("arrow_right.png"));
            this._textures.Add("PANEL_COLLAPSE_RIGHT", texture3);
        }

        private void DrawControllerGUI(int windowId)
        {
            GUILayout.BeginHorizontal();

            if (this._panelCollapsed)
            {
                this.DrawCollapseButton();
            }
            else
            {
                this.DrawCollapseButton();
                this.DrawSettingsButton();
                this.DrawReadoutButtons();
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

            if (GUILayout.Button(this._textures[collapseTextureName], GUILayout.MaxWidth(ControllerWindowHeight), GUILayout.MaxHeight(ControllerWindowHeight)))
            {
                this._panelCollapsed = !this._panelCollapsed;

                // We set the window width to zero so that it has to recalculate the width of the window on
                // every frame, and resize accordingly.
                this._controllerWindowPosition.width = 0;
            }
        }

        private void DrawSettingsButton()
        {
            if (GUILayout.Button(this._textures["SETTINGS"], GUILayout.MaxWidth(ControllerWindowHeight), GUILayout.MaxHeight(ControllerWindowHeight)))
            {
                this._showSettings = !this._showSettings;
            }
        }

        private void DrawReadoutButtons()
        {
            foreach (FlightReadout readout in this._readoutPanels)
            {
                if (GUILayout.Button(readout.GetReadoutName()))
                {
                    readout.ToggleReadout();
                }
            }
        }

        /////////////////////////////
        // Build the settings GUI. //
        /////////////////////////////

        private void DrawSettingsGUI(int windowId)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Test text, please ignore.");

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

                // Clear the flight readout list in case we're re-initializing from an earlier instance.
                this._readoutPanels.Clear();

                // Set up the window position for the controller panel.
                this._controllerWindowId = StaticRandom.Next();
                this._controllerWindowPosition = new Rect(
                    this.Settings.Get("POSITION_X", 200),
                    this.Settings.Get("POSITION_Y", 200),
                    0,
                    0
                );
                this._logger.Log("Initialized flight readout controller window. Window ID: " + this._controllerWindowId);
                this._logger.Log("Window Position: " + this._controllerWindowPosition);

                // Set up the window position for the settings window.
                this._settingsWindowId = StaticRandom.Next();
                this._settingsWindowPosition = new Rect(
                    this.Settings.Get("SETTINGS_POSITION_X", 200),
                    this.Settings.Get("SETTINGS_POSITION_Y", 200),
                    0,
                    0
                );
                this._logger.Log("Initialized settings window. Window ID: " + this._settingsWindowId);
                this._logger.Log("Window Position: " + this._settingsWindowPosition);

                // Initialize the flight readouts
                this._logger.Log("Building list of flight readouts.");
                foreach (KeyValuePair<string, string> setting in this.Settings)
                {
                    if (setting.Key.StartsWith("READOUT_"))
                    {
                        try
                        {
                            FlightReadout readout = new FlightReadout(this, this.vessel, setting.Value.Trim());
                            this._readoutPanels.Add(readout);
                        }
                        catch (Exception)
                        {
                            this._logger.Error("Unable to load readout data. File: " + setting.Value);

                            if (FlightComputer.Debug)
                            {
                                throw;
                            }
                        }
                    }
                }
                this._logger.Log("Readouts initialized and added.");

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

            this._readoutPanels.ForEach(readout => readout.DestroyReadout());
            this._readoutPanels.Clear();

            // Flush the settings out that might have changed.
            this.Settings.SaveToFile();
        }

        //////////////////////////////////
        // Library stuff all down here. //
        //////////////////////////////////

        // Various flight calculations.
        public static double GetCurrentGForces(Vessel vessel)
        {
            return vessel.geeForce;
        }

        // Orbital data calculations.
        public static double GetApoapsisHeight(Vessel vessel)
        {
            return vessel.orbit.ApA;
        }

        public static double GetTimeToApoapsis(Vessel vessel)
        {
            return vessel.orbit.timeToAp;
        }

        public static double GetPeriapsisHeight(Vessel vessel)
        {
            return vessel.orbit.PeA;
        }

        public static double GetTimeToPeriapsis(Vessel vessel)
        {
            return vessel.orbit.timeToPe;
        }

        public static double GetOrbitalInclination(Vessel vessel)
        {
            return vessel.orbit.inclination;
        }

        public static double GetOrbitalEccentricity(Vessel vessel)
        {
            return vessel.orbit.eccentricity;
        }

        public static double GetOrbitalPeriod(Vessel vessel)
        {
            return vessel.orbit.period;
        }

        public static double GetLongitudeOfAN(Vessel vessel)
        {
            return vessel.orbit.LAN;
        }

        public static double GetLongitudeOfPe(Vessel vessel)
        {
            return vessel.orbit.LAN + vessel.orbit.argumentOfPeriapsis;
        }

        public static double GetSemiMajorAxis(Vessel vessel)
        {
            return vessel.orbit.semiMajorAxis;
        }

        public static double GetSemiMinorAxis(Vessel vessel)
        {
            return vessel.orbit.semiMinorAxis;
        }

        // Surface flight calculations.
        public static double GetSeaLevelAltitude(Vessel vessel)
        {
            return vessel.mainBody.GetAltitude(vessel.CoM);
        }

        public static double GetTerrainAltitude(Vessel vessel)
        {
            return FlightComputer.GetSeaLevelAltitude(vessel) - vessel.terrainAltitude;
        }

        public static double GetHorizontalSurfaceSpeed(Vessel vessel)
        {
            return vessel.verticalSpeed;
        }

        public static double GetVerticalSurfaceSpeed(Vessel vessel)
        {
            return vessel.horizontalSrfSpeed;
        }

        public static double GetLongitude(Vessel vessel)
        {
            return vessel.longitude;
        }

        public static double GetLatitude(Vessel vessel)
        {
            return vessel.latitude;
        }

        public static double GetGForce(Vessel vessel)
        {
            return vessel.geeForce;
        }

        // Aerodynamic calculations.
        public static double GetTerminalVelocity(Vessel vessel)
        {
            if (vessel.atmDensity <= 0)
            {
                return 0;
            }

            return Math.Sqrt(
                (2 * FlightComputer.GetTotalPartMass(vessel) * FlightGlobals.getGeeForceAtPosition(vessel.CoM).magnitude)
                / (vessel.atmDensity * FlightComputer.GetTotalPartDrag(vessel) * FlightGlobals.DragMultiplier)
            );
        }

        public static double GetAtmosphericEfficiency(Vessel vessel)
        {
            double terminalVelocity = FlightComputer.GetTerminalVelocity(vessel);
            if (terminalVelocity <= 0)
            {
                return 0;
            }

            return FlightGlobals.ship_srfSpeed / terminalVelocity;
        }

        public static double GetAtmosphericDragForce(Vessel vessel)
        {
            return 0.5 * vessel.atmDensity * Math.Pow(FlightGlobals.ship_srfSpeed, 2) * FlightComputer.GetTotalPartDrag(vessel) * FlightGlobals.DragMultiplier;
        }

        // Craft calculations.
        public static double GetTotalPartMass(Vessel vessel)
        {
            double totalMass = 0;
            foreach (Part part in vessel.parts)
            {
                totalMass += part.mass + part.GetResourceMass();
            }

            return totalMass;
        }

        public static double GetTotalPartDrag(Vessel vessel)
        {
            double totalDrag = 0;
            foreach (Part part in vessel.parts)
            {
                totalDrag += (part.mass + part.GetResourceMass()) * part.maximum_drag;
            }

            return totalDrag;
        }

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