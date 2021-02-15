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
    }
}