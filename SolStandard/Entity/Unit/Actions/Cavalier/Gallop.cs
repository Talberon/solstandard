using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Actions.Lancer;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Cavalier
{
    public class Gallop : UnitAction
    {
        private readonly int gallopDistance;

        public Gallop(int gallopDistance) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Gallop, GameDriver.CellSizeVector),
            name: "Gallop",
            description: "Dash towards a target unit as a free action!" + Environment.NewLine +
                         "Cannot move through obstacles or other units." + Environment.NewLine +
                         $"Cannot move further than maximum {UnitStatistics.Abbreviation[Stats.Mv]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null,
            freeAction: true
        )
        {
            this.gallopDistance = gallopDistance;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            var attackTiles = new List<MapDistanceTile>();

            int limitedGallopDistance = Math.Min(gallopDistance, GlobalContext.ActiveUnit.Stats.Mv);

            for (int i = limitedGallopDistance; i > 1; i--)
            {
                var northTile = new Vector2(origin.X, origin.Y - i);
                var southTile = new Vector2(origin.X, origin.Y + i);
                var eastTile = new Vector2(origin.X + i, origin.Y);
                var westTile = new Vector2(origin.X - i, origin.Y);
                Charge.AddTileWithinMapBounds(attackTiles, northTile, i, TileSprite);
                Charge.AddTileWithinMapBounds(attackTiles, southTile, i, TileSprite);
                Charge.AddTileWithinMapBounds(attackTiles, eastTile, i, TileSprite);
                Charge.AddTileWithinMapBounds(attackTiles, westTile, i, TileSprite);
            }

            Charge.AddAttackTilesToGameGrid(attackTiles, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsUnitInRange(targetSlice, targetUnit))
            {
                if (!Charge.PathIsObstructed(targetSlice, targetUnit))
                {
                    Queue<IEvent> eventQueue = PathingUtil.MoveToCoordinates(
                        GlobalContext.ActiveUnit,
                        targetUnit.UnitEntity.MapCoordinates,
                        true,
                        false,
                        13
                    );
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new AdditionalActionEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Target is obstructed!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not a unit in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}