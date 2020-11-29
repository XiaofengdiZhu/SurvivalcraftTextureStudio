using MaterialDesignThemes.Wpf;
using MultiLanguageForXAML;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SurvivalcraftTextureStudio
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow MW;
        public static DialogHost DH;
        private static Configuration CF { get; set; }

        public static Snackbar Snackbar;

        public MainWindow()
        {
            InitiateLanguage();
            InitializeComponent();
            MW = this;
            DH = this.MainWindowOutest;
            CF = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            LoadWindowSettings();
            DataContext = new MainWindowViewModel(MainSnackbar.MessageQueue);
            Snackbar = MainSnackbar;
        }

        public void InitiateLanguage()
        {
            string language = CultureInfo.CurrentCulture.Name;
            if (language != "zh-CN")
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
                language = "en";
            }
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "Languages");
            LanService.Init(new JsonDB(path), true, language);
        }

        public void LoadWindowSettings()
        {
            WindowState = (WindowState)int.Parse(Config("WindowState"));
            MainWindowOutest.Margin = WindowState == WindowState.Maximized ? new Thickness(8) : new Thickness(0);
            MaximizeWindowButton.Tag = WindowState == WindowState.Normal ? "Max" : "Res";
            WindowControlButtonContent.Kind = WindowState == WindowState.Normal ? PackIconKind.WindowMaximize : PackIconKind.WindowRestore;
            Width = float.Parse(Config("WindowWidth"));
            if (Width < 1180)
            {
                WideDrawerToggleButton.Visibility = Visibility.Collapsed;
                WideDrawerToggleButton.IsChecked = false;
            }
            else
            {
                NarrowDrawerToggleButton.Visibility = Visibility.Collapsed;
            }
            Height = float.Parse(Config("WindowHeight"));
            string tempTop = Config("WindowTop");
            if (tempTop.Length > 0)
            {
                Top = float.Parse(tempTop);
                Left = float.Parse(Config("WindowLeft"));
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            PagesListBox.SelectedIndex = int.Parse(Config("BootPageIndex"));
            if (bool.Parse(Config("isDark")))
            {
                ModifyTheme(theme => theme.SetBaseTheme(Theme.Dark));
            }
            string originalPrimaryColor = "Indigo";
            string originalAccentColor = "DeepPurple";
            string primaryColor = Config("PrimaryColor");
            if (primaryColor != originalPrimaryColor)
            {
                ModifyTheme(theme => theme.SetPrimaryColor((Color)ColorConverter.ConvertFromString(primaryColor)));
            }
            string accentColor = Config("AccentColor");
            if (accentColor != originalAccentColor)
            {
                ModifyTheme(theme => theme.SetSecondaryColor((Color)ColorConverter.ConvertFromString(accentColor)));
            }
        }

        public static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            modificationAction?.Invoke(theme);
            paletteHelper.SetTheme(theme);
        }

        private void MainWindow_CLosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            TrueClose();
        }

        public void TrueClose()
        {
            SaveWindowSettings();
            Application.Current.Shutdown();
        }

        public WindowState WindowStateBeforeHided;

        public void SaveWindowSettings()
        {
            Config("WindowState", (WindowState == WindowState.Minimized ? ((int)WindowStateBeforeHided) : ((int)WindowState)).ToString());
            Config("WindowWidth", ActualWidth.ToString());
            Config("WindowHeight", ActualHeight.ToString());
            Config("WindowTop", Top.ToString());
            Config("WindowLeft", Left.ToString());
            Config("BootPageIndex", PagesListBox.SelectedIndex.ToString());
            CF.Save(ConfigurationSaveMode.Minimal);
        }

        public static string Config(string key, string value = null)
        {
            if (value == null)
            {
                return CF.AppSettings.Settings[key] == null ? String.Empty : CF.AppSettings.Settings[key].Value;
            }
            else
            {
                if (CF.AppSettings.Settings[key] == null)
                {
                    CF.AppSettings.Settings.Add(key, value);
                }
                else
                {
                    CF.AppSettings.Settings[key].Value = value;
                }
                return value;
            }
        }

        private void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {
            /*var sampleMessageDialog = new MessageDialog
            {
                Message = { Text = ((ButtonBase)sender).Content.ToString() }
            };

            await DialogHost.Show(sampleMessageDialog, "RootDialog");*/
        }

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string stringValue)
            {
                try
                {
                    Clipboard.SetDataObject(stringValue);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };
            scrollViewer.RaiseEvent(eventArg);
        }

        private void NotifyIconMenuExit_Click(object sender, RoutedEventArgs e)
        {
            TrueClose();
        }

        public void RecoverWindow()
        {
            ShowInTaskbar = true;
            WindowState = WindowStateBeforeHided;
            Activate();
        }

        public void NotifyIconMenuOpen_Click(object sender, RoutedEventArgs e)
        {
            RecoverWindow();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Minimized)
            {
                WindowStateBeforeHided = WindowState;
                if (!ShowInTaskbar) { ShowInTaskbar = true; }
                MainWindowOutest.Margin = WindowState == WindowState.Maximized ? new Thickness(8) : new Thickness(0);
                MaximizeWindowButton.Tag = WindowState == WindowState.Normal ? "Max" : "Res";
                WindowControlButtonContent.Kind = WindowState == WindowState.Normal ? PackIconKind.WindowMaximize : PackIconKind.WindowRestore;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Width < 1180)
            {
                WideDrawerToggleButton.Visibility = Visibility.Collapsed;
                if (NarrowDrawerToggleButton.Visibility != Visibility.Visible)
                {
                    NarrowDrawerToggleButton.Visibility = Visibility.Visible;
                }
                WideDrawerToggleButton.IsChecked = false;
            }
            else
            {
                NarrowDrawerToggleButton.Visibility = Visibility.Collapsed;
                if (WideDrawerToggleButton.Visibility != Visibility.Visible)
                {
                    WideDrawerToggleButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void WindowControlButton_Selected(object sender, RoutedEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;
            listBoxItem.IsEnabled = false;
            string tag = listBoxItem.Tag as string;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(300);
            }).ContinueWith(t =>
            {
                listBoxItem.IsSelected = false;
                listBoxItem.IsEnabled = true;
                switch (tag)
                {
                    case "Min": WindowState = WindowState.Minimized; break;
                    case "Max": WindowState = WindowState.Maximized; break;
                    case "Res": WindowState = WindowState.Normal; break;
                    case "CLose": Close(); break;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ImportBlocksTextureButton_Click(object sender, RoutedEventArgs e)
        {
            BlocksPageViewModel.BPVM.ImportBlocksTextureBySelectingFile();
        }
    }
}