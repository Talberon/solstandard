using Microsoft.Xna.Framework;
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
        Relic,
        Piston,
        Launchpad,
        SpringTrap,
        Crossing,
        CreepDeploy
    }

    public class TerrainEntity : MapEntity
    {
        public static readonly Color PositiveColor = new Color(30, 200, 30);
        public static readonly Color NegativeColor = new Color(250, 10, 10);
        protected static readonly Color InnerWindowColor = new Color(25, 25, 25, 80);

        private IRenderable InfoHeader { get; }
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
                            new RenderText(AssetManager.WindowFont,
                                $"[ X: {MapCoordinates.X}, Y: {MapCoordinates.Y} ]"),
                            RenderBlank.Blank
                        },
                        {
                            Sprite.Clone(),
                            NameText
                        },
                        {
                            TypeText,
                            RenderBlank.Blank
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
                        RenderBlank.Blank
                    },
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Mv),
                        new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                            (CanMove) ? PositiveColor : NegativeColor)
                    },
                    {
                        this is IActionTile
                            ? StatusIconProvider.GetStatusIcon(StatusIcon.PickupRange, GameDriver.CellSizeVector)
                            : RenderBlank.Blank,
                        this is IActionTile actionTile
                            ? new RenderText(AssetManager.WindowFont,
                                $": [{string.Join(",", actionTile.InteractRange)}]")
                            : RenderBlank.Blank
                    },
                    {
                        new Window(EntityInfo, InnerWindowColor),
                        RenderBlank.Blank
                    }
                },
                1,
                HorizontalAlignment.Centered
            );

        protected virtual IRenderable EntityInfo => RenderBlank.Blank;
    }
}