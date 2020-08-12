using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.General
{
    public class PressurePlate : TerrainEntity, IEffectTile, ITriggerable
    {
        private readonly string triggersId;
        private bool wasPressed;
        private readonly bool triggerOnRelease;
        public bool HasTriggered { get; set; }
        private const EffectTriggerTime TriggerTime = EffectTriggerTime.EndOfTurn;
        private int? lastOccupant;

        public PressurePlate(string name, string type, IRenderable sprite, Vector2 mapCoordinates, string triggersId,
            bool triggerOnRelease) :
            base(name, type, sprite, mapCoordinates)
        {
            this.triggersId = triggersId;
            this.triggerOnRelease = triggerOnRelease;
            wasPressed = false;
            HasTriggered = false;
            lastOccupant = null;
        }

        public bool IsExpired => false;

        public bool WillTrigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != TriggerTime || HasTriggered) return false;

            bool plateStateChanged = ((PlateIsPressed && !wasPressed) || (!PlateIsPressed && wasPressed));
            
            return plateStateChanged;
        }


        public bool Trigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != TriggerTime || HasTriggered) return false;

            HasTriggered = true;

            if (PlateIsPressed)
            {
                if (
                    (wasPressed && lastOccupant == CurrentOccupant) ||
                    !ToggleSwitchAction.NothingObstructingSwitchTarget(TriggerTiles)
                ) return true;
                
                lastOccupant = CurrentOccupant;
                TriggerTiles.ToList().ForEach(tile => tile.RemoteTrigger());
                wasPressed = true;
            }
            else
            {
                if (TriggeringOnRelease && ToggleSwitchAction.NothingObstructingSwitchTarget(TriggerTiles))
                {
                    TriggerTiles.ForEach(tile => tile.RemoteTrigger());
                    AssetManager.DoorSFX.Play();
                }

                wasPressed = false;
            }

            return true;
        }

        private int? CurrentOccupant
        {
            get
            {
                MapSlice plateSlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
                return plateSlice.UnitEntity?.GetHashCode() ?? plateSlice.ItemEntity?.GetHashCode();
            }
        }

        private bool TriggeringOnRelease => wasPressed && triggerOnRelease;

        private List<IRemotelyTriggerable> TriggerTiles
        {
            get
            {
                var fetchedTiles = new List<IRemotelyTriggerable>();

                foreach (MapElement element in MapContainer.GameGrid[(int) Layer.Entities])
                {
                    var entity = element as MapEntity;

                    if (entity is IRemotelyTriggerable triggerTile && entity.Name == triggersId)
                    {
                        fetchedTiles.Add(triggerTile);
                    }
                }

                return fetchedTiles;
            }
        }

        private bool PlateIsPressed => UnitIsStandingOnPressurePlate || ItemIsOnPressurePlate;

        private bool ItemIsOnPressurePlate => MapContainer.GetMapElementsFromLayer(Layer.Items)
            .Any(item => item.MapCoordinates == MapCoordinates);

        private bool UnitIsStandingOnPressurePlate =>
            GlobalContext.Units.Any(unit => unit.UnitEntity != null && unit.UnitEntity.MapCoordinates == MapCoordinates);

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                        new RenderText(AssetManager.WindowFont, "Triggers: " + triggersId)
                    },
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                        new RenderText(
                            AssetManager.WindowFont,
                            (triggerOnRelease) ? "On Press/Release" : "On Press"
                        )
                    }
                },
                1,
                HorizontalAlignment.Centered
            );

        public bool CanTrigger => true;

        public int[] InteractRange => new[] {0};

        public void Trigger()
        {
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
        }
    }
}