using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Skills;
using SolStandard.Entity.Unit.Skills.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Portal : TerrainEntity, IActionTile
    {
        private readonly bool canMove;
        private readonly string destinationId;
        private readonly bool oneWay;
        public int[] Range { get; private set; }

        public Portal(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool canMove, string destinationId, bool oneWay, int[] range) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            this.canMove = canMove;
            this.destinationId = destinationId;
            this.oneWay = oneWay;
            Range = range;
        }

        public UnitSkill TileAction()
        {
            Vector2 targetTileCoordinates = Vector2.One;

            foreach (MapElement entity in MapContainer.GameGrid[(int) Layer.Entities])
            {
                Portal targetPortal = entity as Portal;
                if (targetPortal == null) continue;

                if (targetPortal.Name == destinationId)
                {
                    targetTileCoordinates = targetPortal.MapCoordinates;
                }
            }

            return new Transport(targetTileCoordinates);
        }

        public override IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
                    new[,]
                    {
                        {
                            Sprite,
                            new RenderText(AssetManager.HeaderFont, Name)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "~~~~~~~~~~~"),
                            new RenderBlank()
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(AssetManager.WindowFont, (canMove) ? "Can Move" : "No Move",
                                (canMove) ? PositiveColor : NegativeColor)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "Destination: " + destinationId),
                            new RenderBlank()
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (oneWay) ? "One-Way" : "Portal"),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}