namespace FlightComputer.Indicators.Labels
{
    public class Longitude : AngleReadout
    {
        public static string TypeIdentifier = "LONGITUDE";

        protected override string Label { get { return "Longitude"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetLongitude(); }
        }

        public Longitude(FlightReadout readout)
            : base(readout) { }
    }
}