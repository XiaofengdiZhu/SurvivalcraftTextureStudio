using MaterialDesignThemes.Wpf;
using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

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
                new Page("BlocksPageTitle", new BlocksPage(),PackIconKind.CubeOutline){MarginRequirement=new System.Windows.Thickness(0) },
            };
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionText = "v" + version.Major + "." + version.Minor + (version.Build > 0 ? ("." + version.Build) : "") + (version.Revision > 0 ? " beta" + (version.Revision > 1 ? (" " + version.Revision) : "") : "");
            System.Diagnostics.Debug.WriteLine("主界面VM加载完成");
        }

        public Page[] Pages { get; }
        public string VersionText { get; set; }
        public ICommand OpenImportBlocksTextureDialogCommand => new AnotherCommandImplementation(OpenImportBlocksTextureDialog);
        public async void OpenImportBlocksTextureDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new ImportBlocksTextureDialog
            {
                DataContext = new ImportBlocksTextureDialogViewModel()
            };

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);

            //check the result...
            Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }
        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {
        }

        private void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
        }
    }
}