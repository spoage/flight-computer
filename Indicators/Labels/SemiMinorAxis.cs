namespace FlightComputer.Indicators.Labels
{
    public class SemiMinorAxis : SIReadout
    {
        public static string TypeIdentifier = "SEMIMINOR_AXIS";

        protected override string Label { get { return "Semi-Minor Axis"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetSemiMinorAxis(); }
        }

        public SemiMinorAxis(FlightReadout readout)
            : base(readout, FlightComputer.SIUnitType.Distance) { }
    }
}
