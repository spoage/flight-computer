namespace FlightComputer.Indicators.Labels
{
    public class AtmosphericDragForce : SIReadout
    {
        public static string TypeIdentifier = "ATMO_DRAG";

        protected override string Label { get { return "Atmospheric Drag"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetAtmosphericDragForce(); }
        }

        public AtmosphericDragForce(FlightReadout readout)
            : base(readout, FlightComputer.SIUnitType.Force) { }
    }
}
