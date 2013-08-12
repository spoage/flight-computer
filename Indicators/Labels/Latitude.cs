namespace FlightComputer.Indicators.Labels
{
    public class Latitude : AngleReadout
    {
        public static string TypeIdentifier = "LATITUDE";

        protected override string Label { get { return "Latitude"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetLatitude(); }
        }

        public Latitude(FlightReadout readout)
            : base(readout) { }
    }
}