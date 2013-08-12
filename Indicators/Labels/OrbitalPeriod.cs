namespace FlightComputer.Indicators.Labels
{
    public class OrbitalPeriod : TimeReadout
    {
        public static string TypeIdentifier = "ORBITAL_PERIOD";

        protected override string Label { get { return "Orbital Period"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetOrbitalPeriod(); }
        }

        public OrbitalPeriod(FlightReadout readout)
            : base(readout) { }
    }
}
