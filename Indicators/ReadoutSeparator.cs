using UnityEngine;

namespace FlightComputer.Indicators
{
    public class ReadoutSeparator : FlightReadoutIndicator
    {
        public ReadoutSeparator(FlightReadout readout)
        {
            this.Readout = readout;
        }

        public override void Render()
        {
            GUIStyle style = this.GetIndicatorStyle();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(15 * style.fontSize);
            }
            GUILayout.EndHorizontal();
        }
    }
}