namespace FlightComputer.Indicators.Labels
{
    public class LongitudePe : AngleReadout
    {
        public static string TypeIdentifier = "LPE";

        protected override string Label { get { return "Longitude of Pe"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetLongitudeOfPe(); }
        }

        public LongitudePe(FlightReadout readout)
            : base(readout) { }
    }
}