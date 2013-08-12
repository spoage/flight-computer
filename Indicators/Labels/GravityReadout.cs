using System;

namespace FlightComputer.Indicators.Labels
{
    public abstract class GravityReadout : ReadoutLabel
    {
        protected override string Value { get { return Math.Round(this.RawValue, 3) + "g"; } }
        protected virtual double RawValue { get { return 0; } }

        protected GravityReadout(FlightReadout readout)
            : base(readout) { }
    }
}
