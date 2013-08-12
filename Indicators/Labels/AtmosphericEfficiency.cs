namespace FlightComputer.Indicators.Labels
{
    public class AtmosphericEfficiency : PercentageReadout
    {
        public static string TypeIdentifier = "ATMO_EFFICIENCY";

        protected override string Label { get { return "Atmospheric Efficiency"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetAtmosphericEfficiency(); }
        }

        public AtmosphericEfficiency(FlightReadout readout)
            : base(readout) { }
    }
}
