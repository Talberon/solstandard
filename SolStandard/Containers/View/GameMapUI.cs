using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Window;

namespace SolStandard.Containers.View
{
    /*
     * GameMapController is where the HUD elements for the SelectMapEntity Scene are handled.
     * HUD Elements in this case includes various map-screen windows.
     */
    public class GameMapUI : IUserInterface
    {
        private const int WindowEdgeBuffer = 5;

        public Window LeftUnitPortraitWindow { get; set; }
        public Window LeftUnitDetailWindow { get; set; }
        public Window LeftUnitStatusWindow { get; set; }
        public Window LeftUnitInventoryWindow { get; set; }

        public Window RightUnitPortraitWindow { get; set; }
        public Window RightUnitDetailWindow { get; set; }
        public Window RightUnitStatusWindow { get; set; }
        public Window RightUnitInventoryWindow { get; set; }

        public Window TurnWindow { get; set; }
        public Window InitiativeWindow { get; set; }
        public Window EntityWindow { get; set; }
        public Window HelpTextWindow { get; set; }
        public Window ObjectiveWindow { get; set; }

        public Window UserPromptWindow { get; set; }

        public VerticalMenu ActionMenu { get; set; }
        public Window ActionMenuDescriptionWindow { get; set; }

        private bool visible;

        public GameMapUI()
        {
            visible = true;
        }


        #region Window Positions

        private Vector2 ActionMenuPosition()
        {
            //Center of screen, above Initiative List
            return new Vector2(
                GameDriver.ScreenSize.X / 2 - ActionMenu.Width,
                InitiativeWindowPosition().Y - ActionMenu.Height - WindowEdgeBuffer
            );
        }

        private Vector2 ActionMenuDescriptionPosition()
        {
            //Right of Action Menu
            return new Vector2(
                WindowEdgeBuffer + ActionMenuPosition().X + ActionMenu.Width,
                ActionMenuPosition().Y
            );
        }

        private Vector2 LeftUnitPortraitWindowPosition()
        {
            //Bottom-left, above initiative window
            return new Vector2(WindowEdgeBuffer,
                GameDriver.ScreenSize.Y - LeftUnitPortraitWindow.Height - InitiativeWindow.Height
            );
        }

        private Vector2 LeftUnitDetailWindowPosition()
        {
            //Bottom-left, right of portrait, above initiative window
            return new Vector2(
                WindowEdgeBuffer + LeftUnitPortraitWindow.Width,
                LeftUnitPortraitWindowPosition().Y + LeftUnitPortraitWindow.Height - LeftUnitDetailWindow.Height
            );
        }

        private Vector2 LeftUnitStatusWindowPosition()
        {
            //Bottom-left, above portrait
            return new Vector2(
                LeftUnitPortraitWindowPosition().X,
                LeftUnitPortraitWindowPosition().Y - LeftUnitStatusWindow.Height - WindowEdgeBuffer
            );
        }


        private Vector2 LeftUnitInventoryWindowPosition()
        {
            //Bottom-left, right of stats
            return new Vector2(
                LeftUnitDetailWindowPosition().X + LeftUnitDetailWindow.Width + WindowEdgeBuffer,
                LeftUnitDetailWindowPosition().Y
            );
        }


        private Vector2 RightUnitPortraitWindowPosition()
        {
            //Bottom-right, above intiative window
            return new Vector2(
                GameDriver.ScreenSize.X - RightUnitPortraitWindow.Width - WindowEdgeBuffer,
                GameDriver.ScreenSize.Y - RightUnitPortraitWindow.Height - InitiativeWindow.Height
            );
        }

        private Vector2 RightUnitDetailWindowPosition()
        {
            //Bottom-right, left of portrait, above intiative window
            return new Vector2(
                GameDriver.ScreenSize.X - RightUnitDetailWindow.Width - RightUnitPortraitWindow.Width -
                WindowEdgeBuffer,
                RightUnitPortraitWindowPosition().Y + RightUnitPortraitWindow.Height - RightUnitDetailWindow.Height
            );
        }


        private Vector2 RightUnitStatusWindowPosition()
        {
            //Bottom-right, above portrait
            return new Vector2(
                RightUnitPortraitWindowPosition().X + RightUnitPortraitWindow.Width - RightUnitStatusWindow.Width,
                RightUnitPortraitWindowPosition().Y - RightUnitStatusWindow.Height - WindowEdgeBuffer
            );
        }


