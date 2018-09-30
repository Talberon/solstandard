using System.Collections;
using System.Collections.Generic;

namespace SolStandard.Utility
{
    public class ArrayDeepCopier<T>
    {
        public static T[] DeepCopyArray(T[] array)
        {
            T[] newArray = new T[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }

            return newArray;
        }
    }
}