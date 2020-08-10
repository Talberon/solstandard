using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Piston : TerrainEntity, IRemotelyTriggerable
    {
        private enum PistonDirection
        {
            West,
            East,
            South,
            North,
        }

        private readonly PistonDirection pistonDirection;

        public Piston(string name, string type, string direction, Vector2 mapCoordinates) :
            base(name, type, BuildPistonSprite(GetPistonDirection(direction)), mapCoordinates)
        {
            pistonDirection = GetPistonDirection(direction);
            CanMove = false;
        }

        private static AnimatedSpriteSheet BuildPistonSprite(PistonDirection pistonDirection)
        {
            const int pistonCellSize = 48;
            var sprite = new AnimatedSpriteSheet(AssetManager.PistonTexture, pistonCellSize,
                GameDriver.CellSizeVector * 3, 6, false, Color.White);
            sprite.SetSpriteCell(0, (int) pistonDirection);
            sprite.Pause();
            return sprite;
        }

        private static PistonDirection GetPistonDirection(string direction)
        {
            return (PistonDirection) Enum.Parse(typeof(PistonDirection), direction, true);
        }

        public void RemoteTrigger()
        {
            MapSlice targetSlice = pistonDirection switch
            {
                PistonDirection.North => MapContainer.GetMapSliceAtCoordinates(new Vector2(MapCoordinates.X,
                    MapCoordinates.Y - 1)),
                PistonDirection.South => MapContainer.GetMapSliceAtCoordinates(new Vector2(MapCoordinates.X,
                    MapCoordinates.Y + 1)),
                PistonDirection.East => MapContainer.GetMapSliceAtCoordinates(new Vector2(MapCoordinates.X + 1,
                    MapCoordinates.Y)),
                PistonDirection.West => MapContainer.GetMapSliceAtCoordinates(new Vector2(MapCoordinates.X - 1,
                    MapCoordinates.Y)),
                _ => throw new ArgumentOutOfRangeException()
            };

            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (targetUnit != null)
            {
                if (CanPush(targetUnit))
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCellCoordinates("PUSHING!",
                        new Vector2(MapCoordinates.X, MapCoordinates.Y - 1),
                        50);
                    (Sprite as AnimatedSpriteSheet)?.PlayOnce();
                    PushTarget(targetUnit);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCellCoordinates("Target is obstructed!",
                        MapCoordinates, 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCellCoordinates("No unit in range!",
                    MapCoordinates, 50);
            }
        }

        private bool CanPush(GameUnit targetUnit)
        {
            if (targetUnit == null) return false;

            Vector2 targetCoordinates = targetUnit.UnitEntity.MapCoordinates;
            Vector2 oppositeCoordinates = UnitAction.DetermineOppositeTileOfUnit(MapCoordinates, targetCoordinates);

            return UnitMovingPhase.CanEndMoveAtCoordinates(targetUnit.UnitEntity, oppositeCoordinates) &&
                   targetUnit.IsMovable;
        }

        private void PushTarget(GameUnit target)
        {
            Vector2 oppositeCoordinates =
                UnitAction.DetermineOppositeTileOfUnit(MapCoordinates, target.UnitEntity.MapCoordinates);
            target.UnitEntity?.SlideToCoordinates(oppositeCoordinates);
            AssetManager.CombatBlockSFX.Play();
        }


        protected override IRenderable EntityInfo =>
            new WindowContentGrid(new IRenderable[,]
                {
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                        new RenderText(AssetManager.WindowFont, "Pushes: " + pistonDirection)
                    }
                }
            );
    }
}