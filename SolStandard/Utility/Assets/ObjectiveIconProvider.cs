using Microsoft.Xna.Framework;
using SolStandard.Containers.Scenario;

namespace SolStandard.Utility.Assets
{
    public static class ObjectiveIconProvider
    {
        public static SpriteAtlas GetObjectiveIcon(VictoryConditions victoryConditions, Vector2 iconSize)
        {
            return new SpriteAtlas(
                AssetManager.ObjectiveIcons,
                new Vector2(16),
                iconSize,
                (int) victoryConditions,
                Color.White
            );
        }
    }
}