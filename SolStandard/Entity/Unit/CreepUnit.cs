using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Events;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public class CreepUnit : GameUnit
    {
        private IRoutine nextRoutine;
        private readonly IRoutine fallbackRoutine;

        // ReSharper disable once SuggestBaseTypeForParameter
        public CreepUnit(string id, Team team, Role role, CreepEntity unitEntity, UnitStatistics stats,
            ITexture2D portrait, List<UnitAction> actions, bool isBoss, IRoutine fallbackRoutine) :
            base(id, team, role, unitEntity, stats, portrait, actions, isBoss)
        {
            this.fallbackRoutine = fallbackRoutine;
            ReadyNextRoutine();
        }

        private CreepEntity CreepEntity
        {
            get { return UnitEntity as CreepEntity; }
        }

        public void ExecuteNextRoutine()
        {
            MapSlice creepSlice = MapContainer.GetMapSliceAtCoordinates(UnitEntity.MapCoordinates);

            if (nextRoutine.CanExecute)
            {
                nextRoutine.ExecuteAction(creepSlice);
            }
            else
            {
                GlobalEventQueue.QueueSingleEvent(new ToastAtCursorEvent("Can't fulfill intentions!"));
                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));

                fallbackRoutine.ExecuteAction(creepSlice);
            }
        }

        public void ReadyNextRoutine()
        {
            List<IRoutine> readyableActions =
                Actions.Cast<IRoutine>().Where(routine => routine.CanBeReadied(this)).ToList();

            if (readyableActions.Count > 0)
            {
                IRoutine randomRoutine = readyableActions[GameDriver.Random.Next(readyableActions.Count)];
                UpdateUnitRoutine(randomRoutine);
            }
            else
            {
                UpdateUnitRoutine(fallbackRoutine);
            }
        }

        private void UpdateUnitRoutine(IRoutine newRoutine)
        {
            nextRoutine = newRoutine;
            CreepEntity.UpdateRoutineIcon(newRoutine);
        }
    }
}