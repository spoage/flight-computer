using UnityEngine;

namespace FlightComputer
{
    public abstract class FlightReadoutIndicator
    {
        protected FlightReadout Readout;

        abstract public void Render(GUIStyle style = null);

        public static FlightReadoutIndicator Factory(FlightReadout readout, string identifier)
        {
            string indicatorTypeIdentifier = readout.ReadoutSettings.Get<string>(identifier);

            if (indicatorTypeIdentifier.StartsWith("LABEL_"))
            {
                string indicatorLabelType = indicatorTypeIdentifier.Split(new char[] { '_' }, 2)[1];
                return FlightReadoutLabel.Factory(readout, indicatorLabelType);
            }
            
            if (indicatorTypeIdentifier == "SEPARATOR")
            {
                return new FlightReadoutSeparator(readout);
            }

            return null;
        }

        protected virtual GUIStyle GetIndicatorStyle()
        {
            return this.ApplyBaseIndicatorStyle(new GUIStyle());
        }

        protected virtual GUIStyle ApplyBaseIndicatorStyle(GUIStyle style)
        {
            style.normal.textColor = style.focused.textColor = Color.white;
            style.hover.textColor = style.active.textColor = Color.yellow;
            style.onNormal.textColor = style.onFocused.textColor = style.onHover.textColor = style.onActive.textColor = Color.green;

            return style;
        }
    }
}
