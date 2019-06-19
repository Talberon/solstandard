using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.General
{
    public class Switch : TerrainEntity, IActionTile, ITriggerable
    {
        public int[] InteractRange { get; private set; }
        private string TriggersId { get; set; }
        private bool active;
        private static readonly Color ActiveColor = new Color(180, 180, 180);

        public Switch(string name, string type, IRenderable sprite, Vector2 mapCoordinates, string triggersId) :
            base(name, type, sprite, mapCoordinates)
        {
            TriggersId = triggersId;
            InteractRange = new[] {1};
            CanMove = false;
            active = false;
        }

        public List<UnitAction> TileActions()
        {
            List<IRemotelyTriggerable> targetTriggerables = FindRemotelyTriggerables();
            return new List<UnitAction>
            {
                new ToggleSwitchAction(this, targetTriggerables)
            };
        }

        private List<IRemotelyTriggerable> FindRemotelyTriggerables()
        {
            List<IRemotelyTriggerable> remotelyTriggerables = new List<IRemotelyTriggerable>();

            foreach (MapElement mapElement in MapContainer.GameGrid[(int) Layer.Entities])
            {
                MapEntity entity = (MapEntity) mapElement;
                IRemotelyTriggerable lockable = mapElement as IRemotelyTriggerable;
                if (lockable != null)
                {
                    if (entity.Name == TriggersId)
                    {
                        remotelyTriggerables.Add(lockable);
                    }
                }
            }

            return remotelyTriggerables;
        }

        public void Trigger()
        {
            if (!CanTrigger) return;

            UnitAction toggleAction = TileActions().First();
            toggleAction.GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
            toggleAction.ExecuteAction(MapContainer.GetMapSliceAtCoordinates(MapCoordinates));
            MapContainer.ClearDynamicAndPreviewGrids();
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
        }

        public bool CanTrigger
        {
            get { return true; }
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