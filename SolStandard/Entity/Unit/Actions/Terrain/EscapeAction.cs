using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class EscapeAction : UnitAction
    {
        private readonly Vector2 tileCoordinates;
        private readonly EscapeEntity escapeEntity;

        public EscapeAction(EscapeEntity escapeEntity, Vector2 tileCoordinates) : base(
            icon: escapeEntity.RenderSprite.Clone(),
            name: "Escape",
            description: "Evacuate the map with the current unit.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null,
            freeAction: false
        )
        {
            this.tileCoordinates = tileCoordinates;
            this.escapeEntity = escapeEntity;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) tileCoordinates.X, (int) tileCoordinates.Y] =
                new MapDistanceTile(TileSprite, tileCoordinates);
            GameContext.GameMapContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(tileCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (SelectingTileAtUnitLocation(targetSlice))
            {
                if (UnitCanUseTile)
                {
                    if (UnitIsAllowedToEscape(targetUnit))
                    {
                        MapContainer.ClearDynamicAndPreviewGrids();

                        var eventQueue = new Queue<IEvent>();
                        eventQueue.Enqueue(
                            new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetSlice.MapCoordinates)
                        );
                        eventQueue.Enqueue(new EscapeObjectiveEvent(GameContext.ActiveUnit));
                        eventQueue.Enqueue(new WaitFramesEvent(10));
                        eventQueue.Enqueue(new EndTurnEvent());
                        GlobalEventQueue.QueueEvents(eventQueue);
                    }
                    else
                    {
                        GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                            "Not allowed to escape; Commander must escape last!",
                            50
                        );
                        AssetManager.WarningSFX.Play();
                    }
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Cannot be used by this team!",
                        50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Invalid selection!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static void EscapeWithUnit(GameUnit escapingUnit)
        {
            ((Escape) GameContext.Scenario.Objectives.Single(obj => obj.Key == VictoryConditions.Escape).Value)
                .AddUnitToEscapeeList(escapingUnit);
            escapingUnit.Escape();
            AssetManager.SkillBlinkSFX.Play();
        }

        private static bool UnitIsAllowedToEscape(GameUnit targetUnit)
        {
            if (targetUnit == GameContext.ActiveUnit && !targetUnit.IsCommander)
            {
                return true;
            }

            if (targetUnit.IsCommander)
            {
                if (!AnyUnitTeamMembersAliveAndNotEscaped(targetUnit))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool AnyUnitTeamMembersAliveAndNotEscaped(GameUnit targetUnit)
        {
            return GameContext.Units.Where(unit => unit.Team == targetUnit.Team && unit != targetUnit)
                .Any(
                    unit => unit.IsAlive &&
                            !((Escape) GameContext.Scenario.Objectives
                                .Single(obj => obj.Key == VictoryConditions.Escape)
                                .Value).EscapedUnits.Contains(unit)
                );
        }

        private bool SelectingTileAtUnitLocation(MapSlice targetSlice)
        {
            return tileCoordinates == targetSlice.MapCoordinates && targetSlice.UnitEntity != null &&
                   targetSlice.DynamicEntity != null;
        }

        private bool UnitCanUseTile
        {
            get
            {
                return GameContext.ActiveTeam switch
                {
                    Team.Red => escapeEntity.UseableByRed,
                    Team.Blue => escapeEntity.UseableByBlue,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}