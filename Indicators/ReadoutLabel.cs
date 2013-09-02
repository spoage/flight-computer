using UnityEngine;

namespace FlightComputer.Indicators
{
    public abstract class ReadoutLabel : FlightReadoutIndicator
    {
        protected virtual string Label { get { return null; } }
        protected virtual string Value { get { return null; } }

        protected ReadoutLabel(FlightReadout readout)
        {
            this.Readout = readout;
        }

        public override void Render()
        {
            GUIStyle style = this.GetIndicatorStyle();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                // Left-align the label.
                GUIStyle labelStyle = new GUIStyle(style);
                labelStyle.normal.textColor = Color.white;
                labelStyle.alignment = TextAnchor.MiddleLeft;
                GUILayout.Label(this.Label, labelStyle);

                // Right-align the value and highlight it in a different color for contrast.
                GUIStyle valueStyle = new GUIStyle(style);
                valueStyle.alignment = TextAnchor.MiddleRight;
                GUILayout.Label(this.Value, valueStyle, GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();
        }

        public override void RenderConfigRow()
        {
            GUILayout.Label("--------");
        }

        protected override GUIStyle GetIndicatorStyle()
        {
            GUIStyle style = base.GetIndicatorStyle(GUI.skin.label);
            style.wordWrap = false;

            return style;
        }
    }
}
