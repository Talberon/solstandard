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

namespace SolStandard.Entity.Unit.Actions.Marauder
{
    public class Brace : UnitAction
    {
        private readonly int duration;

        public Brace(int duration) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Brace, new Vector2(GameDriver.CellSize)),
            name: "Brace",
            description: "Prevent other units from moving this unit with abilities for <" + duration + "> turns.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0}
        )
        {
            this.duration = duration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                AssetManager.SkillBuffSFX.Play();
                
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit, new ImmovableStatus(Icon, duration)));
                eventQueue.Enqueue(new EndTurnEvent());
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