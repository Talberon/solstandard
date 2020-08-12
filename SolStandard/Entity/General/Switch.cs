using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
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
        public int[] InteractRange { get; }
        private string TriggersId { get; }
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
            var remotelyTriggerables = new List<IRemotelyTriggerable>();

            foreach (MapElement mapElement in MapContainer.GameGrid[(int) Layer.Entities])
            {
                var entity = (MapEntity) mapElement;
                if (mapElement is IRemotelyTriggerable lockable)
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
            toggleAction.GenerateActionGrid(GlobalContext.ActiveUnit.UnitEntity.MapCoordinates);
            toggleAction.ExecuteAction(MapContainer.GetMapSliceAtCoordinates(MapCoordinates));
            MapContainer.ClearDynamicAndPreviewGrids();
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
        }

        public bool CanTrigger => true;

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

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                        new RenderText(AssetManager.WindowFont, "Triggers: " + TriggersId)
                    },
                }
            );
    }
}