using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
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
            AnimatedSpriteSheet sprite = new AnimatedSpriteSheet(AssetManager.PistonTexture, pistonCellSize,
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
            MapSlice targetSlice;

            switch (pistonDirection)
            {
                case PistonDirection.North:
                    targetSlice =
                        MapContainer.GetMapSliceAtCoordinates(new Vector2(MapCoordinates.X, MapCoordinates.Y - 1));
                    break;
                case PistonDirection.South:
                    targetSlice =
                        MapContainer.GetMapSliceAtCoordinates(new Vector2(MapCoordinates.X, MapCoordinates.Y + 1));
                    break;
                case PistonDirection.East:
                    targetSlice =
                        MapContainer.GetMapSliceAtCoordinates(new Vector2(MapCoordinates.X + 1, MapCoordinates.Y));
                    break;
                case PistonDirection.West:
                    targetSlice =
                        MapContainer.GetMapSliceAtCoordinates(new Vector2(MapCoordinates.X - 1, MapCoordinates.Y));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (targetUnit != null)
            {
                if (CanPush(targetUnit))
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates("PUSHING!",
                        new Vector2(MapCoordinates.X, MapCoordinates.Y - 1),
                        50);
                    (Sprite as AnimatedSpriteSheet)?.PlayOnce();
                    PushTarget(targetUnit);
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates("Target is obstructed!",
                        MapCoordinates, 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates("No unit in range!",
                    MapCoordinates, 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private bool CanPush(GameUnit targetUnit)
        {
            if (targetUnit == null) return false;

            Vector2 targetCoordinates = targetUnit.UnitEntity.MapCoordinates;
            Vector2 oppositeCoordinates = UnitAction.DetermineOppositeTileOfUnit(MapCoordinates, targetCoordinates);

            return UnitMovingContext.CanEndMoveAtCoordinates(oppositeCoordinates) && targetUnit.IsMovable;
        }

        private void PushTarget(GameUnit target)
        {
            Vector2 oppositeCoordinates =
                UnitAction.DetermineOppositeTileOfUnit(MapCoordinates, target.UnitEntity.MapCoordinates);
            target.UnitEntity?.SlideToCoordinates(oppositeCoordinates);
            AssetManager.CombatBlockSFX.Play();
        }


        public override IRenderable TerrainInfo =>
            new WindowContentGrid(
                new[,]
                {
                    {
                        InfoHeader,
                        new RenderBlank()
                    },
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Mv),
                        new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                            (CanMove) ? PositiveColor : NegativeColor)
                    },
                    {
                        new Window(
                            new IRenderable[,]
                            {
                                {
                                    UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                                    new RenderText(AssetManager.WindowFont, "Pushes: " + pistonDirection)
                                }
                            },
                            InnerWindowColor
                        ),
                        new RenderBlank()
                    }
                },
                1,
                HorizontalAlignment.Centered
            );
    }
}