namespace FlightComputer.Indicators.Labels
{
    public class TerminalVelocity : SIReadout
    {
        public static string TypeIdentifier = "TERMINAL_VELOCITY";

        protected override string Label { get { return "Terminal Velocity"; } }
        protected override double RawValue
        {
            get { return this.Readout.Computer.VesselComputer.GetTerminalVelocity(); }
        }

        public TerminalVelocity(FlightReadout readout)
            : base(readout, FlightComputer.SIUnitType.Speed) { }
    }
}
