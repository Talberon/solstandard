﻿using SolStandard.Containers;
using SolStandard.Containers.Contexts;
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
            GameContext.InitiativeContext.AddGoldToTeam(spoils.Gold, GameContext.ActiveTeam);

            if (unitTakingSpoils.IsAlive && spoils.Gold > 0)
            {
                GameContext.GameMapContext.PlayAnimationAtCoordinates(
                    AnimatedIconProvider.GetAnimatedIcon(AnimatedIconType.FallingCoins, GameDriver.CellSizeVector),
                    unitTakingSpoils.UnitEntity.MapCoordinates
                );
            }

            foreach (IItem item in spoils.Items)
            {
                unitTakingSpoils.AddItemToInventory(item);
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