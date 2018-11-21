using Microsoft.Xna.Framework;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Mage
{
    public class Inferno : LayTrap
    {
        public Inferno(int damage, int maxTriggers) : base(
            skillIcon: SkillIconProvider.GetSkillIcon(SkillIcon.Inferno, new Vector2(GameDriver.CellSize)),
            tileSprite: new AnimatedSpriteSheet(
                AssetManager.FireTexture, AssetManager.FireTexture.Height, new Vector2(GameDriver.CellSize), 6, false,
                Color.White
            ),
            title: "Inferno",
            damage: damage,
            maxTriggers: maxTriggers
        )
        {
        }
    }
}