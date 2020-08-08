using System;
using System.Collections.Generic;

namespace Steelbreakers.Utility.General
{
    public static class DictionaryExtensions
    {
        public static bool GetBoolean(this Dictionary<string, string> me, string key)
        {
            return Convert.ToBoolean(me[key]);
        }

        public static int GetInt(this Dictionary<string, string> me, string key)
        {
            return Convert.ToInt32(me[key]);
        }

        public static float GetFloat(this Dictionary<string, string> me, string key)
        {
            return Convert.ToSingle(me[key]);
        }

        public static string GetString(this Dictionary<string, string> me, string key)
        {
            return me[key];
        }
    }
}