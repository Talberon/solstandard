using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class WaitRoutine : UnitAction, IRoutine
    {
        private const SkillIcon RoutineIcon = SkillIcon.Wait;

        public WaitRoutine()
            : base(
                icon: SkillIconProvider.GetSkillIcon(RoutineIcon, GameDriver.CellSizeVector),
                name: "Wait Routine",
                description: "Wait in position.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {0},
                freeAction: false
            )
        {
        }

        public IRenderable MapIcon => SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2((float) GameDriver.CellSize / 3));

        public bool CanBeReadied(CreepUnit unit)
        {
            return true;
        }

        public bool CanExecute => true;

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GlobalEventQueue.QueueSingleEvent(new ToastAtCursorEvent("Waiting...", 50));
            GlobalEventQueue.QueueSingleEvent(new SkippableWaitFramesEvent(30));
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
        }
    }
}