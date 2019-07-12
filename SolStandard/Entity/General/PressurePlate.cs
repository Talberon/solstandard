using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
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
        private const EffectTriggerTime TriggerTime = EffectTriggerTime.EndOfTurn;

        public PressurePlate(string name, string type, IRenderable sprite, Vector2 mapCoordinates, string triggersId,
            bool triggerOnRelease) :
            base(name, type, sprite, mapCoordinates)
        {
            this.triggersId = triggersId;
            this.triggerOnRelease = triggerOnRelease;
            wasPressed = false;
        }

        public bool IsExpired => false;

        public bool WillTrigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != TriggerTime) return false;

            return ((PlateIsPressed && !wasPressed) || (!PlateIsPressed && wasPressed));
        }

        public bool Trigger(EffectTriggerTime triggerTime)
        {
            if (triggerTime != TriggerTime) return false;

            if (PlateIsPressed)
            {
                if (!wasPressed && ToggleSwitchAction.NothingObstructingSwitchTarget(TriggerTiles))
                {
                    TriggerTiles.ForEach(tile => tile.RemoteTrigger());
                    AssetManager.DoorSFX.Play();
                    wasPressed = true;
                }
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

        private bool TriggeringOnRelease => wasPressed && triggerOnRelease;

        private List<IRemotelyTriggerable> TriggerTiles
        {
            get
            {
                List<IRemotelyTriggerable> fetchedTiles = new List<IRemotelyTriggerable>();

                foreach (MapElement element in MapContainer.GameGrid[(int) Layer.Entities])
                {
                    MapEntity entity = element as MapEntity;

                    if (entity is IRemotelyTriggerable triggerTile && entity.Name == triggersId)
                    {
                        fetchedTiles.Add(triggerTile);
                    }
                }

                return fetchedTiles;
            }
        }

        private bool PlateIsPressed => UnitIsStandingOnPressurePlate || ItemIsOnPressurePlate;

        private bool ItemIsOnPressurePlate
        {
            get
            {
                return MapContainer.GetMapElementsFromLayer(Layer.Items)
                    .Any(item => item.MapCoordinates == MapCoordinates);
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

        public override IRenderable TerrainInfo =>
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
                    },
                    {
                        new Window(
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
                            InnerWindowColor
                        ),
                        new RenderBlank()
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