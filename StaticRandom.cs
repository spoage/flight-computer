using System;

namespace FlightComputer
{
    // This is primarily used to generate random window IDs across the setting manager without
    // having to worry about ID clashes, as is a common problem with the C# Random class.
    public static class StaticRandom
    {
        private static readonly Random Randomizer = new Random();

        public static int Next()
        {
            return Randomizer.Next();
        }

        public static int Next(int upperBound)
        {
            return Randomizer.Next(upperBound);
        }

        public static int Next(int upperBound, int lowerBound)
        {
            return Randomizer.Next(upperBound, lowerBound);
        }

        public static void NextBytes(Byte[] bytes)
        {
            Randomizer.NextBytes(bytes);
        }

        public static double NextDouble()
        {
            return Randomizer.NextDouble();
        }
    }
}
