﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Weapon : TerrainEntity, IItem, IActionTile
    {
        public int[] InteractRange { get; private set; }
        private WeaponStatistics WeaponStatistics { get; set; }
        private readonly Window statWindow;

        public Weapon(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] pickupRange,
            int atkValue, int luckModifier, int[] atkRange, int usesRemaining)
            : base(name, type, sprite, mapCoordinates, new Dictionary<string, string>())
        {
            InteractRange = pickupRange;
            WeaponStatistics = new WeaponStatistics(atkValue, luckModifier, atkRange, usesRemaining);
            statWindow = BuildStatWindow(WeaponStatistics);
        }

        public static Window BuildStatWindow(WeaponStatistics weaponStatistics)
        {
            IRenderable[,] statWindowGrid =
            {
                {new RenderText(AssetManager.WindowFont, "-Stats-")},
                {weaponStatistics.GenerateStatGrid(AssetManager.WindowFont)}
            };

            return new Window(new WindowContentGrid(statWindowGrid, 2, HorizontalAlignment.Centered), InnerWindowColor);
        }

        public bool IsBroken
        {
            get { return WeaponStatistics.IsBroken; }
        }

        public IRenderable Icon
        {
            get { return Sprite; }
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new PickUpItemAction(this, MapCoordinates)
            };
        }

        public UnitAction UseAction()
        {
            return new WeaponAttack(Sprite, Name, WeaponStatistics);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new Weapon(Name, Type, Sprite, MapCoordinates, InteractRange, WeaponStatistics.AtkValue,
                WeaponStatistics.LuckModifier, WeaponStatistics.AtkRange, WeaponStatistics.UsesRemaining);
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
                            StatusIconProvider.GetStatusIcon(StatusIcon.PickupRange, new Vector2(GameDriver.CellSize)),
                            new RenderText(
                                AssetManager.WindowFont,
                                ": " + string.Format("[{0}]", string.Join(",", InteractRange))
                            )
                        },
                        {
                            statWindow,
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}