using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public class BasicAttack : UnitAction
    {
        public BasicAttack() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, GameDriver.CellSizeVector),
            name: "Basic Attack",
            description: "Attack a target based on your " + UnitStatistics.Abbreviation[Stats.Atk] +
                         " and " + UnitStatistics.Abbreviation[Stats.Luck] + " statistics." + Environment.NewLine +
                         "Can be used against certain types of terrain.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null,
            freeAction: false
        )
        {
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Range = GlobalContext.ActiveUnit.Stats.CurrentAtkRange;
            base.GenerateActionGrid(origin, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                GlobalEventQueue.QueueSingleEvent(new StartCombatEvent(targetUnit));
            }
            else if (TargetIsABreakableObstacleInRange(targetSlice))
            {
                DamageTerrain(targetSlice);

                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(10));
                GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static void DamageTerrain(MapSlice targetSlice)
        {
            var targetObstacle = (BreakableObstacle) targetSlice.TerrainEntity;
            targetObstacle.DealDamage(1);

            if (targetObstacle.IsBroken)
            {
                MapContainer.GameGrid[(int) Layer.Entities]
                    [(int) targetObstacle.MapCoordinates.X, (int) targetObstacle.MapCoordinates.Y] = null;
            }
        }
    }
}