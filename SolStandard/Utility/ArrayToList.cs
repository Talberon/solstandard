using System.Collections.Generic;

namespace SolStandard.Utility
{
    public static class ArrayToList<T>
    {
        public static List<List<T>> Convert2DArrayToNestedList(T[,] array)
        {
            List<List<T>> result = new List<List<T>>();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                List<T> row = new List<T>();
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