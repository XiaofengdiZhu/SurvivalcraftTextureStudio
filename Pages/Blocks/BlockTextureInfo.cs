using Prism.Commands;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SurvivalcraftTextureStudio
{
    public class BlockTextureInfo : INotifyPropertyChanged
    {
        public BlockTextureInfo(int index, string name = "undefined", string description = "undefined too", float rotation = 0)
        {
            Index = index;
            Name = name;
            Description = description;
            Rotation = rotation;
            GotFucusCommand = new DelegateCommand<ExCommandParameter>((p) =>
            {
                if(!IsFocused&& Index != BlocksPageViewModel.BPVM.BlockIndexOnFocus)
                {
                    BlocksPageViewModel.BPVM.BlockIndexOnFocus = Index;
                    ((System.Windows.Controls.Border)p.Sender).Focus();
                }
            }, (p) => { return true; });
            LostFucusCommand = new DelegateCommand<ExCommandParameter>((p) =>
            {
                if (((RoutedEventArgs)p.EventArgs).RoutedEvent.Name=="MouseLeave")
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

        public string _Name;

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public string _Description;

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Description"));
                }
            }
        }

        public float _Rotation;

        public float Rotation
        {
            get { return _Rotation; }
            set
            {
                if (_Rotation != value)
                {
                    _Rotation = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Rotation"));
                }
            }
        }

        public BitmapImage _Texture;

        public BitmapImage Texture
        {
            get { return _Texture; }
            set
            {
                if (_Texture != value)
                {
                    _Texture = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Texture"));
                }
            }
        }

        public bool IsTextureExist
        {
            get { return Texture is null; }
        }

        public Bitmap _BitmapCache;

        public Bitmap BitmapCache
        {
            get { return _BitmapCache; }
            set
            {
                if (_BitmapCache != value)
                {
                    _BitmapCache = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("BitmapCache"));
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