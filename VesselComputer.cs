using System;

namespace FlightComputer
{
    public class VesselComputer
    {
        public Vessel Vessel;

        public VesselComputer(Vessel trackedVessel)
        {
            this.Vessel = trackedVessel;
        }

        // Various flight calculations.
        public double GetCurrentGForces()
        {
            return this.Vessel.geeForce;
        }

        // Orbital data calculations.
        public double GetApoapsisHeight()
        {
            return this.Vessel.orbit.ApA;
        }

        public double GetTimeToApoapsis()
        {
            return this.Vessel.orbit.timeToAp;
        }

        public double GetPeriapsisHeight()
        {
            return this.Vessel.orbit.PeA;
        }

        public double GetTimeToPeriapsis()
        {
            return this.Vessel.orbit.timeToPe;
        }

        public double GetOrbitalInclination()
        {
            return this.Vessel.orbit.inclination;
        }

        public double GetOrbitalEccentricity()
        {
            return this.Vessel.orbit.eccentricity;
        }

        public double GetOrbitalPeriod()
        {
            return this.Vessel.orbit.period;
        }

        public double GetLongitudeOfAN()
        {
            return this.Vessel.orbit.LAN;
        }

        public double GetLongitudeOfPe()
        {
            return this.Vessel.orbit.LAN + this.Vessel.orbit.argumentOfPeriapsis;
        }

        public double GetSemiMajorAxis()
        {
            return this.Vessel.orbit.semiMajorAxis;
        }

        public double GetSemiMinorAxis()
        {
            return this.Vessel.orbit.semiMinorAxis;
        }

        // Surface flight calculations.
        public double GetSeaLevelAltitude()
        {
            return this.Vessel.mainBody.GetAltitude(this.Vessel.CoM);
        }

        public double GetTerrainAltitude()
        {
            return this.GetSeaLevelAltitude() - this.Vessel.terrainAltitude;
        }

        public double GetHorizontalSurfaceSpeed()
        {
            return this.Vessel.verticalSpeed;
        }

        public double GetVerticalSurfaceSpeed()
        {
            return this.Vessel.horizontalSrfSpeed;
        }

        public double GetLongitude()
        {
            return this.Vessel.longitude;
        }

        public double GetLatitude()
        {
            return this.Vessel.latitude;
        }

        public double GetGForce()
        {
            return this.Vessel.geeForce;
        }

        // Aerodynamic calculations.
        public double GetTerminalVelocity()
        {
            if (this.Vessel.atmDensity <= 0)
            {
                return 0;
            }

            return Math.Sqrt(
                (2 * this.GetTotalPartMass() * FlightGlobals.getGeeForceAtPosition(this.Vessel.CoM).magnitude)
                / (this.Vessel.atmDensity * this.GetTotalPartDrag() * FlightGlobals.DragMultiplier)
            );
        }

        public double GetAtmosphericEfficiency()
        {
            double terminalVelocity = this.GetTerminalVelocity();
            if (terminalVelocity <= 0)
            {
                return 0;
            }

            return FlightGlobals.ship_srfSpeed / terminalVelocity;
        }

        public double GetAtmosphericDragForce()
        {
            return (this.Vessel.atmDensity * Math.Pow(FlightGlobals.ship_srfSpeed, 2)
                * this.GetTotalPartDrag() * FlightGlobals.DragMultiplier) / 2;
        }

        // Craft calculations.
        public double GetTotalPartMass()
        {
            double totalMass = 0;
            foreach (Part part in this.Vessel.parts)
            {
                totalMass += part.mass + part.GetResourceMass();
            }

            return totalMass;
        }

        public double GetTotalPartDrag()
        {
            double totalDrag = 0;
            foreach (Part part in this.Vessel.parts)
            {
                totalDrag += (part.mass + part.GetResourceMass()) * part.maximum_drag;
            }

            return totalDrag;
        }
    }
}
