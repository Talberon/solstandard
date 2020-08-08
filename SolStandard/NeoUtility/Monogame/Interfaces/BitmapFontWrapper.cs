using MonoGame.Extended.BitmapFonts;

namespace SolStandard.NeoUtility.Monogame.Interfaces
{
    public class BitmapFontWrapper : IBitmapFont
    {
        public BitmapFont MonogameExtendedFont { get; }

        public BitmapFontWrapper(BitmapFont monogameExtendedFont)
        {
            MonogameExtendedFont = monogameExtendedFont;
        }
    }
}