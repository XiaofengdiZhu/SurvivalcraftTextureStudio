using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Windows.Input;

namespace SurvivalcraftTextureStudio
{
    public class PaletteSelectorViewModel
    {
        public PaletteSelectorViewModel()
        {
            Swatches = new SwatchesProvider().Swatches;
            NowPrimaryName = MainWindow.Config("PrimaryColor");
            NowAccentName = MainWindow.Config("AccentColor");
        }

        public ICommand ToggleBaseCommand { get; } = new AnotherCommandImplementation(o => ApplyBase((bool)o));

        private static void ApplyBase(bool isDark)
        {
            MainWindow.Config("isDark", isDark.ToString());
            //new PaletteHelper().SetLightDark(isDark);
            MainWindow.ModifyTheme(theme => theme.SetBaseTheme(isDark ? Theme.Dark : Theme.Light));
        }

        public IEnumerable<Swatch> Swatches { get; }
        public static string NowPrimaryName { get; set; }
        public static string NowAccentName { get; set; }

        public ICommand ApplyPrimaryCommand { get; } = new AnotherCommandImplementation(o => ApplyPrimary((Swatch)o));

        private static void ApplyPrimary(Swatch swatch)
        {
            MainWindow.Config("PrimaryColor", swatch.Name);
            //new PaletteHelper().ReplacePrimaryColor(swatch);
            MainWindow.ModifyTheme(theme => theme.SetPrimaryColor(swatch.ExemplarHue.Color));
            NowPrimaryName = swatch.Name;
        }

        public ICommand ApplyAccentCommand { get; } = new AnotherCommandImplementation(o => ApplyAccent((Swatch)o));

        private static void ApplyAccent(Swatch swatch)
        {
            MainWindow.Config("AccentColor", swatch.Name);
            //new PaletteHelper().ReplaceAccentColor(swatch);
            MainWindow.ModifyTheme(theme => theme.SetSecondaryColor(swatch.AccentExemplarHue.Color));
            NowAccentName = swatch.Name;
        }
    }
}