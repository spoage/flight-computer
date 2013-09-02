namespace FlightComputer.Indicators.Labels
{
    public class TimeToApoapsis : TimeReadout
    {
        public static string TypeIdentifier = "TIME_TO_AP";

        protected override string Label { get { return "Time to Apoapsis"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetTimeToApoapsis(); }
        }

        public TimeToApoapsis(FlightReadout readout)
            : base(readout) { }
    }
}
