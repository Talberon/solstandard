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
using SolStandard.Utility.Exceptions;

namespace SolStandard.Entity.General
{
    public class Switch : TerrainEntity, IActionTile
    {
        public int[] Range { get; private set; }
        public string TriggersId { get; private set; }

        public Switch(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, string triggersId) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            TriggersId = triggersId;
            Range = new[] {1};
            CanMove = false;
        }

        public UnitAction TileAction()
        {
            ILockable targetLockable = FindLockable();
            return new ToggleSwitchAction(this, targetLockable);
        }

        private ILockable FindLockable()
        {
            foreach (MapEntity entity in MapContainer.GameGrid[(int) Layer.Entities])
            {
                ILockable door = entity as ILockable;
                if (door != null)
                {
                    if (entity.Name == TriggersId)
                    {
                        return door;
                    }
                }
            }

            throw new TileNotFoundException();
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
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(AssetManager.WindowFont, "Triggers: " + TriggersId)
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor)
                        }
                    },
                    1
                );
            }
        }
    }
}