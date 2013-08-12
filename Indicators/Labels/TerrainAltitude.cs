namespace FlightComputer.Indicators.Labels
{
    public class TerrainAltitude : SIReadout
    {
        public static string TypeIdentifier = "ALTITUDE_TERRAIN";

        protected override string Label { get { return "Altitude (Terrain)"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetTerrainAltitude(); }
        }

        public TerrainAltitude(FlightReadout readout)
            : base(readout, FlightComputer.SIUnitType.Distance) { }
    }
}
