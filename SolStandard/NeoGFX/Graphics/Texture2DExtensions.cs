using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steelbreakers.Utility.Monogame.Assets;
using Steelbreakers.Utility.Monogame.Interfaces;

namespace Steelbreakers.Utility.Graphics
{
    public static class Texture2DExtensions
    {
        public static Color GetAverageColor(this ITexture2D texture2D)
        {
            var textureColors = new Color[texture2D.Width * texture2D.Height];
            texture2D.MonoGameTexture.GetData(textureColors);

            return textureColors.GetAverageColor();
        }

        public static Color GetAverageColor(this ITexture2D texture2D, Rectangle region)
        {
            return PixelsInRegion(texture2D, region).GetAverageColor();
        }

        public static Texture2D ToCircle(this Texture2D me, int pixelShaveRadius = 0, int? cellSize = null)
        {
            //HACK Height is normally the right value for exported sprite sheets
            int cellDimensions = cellSize ?? me.Height;
            int fullCellRadius = cellDimensions / 2;

            if (fullCellRadius <= pixelShaveRadius)
                throw new Exception("Radius should be smaller than half dimensions");

            int shavedRadius = fullCellRadius - pixelShaveRadius;

            var myPixels = new Color[me.Width * me.Height];
            me.GetData(myPixels);

            var cells = new List<Rectangle>();
            int rows = me.Height / cellDimensions;
            int columns = me.Width / cellDimensions;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    cells.Add(new Rectangle(
                        column * cellDimensions,
                        row * cellDimensions,
                        cellDimensions,
                        cellDimensions
                    ));
                }
            }

            foreach (Rectangle cell in cells)
            {
                SetPixelsOutsideRadiusToColor(
                    me.Width,
                    cell,
                    shavedRadius,
                    cell.Size.ToVector2() / 2,
                    myPixels,
                    Color.Transparent
                );
            }

            var roundedBorderTexture = new Texture2D(AssetManager.GraphicsDevice, me.Width, me.Height);
            roundedBorderTexture.SetData(myPixels);
            return roundedBorderTexture;
        }

        private static void SetPixelsOutsideRadiusToColor(int textureWidth, Rectangle region, int radius,
            Vector2 radiusOrigin, Color[] pixels, Color colorToSet)
        {
            int rows = region.Height;
            int columns = region.Width;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    {
                        if (PixelIsOutsideRadius(radiusOrigin, radius, new Point(column, row)))
                        {
                            pixels[region.X + column + (row) * textureWidth] = colorToSet;
                        }
                    }
                }
            }
        }

        private static bool PixelIsOutsideRadius(Vector2 circleCenter, float radius, Point pixelPosition)
        {
            (float centerX, float centerY) = circleCenter;
            (int x, int y) = pixelPosition;
            return Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2) > Math.Pow(radius, 2);
        }

        private static Color[] PixelsInRegion(ITexture2D texture2D, Rectangle region)
        {
            var texturePixels = new Color[texture2D.Width * texture2D.Height];
            texture2D.MonoGameTexture.GetData(texturePixels);

            return PixelsInRegion(texturePixels, region, texture2D.Width);
        }

        private static Color[] PixelsInRegion(Color[] pixels, Rectangle region, int textureWidth)
        {
            (int x, int y, int width, int height) = region;

            var regionColors = new Color[width * height];
            int index = 0;

            for (int row = y; row < y + height; row++)
            {
                for (int column = x; column < x + width; column++)
                {
                    regionColors[index] = pixels.GetPixel(column, row, textureWidth);
                    index++;
                }
            }

            return regionColors;
        }

        public static Color GetPixel(this Color[] colors, int x, int y, int width)
        {
            return colors[x + (y * width)];
        }

        public static Color GetAverageColor(this Color[] colors)
        {
            Color[] withoutTransparents = colors.Where(color => color.A != 0).ToArray();

            float r = 0;
            float g = 0;
            float b = 0;

            int pixelCount = withoutTransparents.Length;

            for (int i = 0; i < pixelCount; i++)
            {
                r += withoutTransparents[i].R;
                g += withoutTransparents[i].G;
                b += withoutTransparents[i].B;
            }

            r /= pixelCount;
            g /= pixelCount;
            b /= pixelCount;

            return new Color((int) r, (int) g, (int) b);
        }

        public static ITexture2D ToWrapped(this Texture2D me)
        {
            return new Texture2DWrapper(me);
        }

        public static Vector2 GetSize(this ITexture2D me)
        {
            return new Vector2(me.Width, me.Height);
        }

        public static Vector2 GetHalfSize(this ITexture2D me)
        {
            return GetSize(me) / 2;
        }

        public static Vector2 GetCoordinatesToCenterOnPoint(this IRenderable me, Vector2 pointToCenterOn)
        {
            return pointToCenterOn - new Vector2(me.Width, me.Height);
        }

        public static SpriteAtlas ToSingleImageSprite(this ITexture2D me, Vector2? size = null, int layerDepth = 1)
        {
            Vector2 cellSize = me.GetSize();
            Vector2 renderSize = size ?? cellSize;
            return new SpriteAtlas(me, cellSize, renderSize, 0, layerDepth);
        }

        public static SpriteAtlas WithColor(this SpriteAtlas me, Color color)
        {
            me.DefaultColor = color;
            return me;
        }
    }
}