using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class DefenderRoutine : UnitAction, IRoutine
    {
        //TODO Implement unique routine icon
        private const SkillIcon RoutineIcon = SkillIcon.Bulwark;
        private const int ArmorToRecover = 3;

        public DefenderRoutine()
            : base(
                icon: SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2(GameDriver.CellSize)),
                name: "Defender Routine",
                description: "Wander and then defend to recover " + UnitStatistics.Abbreviation[Stats.Armor] + ".",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
                range: new[] {0},
                freeAction: false
            )
        {
        }

        public IRenderable MapIcon
        {
            get { return SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2((float) GameDriver.CellSize / 3)); }
        }

        public bool CanExecute
        {
            get { return true; }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit activeCreep = GameContext.ActiveUnit;
            GlobalEventQueue.QueueSingleEvent(new ToastAtCursorEvent("Defending...", 50));
            WanderRoutine.Roam(activeCreep);
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
            GlobalEventQueue.QueueSingleEvent(new RegenerateArmorEvent(activeCreep, ArmorToRecover));
            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCursorEvent(
                    "Guard!" + Environment.NewLine +
                    "Recovered [" + ArmorToRecover + "] " + UnitStatistics.Abbreviation[Stats.Armor] + "!",
                    50
                )
            );
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
        }
    }
}