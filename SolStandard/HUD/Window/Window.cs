using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Window
{
    public enum HorizontalAlignment
    {
        Left,
        Centered,
        Right
    }

    public class Window : IWindow
    {
        private static readonly Color InnerPaneColor = new Color(0, 0, 0, 50);

        private IRenderable WindowContents { get; }
        public int InsidePadding { get; }
        public int ElementSpacing { get; }
        public Color DefaultColor { get; set; }
        public bool Visible { get; set; }

        private readonly ITexture2D windowTexture;
        private HorizontalAlignment HorizontalAlignment { get; }
        private Vector2 WindowPixelSize { get; set; }
        private Vector2 lastPosition;
        private Rectangle innerPane;
        private Rectangle borderPane;

        public Window(IRenderable windowContent, Color color, Vector2 pixelSizeOverride,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Centered, int elementSpacing = 2,
            int insidePadding = 2)
        {
            windowTexture = AssetManager.WindowTexture;
            DefaultColor = color;
            WindowContents = windowContent;
            InsidePadding = insidePadding;
            ElementSpacing = elementSpacing;
            WindowPixelSize = DeriveSizeFromContent(pixelSizeOverride);
            Visible = true;
            HorizontalAlignment = horizontalAlignment;
            lastPosition = Vector2.Zero;
        }

        public Window(IRenderable windowContent, Color color, HorizontalAlignment horizontalAlignment) :
            this(windowContent, color, Vector2.Zero, horizontalAlignment)
        {
        }

        public Window(IRenderable windowContent, Color color) : this(windowContent, color, Vector2.Zero)
        {
        }

        public Window(IRenderable[,] windowContentGrid, Color color, Vector2 pixelSizeOverride,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            int elementSpacing = 2, int insidePadding = 2)
            : this(
                new WindowContentGrid(windowContentGrid),
                color,
                pixelSizeOverride,
                horizontalAlignment,
                elementSpacing,
                insidePadding
            )
        {
        }

        public Window(IRenderable[,] windowContentGrid, Color color,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, int elementSpacing = 2,
            int insidePadding = 2)
            : this(
                windowContentGrid,
                color,
                Vector2.Zero,
                horizontalAlignment,
                elementSpacing,
                insidePadding
            )
        {
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


        public int Height
        {
            get => (int) WindowPixelSize.Y + (InsidePadding * 2);
            set => WindowPixelSize = DeriveSizeFromContent(new Vector2(0, value));
        }

        public int Width
        {
            get => (int) WindowPixelSize.X + (InsidePadding * 2);
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
            contentRenderCoordinates.X += InsidePadding;
            contentRenderCoordinates.Y = VerticalCenterContent(windowCoordinates);

            return contentRenderCoordinates;
        }

        private Vector2 CenteredContentCoordinates(Vector2 windowCoordinates)
        {
            Vector2 contentRenderCoordinates = windowCoordinates;

            contentRenderCoordinates.X +=
                ((float) Width / 2) - (new Vector2(WindowContents.Width, WindowContents.Height).X / 2);
            contentRenderCoordinates.X = (float) Math.Round(contentRenderCoordinates.X);

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
                ((float) Height / 2) - (new Vector2(WindowContents.Width, WindowContents.Height).Y / 2);
            return (float) Math.Round(contentRenderCoordinates);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            Draw(spriteBatch, coordinates, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates, Color colorOverride)
        {
            if (!Visible) return;

            if (lastPosition != coordinates)
            {
                lastPosition = coordinates;
                innerPane = new Rectangle(
                    (int) lastPosition.X + ElementSpacing, (int) lastPosition.Y + ElementSpacing,
                    Width - (ElementSpacing * 2), Height - (ElementSpacing * 2)
                );
                borderPane = new Rectangle((int) lastPosition.X, (int) lastPosition.Y, Width, Height);
            }

            if (colorOverride.A != 0) spriteBatch.Draw(windowTexture.MonoGameTexture, borderPane, InnerPaneColor);

            spriteBatch.Draw(windowTexture.MonoGameTexture, innerPane, colorOverride);
            WindowContents.Draw(spriteBatch, GetCoordinatesBasedOnAlignment(coordinates));
        }

        public IRenderable Clone()
        {
            return new Window(WindowContents, DefaultColor, WindowPixelSize, HorizontalAlignment);
        }
    }
}