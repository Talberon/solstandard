using MonoGame.Extended.BitmapFonts;

namespace Steelbreakers.Utility.Monogame.Interfaces
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