namespace FlightComputer.Indicators.Labels
{
    public class ApoapsisHeight : SIReadout
    {
        public static string TypeIdentifier = "AP_HEIGHT";

        protected override string Label { get { return "Apoapsis Height"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetApoapsisHeight(); }
        }

        public ApoapsisHeight(FlightReadout readout)
            : base(readout, FlightComputer.SIUnitType.Distance) { }
    }
}
