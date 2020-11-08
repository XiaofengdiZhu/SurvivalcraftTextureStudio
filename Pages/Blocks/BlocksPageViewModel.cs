using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SurvivalcraftTextureStudio
{
    public class BlocksPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public int NowPerBlockSize = 32;
        public PixelFormat NowPixelFormat = PixelFormat.Format32bppArgb;

        public BlocksPageViewModel()
        {
            BlockTexturesDictionary = new Dictionary<int, BlockTextureInfo>();
            Bitmap OrigianlBlocksTexture = new Bitmap(Application.GetResourceStream(new Uri("pack://application:,,,/Resources/OriginalBlocksTextureFrom2.2.png", UriKind.RelativeOrAbsolute)).Stream);
            NowPerBlockSize = OrigianlBlocksTexture.Width / 16;
            NowPixelFormat = OrigianlBlocksTexture.PixelFormat;
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Bitmap tempBitmap = ImageHelper.GetBlockBitmapFromTexture(OrigianlBlocksTexture, i * 16 + j, NowPerBlockSize);
                    BlockTexturesDictionary.Add(i * 16 + j, new BlockTextureInfo(i * 16 + j) { Texture = ImageHelper.Bitmap2BitmapImage(tempBitmap), BitmapCache = tempBitmap });
                }
            }
            InitiateCommands();
        }

        public Dictionary<int, BlockTextureInfo> _BlockTexturesDictionary;

        public Dictionary<int, BlockTextureInfo> BlockTexturesDictionary
        {
            get { return _BlockTexturesDictionary; }
            set
            {
                if (_BlockTexturesDictionary != value)
                {
                    _BlockTexturesDictionary = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("BlockTexturesDictionary"));
                }
            }
        }

        public void InitiateCommands()
        {
            ChangeImageCommand = new AnotherCommandImplementation(o =>
            {
                ChangeImage((BlockTextureInfo)o);
            });
            ExportBlocksTextureCommand = new AnotherCommandImplementation(o =>
            {
                ExportBlocksTexture();
            });
        }

        //public List<BlockTextureInfo> BlockTextures { get; set; }
        public ICommand ChangeImageCommand { get; set; }

        public ICommand ExportBlocksTextureCommand { get; set; }
        public bool _IsOperatingBlocksTexture = false;

        public bool IsOperatingBlocksTexture
        {
            get { return _IsOperatingBlocksTexture; }
            set
            {
                if (_IsOperatingBlocksTexture != value)
                {
                    _IsOperatingBlocksTexture = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsOperatingBlocksTexture"));
                }
            }
        }

        public bool _IsExportComplete = false;

        public bool IsExportComplete
        {
            get { return _IsExportComplete; }
            set
            {
                if (_IsExportComplete != value)
                {
                    _IsExportComplete = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExportComplete"));
                }
            }
        }

        public void ChangeImage(BlockTextureInfo block)
        {
            if (IsOperatingBlocksTexture)
            {
                return;
            }
            IsOperatingBlocksTexture = true;
            Thread ChangeImageThread = new Thread(() =>
            {
                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                openFileDialog.Title = "选择图片";
                openFileDialog.Filter = "png文件|*.png|所有文件|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;
                openFileDialog.DefaultExt = "png";
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    lock (block)
                    {
                        Bitmap bitmap = ImageHelper.ResizeBitmapByImageSharp(new Bitmap(new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read)), NowPerBlockSize, NowPerBlockSize);
                        BitmapImage image = ImageHelper.Bitmap2BitmapImage(bitmap);
                        block.BitmapCache = bitmap;
                        block.Texture = image;
                    }
                }
                IsOperatingBlocksTexture = false;
            });
            ChangeImageThread.SetApartmentState(ApartmentState.STA);
            ChangeImageThread.Start();
        }

        public void ExportBlocksTexture()
        {
            if (IsOperatingBlocksTexture)
            {
                return;
            }
            IsOperatingBlocksTexture = true;
            IsExportComplete = false;
            Thread ExportThread = new Thread(() =>
            {
                Bitmap tempBitmap = new Bitmap(NowPerBlockSize * 16, NowPerBlockSize * 16, NowPixelFormat);
                lock (BlockTexturesDictionary)
                {
                    using (Graphics g = Graphics.FromImage(tempBitmap))
                    {
                    for (int i = 0; i < 16; i++)
                    {
                            for (int j = 0; j < 16; j++)
                            {
                                g.DrawImage(BlockTexturesDictionary[i * 16 + j].BitmapCache, j * NowPerBlockSize, i * NowPerBlockSize);
                            }
                        }
                    }
                }
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog()
                {
                    Title = "选择保存位置",
                    Filter = "png文件|*.png|所有文件|*.*",
                    FileName = "Blocks.png",
                    FilterIndex = 1,
                    RestoreDirectory = true,
                    DefaultExt = "png"
                };
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    tempBitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                    IsExportComplete = true;
                }
                IsOperatingBlocksTexture = false;
            });
            ExportThread.SetApartmentState(ApartmentState.STA);
            ExportThread.Start();
        }
    }
}