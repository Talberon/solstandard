using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.General
{
    public enum SpringType
    {
        Free,
        Trap,
        Dark
    }

    public class SpringTrap : TerrainEntity, ITriggerable, IEffectTile
    {
        public int[] InteractRange { get; }
        private readonly Vector2 trapLaunchCoordinates;
        private readonly EffectTriggerTime effectTriggerTime;
        public bool HasTriggered { get; set; }

        public SpringTrap(string name, string type, Vector2 mapCoordinates, Vector2 trapLaunchCoordinates) :
            base(name, type, BuildSpringSprite(SpringType.Trap), mapCoordinates)
        {
            this.trapLaunchCoordinates = trapLaunchCoordinates;
            CanMove = true;
            InteractRange = new[] {0};
            effectTriggerTime = EffectTriggerTime.EndOfTurn;
            HasTriggered = false;
        }

        public static AnimatedSpriteSheet BuildSpringSprite(SpringType springType)
        {
            const int springCellSize = 48;
            var sprite = new AnimatedSpriteSheet(AssetManager.SpringTexture, springCellSize,
                GameDriver.CellSizeVector * 3, 6, false, Color.White);
            sprite.SetSpriteCell(0, (int) springType);
            sprite.Pause();
            return sprite;
        }

        public bool CanTrigger => UnitStandingOnSpring && !TargetTileIsObstructed;
        private bool UnitStandingOnSpring => MapContainer.GetMapSliceAtCoordinates(MapCoordinates).UnitEntity != null;

        private bool TargetTileIsObstructed
        {
            get
            {
                UnitEntity target = MapContainer.GetMapSliceAtCoordinates(MapCoordinates).UnitEntity;
                return !UnitMovingPhase.CanEndMoveAtCoordinates(target, trapLaunchCoordinates);
            }
        }


        public bool IsExpired => false;

        public bool Trigger(EffectTriggerTime triggerTime)
        {
            if (effectTriggerTime != triggerTime) return false;

            if (UnitStandingOnSpring)
            {
                UnitEntity unitEntityOnSpring = MapContainer.GetMapSliceAtCoordinates(MapCoordinates).UnitEntity;
                GameUnit unitOnSpring = UnitSelector.SelectUnit(unitEntityOnSpring);
                HasTriggered = true;

                if (!TargetTileIsObstructed)
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(unitEntityOnSpring,
                        "LAUNCHED!", 50);
                    (Sprite as AnimatedSpriteSheet)?.PlayOnce();
                    MoveUnitToCoordinates(unitOnSpring, trapLaunchCoordinates);
                    AssetManager.DoorSFX.Play();
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(unitEntityOnSpring,
                        "Destination is obstructed; can't launch!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCellCoordinates(
                    "No unit on spring!", MapCoordinates, 50);
                AssetManager.WarningSFX.Play();
            }

            return true;
        }

        public bool WillTrigger(EffectTriggerTime triggerTime)
        {
            return effectTriggerTime == triggerTime && UnitStandingOnSpring && !HasTriggered;
        }

        public void Trigger()
        {
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
        }

        private static void MoveUnitToCoordinates(GameUnit unitOnSpring, Vector2 targetCoordinates)
        {
            unitOnSpring.MoveUnitToCoordinates(targetCoordinates);
        }


        protected override IRenderable EntityInfo =>
            new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                        new RenderText(AssetManager.WindowFont, "Target: " + trapLaunchCoordinates)
                    }
                }
            );
    }
}