using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Assets
{
    public static class TeamIconProvider
    {
        public static SpriteAtlas GetTeamIcon(Team team, Vector2 iconSize)
        {
            return new SpriteAtlas(
                AssetManager.TeamIcons,
                new Vector2(16),
                iconSize,
                (int) team,
                Color.White
            );
        }
    }
}