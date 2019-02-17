using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Containers.View
{
    public class DeploymentView : IUserInterface
    {
        private bool visible;

        public DeploymentView()
        {
            visible = true;
        }


        //Show current unit being deployed
        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //TODO Draw deployment HUD
        }
    }
}