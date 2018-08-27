using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.UI
{
    public class BattleUI : IUserInterface
    {
        private readonly Vector2 screenSize;
        private const int WindowEdgeBuffer = 5;
        private const int WindowSpacing = 5;

        //TODO decide if this should stay or be removed
        public Window DebugWindow { get; set; }

        public Window AttackerLabelWindow { get; set; }
        public Window AttackerPortraitWindow { get; set; }
        public Window AttackerHpWindow { get; set; }
        public Window AttackerAtkWindow { get; set; }
        public Window AttackerBonusWindow { get; set; }
        public Window AttackerRangeWindow { get; set; }
        public Window AttackerDiceLabelWindow { get; set; }
        public Window AttackerDiceWindow { get; set; }

        public Window DefenderLabelWindow { get; set; }
        public Window DefenderPortraitWindow { get; set; }
        public Window DefenderHpWindow { get; set; }
        public Window DefenderAtkWindow { get; set; }
        public Window DefenderBonusWindow { get; set; }
        public Window DefenderRangeWindow { get; set; }
        public Window DefenderDiceLabelWindow { get; set; }
        public Window DefenderDiceWindow { get; set; }

        public Window HelpTextWindow { get; set; }

        private bool visible;

        public BattleUI(Vector2 screenSize)
        {
            this.screenSize = screenSize;
            visible = true;
        }

        private Vector2 HelpTextWindowPosition()
        {
            //Top-left
            return new Vector2(WindowEdgeBuffer);
        }

        #region Attacker

        private Vector2 AttackerLabelWindowPosition()
        {
            //Top-left, below help window
            return new Vector2(WindowEdgeBuffer, HelpTextWindowPosition().Y + HelpTextWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerPortraitWindowPosition()
        {
            //Anchored beneath below label window
            Vector2 attackerLabelWindowPosition = AttackerLabelWindowPosition();

            return new Vector2(attackerLabelWindowPosition.X,
                attackerLabelWindowPosition.Y + AttackerLabelWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerHpWindowPosition()
        {
            //Anchored beneath portrait window
            Vector2 attackerPortraitWindowPosition = AttackerPortraitWindowPosition();

            return new Vector2(attackerPortraitWindowPosition.X,
                attackerPortraitWindowPosition.Y + AttackerPortraitWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerAtkWindowPosition()
        {
            //Anchored beneath HP window
            Vector2 attackerHpWindowPosition = AttackerHpWindowPosition();

            return new Vector2(attackerHpWindowPosition.X,
                attackerHpWindowPosition.Y + AttackerHpWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerBonusWindowPosition()
        {
            //Anchored beneath ATK window
            Vector2 attackerAtkWindowPosition = AttackerAtkWindowPosition();

            return new Vector2(attackerAtkWindowPosition.X,
                attackerAtkWindowPosition.Y + AttackerAtkWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerRangeWindowPosition()
        {
            //Anchored beneath Bonus window
            Vector2 attackerBonusWindowPosition = AttackerBonusWindowPosition();

            return new Vector2(attackerBonusWindowPosition.X,
                attackerBonusWindowPosition.Y + AttackerBonusWindow.Height + WindowSpacing);
        }

        private Vector2 AttackerDiceLabelPosition()
        {
            //Anchored right of class label window
            Vector2 attackerLabelWindowPosition = AttackerLabelWindowPosition();

            return new Vector2(attackerLabelWindowPosition.X + AttackerLabelWindow.Width + WindowSpacing,
                attackerLabelWindowPosition.Y);
        }

        private Vector2 AttackerDicePosition()
        {
            //Anchored right of class portrait window
            Vector2 attackerPortraitWindowPosition = AttackerPortraitWindowPosition();

            return new Vector2(attackerPortraitWindowPosition.X + AttackerPortraitWindow.Width + WindowSpacing,
                attackerPortraitWindowPosition.Y);
        }

        #endregion Attacker

        #region Defender

        private Vector2 DefenderLabelWindowPosition()
        {
            //Top-left, below help window
            return new Vector2(screenSize.X - DefenderLabelWindow.Width - WindowEdgeBuffer,
                HelpTextWindowPosition().Y + HelpTextWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderPortraitWindowPosition()
        {
            //Anchored beneath below label window
            Vector2 defenderLabelWindowPosition = DefenderLabelWindowPosition();

            return new Vector2(screenSize.X - DefenderPortraitWindow.Width - defenderLabelWindowPosition.X,
                defenderLabelWindowPosition.Y + DefenderLabelWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderHpWindowPosition()
        {
            //Anchored beneath portrait window
            Vector2 defenderPortraitWindowPosition = DefenderPortraitWindowPosition();

            return new Vector2(screenSize.X - DefenderHpWindow.Width - defenderPortraitWindowPosition.X,
                defenderPortraitWindowPosition.Y + DefenderPortraitWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderAtkWindowPosition()
        {
            //Anchored beneath HP window
            Vector2 defenderHpWindowPosition = DefenderHpWindowPosition();

            return new Vector2(screenSize.X - DefenderAtkWindow.Width - defenderHpWindowPosition.X,
                defenderHpWindowPosition.Y + DefenderHpWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderBonusWindowPosition()
        {
            //Anchored beneath ATK window
            Vector2 defenderAtkWindowPosition = DefenderAtkWindowPosition();

            return new Vector2(screenSize.X - DefenderBonusWindow.Width - defenderAtkWindowPosition.X,
                defenderAtkWindowPosition.Y + DefenderHpWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderRangeWindowPosition()
        {
            //Anchored beneath Bonus window
            Vector2 defenderBonusWindowPosition = DefenderBonusWindowPosition();

            return new Vector2(screenSize.X - DefenderRangeWindow.Width - defenderBonusWindowPosition.X,
                defenderBonusWindowPosition.Y + DefenderBonusWindow.Height + WindowSpacing);
        }

        private Vector2 DefenderDiceLabelPosition()
        {
            //Anchored right of class label window
            Vector2 defenderLabelWindowPosition = DefenderLabelWindowPosition();

            return new Vector2(
                screenSize.X - DefenderDiceLabelWindow.Width - defenderLabelWindowPosition.X -
                DefenderLabelWindow.Width - WindowSpacing,
                defenderLabelWindowPosition.Y);
        }

        private Vector2 DefenderDicePosition()
        {
            //Anchored right of class portrait window
            Vector2 defenderPortraitWindowPosition = DefenderPortraitWindowPosition();

            return new Vector2(
                screenSize.X - DefenderDiceWindow.Width - defenderPortraitWindowPosition.X - DefenderLabelWindow.Width -
                WindowSpacing,
                defenderPortraitWindowPosition.Y);
        }

        #endregion Defender

        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (HelpTextWindow != null)
            {
                HelpTextWindow.Draw(spriteBatch, HelpTextWindowPosition());

                if (AttackerPortraitWindow != null)
                {
                    AttackerLabelWindow.Draw(spriteBatch, AttackerLabelWindowPosition());
                    AttackerPortraitWindow.Draw(spriteBatch, AttackerPortraitWindowPosition());
                    AttackerHpWindow.Draw(spriteBatch, AttackerHpWindowPosition());
                    AttackerAtkWindow.Draw(spriteBatch, AttackerAtkWindowPosition());
                    AttackerBonusWindow.Draw(spriteBatch, AttackerBonusWindowPosition());
                    AttackerRangeWindow.Draw(spriteBatch, AttackerRangeWindowPosition());
                    AttackerDiceLabelWindow.Draw(spriteBatch, AttackerDiceLabelPosition());
                    AttackerDiceWindow.Draw(spriteBatch, AttackerDicePosition());
                }

                if (DefenderPortraitWindow != null)
                {
                    DefenderLabelWindow.Draw(spriteBatch, DefenderLabelWindowPosition());
                    DefenderPortraitWindow.Draw(spriteBatch, DefenderPortraitWindowPosition());
                    DefenderHpWindow.Draw(spriteBatch, DefenderHpWindowPosition());
                    DefenderAtkWindow.Draw(spriteBatch, DefenderAtkWindowPosition());
                    DefenderBonusWindow.Draw(spriteBatch, DefenderBonusWindowPosition());
                    DefenderRangeWindow.Draw(spriteBatch, DefenderRangeWindowPosition());
                    DefenderDiceLabelWindow.Draw(spriteBatch, DefenderDiceLabelPosition());
                    DefenderDiceWindow.Draw(spriteBatch, DefenderDicePosition());
                }
            }
        }
    }
}