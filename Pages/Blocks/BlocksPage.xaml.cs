using System.Windows.Controls;

namespace SurvivalcraftTextureStudio
{
    /// <summary>
    /// BlocksPage.xaml 的交互逻辑
    /// </summary>
    public partial class BlocksPage : UserControl
    {
        public static BlocksPage BP;

        public BlocksPage()
        {
            BP = this;
            InitializeComponent();
        }

        private void Grid_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Grid grid = (Grid)sender;
            double gridWidth = grid.ActualWidth;
            double gridHeight = grid.ActualHeight;
            double newLength;
            newLength = (gridWidth >= gridHeight?gridHeight:gridWidth) - 40;
            if (newLength < 0) newLength = 0;
            PreviewImageBorder.Width = newLength;
            PreviewImageBorder.Height = newLength;
        }
    }
}