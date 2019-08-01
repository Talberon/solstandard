using System;
using System.Collections.Generic;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Actions.Lancer;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Paladin
{
    public class Rescue : UnitAction
    {
        private enum ActionPhase
        {
            SelectTarget,
            SelectLandingSpace
        }

        private ActionPhase currentPhase = ActionPhase.SelectTarget;
        private const MapDistanceTile.TileType ActionTileType = MapDistanceTile.TileType.Action;

        public Rescue() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Rescue, GameDriver.CellSizeVector),
            name: "Rescue",
            description: "Leap towards an ally in need!" + Environment.NewLine +
                         "Select a target, then select a space to land on next to that target.",
            tileSprite: MapDistanceTile.GetTileSprite(ActionTileType),
            range: new[] {1, 2, 3},
            freeAction: false
        )
        {
        }

        public override void CancelAction()
        {
            currentPhase = ActionPhase.SelectTarget;
            base.CancelAction();
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            switch (currentPhase)
            {
                case ActionPhase.SelectTarget:
                    if (SelectTarget(targetSlice)) currentPhase = ActionPhase.SelectLandingSpace;
                    break;
                case ActionPhase.SelectLandingSpace:
                    if (SelectLandingSpace(targetSlice)) currentPhase = ActionPhase.SelectTarget;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool SelectTarget(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                if (!LeapStrike.SpaceAroundUnitIsEntirelyObstructed(targetUnit))
                {
                    MapContainer.ClearDynamicAndPreviewGrids();
                    LeapStrike.CreateLandingSpacesAroundTarget(ActionTileType, targetUnit.UnitEntity.MapCoordinates);
                    AssetManager.MenuConfirmSFX.Play();
                    return true;
                }

                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("No space to land!", 50);
                AssetManager.WarningSFX.Play();
                return false;
            }

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not an ally in range!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }

        private static bool SelectLandingSpace(MapSlice targetSlice)
        {
            if (targetSlice.DynamicEntity != null && !LeapStrike.CoordinatesAreObstructed(targetSlice.MapCoordinates))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new MoveEntityToCoordinatesEvent(GameContext.ActiveUnit.UnitEntity,
                    targetSlice.MapCoordinates));
                eventQueue.Enqueue(new PlaySoundEffectEvent(AssetManager.CombatDamageSFX));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
                return true;
            }

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Invalid landing space!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }
    }
}