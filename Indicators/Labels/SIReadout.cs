namespace FlightComputer.Indicators.Labels
{
    public abstract class SIReadout : ReadoutLabel
    {
        protected override string Value { get { return FlightComputer.FormatSI(this.RawValue, this._units); } }
        protected virtual double RawValue { get { return 0; } }

        private FlightComputer.SIUnitType _units;

        protected SIReadout(FlightReadout readout, FlightComputer.SIUnitType unit) : base(readout)
        {
            this._units = unit;
        }
    }
}
