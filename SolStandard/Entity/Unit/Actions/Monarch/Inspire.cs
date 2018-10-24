﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Monarch
{
    public class Inspire : UnitAction
    {
        private readonly int statModifier;
        private readonly int duration;

        public Inspire(int duration, int statModifier) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Inspire, new Vector2(32)),
            name: "Inspire",
            description: "Grant a buff that increases an ally's ATK by [+" + statModifier + "] for [" + duration +
                         " ] turns.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1, 2}
        )
        {
            this.statModifier = statModifier;
            this.duration = duration;
        }

        public override void ExecuteAction(MapSlice targetSlice, GameMapContext gameMapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastBuffEvent(ref targetUnit, new AtkStatUp(duration, statModifier)));
                eventQueue.Enqueue(new EndTurnEvent(ref gameMapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                MapContainer.AddNewToastAtMapCursor("Not an ally in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}