        private Vector2 RightUnitInventoryWindowPosition()
        {
            //Bottom-left, right of stats
            return new Vector2(
                RightUnitDetailWindowPosition().X - RightUnitInventoryWindow.Width - WindowEdgeBuffer,
                RightUnitDetailWindowPosition().Y
            );
        }


        private Vector2 InitiativeWindowPosition()
        {
            //Bottom-right
            return new Vector2(
                GameDriver.ScreenSize.X - InitiativeWindow.Width - WindowEdgeBuffer,
                GameDriver.ScreenSize.Y - InitiativeWindow.Height
            );
        }

        private Vector2 TurnWindowPosition()
        {
            //Bottom-right
            return new Vector2(
                WindowEdgeBuffer,
                GameDriver.ScreenSize.Y - TurnWindow.Height
            );
        }

        private Vector2 EntityWindowPosition()
        {
            //Top-right
            return new Vector2(
                GameDriver.ScreenSize.X - EntityWindow.Width - WindowEdgeBuffer,
                WindowEdgeBuffer
            );
        }

        private Vector2 HelpTextWindowPosition()
        {
            //Top-left
            return new Vector2(WindowEdgeBuffer);
        }

        private Vector2 UserPromptWindowPosition()
        {
            //Middle of the screen
            return new Vector2(
                GameDriver.ScreenSize.X / 2 - (float) UserPromptWindow.Width / 2,
                GameDriver.ScreenSize.Y / 2 - (float) UserPromptWindow.Height / 2
            );
        }

        private Vector2 ObjectiveWindowPosition()
        {
            return new Vector2(GameDriver.ScreenSize.X / 2 - (float) ObjectiveWindow.Width / 2, WindowEdgeBuffer);
        }

        #endregion Window Positions

        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            if (HelpTextWindow != null)
            {
                HelpTextWindow.Draw(spriteBatch, HelpTextWindowPosition());
            }

            if (EntityWindow != null)
            {
                EntityWindow.Draw(spriteBatch, EntityWindowPosition());
            }

            if (TurnWindow != null)
            {
                TurnWindow.Draw(spriteBatch, TurnWindowPosition());
            }

            if (InitiativeWindow != null)
            {
                InitiativeWindow.Draw(spriteBatch, InitiativeWindowPosition());

                if (LeftUnitPortraitWindow != null)
                {
                    LeftUnitPortraitWindow.Draw(spriteBatch, LeftUnitPortraitWindowPosition());

                    if (LeftUnitDetailWindow != null)
                    {
                        LeftUnitDetailWindow.Draw(spriteBatch, LeftUnitDetailWindowPosition());
                    }

                    if (LeftUnitStatusWindow != null)
                    {
                        LeftUnitStatusWindow.Draw(spriteBatch, LeftUnitStatusWindowPosition());
                    }

                    if (LeftUnitInventoryWindow != null)
                    {
                        LeftUnitInventoryWindow.Draw(spriteBatch, LeftUnitInventoryWindowPosition());
                    }
                }

                if (RightUnitPortraitWindow != null)
                {
                    RightUnitPortraitWindow.Draw(spriteBatch, RightUnitPortraitWindowPosition());

                    if (RightUnitDetailWindow != null)
                    {
                        RightUnitDetailWindow.Draw(spriteBatch, RightUnitDetailWindowPosition());
                    }

                    if (RightUnitStatusWindow != null)
                    {
                        RightUnitStatusWindow.Draw(spriteBatch, RightUnitStatusWindowPosition());
                    }

                    if (RightUnitInventoryWindow != null)
                    {
                        RightUnitInventoryWindow.Draw(spriteBatch, RightUnitInventoryWindowPosition());
                    }
                }

                if (UserPromptWindow != null)
                {
                    UserPromptWindow.Draw(spriteBatch, UserPromptWindowPosition());
                }
            }

            if (ActionMenu != null)
            {
                ActionMenu.Draw(spriteBatch, ActionMenuPosition());

                if (ActionMenuDescriptionWindow != null)
                {
                    ActionMenuDescriptionWindow.Draw(spriteBatch, ActionMenuDescriptionPosition());
                }
            }

            if (ObjectiveWindow != null)
            {
                ObjectiveWindow.Draw(spriteBatch, ObjectiveWindowPosition());
            }
        }
    }
}