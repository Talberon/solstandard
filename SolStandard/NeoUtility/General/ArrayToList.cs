using System.Collections.Generic;

namespace SolStandard.NeoUtility.General
{
    public static class ArrayToList<T>
    {
        public static List<List<T>> Convert2DArrayToNestedList(T[,] array)
        {
            var nestedList = new List<List<T>>();
            for (int column = 0; column < array.GetLength(0); column++)
            {
                var rowItems = new List<T>();
                for (int row = 0; row < array.GetLength(1); row++)
                {
                    rowItems.Add(array[column, row]);
                }

                nestedList.Add(rowItems);
            }

            return nestedList;
        }
    }
}