using System;
using UnityEngine;

namespace FlightComputer
{
    public delegate double RawIndicatorValueProcessor(Vessel vessel);

    class SIReadoutLabel : FlightReadoutLabel
    {
        private FlightComputer.SIUnitType _units;

        public SIReadoutLabel(FlightReadout readout, string label, FlightComputer.SIUnitType unit, RawIndicatorValueProcessor valueCallback)
            : base(readout, label, valueCallback)
        {
            this._units = unit;
        }

        public override string GetIndicatorValue()
        {
            return FlightComputer.FormatSI(this.UpdateLabelValue(), _units);
        }
    }

    class TimeReadoutLabel : FlightReadoutLabel
    {
        public TimeReadoutLabel(FlightReadout readout, string label, RawIndicatorValueProcessor valueCallback)
            : base(readout, label, valueCallback) { }

        public override string GetIndicatorValue()
        {
            return FlightComputer.FormatTime(this.UpdateLabelValue());
        }
    }

    class AngleReadoutLabel : FlightReadoutLabel
    {
        public AngleReadoutLabel(FlightReadout readout, string label, RawIndicatorValueProcessor valueCallback)
            : base(readout, label, valueCallback) { }

        public override string GetIndicatorValue()
        {
            return Math.Round(this.UpdateLabelValue(), 6) + "°";
        }
    }

    class PercentageReadoutLabel : FlightReadoutLabel
    {
        public PercentageReadoutLabel(FlightReadout readout, string label, RawIndicatorValueProcessor valueCallback)
            : base(readout, label, valueCallback) { }

        public override string GetIndicatorValue()
        {
            return Math.Round((this.UpdateLabelValue() * 100), 2) + "%";
        }
    }

    class GravityReadoutLabel : FlightReadoutLabel
    {
        public GravityReadoutLabel(FlightReadout readout, string label, RawIndicatorValueProcessor valueCallback)
            : base(readout, label, valueCallback) { }

        public override string GetIndicatorValue()
        {
            return Math.Round(this.UpdateLabelValue(), 3) + "g";
        }
    }

    public abstract class FlightReadoutLabel : FlightReadoutIndicator
    {
        private double _value;
        private string _label;
        private RawIndicatorValueProcessor _updateValueCallback;

        public abstract string GetIndicatorValue();

        protected FlightReadoutLabel(FlightReadout readout, string label, RawIndicatorValueProcessor valueCallback)
        {
            this.Readout = readout;
            this._label = label;
            this._updateValueCallback = valueCallback;
        }

