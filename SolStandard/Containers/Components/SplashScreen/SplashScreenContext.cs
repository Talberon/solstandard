using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SolStandard.Containers.Components.SplashScreen
{
    public class SplashScreenContext : IUpdate
    {
        public IUserInterface SplashScreenHUD => splashScreenHUD;
        private readonly SplashScreenHUD splashScreenHUD;

        public SplashScreenContext(SplashScreenHUD splashScreenHUD)
        {
            this.splashScreenHUD = splashScreenHUD;
        }

        public void Update(GameTime gameTime)
        {
            splashScreenHUD.Update(gameTime);
        }
    }
}