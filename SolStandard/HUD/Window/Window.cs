using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Exceptions;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Window
{
    public enum HorizontalAlignment
    {
        Left,
        Centered,
        Right
    }

    public class Window : IRenderable
    {
        private static readonly Color InnerPaneColor = new Color(0, 0, 0, 50);
        private readonly ITexture2D windowTexture;
        private readonly int windowCellSize;
        private readonly IRenderable windowContents;
        public Color DefaultColor { get; set; }
        private HorizontalAlignment HorizontalAlignment { get; set; }
        private Vector2 WindowPixelSize { get; set; }
        public bool Visible { get; set; }
        private Vector2 lastPosition;
        private Rectangle innerPane;
        private Rectangle borderPane;
        private readonly int padding;

        public Window(IRenderable windowContent, Color color, Vector2 pixelSizeOverride,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Centered, int padding = 2)
        {
            windowTexture = AssetManager.WindowTexture;
            DefaultColor = color;
            windowContents = windowContent;
            windowCellSize = CalculateCellSize(windowTexture);
            WindowPixelSize = DeriveSizeFromContent(pixelSizeOverride);
            Visible = true;
            HorizontalAlignment = horizontalAlignment;
            this.padding = padding;
            lastPosition = Vector2.Zero;
        }

        public Window(IRenderable windowContent, Color color, HorizontalAlignment horizontalAlignment) :
            this(windowContent, color, Vector2.Zero, horizontalAlignment)
        {
        }

        public Window(IRenderable windowContent, Color color) : this(windowContent, color, Vector2.Zero)
        {
        }

        private int CalculateCellSize(ITexture2D windowTextureTemplate)
        {
            //Window Texture must be a square
            if (windowTextureTemplate.Width == windowTextureTemplate.Height)
            {
                return windowTexture.Height / 3;
            }

            throw new InvalidWindowTextureException();
        }

        private Vector2 DeriveSizeFromContent(Vector2 sizeOverride)
        {
            Vector2 calculatedSize = new Vector2();
            Vector2 contentGridSize = new Vector2(windowContents.Width, windowContents.Height);

            //Adjust for border
            int borderSize = windowCellSize * 2;

            //Default to content size if size is set to 0
            if (Math.Abs(sizeOverride.X) < .001)
            {
                calculatedSize.X = contentGridSize.X;
                calculatedSize.X += borderSize;
            }
            else
            {
                calculatedSize.X = sizeOverride.X;
            }

            if (Math.Abs(sizeOverride.Y) < .001)
            {
                calculatedSize.Y = contentGridSize.Y;
                calculatedSize.Y += borderSize;
            }
            else
            {
                calculatedSize.Y = sizeOverride.Y;
            }

            return calculatedSize;
        }


        public int Height
        {
            get { return (int) WindowPixelSize.Y + (padding * 2); }
            set { WindowPixelSize = DeriveSizeFromContent(new Vector2(0, value)); }
        }

        public int Width
        {
            get { return (int) WindowPixelSize.X + (padding * 2); }
            set { WindowPixelSize = DeriveSizeFromContent(new Vector2(value, 0)); }
        }


        private Vector2 GetCoordinatesBasedOnAlignment(Vector2 windowCoordinates)
        {
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    return LeftAlignedContentCoordinates(windowCoordinates);
                case HorizontalAlignment.Centered:
                    return CenteredContentCoordinates(windowCoordinates);
                case HorizontalAlignment.Right:
                    return RightAlignedContentCoordinates(windowCoordinates);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Vector2 LeftAlignedContentCoordinates(Vector2 windowCoordinates)
        {
            Vector2 contentRenderCoordinates = windowCoordinates;
            contentRenderCoordinates.X += windowCellSize;
            contentRenderCoordinates.Y = VerticalCenterContent(windowCoordinates);

            return contentRenderCoordinates;
        }

        private Vector2 CenteredContentCoordinates(Vector2 windowCoordinates)
        {
            Vector2 contentRenderCoordinates = windowCoordinates;

            contentRenderCoordinates.X +=
                ((float) Width / 2) - (new Vector2(windowContents.Width, windowContents.Height).X / 2);
            contentRenderCoordinates.X = (float) Math.Round(contentRenderCoordinates.X);

            contentRenderCoordinates.Y = VerticalCenterContent(windowCoordinates);

            return contentRenderCoordinates;
        }

        private Vector2 RightAlignedContentCoordinates(Vector2 windowCoordinates)
        {
            Vector2 contentRenderCoordinates = windowCoordinates;

            contentRenderCoordinates.X = windowCoordinates.X + Width - windowContents.Width - windowCellSize;

            contentRenderCoordinates.Y = VerticalCenterContent(windowCoordinates);

            return contentRenderCoordinates;
        }

        private float VerticalCenterContent(Vector2 windowCoordinates)
        {
            float contentRenderCoordinates = windowCoordinates.Y;
            contentRenderCoordinates +=
                ((float) Height / 2) - (new Vector2(windowContents.Width, windowContents.Height).Y / 2);
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
                    (int) lastPosition.X + padding, (int) lastPosition.Y + padding,
                    Width - (padding * 2), Height - (padding * 2)
                );
                borderPane = new Rectangle((int) lastPosition.X, (int) lastPosition.Y, Width, Height);
            }

            spriteBatch.Draw(windowTexture.MonoGameTexture, borderPane, InnerPaneColor);
            spriteBatch.Draw(windowTexture.MonoGameTexture, innerPane, colorOverride);
            windowContents.Draw(spriteBatch, GetCoordinatesBasedOnAlignment(coordinates));
        }


        public IRenderable Clone()
        {
            return new Window(windowContents, DefaultColor, WindowPixelSize, HorizontalAlignment);
        }
    }
}