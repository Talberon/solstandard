namespace Steelbreakers.Utility.General
{
    public class MutableKeyValuePair<TKey, TValue>
    {
        public TKey Key { get; }
        public TValue Value { get; set; }

        public MutableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}