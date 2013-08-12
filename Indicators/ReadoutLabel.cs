using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace FlightComputer.Indicators
{
    public abstract class ReadoutLabel : FlightReadoutIndicator
    {
        private static Dictionary<string, Type> _labelTypes;

        protected virtual string Label { get { return null; } }
        protected virtual string Value { get { return null; } }

        protected ReadoutLabel(FlightReadout readout)
        {
            this.Readout = readout;
        }

        public static new ReadoutLabel Factory(FlightReadout readout, string identifier)
        {
            if (ReadoutLabel._labelTypes == null)
            {
                Type baseLabelType = Type.GetType("FlightComputer.Indicators.ReadoutLabel", true);

                // Iterate through all assemblies accessible to the application and grab the ones
                // that correspond to the FlightComputer plugin. This should (in theory, though this
                // is untested currently) allow extension of the flight computer via external DLLs.
                IEnumerable<Type> flightComputerLabelTypes = Enumerable.Empty<Type>();
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    flightComputerLabelTypes = flightComputerLabelTypes
                        .Concat(assembly.GetTypes().Where(t => baseLabelType != null && (baseLabelType.IsAssignableFrom(t) && !t.IsAbstract)));
                }

                ReadoutLabel._labelTypes = new Dictionary<string, Type>();

                foreach (Type labelType in flightComputerLabelTypes)
                {
                    Debug.Log(labelType);
                    Debug.Log(labelType.GetField("TypeIdentifier"));
                    Debug.Log(labelType.GetField("TypeIdentifier").GetValue(null));


                    string labelTypeIdentifier = (string)labelType.GetField("TypeIdentifier").GetValue(null);
                    Debug.Log(labelTypeIdentifier);

                    ReadoutLabel._labelTypes.Add(labelTypeIdentifier, labelType);
                }
            }

            if (ReadoutLabel._labelTypes.ContainsKey(identifier))
            {
                return (ReadoutLabel)Activator.CreateInstance(ReadoutLabel._labelTypes[identifier], new object[] {readout});
            }

            return null;
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

        protected override GUIStyle GetIndicatorStyle()
        {
            GUIStyle style = base.GetIndicatorStyle(GUI.skin.label);
            style.wordWrap = false;

            return style;
        }
    }
}
