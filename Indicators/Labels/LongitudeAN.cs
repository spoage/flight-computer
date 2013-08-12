namespace FlightComputer.Indicators.Labels
{
    public class LongitudeAN : AngleReadout
    {
        public static string TypeIdentifier = "LAN";

        protected override string Label { get { return "Longitude of AN"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetLongitudeOfAN(); }
        }

        public LongitudeAN(FlightReadout readout)
            : base(readout) { }
    }
}