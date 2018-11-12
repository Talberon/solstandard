using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class PressurePlate : TerrainEntity, IEffectTile
    {
        private readonly string triggersId;
        private bool wasPressed;
        private readonly bool triggerOnRelease;

        public PressurePlate(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, string triggersId, bool triggerOnRelease) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            this.triggersId = triggersId;
            this.triggerOnRelease = triggerOnRelease;
            wasPressed = false;
        }

        public bool IsExpired
        {
            get { return false; }
        }

        public void TriggerStartOfTurn()
        {
            //Do nothing
        }

        public void TriggerEndOfTurn()
        {
            if (UnitIsStandingOnPressurePlate)
            {
                if (!wasPressed)
                {
                    TriggerTiles.ForEach(tile => tile.Trigger());
                    wasPressed = true;
                }
            }
            else
            {
                if (wasPressed && triggerOnRelease)
                {
                    TriggerTiles.ForEach(tile => tile.Trigger());
                }

                wasPressed = false;
            }
        }

        private List<ITriggerable> TriggerTiles
        {
            get
            {
                List<ITriggerable> fetchedTiles = new List<ITriggerable>();

                foreach (MapElement element in MapContainer.GameGrid[(int) Layer.Entities])
                {
                    MapEntity entity = element as MapEntity;
                    ITriggerable triggerTile = entity as ITriggerable;

                    if (triggerTile != null && entity.Name == triggersId)
                    {
                        fetchedTiles.Add(triggerTile);
                    }
                }

                return fetchedTiles;
            }
        }

        private bool UnitIsStandingOnPressurePlate
        {
            get
            {
                return GameContext.Units.Any(unit =>
                    unit.UnitEntity != null && unit.UnitEntity.MapCoordinates == MapCoordinates);
            }
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
                            UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                            new RenderText(AssetManager.WindowFont, "Triggers: " + triggersId)
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Mv),
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