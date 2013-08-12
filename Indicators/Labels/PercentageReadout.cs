using System;

namespace FlightComputer.Indicators.Labels
{
    public abstract class PercentageReadout : ReadoutLabel
    {
        protected override string Value { get { return Math.Round((this.RawValue * 100), 2) + "%"; } }
        protected virtual double RawValue { get { return 0; } }

        protected PercentageReadout(FlightReadout readout)
            : base(readout) { }
    }
}