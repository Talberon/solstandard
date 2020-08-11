using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NLog;
using SolStandard.Utility.Collections;

namespace SolStandard.Utility
{
    public static class GlobalAsyncActions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly List<MutableKeyValuePair<Action, TimeSpan>> PendingActions =
            new List<MutableKeyValuePair<Action, TimeSpan>>();

        public static void PerformActionAfterTime(Action action, TimeSpan delay)
        {
            Logger.Trace("Adding new action: {}", action);
            PendingActions.Add(new MutableKeyValuePair<Action, TimeSpan>(action, delay));
        }

        public static void Update(GameTime gameTime)
        {
            foreach (MutableKeyValuePair<Action, TimeSpan> kvp in PendingActions)
            {
                kvp.Value -= gameTime.ElapsedGameTime;

                if (kvp.Value > TimeSpan.Zero) continue;

                Logger.Trace("Invoking action: {}", kvp.Key);
                kvp.Key.Invoke();
            }

            PendingActions.RemoveAll(kvp => kvp.Value <= TimeSpan.Zero);
        }

        public static void CancelAllActions()
        {
            PendingActions.Clear();
        }
    }
}