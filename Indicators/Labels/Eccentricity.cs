namespace FlightComputer.Indicators.Labels
{
    public class Eccentricity : AngleReadout
    {
        public static string TypeIdentifier = "ECCENTRICITY";

        protected override string Label { get { return "Eccentricity"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetOrbitalEccentricity(); }
        }

        public Eccentricity(FlightReadout readout)
            : base(readout) { }
    }
}