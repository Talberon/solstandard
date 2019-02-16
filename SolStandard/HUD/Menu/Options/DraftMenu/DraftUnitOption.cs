using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Menu.Options.DraftMenu
{
    public class DraftUnitOption : MenuOption
    {
        private readonly Role role;
        private readonly Team team;
        private const int PortraitSize = 128;

        public DraftUnitOption(Role role, Team team)
            : base(
                DraftUnitLabelContent(role, team),
                TeamUtility.DetermineTeamColor(team),
                HorizontalAlignment.Centered
            )
        {
            this.role = role;
            this.team = team;
        }

        private static IRenderable DraftUnitLabelContent(Role role, Team team)
        {
            ITexture2D unitPortraitTexture = UnitGenerator.GetUnitPortrait(role, team);

            return new SpriteAtlas(
                unitPortraitTexture,
                new Vector2(unitPortraitTexture.Width, unitPortraitTexture.Height),
                new Vector2(PortraitSize)
            );
        }

        public override void Execute()
        {
            GameContext.DraftContext.AddUnitToList(role, team);
        }

        public override IRenderable Clone()
        {
            return new DraftUnitOption(role, team);
        }
    }
}