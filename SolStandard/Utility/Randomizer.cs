using System.Collections.Generic;

namespace SolStandard.Utility
{
    public static class Randomizer
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int listPosition = list.Count;
            while (listPosition > 1)
            {
                listPosition--;
                int randomPosition = GameDriver.Random.Next(listPosition + 1);
                T value = list[randomPosition];
                list[randomPosition] = list[listPosition];
                list[listPosition] = value;
            }
        }
    }
}