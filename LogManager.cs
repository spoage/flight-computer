using UnityEngine;

namespace FlightComputer
{
    public class LogManager
    {
        public string SubSection = null;
        public string UniqueIdentifier = null;

        public LogManager(string subSection = null, string uniqueIdentifier = null)
        {
            this.SubSection = subSection;
            this.UniqueIdentifier = uniqueIdentifier;
        }

        public string GetUniqueID()
        {
            if (this.UniqueIdentifier == null)
            {
                return "";
            }
            
            return "[" + this.UniqueIdentifier + "]";
        }

        public void Error(string message)
        {
            Debug.LogError("[FlightComputer " + this.SubSection + "]" + this.GetUniqueID() + " " + message);
        }

        public void Log(string message)
        {
            if (FlightComputer.Debug)
            {
                Debug.Log("[FlightComputer " + this.SubSection + "]" + this.GetUniqueID() + " " + message);
            }
        }

        // Just like Log() but ignores the debugging flag.
        public void Notice(string message)
        {
            Debug.Log("[FlightComputer " + this.SubSection + "]" + this.GetUniqueID() + " " + message);
        }
    }
}
