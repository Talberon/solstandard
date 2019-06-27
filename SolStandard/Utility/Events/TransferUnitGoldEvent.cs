using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class TransferUnitGoldEvent : IEvent
    {
        private readonly GameUnit givingUnit;
        private readonly GameUnit receivingUnit;
        private readonly int goldToGive;
        private readonly IRenderable icon;

        public TransferUnitGoldEvent(GameUnit givingUnit, GameUnit receivingUnit, int goldToGive, IRenderable icon)
        {
            this.givingUnit = givingUnit;
            this.receivingUnit = receivingUnit;
            this.goldToGive = goldToGive;
            this.icon = icon;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            givingUnit.CurrentGold -= goldToGive;
            receivingUnit.CurrentGold += goldToGive;

            IRenderable toastContent = new WindowContentGrid(
                new[,]
                {
                    {
                        SpriteResizer.TryResizeRenderable(icon, new Vector2(MapContainer.MapToastIconSize)),
                        new RenderText(
                            AssetManager.MapFont,
                            $"{givingUnit.Id} gave {goldToGive + Currency.CurrencyAbbreviation} to {receivingUnit.Id}!"
                        )
                    }
                },
                1
            );

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(toastContent, 50);
            AssetManager.MenuConfirmSFX.Play();
            
            GameMapContext.GameMapView.GenerateObjectiveWindow();

            Complete = true;
        }
    }
}