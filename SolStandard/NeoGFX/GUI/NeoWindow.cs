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

    public class NeoWindow : IWindow, IWindowContent
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
        private NeoWindow(IWindowContent windowContent, WindowBorder borderStyle, Color color, Vector2 pixelSizeOverride,
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
        private NeoWindow(IWindowContent windowContent, WindowBorder borderStyle, Color color, Vector2 currentPosition) :
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
                get => NeoWindow.CurrentPosition;
                set => JuiceBox.MoveTowards(value);
            }

            public float Width => NeoWindow.Width;
            public float Height => NeoWindow.Height;

            internal readonly NeoWindow NeoWindow;
            public JuiceBox JuiceBox { get; }

            public JuicyWindow(NeoWindow neoWindow, JuiceBox juiceBox)
            {
                NeoWindow = neoWindow;
                JuiceBox = juiceBox;
            }

            public void SetTextContent(string text)
            {
                if (NeoWindow.WindowContents is NeoRenderText textContent) textContent.Message = text;
                else throw new InvalidOperationException("This window has different content than solely RenderText");
            }

            public void ResetNewSize(Vector2 newSize)
            {
                JuiceBox.SnapToNewSize(newSize);
                JuiceBox.DefaultSize = newSize;
                NeoWindow.WindowPixelSize = JuiceBox.CurrentSize;
            }

            public void Update(GameTime gameTime)
            {
                JuiceBox.Update();

                NeoWindow.CurrentPosition = JuiceBox.CurrentDrawPosition;
                NeoWindow.DefaultColor = JuiceBox.CurrentColor;
                NeoWindow.WindowPixelSize = JuiceBox.CurrentSize;

                NeoWindow.Update(gameTime);
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                NeoWindow.Draw(spriteBatch);
            }

            public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
            {
                NeoWindow.Draw(spriteBatch, coordinates);
            }
        }

        public class Builder
        {
            private readonly NeoWindow neoWindow;

            public Builder()
            {
                neoWindow = new NeoWindow(RenderBlank.Blank, WindowBorder.Rounded, Color.Black, Vector2.Zero);
            }

            private Builder(NeoWindow fromNeoWindow)
            {
                neoWindow = fromNeoWindow;
            }

            public static Builder Mutate(NeoWindow fromNeoWindow)
            {
                return new Builder(fromNeoWindow);
            }

            public static Builder Mutate(JuicyWindow fromJuicyWindow)
            {
                return new Builder(fromJuicyWindow.NeoWindow);
            }

            public Builder Content(IWindowContent content)
            {
                neoWindow.WindowContents = content;
                return this;
            }

            public Builder InsidePadding(int padding)
            {
                neoWindow.InsidePadding = padding;
                return this;
            }

            public Builder Texture(ITexture2D texture)
            {
                neoWindow.windowTexture = texture;
                return this;
            }

            public Builder HorizontalAlignment(HorizontalAlignment alignment)
            {
                neoWindow.HorizontalAlignment = alignment;
                return this;
            }

            public Builder WindowColor(Color color)
            {
                neoWindow.DefaultColor = color;
                return this;
            }

            public Builder Dimensions(Vector2 dimensions)
            {
                neoWindow.pixelSizeOverride = dimensions;
                neoWindow.WindowPixelSize = dimensions;
                return this;
            }

            public Builder BorderStyle(WindowBorder borderStyle)
            {
                neoWindow.SetWindowTexture(borderStyle);
                return this;
            }

            #region Positional

            public Builder AtPosition(Vector2 position)
            {
                neoWindow.CurrentPosition = position;
                return this;
            }

            public Builder TopLeftOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(neoWindow.TopLeftOf(region, padding));
            }

            public Builder TopCenterOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(neoWindow.TopCenterOf(region, padding));
            }

            public Builder TopRightOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(neoWindow.TopRightOf(region, padding));
            }


            public Builder LeftCenterOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(neoWindow.LeftCenterOf(region, padding));
            }

            public Builder CenterOf(RectangleF region)
            {
                return AtPosition(neoWindow.CenterOf(region));
            }

            public Builder RightCenterOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(neoWindow.RightCenterOf(region, padding));
            }


            public Builder BottomLeftOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(neoWindow.BottomLeftOf(region, padding));
            }

            public Builder BottomCenterOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(neoWindow.BottomCenterOf(region, padding));
            }

            public Builder BottomRightOf(RectangleF region, float padding = 0f)
            {
                return AtPosition(neoWindow.BottomRightOf(region, padding));
            }

            #endregion Positional


            public NeoWindow Build()
            {
                return neoWindow;
            }
        }
    }
}