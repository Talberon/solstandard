using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class TakeSpoilsEvent : IEvent
    {
        private readonly Spoils spoils;

        public TakeSpoilsEvent(Spoils spoils)
        {
            this.spoils = spoils;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.InitiativeContext.AddGoldToTeam(spoils.Gold, GameContext.ActiveTeam);

            if (GameContext.ActiveUnit.IsAlive && spoils.Gold > 0)
            {
                GameContext.GameMapContext.PlayAnimationAtCoordinates(
                    AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.FallingCoins, GameDriver.CellSizeVector),
                    GameContext.ActiveUnit.UnitEntity.MapCoordinates
                );
            }

            foreach (IItem item in spoils.Items)
            {
                GameContext.ActiveUnit.AddItemToInventory(item);
            }

            RemoveItemFromMap();

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Retrieved spoils!", 50);
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