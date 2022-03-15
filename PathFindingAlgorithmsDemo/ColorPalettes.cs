using System.Collections.Generic;
using System.Drawing;

namespace PathFindingAlgorithmsDemo
{
    public static class ColorPalettes
    {
        private readonly static Dictionary<ColorSchemes, ColorPalette> _colors;

        static ColorPalettes()
        {
            _colors = new Dictionary<ColorSchemes, ColorPalette>();

            var blueTheme = new ColorPalette();
            blueTheme.Add(FieldElements.Default, Color.FromArgb(183, 197, 255));
            blueTheme.Add(FieldElements.Wall, Color.FromArgb(45, 31, 84));
            blueTheme.Add(FieldElements.Path, Color.FromArgb(242, 247, 252));
            blueTheme.Add(FieldElements.Start, Color.FromArgb(50, 124, 46));
            blueTheme.Add(FieldElements.Finish, Color.FromArgb(255, 97, 35));
            blueTheme.Add(FieldElements.Visited, Color.FromArgb(157, 170, 219));
            blueTheme.Add(FieldElements.Expensive, Color.FromArgb(200, 255, 0, 110));
            _colors.Add(ColorSchemes.Blue, blueTheme);

            var lightTheme = new ColorPalette();
            lightTheme.Add(FieldElements.Default, Color.FromArgb(252, 240, 252));
            lightTheme.Add(FieldElements.Wall, Color.FromArgb(64, 64, 64));
            lightTheme.Add(FieldElements.Path, Color.FromArgb(141, 52, 255));
            lightTheme.Add(FieldElements.Start, Color.FromArgb(136, 209, 132));
            lightTheme.Add(FieldElements.Finish, Color.FromArgb(255, 52, 52));
            lightTheme.Add(FieldElements.Visited, Color.FromArgb(207, 170, 255));
            lightTheme.Add(FieldElements.Expensive, Color.FromArgb(200, 60, 0, 145));
            _colors.Add(ColorSchemes.Light, lightTheme);

            var darkTheme = new ColorPalette();
            darkTheme.Add(FieldElements.Default, Color.FromArgb(199, 199, 199));
            darkTheme.Add(FieldElements.Wall, Color.FromArgb(30, 30, 30));
            darkTheme.Add(FieldElements.Path, Color.FromArgb(239, 242, 132));
            darkTheme.Add(FieldElements.Start, Color.FromArgb(116, 83, 31));
            darkTheme.Add(FieldElements.Finish, Color.FromArgb(172, 51, 46));
            darkTheme.Add(FieldElements.Visited, Color.FromArgb(158, 158, 158));
            darkTheme.Add(FieldElements.Expensive, Color.FromArgb(200, 117, 19, 19));
            _colors.Add(ColorSchemes.Dark, darkTheme);
        }

        public static ColorPalette GetPalette(ColorSchemes type)
        {
            return _colors[type];
        }
    }
}
