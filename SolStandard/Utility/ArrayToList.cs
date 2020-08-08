using System.Collections.Generic;

namespace SolStandard.Utility
{
    public static class ArrayToList<T>
    {
        public static List<List<T>> Convert2DArrayToNestedList(T[,] array)
        {
            var result = new List<List<T>>();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                var row = new List<T>();
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    row.Add(array[i, j]);
                }
                result.Add(row);
            }

            return result;
        }
    }
}