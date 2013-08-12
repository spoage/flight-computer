namespace FlightComputer.Indicators.Labels
{
    public class SemiMajorAxis : SIReadout
    {
        public static string TypeIdentifier = "SEMIMAJOR_AXIS";

        protected override string Label { get { return "Semi-Major Axis"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetSemiMajorAxis(); }
        }

        public SemiMajorAxis(FlightReadout readout)
            : base(readout, FlightComputer.SIUnitType.Distance) { }
    }
}
