namespace FlightComputer.Indicators.Labels
{
    public class VerticalSurfaceSpeed : SIReadout
    {
        public static string TypeIdentifier = "SURFACE_SPEED_VERTICAL";

        protected override string Label { get { return "Surface Speed (Vertical)"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetVerticalSurfaceSpeed(); }
        }

        public VerticalSurfaceSpeed(FlightReadout readout)
            : base(readout, FlightComputer.SIUnitType.Speed) { }
    }
}
