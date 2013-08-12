namespace FlightComputer.Indicators.Labels
{
    public abstract class TimeReadout : ReadoutLabel
    {
        protected override string Value { get { return FlightComputer.FormatTime(this.RawValue); } }
        protected virtual double RawValue { get { return 0; } }

        protected TimeReadout(FlightReadout readout)
            : base(readout) { }
    }
}