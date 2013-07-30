using UnityEngine;

namespace FlightComputer
{
    public class FlightReadoutSeparator : FlightReadoutIndicator
    {
        public FlightReadoutSeparator(FlightReadout readout)
        {
            this.Readout = readout;
        }

        public override void Render(GUIStyle style = null)
        {
            int fontSize = this.Readout.ReadoutSettings.Get("FONT_SIZE", 12);

            GUILayout.BeginHorizontal();
            GUILayout.Space(30 * (fontSize / 2));
            GUILayout.EndHorizontal();
        }
    }
}
