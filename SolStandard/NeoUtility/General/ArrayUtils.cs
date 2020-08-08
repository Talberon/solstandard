using System.Linq;

namespace SolStandard.NeoUtility.General
{
    public static class ArrayUtils
    {
        public static T[] ToOneDimensionalArray<T>(this T[,] twoDimensionalArray)
        {
            return twoDimensionalArray.Cast<T>().ToArray();
        }
    }
}