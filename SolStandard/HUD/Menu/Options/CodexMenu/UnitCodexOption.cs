using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Codex;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Menu.Options.CodexMenu
{
    public class UnitCodexOption : MenuOption
    {
        private readonly GameUnit unit;
        private const int PortraitSize = 96;

        public UnitCodexOption(GameUnit unit) :
            base(
                UnitLabelContent(unit.Role, unit.Team),
                CodexView.CodexWindowColor,
                HorizontalAlignment.Centered
            )
        {
            this.unit = unit;
        }

        private static IRenderable UnitLabelContent(Role role, Team team)
        {
            ITexture2D unitPortraitTexture = UnitGenerator.GetUnitPortrait(role, team);

            var unitPortraitSprite = new SpriteAtlas(
                unitPortraitTexture,
                new Vector2(unitPortraitTexture.Width, unitPortraitTexture.Height),
                new Vector2(PortraitSize)
            );

            IRenderable[,] unitInfoContent =
            {
                {
                    unitPortraitSprite
                },
                {
                    new RenderText(AssetManager.WindowFont, role.ToString().ToUpper())
                }
            };

            var unitInfoGrid = new WindowContentGrid(unitInfoContent, 5, HorizontalAlignment.Centered);

            return unitInfoGrid;
        }

        public override void Execute()
        {
            GlobalContext.CodexContext.ShowUnitDetails(unit);
        }

        public override IRenderable Clone()
        {
            return new UnitCodexOption(unit);
        }
    }
}