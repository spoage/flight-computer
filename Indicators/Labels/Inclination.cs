namespace FlightComputer.Indicators.Labels
{
    public class Inclination : AngleReadout
    {
        public static string TypeIdentifier = "INCLINATION";

        protected override string Label { get { return "Inclination"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetOrbitalInclination(); }
        }

        public Inclination(FlightReadout readout)
            : base(readout) { }
    }
}