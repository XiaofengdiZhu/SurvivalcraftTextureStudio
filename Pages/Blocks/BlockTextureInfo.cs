using Prism.Commands;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using SixLabors.ImageSharp;

namespace SurvivalcraftTextureStudio
{
    public class BlockTextureInfo : INotifyPropertyChanged
    {
        public BlockTextureInfo(int index)
        {
            Index = index;
            GotFucusCommand = new DelegateCommand<ExCommandParameter>((p) =>
            {
                if (!IsFocused && Index != BlocksPageViewModel.BPVM.BlockIndexOnFocus)
                {
                    BlocksPageViewModel.BPVM.BlockIndexOnFocus = Index;
                    ((System.Windows.Controls.Border)p.Sender).Focus();
                }
            }, (p) => { return true; });
            LostFucusCommand = new DelegateCommand<ExCommandParameter>((p) =>
            {
                if (((RoutedEventArgs)p.EventArgs).RoutedEvent.Name == "MouseLeave")
                {
                    BlocksPageViewModel.BPVM.BlockIndexOnFocus = -1;
                }
            }, (p) => { return true; });
        }

        public int _Index;

        public int Index
        {
            get { return _Index; }
            set
            {
                if (_Index != value)
                {
                    _Index = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Index"));
                }
            }
        }

        public static Dictionary<CultureInfo, string> NoNameText = new Dictionary<CultureInfo, string>
        {
            {CultureInfo.GetCultureInfo("zh-CN"),"£¨¿Õ°×£©" },
            {CultureInfo.GetCultureInfo("en"),", (Blank)" }
        };

        public Dictionary<CultureInfo, string> _Name;

        public string Name
        {
            get
            {
                if (_Name == null) { return NoNameText[CultureInfo.CurrentCulture]; }
                return _Name[CultureInfo.CurrentCulture];
            }
        }

        public Dictionary<CultureInfo, string> _Description;

        public string Description
        {
            get
            {
                if (_Description == null) { return null; }
                string des = _Description[CultureInfo.CurrentCulture];
                if (des.Length == 0)
                {
                    return null;
                }
                else
                {
                    return des;
                }
            }
        }

        public System.Windows.Media.Imaging.BitmapImage Texture
        {
            get { return ImageHelper.Bitmap2BitmapImage(ImageCache); }
        }

        public bool IsTextureExist
        {
            get { return _Name is null; }
        }

        public Image _ImageCache;

        public Image ImageCache
        {
            get { return _ImageCache; }
            set
            {
                if (_ImageCache != value)
                {
                    _ImageCache = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageCache"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Texture"));
                }
            }
        }

        public bool _IsFocused;

        public bool IsFocused
        {
            get { return _IsFocused; }
            set
            {
                if (_IsFocused != value)
                {
                    _IsFocused = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsFocused"));
                }
            }
        }

        public ICommand GotFucusCommand { get; private set; }
        public ICommand LostFucusCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}