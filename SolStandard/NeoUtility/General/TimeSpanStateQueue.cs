using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SolStandard.NeoUtility.General
{
    public class TimeSpanStateQueue<T> : IUpdate where T : Enum
    {
        public T CurrentState => sceneStack.IsEmpty() ? defaultState : sceneStack.Peek().Key;
        public bool JustChanged { get; private set; }

        private readonly Queue<MutableKeyValuePair<T, TimeSpan>> sceneStack;
        private readonly T defaultState;

        private int previousStackCount;

        public TimeSpanStateQueue(T defaultState)
        {
            this.defaultState = defaultState;
            sceneStack = new Queue<MutableKeyValuePair<T, TimeSpan>>();
            JustChanged = true;
        }

        public void PushStateForDuration(T state, TimeSpan duration)
        {
            sceneStack.Enqueue(new MutableKeyValuePair<T, TimeSpan>(state, duration));
        }

        public void Update(GameTime gameTime)
        {
            if (sceneStack.IsEmpty())
            {
                JustChanged = previousStackCount != 0;
                previousStackCount = 0;
                return;
            }

            MutableKeyValuePair<T, TimeSpan> currentState = sceneStack.Peek();
            currentState.Value -= gameTime.ElapsedGameTime;

            if (currentState.Value < TimeSpan.Zero)
            {
                TimeSpan leftoverTime = currentState.Value.Multiply(-1);
                sceneStack.Dequeue();

                if (sceneStack.IsNotEmpty())
                {
                    sceneStack.Peek().Value -= leftoverTime;
                }
            }
            else if (currentState.Value == TimeSpan.Zero)
            {
                sceneStack.Dequeue();
            }

            JustChanged = previousStackCount != sceneStack.Count;
            previousStackCount = sceneStack.Count;
        }

        public void Reset()
        {
            sceneStack.Clear();
            JustChanged = true;
        }
    }
}