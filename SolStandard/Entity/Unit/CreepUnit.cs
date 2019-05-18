using System.Collections.Generic;
using SolStandard.Containers;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Map.Elements.Cursor;
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
                fallbackRoutine.ExecuteAction(creepSlice);
            }
        }

        public void ReadyNextRoutine()
        {
            UnitAction randomRoutine = Actions[GameDriver.Random.Next(Actions.Count)];
            UpdateUnitRoutine(randomRoutine as IRoutine);
        }

        private void UpdateUnitRoutine(IRoutine newRoutine)
        {
            nextRoutine = newRoutine;
            CreepEntity.UpdateRoutineIcon(newRoutine);
        }
    }
}