        public static new FlightReadoutLabel Factory(FlightReadout readout, string identifier)
        {
            switch (identifier)
            {
                case "AP_HEIGHT":
                    return new SIReadoutLabel(
                        readout,
                        "Apoapsis Height",
                        FlightComputer.SIUnitType.Distance,
                        FlightComputer.GetApoapsisHeight
                    );
                case "TIME_TO_AP":
                    return new TimeReadoutLabel(
                        readout,
                        "Time to Apoapsis",
                        FlightComputer.GetPeriapsisHeight
                    );
                case "PE_HEIGHT":
                    return new SIReadoutLabel(
                        readout,
                        "Periapsis Height",
                        FlightComputer.SIUnitType.Distance,
                        FlightComputer.GetPeriapsisHeight
                    );
                case "TIME_TO_PE":
                    return new TimeReadoutLabel(
                        readout,
                        "Time to Periapsis",
                        FlightComputer.GetTimeToPeriapsis
                    );
                case "INCLINATION":
                    return new AngleReadoutLabel(
                        readout,
                        "Inclination",
                        FlightComputer.GetOrbitalInclination
                    );
                case "ECCENTRICITY":
                    return new AngleReadoutLabel(
                        readout,
                        "Eccentricity",
                        FlightComputer.GetOrbitalEccentricity
                    );
                case "ORBITAL_PERIOD":
                    return new TimeReadoutLabel(
                        readout,
                        "Orbital Period",
                        FlightComputer.GetOrbitalPeriod
                    );
                case "LAN":
                    return new AngleReadoutLabel(
                        readout,
                        "Longitude of AN",
                        FlightComputer.GetLongitudeOfAN
                    );
                case "LPE":
                    return new AngleReadoutLabel(
                        readout,
                        "Longitude of Pe",
                        FlightComputer.GetLongitudeOfPe
                    );
                case "SEMIMAJOR_AXIS":
                    return new SIReadoutLabel(
                        readout,
                        "Semi-Major Axis",
                        FlightComputer.SIUnitType.Distance,
                        FlightComputer.GetSemiMajorAxis
                    );
                case "SEMIMINOR_AXIS":
                    return new SIReadoutLabel(
                        readout,
                        "Semi-Minor Axis",
                        FlightComputer.SIUnitType.Distance,
                        FlightComputer.GetSemiMinorAxis
                    );
                case "ALTITUDE":
                    return new SIReadoutLabel(
                        readout,
                        "Altitude (Sea Level)",
                        FlightComputer.SIUnitType.Distance,
                        FlightComputer.GetSeaLevelAltitude
                    );
                case "ALTITUDE_TERRAIN":
                    return new SIReadoutLabel(
                        readout,
                        "Altitude (Terrain)",
                        FlightComputer.SIUnitType.Distance,
                        FlightComputer.GetTerrainAltitude
                    );
                case "SURFACE_SPEED_VERTICAL":
                    return new SIReadoutLabel(
                        readout,
                        "Surface Speed (Vertical)",
                        FlightComputer.SIUnitType.Speed,
                        FlightComputer.GetHorizontalSurfaceSpeed
                    );
                case "SURFACE_SPEED_HORIZONTAL":
                    return new SIReadoutLabel(
                        readout,
                        "Surface Speed (Horizontal)",
                        FlightComputer.SIUnitType.Speed,
                        FlightComputer.GetVerticalSurfaceSpeed
                    );
                case "LONGITUDE":
                    return new AngleReadoutLabel(
                        readout,
                        "Longitdue",
                        FlightComputer.GetLongitude
                    );
                case "LATITUDE":
                    return new AngleReadoutLabel(
                        readout,
                        "Latitude",
                        FlightComputer.GetLatitude
                    );
                case "GFORCE":
                    return new GravityReadoutLabel(
                        readout,
                        "G-Force",
                        FlightComputer.GetGForce
                    );
                case "TERMINAL_VELOCITY":
                    return new SIReadoutLabel(
                        readout,
                        "Terminal Velocity",
                        FlightComputer.SIUnitType.Speed,
                        FlightComputer.GetTerminalVelocity
                    );
                case "ATMO_EFFICIENCY":
                    return new PercentageReadoutLabel(
                        readout,
                        "Atmospheric Efficiency",
                        FlightComputer.GetAtmosphericEfficiency
                    );
                case "ATMO_DRAG":
                    return new SIReadoutLabel(
                        readout,
                        "Atmospheric Drag",
                        FlightComputer.SIUnitType.Force,
                        FlightComputer.GetAtmosphericEfficiency
                    );
                case "ATMO_DENSITY":
                    return new SIReadoutLabel(
                        readout,
                        "Atmospheric Pressure",
                        FlightComputer.SIUnitType.Density,
                        FlightComputer.GetAtmosphericEfficiency
                    );
                case "VESSEL_DELTAV_STAGE":
                    return new SIReadoutLabel(
                        readout,
                        "Atmospheric Pressure",
                        FlightComputer.SIUnitType.Density,
                        FlightComputer.GetAtmosphericEfficiency
                    );
                default:
                    return null;
            }
        }

        public double UpdateLabelValue()
        {
            this._value = this._updateValueCallback(Readout.ReadoutVessel);

            return this._value;
        }

        public override void Render(GUIStyle style = null)
        {
            if (style == null)
            {
                style = this.GetIndicatorStyle();
            }

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            // Left-align the label.
            GUIStyle labelStyle = new GUIStyle(style);
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(this._label, labelStyle);

            // Right-align the value and highlight it in a different color for contrast.
            GUIStyle valueStyle = new GUIStyle(style);
            valueStyle.alignment = TextAnchor.MiddleRight;
            GUILayout.Label(this.GetIndicatorValue(), valueStyle, GUILayout.ExpandWidth(true));

            GUILayout.EndHorizontal();
        }

        protected override GUIStyle GetIndicatorStyle()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.wordWrap = false;
            labelStyle.fontSize = this.Readout.ReadoutSettings.Get("FONT_SIZE", 12);

            int rowPadding = this.Readout.ReadoutSettings.Get("ROW_PADDING", 0);
            labelStyle.padding = new RectOffset(rowPadding, rowPadding, rowPadding, rowPadding);

            return labelStyle;
        }
    }
}
