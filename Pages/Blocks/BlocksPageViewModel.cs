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

        public BlocksPageViewModel()
        {
            BlockTexturesDictionary = new Dictionary<int, BlockTextureInfo>();
            //BlockTextures = new List<BlockTextureInfo>();
            Bitmap OrigianlBlocksTexture = new Bitmap(Application.GetResourceStream(new Uri("pack://application:,,,/Resources/OriginalBlocksTextureFrom2.2.png", UriKind.RelativeOrAbsolute)).Stream);
            int perBlockSize = OrigianlBlocksTexture.Width / 16;
            PixelFormat pixelFormat = OrigianlBlocksTexture.PixelFormat;
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Bitmap tempBitmap = new Bitmap(perBlockSize, perBlockSize, pixelFormat);
                    for (int y = 0; y < perBlockSize; y++)
                    {
                        for (int x = 0; x < perBlockSize; x++)
                        {
                            tempBitmap.SetPixel(x, y, OrigianlBlocksTexture.GetPixel(j * perBlockSize + x, i * perBlockSize + y));
                        }
                    }
                    //BlockTextures.Add(new BlockTextureInfo(i * 16 + j) { Texture= Bitmap2BitmapImage(tempBitmap), BitmapCache = tempBitmap });
                    BlockTexturesDictionary.Add(i * 16 + j, new BlockTextureInfo(i * 16 + j) { Texture = Bitmap2BitmapImage(tempBitmap), BitmapCache = tempBitmap });
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
                    lock (BlockTexturesDictionary)
                    {
                        //BlockTexturesDictionary[block.Index].Name = "test";
                        Bitmap bitmap = new Bitmap(new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read));
                        BitmapImage image = Bitmap2BitmapImage(bitmap);
                        BlockTexturesDictionary[block.Index].BitmapCache = bitmap;
                        BlocksPage.BP.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            BlockTexturesDictionary[block.Index].Texture = image;
                        }));
                    }
                }
                IsOperatingBlocksTexture = false;
            });
            ChangeImageThread.SetApartmentState(ApartmentState.STA);
            ChangeImageThread.Start();
        }

        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
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
                Bitmap tempBitmap;
                lock (BlockTexturesDictionary)
                {
                    Bitmap first = BlockTexturesDictionary[0].BitmapCache;
                    int perBlockSize = first.Width;
                    PixelFormat pixelFormat = first.PixelFormat;
                    tempBitmap = new Bitmap(perBlockSize * 16, perBlockSize * 16, pixelFormat);
                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            for (int y = 0; y < perBlockSize; y++)
                            {
                                for (int x = 0; x < perBlockSize; x++)
                                {
                                    tempBitmap.SetPixel(j * perBlockSize + x, i * perBlockSize + y, BlockTexturesDictionary[i * 16 + j].BitmapCache.GetPixel(x, y));
                                }
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