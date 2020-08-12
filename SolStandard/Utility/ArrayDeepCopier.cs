namespace SolStandard.Utility
{
    public static class ArrayDeepCopier<T>
    {
        public static T[] DeepCopyArray(T[] array)
        {
            var newArray = new T[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }

            return newArray;
        }
    }
}