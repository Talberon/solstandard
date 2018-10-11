using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Map.Elements
{
    public class MapDistanceTile : MapTile
    {
        public enum TileType
        {
            None,
            White,
            Movement,
            Attack,
            Action
        }

        private readonly int distance;
        private readonly RenderText renderText;
        private readonly bool textVisible;

        public MapDistanceTile(SpriteAtlas sprite, Vector2 mapCoordinates, int distance, Color color,
            bool textVisible = true) :
            base(sprite, mapCoordinates)
        {
            this.distance = distance;
            this.textVisible = textVisible;
            renderText = new RenderText(AssetManager.MapFont, distance.ToString());
            ElementColor = color;
        }

        public MapDistanceTile(SpriteAtlas sprite, Vector2 mapCoordinates, int distance, bool textVisible = true) :
            this(sprite, mapCoordinates, distance, Color.White, textVisible)
        {
        }

        public static SpriteAtlas GetTileSprite(TileType tileType)
        {
            return new SpriteAtlas(AssetManager.ActionTiles, new Vector2(GameDriver.CellSize), (int) tileType);
        }

        public int Distance
        {
            get { return distance; }
        }

        public SpriteAtlas SpriteAtlas
        {
            get { return (SpriteAtlas) Sprite; }
        }

        public override string ToString()
        {
            string output = "";

            output += "MapDistanceTile: {";

            output += "X:" + MapCoordinates.X;
            output += ", Y:" + MapCoordinates.Y;
            output += ", Dist:" + Distance;

            output += "}";

            return output;
        }

        private Vector2 CenterTextToTile()
        {
            Vector2 tileCorner = MapCoordinates * GameDriver.CellSize;

            Vector2 centerOfText = new Vector2((float) renderText.Width / 2, (float) renderText.Height / 2);
            Vector2 centerOfTile = new Vector2((float) GameDriver.CellSize / 2, (float) GameDriver.CellSize / 2);

            Vector2 textTarget = new Vector2(tileCorner.X + centerOfTile.X - centerOfText.X,
                tileCorner.Y + centerOfTile.Y - centerOfText.Y);
            return textTarget;
        }

        public override void Draw(SpriteBatch spriteBatch, Color colorOverride)
        {
            Sprite.Draw(spriteBatch, MapCoordinates * GameDriver.CellSize, colorOverride);

            if (textVisible)
            {
                Vector2 centeredText = CenterTextToTile();
                //Black outline
                const int offset = 1;
                renderText.Draw(spriteBatch, new Vector2(centeredText.X - offset, centeredText.Y), Color.Black);
                renderText.Draw(spriteBatch, new Vector2(centeredText.X + offset, centeredText.Y), Color.Black);
                renderText.Draw(spriteBatch, new Vector2(centeredText.X, centeredText.Y - offset), Color.Black);
                renderText.Draw(spriteBatch, new Vector2(centeredText.X, centeredText.Y + offset), Color.Black);

                renderText.Draw(spriteBatch, centeredText);
            }
        }
    }
}