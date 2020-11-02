using System.Globalization;
using System.Windows;

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

        private void BT_Click(object sender, RoutedEventArgs e)
        {
            string str = CultureInfo.CurrentCulture.Name + "\n";
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (ci.Name.StartsWith("zh")) str += ci.Name + ";" + ci.DisplayName + ";" + ci.NativeName + "\n";
            }
            TB.Text = str;
        }
    }
}