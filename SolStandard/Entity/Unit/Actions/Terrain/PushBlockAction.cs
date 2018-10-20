using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class PushBlockAction : UnitAction
    {
        public PushBlockAction(IRenderable icon, string name, string description, SpriteAtlas tileSprite, int[] range) : base(icon, name, description, tileSprite, range)
        {
        }

        public override void ExecuteAction(MapSlice targetSlice, GameMapContext gameMapContext, BattleContext battleContext)
        {
            throw new System.NotImplementedException();
        }
    }
}