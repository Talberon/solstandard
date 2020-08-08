using System.Collections.Generic;
using System.Linq;

namespace SolStandard.NeoUtility.General
{
    public static class ListExtensions
    {
        public static List<T> Flatten<T>(this IEnumerable<List<T>> me)
        {
            return me.SelectMany(subList => subList).ToList();
        }
    }
}