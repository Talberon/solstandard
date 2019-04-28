using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Bomb : TerrainEntity, IItem, IEffectTile, IThreatRange
    {
        public int[] Range { get; private set; }
        public int Damage { get; private set; }
        public IRenderable Icon { get; private set; }
        public string ItemPool { get; private set; }
        public bool IsExpired { get; private set; }

        public Bomb(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] range, int damage,
            string itemPool, Dictionary<string, string> tiledProperties) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            Range = range;
            Damage = damage;
            Icon = sprite;
            ItemPool = itemPool;
            IsExpired = false;
        }

        public bool IsBroken
        {
            get { return false; }
        }

        public UnitAction UseAction()
        {
            return new DeployBombAction(this);
        }

        public UnitAction DropAction()
        {
            return new TradeItemAction(this);
        }

        public IItem Duplicate()
        {
            return new Bomb(Name, Type, Sprite, MapCoordinates, Range, Damage, ItemPool, TiledProperties);
        }

        public bool Trigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != EffectTriggerTime.StartOfTurn) return false;

            GameContext.MapCursor.SnapCursorToCoordinates(MapCoordinates);
            GameContext.MapCamera.SnapCameraCenterToCursor();

            UnitTargetingContext bombTargetContext =
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack));

            bombTargetContext.GenerateTargetingGrid(MapCoordinates, Range);

            List<MapElement> rangeTiles = MapContainer.GetMapElementsFromLayer(Layer.Dynamic);

            string trapMessage = "Bomb exploded!" + Environment.NewLine;

            foreach (MapElement rangeTile in rangeTiles)
            {
                MapSlice slice = MapContainer.GetMapSliceAtCoordinates(rangeTile.MapCoordinates);
                GameUnit trapUnit = UnitSelector.SelectUnit(slice.UnitEntity);

                if (trapUnit != null)
                {
                    trapMessage += trapUnit.Id + " takes [" + Damage + "] damage!" + Environment.NewLine;

                    for (int i = 0; i < Damage; i++) trapUnit.DamageUnit();
                }

                if (EntityAtSliceCanTakeDamage(slice))
                {
                    BreakableObstacle breakableObstacle = (BreakableObstacle) slice.TerrainEntity;
                    breakableObstacle.DealDamage(Damage);
                }
            }

            MapContainer.ClearDynamicAndPreviewGrids();

            IsExpired = true;

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(trapMessage, MapCoordinates, 50);
            AssetManager.CombatDeathSFX.Play();


            return true;
        }

        public override IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
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
                            new Window(new IRenderable[,]
                            {
                                {
                                    UnitStatistics.GetSpriteAtlas(Stats.Atk, new Vector2(GameDriver.CellSize)),
                                    new RenderText(
                                        AssetManager.WindowFont,
                                        UnitStatistics.Abbreviation[Stats.Atk] + ": " + Damage
                                    )
                                },
                                {
                                    UnitStatistics.GetSpriteAtlas(Stats.AtkRange, new Vector2(GameDriver.CellSize)),
                                    new RenderText(
                                        AssetManager.WindowFont,
                                        UnitStatistics.Abbreviation[Stats.AtkRange]
                                        + ": [" + string.Join(",", Range) + "]"
                                    )
                                }
                            }, InnerWindowColor),
                            new RenderBlank()
                        }
                    },
                    1
                );
            }
        }

        private static bool EntityAtSliceCanTakeDamage(MapSlice slice)
        {
            return slice.TerrainEntity != null &&
                   slice.TerrainEntity.GetType().IsAssignableFrom(typeof(BreakableObstacle));
        }

        public int[] AtkRange
        {
            get { return Range; }
        }

        public int MvRange
        {
            get { return 0; }
        }
    }
}