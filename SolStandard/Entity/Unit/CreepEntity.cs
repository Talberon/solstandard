using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Model;

namespace SolStandard.Entity.Unit
{
    public class CreepEntity : UnitEntity
    {
        private IRenderable routineIcon;
        private IRenderable independentIcon;
        private const int IndependentIconSize = 8;
        public CreepRoutineModel Model { get; }
        public string CreepPool { get; }
        public int StartingGold { get; }

        public CreepEntity(string name, string type, UnitSpriteSheet spriteSheet, Vector2 mapCoordinates, Team team,
            Role role, bool isCommander, CreepRoutineModel creepRoutineModel, string[] initialInventory)
            : base(name, type, spriteSheet, mapCoordinates, team, role, isCommander, initialInventory)
        {
            Model = creepRoutineModel;
            CreepPool = creepRoutineModel.CreepPool;
            StartingGold = creepRoutineModel.StartingGold;
        }

        public void UpdateRoutineIcon(IRoutine routine)
        {
            routineIcon = routine.MapIcon.Clone();
        }

        private static Vector2 CenterTopOfTile(Vector2 tileCoordinates, float iconSize)
        {
            Vector2 centerCoordinates = tileCoordinates * GameDriver.CellSize;
            centerCoordinates.X += (float) GameDriver.CellSize / 2;
            centerCoordinates.X -= iconSize / 2;
            centerCoordinates.Y -= iconSize;
            return centerCoordinates;
        }

        private static Vector2 TopLeftOfTile(Vector2 tileCoordinates)
        {
            Vector2 centerCoordinates = tileCoordinates * GameDriver.CellSize;
            return centerCoordinates;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            routineIcon?.Draw(spriteBatch, CenterTopOfTile(MapCoordinates, routineIcon.Width));

            if (Model.IsIndependent)
            {
                IndependentIcon.Draw(spriteBatch, TopLeftOfTile(MapCoordinates));
            }
        }

        public CreepEntity Copy()
        {
            return new CreepEntity(Name, Type, UnitSpriteSheet.Clone(), MapCoordinates, Team, Role, IsCommander,
                Model, InitialInventory);
        }

        private IRenderable IndependentIcon =>
            independentIcon ??= MiscIconProvider.GetMiscIcon(MiscIcon.Independent, new Vector2(IndependentIconSize));
    }
}