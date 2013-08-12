namespace FlightComputer.Indicators.Labels
{
    public class HorizontalSurfaceSpeed : SIReadout
    {
        public static string TypeIdentifier = "SURFACE_SPEED_HORIZONTAL";
        
        protected override string Label { get { return "Surface Speed (Horizontal)"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetHorizontalSurfaceSpeed(); }
        }

        public HorizontalSurfaceSpeed(FlightReadout readout)
            : base(readout, FlightComputer.SIUnitType.Speed) { }
    }
}
