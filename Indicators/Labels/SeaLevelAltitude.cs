namespace FlightComputer.Indicators.Labels
{
    public class SeaLevelAltitude : SIReadout
    {
        public static string TypeIdentifier = "ALTITUDE";

        protected override string Label { get { return "Altitude (Sea Level)"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetSeaLevelAltitude(); }
        }

        public SeaLevelAltitude(FlightReadout readout)
            : base(readout, FlightComputer.SIUnitType.Distance) { }
    }
}
