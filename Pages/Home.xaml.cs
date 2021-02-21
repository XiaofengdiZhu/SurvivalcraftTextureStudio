using System.Diagnostics;

namespace SurvivalcraftTextureStudio
{
    public partial class Home : System.Windows.Controls.UserControl
    {
        public static Home HM;

        public Home()
        {
            InitializeComponent();
            HM = this;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BlocksPageViewModel.BPVM.ImportBlocksTextureBySelectingFile();
        }

        private void EmailButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Process.Start(new ProcessStartInfo("cmd", $"/c start mailto:523084467@qq.com") { CreateNoWindow = true });
            OpenLink("mailto:523084467@qq.com");
        }

        private void DonateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenLink("https://github.com/XiaofengdiZhu/SurvivalcraftTextureStudio/blob/main/CONTRIBUTING.md");
            //Process.Start(new ProcessStartInfo("cmd", $"/c start https://github.com/XiaofengdiZhu/SurvivalcraftTextureStudio/blob/main/CONTRIBUTING.md") { CreateNoWindow = true });
        }
        public void OpenLink(string url)
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        private void WatchChip_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenLink("https://github.com/XiaofengdiZhu/SurvivalcraftTextureStudio/watchers");
        }
        private void StarChip_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenLink("https://github.com/XiaofengdiZhu/SurvivalcraftTextureStudio/stargazers");
        }
        private void ForkChip_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenLink("https://github.com/XiaofengdiZhu/SurvivalcraftTextureStudio/network/members");
        }

        private void GithubButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenLink("https://github.com/XiaofengdiZhu/SurvivalcraftTextureStudio/");
        }
        private void AuthorHeadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenLink("https://github.com/XiaofengdiZhu/");
        }
    }
}