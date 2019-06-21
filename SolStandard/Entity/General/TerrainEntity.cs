﻿using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public enum EntityTypes
    {
        BreakableObstacle,
        BuffTile,
        Chest,
        Decoration,
        Door,
        Drawbridge,
        Movable,
        Currency,
        Portal,
        Switch,
        Key,
        Artillery,
        Railgun,
        Seize,
        Pushable,
        SelectMap,
        PressurePlate,
        Trap,
        Weapon,
        Blink,
        HealthPotion,
        BuffItem,
        Barricade,
        Deployment,
        Bank,
        Vendor,
        RecoveryTile,
        LadderBridge,
        Magnet,
        Bomb,
        Contract,
        RecallCharm,
        Escape,
        Relic
    }

    public class TerrainEntity : MapEntity
    {
        public static readonly Color PositiveColor = new Color(30, 200, 30);
        public static readonly Color NegativeColor = new Color(250, 10, 10);
        protected static readonly Color InnerWindowColor = new Color(25, 25, 25, 80);

        protected IRenderable InfoHeader { get; }
        private IRenderable NameText { get; }
        private IRenderable TypeText { get; }

        public bool CanMove { get; protected set; }

        protected TerrainEntity(string name, string type, IRenderable sprite, Vector2 mapCoordinates) :
            base(name, type, sprite, mapCoordinates)
        {
            CanMove = true;

            NameText = new RenderText(AssetManager.HeaderFont, Name);
            TypeText = new RenderText(AssetManager.WindowFont, "[" + Type + "]");

            InfoHeader = new Window(
                new WindowContentGrid(
                    new[,]
                    {
                        {
                            Sprite.Clone(),
                            NameText
                        },
                        {
                            TypeText,
                            new RenderBlank()
                        }
                    }
                    ,
                    1,
                    HorizontalAlignment.Centered
                ),
                InnerWindowColor,
                HorizontalAlignment.Right
            );
        }

        public virtual IRenderable TerrainInfo =>
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
                    }
                },
                1,
                HorizontalAlignment.Centered
            );
    }
}