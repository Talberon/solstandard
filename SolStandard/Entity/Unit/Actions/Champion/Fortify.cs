using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Champion
{
    public class Fortify : UnitAction
    {
        private readonly int pointsToTrade;
        private readonly int turnDuration;

        public Fortify(int pointsToTrade, int turnDuration) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Fortify, new Vector2(GameDriver.CellSize)),
            name: "Fortify",
            description: "Block " + pointsToTrade + " incoming damage for " + turnDuration +
                         " turn(s) in exchange for " + pointsToTrade + " " + UnitStatistics.Abbreviation[Stats.Luck] +
                         ".",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: true
        )
        {
            this.pointsToTrade = pointsToTrade;
            this.turnDuration = turnDuration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit,
                    new FortifiedStatus(turnDuration, pointsToTrade)));
                eventQueue.Enqueue(new WaitFramesEvent(30));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}