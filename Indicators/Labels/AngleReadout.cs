using System;

namespace FlightComputer.Indicators.Labels
{
    public abstract class AngleReadout : ReadoutLabel
    {
        protected override string Value { get { return Math.Round(this.RawValue, 6) + "°"; } }
        protected virtual double RawValue { get { return 0; } }

        protected AngleReadout(FlightReadout readout)
            : base(readout) { }
    }
}
