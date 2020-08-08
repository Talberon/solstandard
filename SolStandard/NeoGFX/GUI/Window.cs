using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SolStandard.NeoGFX.Graphics;
using SolStandard.NeoGFX.Juice;
using SolStandard.NeoUtility.Monogame.Assets;
using SolStandard.NeoUtility.Monogame.Interfaces;

namespace SolStandard.NeoGFX.GUI
{
    public enum HorizontalAlignment
    {
        Left,
        Centered,
        Right
    }

    public enum WindowBorder
    {
        Sharp,
        Rounded
    }

    public class Window : IWindow, IWindowContent
    {
        public Vector2 CurrentPosition { get; set; }

        public RectangleF Bounds => drawDimensions;

        private int InsidePadding { get; set; }
        public Color DefaultColor { get; private set; }
        private ITexture2D windowTexture;
        private HorizontalAlignment HorizontalAlignment { get; set; }
        private Vector2 WindowPixelSize { get; set; }
        private Vector2 pixelSizeOverride;
        private Vector2 lastPosition;
        private Vector2 lastSize;
        private Rectangle drawDimensions;
        private IWindowContent windowContents;


        private IWindowContent WindowContents
        {
            get => windowContents;
            set
            {
                windowContents = value;
                WindowPixelSize = DeriveSizeFromContent(pixelSizeOverride);
            }
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private Window(IWindowContent windowContent, WindowBorder borderStyle, Color color, Vector2 pixelSizeOverride,
            Vector2 currentPosition, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Centered,
            int insidePadding = 2)
        {
            this.pixelSizeOverride = pixelSizeOverride;
            SetWindowTexture(borderStyle);
            DefaultColor = color;
            CurrentPosition = currentPosition;
            WindowContents = windowContent;
            InsidePadding = insidePadding;
            WindowPixelSize = DeriveSizeFromContent(pixelSizeOverride);
            HorizontalAlignment = horizontalAlignment;
            lastPosition = -Vector2.One;
            lastSize = WindowPixelSize;
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private Window(IWindowContent windowContent, WindowBorder borderStyle, Color color, Vector2 currentPosition) :
            this(windowContent, borderStyle, color, Vector2.Zero, currentPosition)
        {
        }

        private void SetWindowTexture(WindowBorder borderStyle)
        {
            windowTexture = borderStyle switch
            {
                WindowBorder.Sharp => AssetManager.SharpWindowTexture,
                WindowBorder.Rounded => AssetManager.RoundedWindowTexture,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Vector2 DeriveSizeFromContent(Vector2 sizeOverride)
        {
            var calculatedSize = new Vector2();
            (float width, float height) = new Vector2(WindowContents.Width, WindowContents.Height);

            //Adjust for border
            int borderSize = InsidePadding * 2;

            //Default to content size if size is set to 0
            (float overrideX, float overrideY) = sizeOverride;
            if (Math.Abs(overrideX) < .001)
            {
                calculatedSize.X = width;
                calculatedSize.X += borderSize;
            }
            else
            {
                calculatedSize.X = overrideX;
            }

            if (Math.Abs(overrideY) < .001)
            {
                calculatedSize.Y = height;
                calculatedSize.Y += borderSize;
            }
            else
            {
                calculatedSize.Y = overrideY;
            }

            return calculatedSize;
        }


        public float Height
        {
            get => WindowPixelSize.Y + (InsidePadding * 2);
            set => WindowPixelSize = DeriveSizeFromContent(new Vector2(0, value));
        }


        public float Width
        {
            get => WindowPixelSize.X + (InsidePadding * 2);
            set => WindowPixelSize = DeriveSizeFromContent(new Vector2(value, 0));
        }

        private Vector2 GetCoordinatesBasedOnAlignment(Vector2 windowCoordinates)
        {
            return HorizontalAlignment switch
            {
                HorizontalAlignment.Left => LeftAlignedContentCoordinates(windowCoordinates),
                HorizontalAlignment.Centered => CenteredContentCoordinates(windowCoordinates),
                HorizontalAlignment.Right => RightAlignedContentCoordinates(windowCoordinates),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Vector2 LeftAlignedContentCoordinates(Vector2 windowCoordinates)
        {
            Vector2 contentRenderCoordinates = windowCoordinates;
            contentRenderCoordinates.X += (float) InsidePadding / 2;
            contentRenderCoordinates.Y = VerticalCenterContent(windowCoordinates);

            return contentRenderCoordinates;
        }

        private Vector2 CenteredContentCoordinates(Vector2 windowCoordinates)
        {
            Vector2 contentRenderCoordinates = windowCoordinates;

            contentRenderCoordinates.X += (Width / 2) - (WindowContents.Width / 2);
            contentRenderCoordinates.Y = VerticalCenterContent(windowCoordinates);

            return contentRenderCoordinates;
        }

        private Vector2 RightAlignedContentCoordinates(Vector2 windowCoordinates)
        {
            Vector2 contentRenderCoordinates = windowCoordinates;

            contentRenderCoordinates.X = windowCoordinates.X + Width - WindowContents.Width - InsidePadding;

            contentRenderCoordinates.Y = VerticalCenterContent(windowCoordinates);

            return contentRenderCoordinates;
        }

        private float VerticalCenterContent(Vector2 windowCoordinates)
        {
            float contentRenderCoordinates = windowCoordinates.Y;
            contentRenderCoordinates +=
                (Height / 2) - (new Vector2(WindowContents.Width, WindowContents.Height).Y / 2);
            return (float) Math.Round(contentRenderCoordinates);
        }

        public void Update(GameTime gameTime)
        {
            windowContents.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, CurrentPosition);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            Draw(spriteBatch, coordinates, DefaultColor);
        }

        private void Draw(SpriteBatch spriteBatch, Vector2 coordinates, Color colorOverride)
        {
            if (lastPosition != coordinates || lastSize != WindowPixelSize)
            {
                lastPosition = coordinates;
                lastSize = WindowPixelSize;
                drawDimensions = new Rectangle((int) lastPosition.X, (int) lastPosition.Y, (int) Width, (int) Height);
            }

            spriteBatch.DrawRoundedRect(drawDimensions, windowTexture.MonoGameTexture, GameDriver.CellSize,
                colorOverride);
            WindowContents.Draw(spriteBatch, GetCoordinatesBasedOnAlignment(coordinates));
        }

        public class JuicyWindow : IWindow, IWindowContent
        {
            public Vector2 CurrentPosition
            {
                get => Window.CurrentPosition;
                set => JuiceBox.MoveTowards(value);
            }

            public float Width => Window.Width;
            public float Height => Window.Height;

            internal readonly Window Window;
            public JuiceBox JuiceBox { get; }

            public JuicyWindow(Window window, JuiceBox juiceBox)
            {
                Window = window;
                JuiceBox = juiceBox;
            }

            public void SetTextContent(string text)
            {
                if (Window.WindowContents is RenderText textContent) textContent.Message = text;
                else throw new InvalidOperationException("This window has different content than solely RenderText");
            }

            public void ResetNewSize(Vector2 newSize)
            {
                JuiceBox.SnapToNewSize(newSize);
                JuiceBox.DefaultSize = newSize;
                Window.WindowPixelSize = JuiceBox.CurrentSize;
            }

            public void Update(GameTime gameTime)
            {
                JuiceBox.Update();

                Window.CurrentPosition = JuiceBox.CurrentDrawPosition;
                Window.DefaultColor = JuiceBox.CurrentColor;
                Window.WindowPixelSize = JuiceBox.CurrentSize;

                Window.Update(gameTime);
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                Window.Draw(spriteBatch);
            }

            public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
            {
                Window.Draw(spriteBatch, coordinates);
            }
        }

        public class Builder
        {
            private readonly Window window;

            public Builder()
            {
                window = new Window(RenderBlank.Blank, WindowBorder.Rounded, Color.Black, Vector2.Zero);
            }

            private Builder(Window fromWindow)
            {
                window = fromWindow;
            }

            public static Builder Mutate(Window fromWindow)
            {
                return new Builder(fromWindow);
            }

            public static Builder Mutate(JuicyWindow fromJuicyWindow)
            {
                return new Builder(fromJuicyWindow.Window);
            }

            public Builder Content(IWindowContent content)
            {
                window.WindowContents = content;
                return this;
            }

            public Builder InsidePadding(int padding)
            {
                window.InsidePadding = padding;
                return this;
            }

            public Builder Texture(ITexture2D texture)
            {
                window.windowTexture = texture;
                return this;
            }

            public Builder HorizontalAlignment(HorizontalAlignment alignment)
            {
                window.HorizontalAlignment = alignment;
                return this;
            }

            public Builder WindowColor(Color color)
            {
                window.DefaultColor = color;
                return this;
            }

            public Builder Dimensions(Vector2 dimensions)
            {
                window.pixelSizeOverride = dimensions;
                window.WindowPixelSize = dimensions;
                return this;
            }

            public Builder BorderStyle(WindowBorder borderStyle)
            {
                window.SetWindowTexture(borderStyle);
                return this;
            }

            #region Positional

            public Builder AtPosition(Vector2 position)
            {
                window.CurrentPosition = position;
                return this;
            }

            public Builder TopLeftOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(window.TopLeftOf(region, padding));
            }

            public Builder TopCenterOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(window.TopCenterOf(region, padding));
            }

            public Builder TopRightOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(window.TopRightOf(region, padding));
            }


            public Builder LeftCenterOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(window.LeftCenterOf(region, padding));
            }

            public Builder CenterOf(RectangleF region)
            {
                return AtPosition(window.CenterOf(region));
            }

            public Builder RightCenterOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(window.RightCenterOf(region, padding));
            }


            public Builder BottomLeftOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(window.BottomLeftOf(region, padding));
            }

            public Builder BottomCenterOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(window.BottomCenterOf(region, padding));
            }

            public Builder BottomRightOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(window.BottomRightOf(region, padding));
            }

            #endregion Positional


            public Window Build()
            {
                return window;
            }
        }
    }
}