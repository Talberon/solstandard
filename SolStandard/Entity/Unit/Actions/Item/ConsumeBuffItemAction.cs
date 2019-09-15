﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class ConsumeBuffItemAction : UnitAction
    {
        private readonly IConsumable item;

        public ConsumeBuffItemAction(IConsumable item, Stats statistic, int statModifier, int buffDuration,
            int[] range) : base(
            icon: item.Icon.Clone(),
            name: "Consume",
            description: ItemDescription(statistic, statModifier, buffDuration),
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: range,
            freeAction: true
        )
        {
            this.item = item;
        }

        private static IRenderable ItemDescription(Stats statistic, int statModifier, int buffDuration)
        {
            ISpriteFont descriptionFont = AssetManager.WindowFont;
            Vector2 iconSize = new Vector2(descriptionFont.MeasureString("A").Y);
            return new WindowContentGrid(new IRenderable[,]
                {
                    {
                        new RenderText(descriptionFont, "Single use. Target increases"),
                        UnitStatistics.GetSpriteAtlas(statistic, iconSize),
                        new RenderText(
                            AssetManager.WindowFont,
                            $"{UnitStatistics.Abbreviation[statistic]} by [{statModifier}] for [{buffDuration}] turns."
                        )
                    }
                }
            );
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit) || TargetIsSelfInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                item.Consume(targetUnit);

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new WaitFramesEvent(50));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }

            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not a friendly unit in range!", 50);
            }
        }
    }
}