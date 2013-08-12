namespace FlightComputer.Indicators.Labels
{
    public class TimeToPeriapsis : TimeReadout
    {
        public static string TypeIdentifier = "TIME_TO_PE";

        protected override string Label { get { return "Time to Periapsis"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetTimeToPeriapsis(); }
        }

        public TimeToPeriapsis(FlightReadout readout)
            : base(readout) { }
    }
}
