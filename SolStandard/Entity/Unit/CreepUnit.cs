using System.Collections.Generic;
using System.Linq;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Statuses.Creep;
using SolStandard.Map;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Events;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit
{
    public class CreepUnit : GameUnit
    {
        private IRoutine nextRoutine;
        private readonly IRoutine fallbackRoutine;

        public CreepUnit(string id, Team team, Role role, CreepEntity unitEntity, UnitStatistics stats,
            ITexture2D portrait, bool isBoss) :
            base(id, team, role, unitEntity, stats, portrait, unitEntity.Model.Actions, isBoss)
        {
            fallbackRoutine = unitEntity.Model.FallbackRoutine;
            if (unitEntity.Model.IsIndependent) AddStatusEffect(new IndependentStatus());
        }

        private CreepEntity CreepEntity => UnitEntity as CreepEntity;

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
                GlobalEventQueue.QueueSingleEvent(new SkippableWaitFramesEvent(30));

                fallbackRoutine.ExecuteAction(creepSlice);
            }
        }

        public void ReadyNextRoutine()
        {
            List<IRoutine> readyableActions =
                Actions.Cast<IRoutine>().Where(routine => routine.CanBeReadied(this)).ToList();

            if (readyableActions.Count > 0)
            {
                //TODO Weigh various routines so that they are not all equally likely
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
            AddStatusEffect(new NextRoutineStatus(newRoutine));
        }
    }
}