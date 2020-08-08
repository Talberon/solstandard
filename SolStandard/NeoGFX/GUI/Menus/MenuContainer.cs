using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.NeoGFX.Graphics;
using SolStandard.NeoGFX.GUI.Menus.Implementations;
using SolStandard.NeoUtility.Directions;
using SolStandard.NeoUtility.Monogame.Assets;

namespace SolStandard.NeoGFX.GUI.Menus
{
    public class MenuContainer : IPositionedRenderable
    {
        public MenuOption CurrentOption => CurrentMenu.CurrentOption;

        public Menu CurrentMenu => menuStack.Peek();
        public bool IsAtRootMenu => menuStack.Count == 1;

        private readonly Stack<Menu> menuStack;
        private readonly bool drawAllMenus;

        public MenuContainer(Menu initialMenu, bool drawAllMenus = false)
        {
            this.drawAllMenus = drawAllMenus;

            menuStack = new Stack<Menu>();
            menuStack.Push(initialMenu);
        }

        public static MenuContainer Empty(bool drawAllMenus = false)
        {
            var options = new List<List<MenuOption>>
            {
                new List<MenuOption>
                {
                    new UnselectableOption(
                        MenuOptionEffects.HighlightOnHover(Color.Transparent),
                        MenuOptionEffects.HighlightOffHover(Color.Transparent),
                        "",
                        Color.Transparent
                    ),
                }
            };

            var verticalMenu = new GridMenu(options, GameDriver.CenterScreen, 0, Point.Zero);
            return new MenuContainer(verticalMenu, drawAllMenus);
        }

        public void OpenSubmenu(Menu nextMenu)
        {
            menuStack.Push(nextMenu);
            CurrentMenu.Open();
        }

        public void Cancel()
        {
            if (IsAtRootMenu)
            {
                SoundBox.MenuError.Play();
            }
            else if (CurrentMenu.Close())
            {
                CloseCurrent();
            }
        }

        public void CloseCurrent()
        {
            if (IsAtRootMenu)
            {
                SoundBox.MenuError.Play();
            }
            else
            {
                SoundBox.MenuCancel.Play();

                menuStack.Pop();
            }
        }

        public void CloseAllSubmenus()
        {
            SoundBox.MenuCancel.Play();

            while (menuStack.Count > 1)
            {
                menuStack.Pop();
            }
        }

        public void MoveCursor(CardinalDirection direction)
        {
            CurrentMenu.MoveCursor(direction);
        }

        public void Confirm()
        {
            CurrentMenu.Confirm();
        }

        //Window stuff

        public float Width => CurrentMenu.Width;
        public float Height => CurrentMenu.Height;

        public void Update(GameTime gameTime)
        {
            foreach (Menu menu in menuStack)
            {
                menu.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (drawAllMenus)
            {
                foreach (Menu menu in menuStack.Reverse())
                {
                    menu.Draw(spriteBatch);
                }
            }
            else
            {
                CurrentMenu.Draw(spriteBatch);
            }
        }

        public Vector2 TopLeftPoint
        {
            get => CurrentMenu.TopLeftPoint;
            set => CurrentMenu.TopLeftPoint = value;
        }
    }
}