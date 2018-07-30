using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace SolStandard.Utility.Load
{
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

        public static Texture2D LoadTerrainSpriteTexture(ContentManager content)
        {
            Texture2D spriteTextures = content.Load<Texture2D>("Graphics/Map/Tiles/Tiles");

            return spriteTextures;
        }

        public static List<Texture2D> LoadGuiTextures(ContentManager content)
        {
            List<Texture2D> guiTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("GUI/Cursor"),
                content.Load<Texture2D>("GUI/UnitCursorBlue"),
                content.Load<Texture2D>("GUI/UnitCursorRed"),
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
                content.Load<Texture2D>("GUI/PipGrey")
            };

            return guiTextures;
        }
        
        public static List<Texture2D> LoadUnitSpriteTextures(ContentManager content)
        {
            List<Texture2D> spriteTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Units/BlueMarauder"),
                content.Load<Texture2D>("Units/BlueFencer"),
                content.Load<Texture2D>("Units/BlueLancer"),
                content.Load<Texture2D>("Units/BlueArcher"),
                content.Load<Texture2D>("Units/BlueMage"),
                content.Load<Texture2D>("Units/BlueCleric"),
                content.Load<Texture2D>("Units/BlueChampion"),
                content.Load<Texture2D>("Units/BlueRogue"),

                content.Load<Texture2D>("Units/RedMarauder"),
                content.Load<Texture2D>("Units/RedFencer"),
                content.Load<Texture2D>("Units/RedLancer"),
                content.Load<Texture2D>("Units/RedArcher"),
                content.Load<Texture2D>("Units/RedMage"),
                content.Load<Texture2D>("Units/RedCleric"),
                content.Load<Texture2D>("Units/RedChampion"),
                content.Load<Texture2D>("Units/RedRogue")
            };

            return spriteTextures;
        }
        
        public static List<Texture2D> LoadUnitPortraitTextures(ContentManager content)
        {
            List<Texture2D> portraitTextures = new List<Texture2D>
            {
                content.Load<Texture2D>("Images/PortraitsMain"),
                content.Load<Texture2D>("Images/PortraitsMedium"),
                content.Load<Texture2D>("Images/PortraitsSmall"),
                content.Load<Texture2D>("Images/PortraitsFull")
            };

            return portraitTextures;
        }


    }
}
