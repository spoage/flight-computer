namespace FlightComputer.Indicators.Labels
{
    public class PeriapsisHeight : SIReadout
    {
        public static string TypeIdentifier = "PE_HEIGHT";

        protected override string Label { get { return "Periapsis Height"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetPeriapsisHeight(); }
        }

        public PeriapsisHeight(FlightReadout readout)
            : base(readout, FlightComputer.SIUnitType.Distance) { }
    }
}
