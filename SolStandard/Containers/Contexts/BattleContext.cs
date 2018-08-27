using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts
{
    public class BattleContext
    {
        private readonly BattleUI battleUI;
        private readonly ITexture2D windowTexture;
        private const int HpBarHeight = 20;

        public BattleContext(BattleUI battleUI, ITexture2D windowTexture)
        {
            this.battleUI = battleUI;
            this.windowTexture = windowTexture;
            //TODO determine CombatFlow
        }

        public void SetupCombatUI(GameUnit attacker, MapSlice attackerSlice,
            GameUnit defender, MapSlice defenderSlice)
        {
            //Help Window
            IRenderable textToRender = new RenderText(GameDriver.WindowFont, "Sample Combat Help Text");
            battleUI.HelpTextWindow =
                new Window("Help Window", windowTexture, textToRender, new Color(30, 30, 30, 150));

            SetupAttackerWindows(attacker, attackerSlice);
            SetupDefenderWindows(defender, defenderSlice);
        }

        private void SetupAttackerWindows(GameUnit attacker, MapSlice attackerSlice)
        {
            Color attackerWindowColor = MapHudGenerator.DetermineTeamColor(attacker.UnitTeam);

            //Portrait Window
            IRenderable attackerPortrait =
                new WindowContent(new TextureCell(attacker.LargePortrait, attacker.LargePortrait.Height, 1));
            battleUI.AttackerPortraitWindow =
                new Window("Attacker Portrait", windowTexture, attackerPortrait, attackerWindowColor);


            Vector2 portraitWidthOverride = new Vector2(battleUI.AttackerPortraitWindow.Width, 0);

            //Name Label Window
            IRenderable attackerLabelText = new RenderText(GameDriver.WindowFont, attacker.Id);
            battleUI.AttackerLabelWindow = new Window("Attacker Label", windowTexture, attackerLabelText,
                attackerWindowColor, portraitWidthOverride);

            //HP Window
            IRenderable[,] attackerHpContent = new IRenderable[1, 2];
            IRenderable hpLabel = new WindowContent(new RenderText(GameDriver.WindowFont, "HP:"));
            Vector2 hpBarSize = new Vector2(attacker.LargePortrait.Width - hpLabel.Width, HpBarHeight);
            IRenderable hpBar = attacker.GetCustomHealthBar(hpBarSize);
            attackerHpContent[0, 0] = hpLabel;
            attackerHpContent[0, 1] = hpBar;
            WindowContentGrid attackerHpContentGrid = new WindowContentGrid(attackerHpContent, 1);
            battleUI.AttackerHpWindow =
                new Window("Attacker HP Bar", windowTexture, attackerHpContentGrid, attackerWindowColor,
                    portraitWidthOverride);

            //ATK Window
            IRenderable[,] attackerAtkContent = new IRenderable[1, 2];
            IRenderable atkLabel = new WindowContent(new RenderText(GameDriver.WindowFont, "ATK:"));
            IRenderable atkValue =
                new WindowContent(new RenderText(GameDriver.WindowFont, attacker.Stats.Atk.ToString()));
            attackerAtkContent[0, 0] = atkLabel;
            attackerAtkContent[0, 1] = atkValue;
            WindowContentGrid attackerAtkContentGrid = new WindowContentGrid(attackerAtkContent, 1);
            battleUI.AttackerAtkWindow = new Window("Attacker ATK Info", windowTexture, attackerAtkContentGrid,
                attackerWindowColor, portraitWidthOverride);

            //Bonus Window
            string terrainAttackBonus = "0";
            if (attackerSlice.GeneralEntity != null)
            {
                if (attackerSlice.GeneralEntity.TiledProperties.ContainsKey("Stat") &&
                    attackerSlice.GeneralEntity.TiledProperties["Stat"] == "ATK")
                {
                    if (attackerSlice.GeneralEntity.TiledProperties.ContainsKey("Modifier"))
                    {
                        terrainAttackBonus = attackerSlice.GeneralEntity.TiledProperties["Modifier"];
                    }
                }
            }

            IRenderable[,] attackerBonusContent = new IRenderable[1, 2];
            IRenderable bonusLabel = new WindowContent(new RenderText(GameDriver.WindowFont, "Bonus:"));
            IRenderable bonusValue =
                new WindowContent(new RenderText(GameDriver.WindowFont, terrainAttackBonus));
            attackerBonusContent[0, 0] = bonusLabel;
            attackerBonusContent[0, 1] = bonusValue;
            WindowContentGrid attackerBonusContentGrid = new WindowContentGrid(attackerBonusContent, 1);
            battleUI.AttackerBonusWindow = new Window("Attacker Bonus Info", windowTexture, attackerBonusContentGrid,
                attackerWindowColor, portraitWidthOverride);

            //AttackerRangeWindow
            string attackerInRange = true.ToString();
            IRenderable[,] attackerRangeContent = new IRenderable[1, 2];
            IRenderable rangeLabel = new WindowContent(new RenderText(GameDriver.WindowFont, "In Range:"));
            IRenderable rangeValue =
                new WindowContent(new RenderText(GameDriver.WindowFont, attackerInRange));
            attackerRangeContent[0, 0] = rangeLabel;
            attackerRangeContent[0, 1] = rangeValue;
            WindowContentGrid attackerRangeContentGrid = new WindowContentGrid(attackerRangeContent, 1);
            battleUI.AttackerRangeWindow = new Window("Attacker Range Info", windowTexture, attackerRangeContentGrid,
                attackerWindowColor, portraitWidthOverride);

            //Dice Label Window
            battleUI.AttackerDiceLabelWindow = new Window("Dice Label", windowTexture,
                new RenderText(GameDriver.WindowFont, "Attacking"), attackerWindowColor);

            battleUI.AttackerDiceWindow = new Window("Dice Rolling Window", windowTexture,
                new RenderText(GameDriver.WindowFont, "PLACEHOLDER\nDICE WILL GO HERE"), attackerWindowColor);
        }

        //FIXME This is almost entirely duplicated logic and it's ugly; figure out how to DRY this
        private void SetupDefenderWindows(GameUnit defender, MapSlice defenderSlice)
        {
            Color defenderWindowColor = MapHudGenerator.DetermineTeamColor(defender.UnitTeam);

            //Portrait Window
            IRenderable defenderPortrait =
                new WindowContent(new TextureCell(defender.LargePortrait, defender.LargePortrait.Height, 1));
            battleUI.DefenderPortraitWindow =
                new Window("Defender Portrait", windowTexture, defenderPortrait, defenderWindowColor);


            Vector2 portraitWidthOverride = new Vector2(battleUI.DefenderPortraitWindow.Width, 0);

            //Name Label Window
            IRenderable defenderLabelText = new RenderText(GameDriver.WindowFont, defender.Id);
            battleUI.DefenderLabelWindow = new Window("Defender Label", windowTexture, defenderLabelText,
                defenderWindowColor, portraitWidthOverride);

            //HP Window
            IRenderable[,] defenderHpContent = new IRenderable[1, 2];
            IRenderable hpLabel = new WindowContent(new RenderText(GameDriver.WindowFont, "HP:"));
            Vector2 hpBarSize = new Vector2(defender.LargePortrait.Width - hpLabel.Width, HpBarHeight);
            IRenderable hpBar = defender.GetCustomHealthBar(hpBarSize);
            defenderHpContent[0, 0] = hpLabel;
            defenderHpContent[0, 1] = hpBar;
            WindowContentGrid defenderHpContentGrid = new WindowContentGrid(defenderHpContent, 1);
            battleUI.DefenderHpWindow =
                new Window("Defender HP Bar", windowTexture, defenderHpContentGrid, defenderWindowColor,
                    portraitWidthOverride);

            //DEF Window
            IRenderable[,] defenderAtkContent = new IRenderable[1, 2];
            IRenderable atkLabel = new WindowContent(new RenderText(GameDriver.WindowFont, "DEF:"));
            IRenderable atkValue =
                new WindowContent(new RenderText(GameDriver.WindowFont, defender.Stats.Def.ToString()));
            defenderAtkContent[0, 0] = atkLabel;
            defenderAtkContent[0, 1] = atkValue;
            WindowContentGrid defenderAtkContentGrid = new WindowContentGrid(defenderAtkContent, 1);
            battleUI.DefenderAtkWindow = new Window("Defender DEF Info", windowTexture, defenderAtkContentGrid,
                defenderWindowColor, portraitWidthOverride);

            //Bonus Window
            string terrainAttackBonus = "0";
            if (defenderSlice.GeneralEntity != null)
            {
                if (defenderSlice.GeneralEntity.TiledProperties.ContainsKey("Stat") &&
                    defenderSlice.GeneralEntity.TiledProperties["Stat"] == "DEF")
                {
                    if (defenderSlice.GeneralEntity.TiledProperties.ContainsKey("Modifier"))
                    {
                        terrainAttackBonus = defenderSlice.GeneralEntity.TiledProperties["Modifier"];
                    }
                }
            }

            IRenderable[,] defenderBonusContent = new IRenderable[1, 2];
            IRenderable bonusLabel = new WindowContent(new RenderText(GameDriver.WindowFont, "Bonus:"));
            IRenderable bonusValue =
                new WindowContent(new RenderText(GameDriver.WindowFont, terrainAttackBonus));
            defenderBonusContent[0, 0] = bonusLabel;
            defenderBonusContent[0, 1] = bonusValue;
            WindowContentGrid defenderBonusContentGrid = new WindowContentGrid(defenderBonusContent, 1);
            battleUI.DefenderBonusWindow = new Window("Defender Bonus Info", windowTexture, defenderBonusContentGrid,
                defenderWindowColor, portraitWidthOverride);

            //DefenderRangeWindow
            string defenderInRange = true.ToString(); //TODO For defender, check range
            IRenderable[,] defenderRangeContent = new IRenderable[1, 2];
            IRenderable rangeLabel = new WindowContent(new RenderText(GameDriver.WindowFont, "In Range:"));
            IRenderable rangeValue =
                new WindowContent(new RenderText(GameDriver.WindowFont, defenderInRange));
            defenderRangeContent[0, 0] = rangeLabel;
            defenderRangeContent[0, 1] = rangeValue;
            WindowContentGrid defenderRangeContentGrid = new WindowContentGrid(defenderRangeContent, 1);
            battleUI.DefenderRangeWindow = new Window("Defender Range Info", windowTexture, defenderRangeContentGrid,
                defenderWindowColor, portraitWidthOverride);

            //Dice Label Window
            battleUI.DefenderDiceLabelWindow = new Window("Dice Label", windowTexture,
                new RenderText(GameDriver.WindowFont, "Defending"), defenderWindowColor);

            battleUI.DefenderDiceWindow = new Window("Dice Rolling Window", windowTexture,
                new RenderText(GameDriver.WindowFont, "PLACEHOLDER\nDICE WILL GO HERE"), defenderWindowColor);
        }

        //TODO Figure out if this is where the draw should sit
        public void Draw(SpriteBatch spriteBatch)
        {
            battleUI.Draw(spriteBatch);
        }
    }
}