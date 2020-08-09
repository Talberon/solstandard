using System;
using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Archer
{
    public class CmdHuntingCompanion : UnitAction, ICommandAction
    {
        private readonly int cmdCost;
        private const Role PetType = Role.Boar;

        public CmdHuntingCompanion(int cmdCost) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Hunting Companion",
            description: $"Summon a {PetType.ToString()} companion to aid you in combat!" + Environment.NewLine +
                         $"If a {PetType.ToString()} is already on the team, it will be removed and replaced with a new one." +
                         Environment.NewLine +
                         $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: false
        )
        {
            this.cmdCost = cmdCost;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit actor = GlobalContext.ActiveUnit;

            if (!CanAffordCommandCost(actor, cmdCost))
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                    $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            if (SpawnUnitAction.TargetIsUnoccupiedTileInRange(targetSlice))
            {
                actor.RemoveCommandPoints(cmdCost);
                
                if (CompanionAlreadySummoned)
                {
                    GameUnit summonedPet = GlobalContext.Units.FirstOrDefault(pet =>
                        pet.Role == PetType && pet.Team == actor.Team);

                    if (summonedPet != null)
                    {
                        for (int i = 0; i < summonedPet.Stats.CurrentHP; i++)
                        {
                            summonedPet.DamageUnit(true);
                        }

                        GlobalContext.Units.Remove(summonedPet);
                    }
                }

                new SpawnUnitAction(PetType).ExecuteAction(targetSlice);

                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));
                GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must target unoccupied tile!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool CompanionAlreadySummoned
        {
            get
            {
                GameUnit summonedPet =
                    GlobalContext.Units.FirstOrDefault(pet =>
                        pet.Role == PetType && pet.Team == GlobalContext.ActiveTeam);

                return summonedPet != null;
            }
        }
    }
}