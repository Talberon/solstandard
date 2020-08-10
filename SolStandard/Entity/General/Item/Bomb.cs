using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Targeting;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Bomb : PushBlock, IItem, IEffectTile, IThreatRange
    {
        public int[] Range { get; }
        public int Damage { get; }
        public IRenderable Icon { get; }
        public string ItemPool { get; }
        public bool HasTriggered { get; set; }
        public bool IsExpired { get; private set; }
        private int turnsRemaining;

        public Bomb(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] range, int damage,
            int turnsRemaining, string itemPool) :
            base(name, type, sprite, mapCoordinates)
        {
            Range = range;
            Damage = damage;
            Icon = sprite;
            ItemPool = itemPool;
            this.turnsRemaining = turnsRemaining;
            HasTriggered = false;
            IsExpired = false;
            CanMove = false;
        }

        public bool IsBroken => false;

        public UnitAction UseAction()
        {
            return new DeployBombAction(this, turnsRemaining);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new Bomb(Name, Type, Sprite.Clone(), MapCoordinates, Range, Damage, turnsRemaining, ItemPool);
        }

        public bool Trigger(EffectTriggerTime triggerTime)
        {
            if (!WillTrigger(triggerTime)) return false;

            turnsRemaining--;

            if (turnsRemaining > 0)
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCellCoordinates(
                    "Fuse is burning...",
                    MapCoordinates,
                    50
                );
                AssetManager.CombatBlockSFX.Play();
                return true;
            }

            GlobalContext.MapCursor.SnapCameraAndCursorToCoordinates(MapCoordinates);
            GlobalContext.MapCamera.SnapCameraCenterToCursor();

            var bombTargetContext =
                new UnitTargetingPhase(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack));

            MapContainer.ClearDynamicAndPreviewGrids();
            bombTargetContext.GenerateTargetingGrid(MapCoordinates, Range);

            List<MapElement> rangeTiles = MapContainer.GetMapElementsFromLayer(Layer.Dynamic);

            string trapMessage = "Bomb exploded!" + Environment.NewLine;

            foreach (MapElement rangeTile in rangeTiles)
            {
                MapSlice slice = MapContainer.GetMapSliceAtCoordinates(rangeTile.MapCoordinates);
                GameUnit unitInRange = UnitSelector.SelectUnit(slice.UnitEntity);

                if (unitInRange != null)
                {
                    trapMessage += unitInRange.Id + " takes [" + Damage + "] damage!" + Environment.NewLine;

                    for (int i = 0; i < Damage; i++) unitInRange.DamageUnit();
                }

                if (EntityAtSliceCanTakeDamage(slice))
                {
                    var breakableObstacle = (BreakableObstacle) slice.TerrainEntity;
                    breakableObstacle.DealDamage(Damage);
                }
            }

            MapContainer.ClearDynamicAndPreviewGrids();
            IsExpired = true;
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCellCoordinates(trapMessage, MapCoordinates,
                50);
            AssetManager.CombatDeathSFX.Play();

            return true;
        }

        public bool WillTrigger(EffectTriggerTime triggerTime)
        {
            return triggerTime == EffectTriggerTime.StartOfRound && !HasTriggered;
        }

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Atk, GameDriver.CellSizeVector),
                        new RenderText(
                            AssetManager.WindowFont,
                            UnitStatistics.Abbreviation[Stats.Atk] + ": " + Damage
                        )
                    },
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.AtkRange, GameDriver.CellSizeVector),
                        new RenderText(
                            AssetManager.WindowFont,
                            UnitStatistics.Abbreviation[Stats.AtkRange] + ": [" + string.Join(",", Range) + "]"
                        )
                    }
                }
            );

        public override void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, ElementColor);
        }

        protected override void Draw(SpriteBatch spriteBatch, Color colorOverride)
        {
            base.Draw(spriteBatch, colorOverride);
            if (Visible)
            {
                Timer.Draw(spriteBatch, TimerCoordinates);
            }
        }

        private IRenderable Timer => new RenderText(AssetManager.MapFont, turnsRemaining.ToString(), Color.White);

        private Vector2 TimerCoordinates
        {
            get
            {
                Vector2 timerCoordinates = MapCoordinates * GameDriver.CellSize;
                timerCoordinates.X += (GameDriver.CellSize / 2) - (Timer.Width / 2);
                return timerCoordinates;
            }
        }

        private static bool EntityAtSliceCanTakeDamage(MapSlice slice)
        {
            return slice.TerrainEntity != null &&
                   slice.TerrainEntity.GetType().IsAssignableFrom(typeof(BreakableObstacle));
        }

        public int[] AtkRange => Range;

        public int MvRange => 0;
    }
}