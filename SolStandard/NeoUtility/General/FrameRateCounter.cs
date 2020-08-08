using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.NeoUtility.General
{
    public class FrameRateCounter
    {
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private int frameCounter;
        
        public int FrameRate { get; private set; }
        public bool GameIsRunningSlow { get; private set; }

        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            GameIsRunningSlow = gameTime.IsRunningSlowly;

            if (elapsedTime <= TimeSpan.FromSeconds(1)) return;
            
            elapsedTime -= TimeSpan.FromSeconds(1);
            FrameRate = frameCounter;
            frameCounter = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            frameCounter++;
        }
    }
}