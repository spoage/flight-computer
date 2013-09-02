using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace FlightComputer
{
    public abstract class FlightReadoutIndicator
    {
        private static Dictionary<string, Type> _indicatorTypes;

        protected FlightReadout Readout;

        abstract public void Render();
        abstract public void RenderConfigRow();

        public static void ClearIndicatorTypes()
        {
            FlightReadoutIndicator._indicatorTypes = null;
        }

        public static FlightReadoutIndicator Factory(FlightReadout readout, string identifier)
        {
            if (FlightReadoutIndicator._indicatorTypes == null)
            {
                Type baseLabelType = Type.GetType("FlightComputer.FlightReadoutIndicator", true);

                // Iterate through all assemblies accessible to the application and grab the ones
                // that correspond to the FlightComputer plugin. This should (in theory, though this
                // is untested currently) allow extension of the flight computer via external DLLs.
                IEnumerable<Type> flightComputerIndicatorTypes = Enumerable.Empty<Type>();
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    flightComputerIndicatorTypes = flightComputerIndicatorTypes
                        .Concat(assembly.GetTypes().Where(t => baseLabelType != null && (baseLabelType.IsAssignableFrom(t) && !t.IsAbstract)));
                }

                FlightReadoutIndicator._indicatorTypes = new Dictionary<string, Type>();
                foreach (Type indicatorType in flightComputerIndicatorTypes)
                {
                    string indicatorTypeIdentifier = (string)indicatorType.GetField("TypeIdentifier").GetValue(null);

                    FlightReadoutIndicator._indicatorTypes.Add(indicatorTypeIdentifier, indicatorType);
                }
            }

            if (FlightReadoutIndicator._indicatorTypes.ContainsKey(identifier))
            {
                return (FlightReadoutIndicator)Activator.CreateInstance(FlightReadoutIndicator._indicatorTypes[identifier], new object[] { readout });
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
