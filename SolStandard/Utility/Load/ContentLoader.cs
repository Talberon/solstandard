using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace SolStandard.Utility.Load
{
    using Monogame;

    /**
     * ContentLoader
     * Holds a series of loader methods that are used for the game
     */
    public class ContentLoader
    {
        public static List<SpriteFont> LoadFonts(ContentManager content)
        {
            List<SpriteFont> fonts = new List<SpriteFont>
            {
                content.Load<SpriteFont>("Fonts/GUIFont"),
                content.Load<SpriteFont>("Fonts/MapFont"),
                content.Load<SpriteFont>("Fonts/NotificationFont"),
                content.Load<SpriteFont>("Fonts/WindowFont")
            };

            return fonts;
        }

        public static ITexture2D LoadTerrainSpriteTexture(ContentManager content)
        {
            Texture2D spriteTextures = content.Load<Texture2D>("Graphics/Map/Tiles/Tiles");

            return new Texture2DWrapper(spriteTextures);
        }

        public static List<ITexture2D> LoadGuiTextures(ContentManager content)
        {
            List<Texture2D> loadGuiTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Map/Cursor/Cursor"),
                content.Load<Texture2D>("Graphics/Map/Cursor/UnitCursorBlue"),
                content.Load<Texture2D>("Graphics/Map/Cursor/UnitCursorRed")
                /* TODO Re-add these eventually
                content.Load<Texture2D>("GUI/MoveGrid"),
                content.Load<Texture2D>("GUI/AttackGrid"),
                content.Load<Texture2D>("GUI/ActionMenuTileBlue"),
                content.Load<Texture2D>("GUI/ActionMenuTileRed"),
                content.Load<Texture2D>("GUI/ActionMenuTileGray"),
                content.Load<Texture2D>("GUI/ActionMenuTileBlueCarbon"),
                content.Load<Texture2D>("GUI/ActionMenuTileRedCarbon"),
                content.Load<Texture2D>("GUI/ActionMenuTileGrayCarbon"),
                content.Load<Texture2D>("GUI/NotificationTile"),
                content.Load<Texture2D>("GUI/WhitePixel"),
                content.Load<Texture2D>("GUI/Pointer"),
                content.Load<Texture2D>("GUI/PipGreen"),
                content.Load<Texture2D>("GUI/PipGrey")*/
            };

            List<ITexture2D> guiTextures = new List<ITexture2D>();
            foreach (Texture2D texture in loadGuiTextures)
            {
                guiTextures.Add(new Texture2DWrapper(texture));
            }

            return guiTextures;
        }
        
        public static List<ITexture2D> LoadUnitSpriteTextures(ContentManager content)
        {
            List<Texture2D> loadSpriteTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueArcher"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueMage"),
                content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueChampion"),

                content.Load<Texture2D>("Graphics/Map/Units/Red/RedArcher"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedMage"),
                content.Load<Texture2D>("Graphics/Map/Units/Red/RedChampion")
            };
            
            List<ITexture2D> spriteTextures = new List<ITexture2D>();
            foreach (Texture2D texture in loadSpriteTextures)
            {
                spriteTextures.Add(new Texture2DWrapper(texture));
            }

            return spriteTextures;
        }
        
        public static List<ITexture2D> LoadUnitPortraitTextures(ContentManager content)
        {
            List<Texture2D> loadPortraitTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Images/PortraitsMedium"),
                content.Load<Texture2D>("Images/PortraitsSmall"),
                content.Load<Texture2D>("Images/PortraitsFull")
            };
            
            List<ITexture2D> portraitTextures = new List<ITexture2D>();
            foreach (Texture2D texture in loadPortraitTextures)
            {
                portraitTextures.Add(new Texture2DWrapper(texture));
            }

            return portraitTextures;
        }


    }
}
