using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.HUD.Sprite
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

        public static SpriteAtlas ToSingleImageSprite(this ITexture2D me, Vector2? size = null)
        {
            Vector2 cellSize = me.GetSize();
            Vector2 renderSize = size ?? cellSize;
            return new SpriteAtlas(me, cellSize, renderSize);
        }

        public static SpriteAtlas WithColor(this SpriteAtlas me, Color color)
        {
            me.DefaultColor = color;
            return me;
        }
    }
}