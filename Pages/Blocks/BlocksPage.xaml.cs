using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SurvivalcraftTextureStudio
{
    /// <summary>
    /// BlocksPage.xaml 的交互逻辑
    /// </summary>
    public partial class BlocksPage : UserControl
    {
        public static BlocksPage BP;
        Storyboard RotatePreviewingImageStoryboard;
        Storyboard ScaleXPreviewingImageStoryboard;
        Storyboard ScaleYPreviewingImageStoryboard;

        public BlocksPage()
        {
            BP = this;
            InitializeComponent();
            RotatePreviewingImageStoryboard = (Storyboard)this.FindResource("RotatePreviewingImageStoryboard");
            ScaleXPreviewingImageStoryboard = (Storyboard)this.FindResource("ScaleXPreviewingImageStoryboard");
            ScaleYPreviewingImageStoryboard = (Storyboard)this.FindResource("ScaleYPreviewingImageStoryboard");
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
        private void RotateLeftButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((DoubleAnimation)RotatePreviewingImageStoryboard.Children[0]).To = PreviewingImageRotateTransform.Angle - 90;
            RotatePreviewingImageStoryboard.Begin(this);
        }
        private void RotateRightButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((DoubleAnimation)RotatePreviewingImageStoryboard.Children[0]).To = PreviewingImageRotateTransform.Angle + 90;
            RotatePreviewingImageStoryboard.Begin(this);
        }
        private void FlipXButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((DoubleAnimation)ScaleXPreviewingImageStoryboard.Children[0]).To = -PreviewingImageScaleTransform.ScaleX;
            ScaleXPreviewingImageStoryboard.Begin(this);
        }
        private void FlipYButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((DoubleAnimation)ScaleYPreviewingImageStoryboard.Children[0]).To = -PreviewingImageScaleTransform.ScaleY;
            ScaleYPreviewingImageStoryboard.Begin(this);
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ((BlocksPageViewModel)DataContext).InitiateBlockTexturesDictionary();
        }
    }
}