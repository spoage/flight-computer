using UnityEngine;
using FlightComputer.Indicators;

namespace FlightComputer
{
    public abstract class FlightReadoutIndicator
    {
        protected FlightReadout Readout;

        abstract public void Render();

        public static FlightReadoutIndicator Factory(FlightReadout readout, string identifier)
        {
            string indicatorTypeIdentifier = readout.Settings.Get<string>(identifier);

            if (indicatorTypeIdentifier.StartsWith("LABEL_"))
            {
                string indicatorLabelType = indicatorTypeIdentifier.Split(new char[] { '_' }, 2)[1];
                return ReadoutLabel.Factory(readout, indicatorLabelType);
            }

            if (indicatorTypeIdentifier == "SEPARATOR")
            {
                return new ReadoutSeparator(readout);
            }

            return null;
        }

        protected virtual GUIStyle GetIndicatorStyle()
        {
            return this.GetIndicatorStyle(new GUIStyle());
        }

        protected GUIStyle GetIndicatorStyle(GUIStyle style)
        {
            style.fontSize = this.Readout.Settings.Get("FONT_SIZE", 12);

            int padding = this.Readout.Settings.Get("ROW_PADDING", 0);
            style.padding = new RectOffset(padding, padding, padding, padding);

            return style;
        }
    }
}
