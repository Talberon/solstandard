using SolStandard.Containers.Components.Global;
using SolStandard.Entity;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class TakeSpoilsEvent : IEvent
    {
        private readonly Spoils spoils;
        private readonly GameUnit unitTakingSpoils;

        public TakeSpoilsEvent(Spoils spoils, GameUnit unitTakingSpoils)
        {
            this.spoils = spoils;
            this.unitTakingSpoils = unitTakingSpoils;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            GlobalContext.InitiativePhase.AddGoldToTeam(spoils.Gold, GlobalContext.ActiveTeam);

            if (unitTakingSpoils.IsAlive && spoils.Gold > 0)
            {
                GlobalContext.WorldContext.PlayAnimationAtCoordinates(
                    AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.FallingCoins, GameDriver.CellSizeVector),
                    unitTakingSpoils.UnitEntity.MapCoordinates
                );
            }

            foreach (IItem item in spoils.Items)
            {
                unitTakingSpoils.AddItemToInventory(item);
            }

            RemoveItemFromMap();

            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Retrieved spoils!", 50);
            AssetManager.MenuConfirmSFX.Play();

            Complete = true;
        }


        private void RemoveItemFromMap()
        {
            MapContainer.GameGrid[(int) Layer.Items][(int) spoils.MapCoordinates.X, (int) spoils.MapCoordinates.Y] =
                null;
        }
    }
}