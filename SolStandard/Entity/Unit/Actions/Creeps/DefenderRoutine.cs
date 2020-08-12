using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class DefenderRoutine : UnitAction, IRoutine
    {
        private const int ArmorToRecover = 3;

        public DefenderRoutine()
            : base(
                icon: UnitStatistics.GetSpriteAtlas(Stats.Armor, GameDriver.CellSizeVector),
                name: "Defender Routine",
                description: "Wander and then defend to recover " + UnitStatistics.Abbreviation[Stats.Armor] + ".",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
                range: new[] {0},
                freeAction: false
            )
        {
        }

        public IRenderable MapIcon => UnitStatistics.GetSpriteAtlas(Stats.Armor, new Vector2((float) GameDriver.CellSize / 3));

        public bool CanBeReadied(CreepUnit creepUnit)
        {
            return creepUnit.Stats.CurrentArmor < creepUnit.Stats.MaxArmor;
        }

        public bool CanExecute => true;

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit activeCreep = GlobalContext.ActiveUnit;
            GlobalEventQueue.QueueSingleEvent(new ToastAtCursorEvent("Defending...", 50));
            WanderRoutine.Roam(activeCreep);
            GlobalEventQueue.QueueSingleEvent(new SkippableWaitFramesEvent(30));
            GlobalEventQueue.QueueSingleEvent(new RegenerateArmorEvent(activeCreep, ArmorToRecover));
            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCursorEvent(
                    "Guard!" + Environment.NewLine +
                    "Recovered [" + ArmorToRecover + "] " + UnitStatistics.Abbreviation[Stats.Armor] + "!",
                    50
                )
            );
            GlobalEventQueue.QueueSingleEvent(new SkippableWaitFramesEvent(50));
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
        }
    }
}