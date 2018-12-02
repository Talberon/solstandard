using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Switch : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; private set; }
        private string TriggersId { get; set; }
        private bool active;
        private static readonly Color ActiveColor = new Color(180, 180, 180);

        public Switch(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, string triggersId) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            TriggersId = triggersId;
            InteractRange = new[] {1};
            CanMove = false;
            active = false;
        }

        public List<UnitAction> TileActions()
        {
            List<ITriggerable> targetTriggerables = FindTriggerables();
            return new List<UnitAction>
            {
                new ToggleSwitchAction(this, targetTriggerables)
            };
        }

        private List<ITriggerable> FindTriggerables()
        {
            List<ITriggerable> lockables = new List<ITriggerable>();

            foreach (MapElement mapElement in MapContainer.GameGrid[(int) Layer.Entities])
            {
                MapEntity entity = (MapEntity) mapElement;
                ITriggerable lockable = mapElement as ITriggerable;
                if (lockable != null)
                {
                    if (entity.Name == TriggersId)
                    {
                        lockables.Add(lockable);
                    }
                }
            }

            return lockables;
        }

        public void ToggleActive()
        {
            if (active)
            {
                active = false;
                ElementColor = Color.White;
            }
            else
            {
                active = true;
                ElementColor = ActiveColor;
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
                            new RenderText(AssetManager.WindowFont, "Triggers: " + TriggersId)
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