using System;
using System.Collections;
using System.Collections.Generic;

namespace Steelbreakers.Utility.General
{
    public static class CollectionExtensions
    {
        public static bool IsEmpty(this ICollection me) => me.Count == 0;
        public static bool IsNotEmpty(this ICollection me) => !IsEmpty(me);

        public static Stack<T> Clone<T>(this Stack<T> original)
        {
            var arr = new T[original.Count];
            original.CopyTo(arr, 0);
            Array.Reverse(arr);
            return new Stack<T>(arr);
        }
    }
}