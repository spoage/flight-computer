namespace FlightComputer.Indicators.Labels
{
    public class GForce : GravityReadout
    {
        public static string TypeIdentifier = "GFORCE";

        protected override string Label { get { return "G-Force"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetGForce(); }
        }

        public GForce(FlightReadout readout)
            : base(readout) { }
    }
}
