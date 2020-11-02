using MaterialDesignThemes.Wpf;
using System;
using System.Reflection;
using System.Windows.Controls;

namespace SurvivalcraftTextureStudio
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (snackbarMessageQueue == null) throw new ArgumentNullException(nameof(snackbarMessageQueue));

            Pages = new[]
            {
                new Page("HomePageTitle", new Home(), PackIconKind.Home),
                new Page("UISettingsPageTitle", new PaletteSelector() { DataContext = new PaletteSelectorViewModel() },PackIconKind.Theme){ VerticalScrollBarVisibilityRequirement = ScrollBarVisibility.Auto},
                new Page("BlocksPageTitle", new BlocksPage() { DataContext = new BlocksPageViewModel() },PackIconKind.CubeOutline){ VerticalScrollBarVisibilityRequirement = ScrollBarVisibility.Auto},
            };
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionText = "v" + version.Major + "." + version.Minor + (version.Build > 0 ? ("." + version.Build) : "") + (version.Revision > 0 ? " beta" + (version.Revision > 1 ? (" " + version.Revision) : "") : "");
        }

        public Page[] Pages { get; }
        public string VersionText { get; set; }
    }

    public enum PageIndex
    {
        Home,
        PaletteSelector,
    }
}