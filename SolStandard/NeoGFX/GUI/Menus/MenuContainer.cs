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
    public class MenuContainer : IPositionedNeoRenderable
    {
        public MenuOption CurrentOption => CurrentNeoMenu.CurrentOption;

        public NeoMenu CurrentNeoMenu => menuStack.Peek();
        public bool IsAtRootMenu => menuStack.Count == 1;

        private readonly Stack<NeoMenu> menuStack;
        private readonly bool drawAllMenus;

        public MenuContainer(NeoMenu initialNeoMenu, bool drawAllMenus = false)
        {
            this.drawAllMenus = drawAllMenus;

            menuStack = new Stack<NeoMenu>();
            menuStack.Push(initialNeoMenu);
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

            var verticalMenu = new GridNeoMenu(options, GameDriver.CenterScreen, 0, Point.Zero);
            return new MenuContainer(verticalMenu, drawAllMenus);
        }

        public void OpenSubmenu(NeoMenu nextNeoMenu)
        {
            menuStack.Push(nextNeoMenu);
            CurrentNeoMenu.Open();
        }

        public void Cancel()
        {
            if (IsAtRootMenu)
            {
                SoundBox.MenuError.Play();
            }
            else if (CurrentNeoMenu.Close())
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
            CurrentNeoMenu.MoveCursor(direction);
        }

        public void Confirm()
        {
            CurrentNeoMenu.Confirm();
        }

        //Window stuff

        public float Width => CurrentNeoMenu.Width;
        public float Height => CurrentNeoMenu.Height;

        public void Update(GameTime gameTime)
        {
            foreach (NeoMenu menu in menuStack)
            {
                menu.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (drawAllMenus)
            {
                foreach (NeoMenu menu in menuStack.Reverse())
                {
                    menu.Draw(spriteBatch);
                }
            }
            else
            {
                CurrentNeoMenu.Draw(spriteBatch);
            }
        }

        public Vector2 TopLeftPoint
        {
            get => CurrentNeoMenu.TopLeftPoint;
            set => CurrentNeoMenu.TopLeftPoint = value;
        }
    }
}