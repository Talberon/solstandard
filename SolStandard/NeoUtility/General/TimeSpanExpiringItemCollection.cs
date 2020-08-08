using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SolStandard.NeoUtility.General
{
    public class TimeSpanExpiringItemCollection<T> : IUpdate
    {
        private List<MutableKeyValuePair<T, TimeSpan>> expiringItems;

        public IEnumerable<T> Items => expiringItems.Select(kvp => kvp.Key);

        public TimeSpanExpiringItemCollection()
        {
            expiringItems = new List<MutableKeyValuePair<T, TimeSpan>>();
        }

        public TimeSpanExpiringItemCollection<T> From(List<MutableKeyValuePair<T, TimeSpan>> initialItems)
        {
            var collection = new TimeSpanExpiringItemCollection<T>
            {
                expiringItems = initialItems
            };
            return collection;
        }

        public void AddExpiringItem(T item, TimeSpan lifetime)
        {
            expiringItems.Add(new MutableKeyValuePair<T, TimeSpan>(item, lifetime));
        }

        public void Update(GameTime gameTime)
        {
            foreach (MutableKeyValuePair<T, TimeSpan> kvp in expiringItems)
            {
                kvp.Value -= gameTime.ElapsedGameTime;
            }

            expiringItems.RemoveAll(kvp => kvp.Value <= TimeSpan.Zero);
        }
    }
}