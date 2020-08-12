using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;
using SolStandard.Utility.HUD.Juice;
using SolStandard.Utility.HUD.Sprite;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.HUD.Neo
{
    public enum HorizontalAlignment
    {
        Left,
        Centered,
        Right
    }

    public enum WindowBorder
    {
        Pixel
    }

    public class NeoWindow : IWindow
    {
        public Vector2 CurrentPosition { get; set; }

        public RectangleF Bounds => drawDimensions;
        public Color DefaultColor { get; set; }
        private int InsidePadding { get; set; }
        private ITexture2D windowTexture;
        private HorizontalAlignment HorizontalAlignment { get; set; }
        private Vector2 WindowPixelSize { get; set; }
        private Vector2 pixelSizeOverride;
        private Vector2 lastPosition;
        private Vector2 lastSize;
        private Rectangle drawDimensions;
        private IRenderable windowContents;

        private IRenderable WindowContents
        {
            get => windowContents;
            set
            {
                windowContents = value;
                WindowPixelSize = DeriveSizeFromContent(pixelSizeOverride);
            }
        }

        // ReSharper disable once NotNullMemberIsNotInitialized
        private NeoWindow(IRenderable windowContent, WindowBorder borderStyle, Color color, Vector2 pixelSizeOverride,
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
        private NeoWindow(IRenderable windowContent, WindowBorder borderStyle, Color color, Vector2 currentPosition) :
            this(windowContent, borderStyle, color, Vector2.Zero, currentPosition)
        {
        }

        private void SetWindowTexture(WindowBorder borderStyle)
        {
            windowTexture = borderStyle switch
            {
                WindowBorder.Pixel => AssetManager.WhitePixel,
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

        int IRenderable.Width => (int) Width;
        int IRenderable.Height => (int) Height;

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

            contentRenderCoordinates.X += (Width / 2) - ((float) WindowContents.Width / 2);
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
            //FIXME Other things should be able to update; not just AsepriteWrappers
            (windowContents as AsepriteWrapper)?.Update(gameTime);
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

        void IRenderable.Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            Draw(spriteBatch, position, colorOverride);
        }


        public IRenderable Clone()
        {
            throw new NotImplementedException();
        }

        public class JuicyWindow : IWindow
        {
            public Vector2 CurrentPosition
            {
                get => Window.CurrentPosition;
                set => JuiceBox.MoveTowards(value);
            }

            int IRenderable.Height => (int) Height;
            int IRenderable.Width => (int) Width;

            public float Width => Window.Width;
            public float Height => Window.Height;

            internal readonly NeoWindow Window;
            public JuiceBox JuiceBox { get; }

            public JuicyWindow(NeoWindow window, JuiceBox juiceBox)
            {
                Window = window;
                JuiceBox = juiceBox;
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

            public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
            {
                throw new NotImplementedException();
            }

            public Color DefaultColor { get; set; }

            public IRenderable Clone()
            {
                throw new NotImplementedException();
            }
        }

        public class Builder
        {
            private readonly NeoWindow window;

            public Builder()
            {
                window = new NeoWindow(RenderBlank.Blank, WindowBorder.Pixel, Color.Black, Vector2.Zero);
            }

            private Builder(NeoWindow fromWindow)
            {
                window = fromWindow;
            }

            public static Builder Mutate(NeoWindow fromWindow)
            {
                return new Builder(fromWindow);
            }

            public static Builder Mutate(JuicyWindow fromJuicyWindow)
            {
                return new Builder(fromJuicyWindow.Window);
            }

            public Builder Content(IRenderable content)
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


            public NeoWindow Build()
            {
                return window;
            }
        }
    }
}