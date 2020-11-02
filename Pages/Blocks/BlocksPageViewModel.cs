using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SurvivalcraftTextureStudio
{
    public class BlocksPageViewModel : INotifyPropertyChanged
    {
        private static BlocksPageViewModel BPV;

        public event PropertyChangedEventHandler PropertyChanged;

        public BlocksPageViewModel()
        {
            BPV = this;
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
        }

        public Dictionary<int, BlockTextureInfo> BlockTexturesDictionary { get; set; }
        public Dictionary<int, BlockTextureInfo>.ValueCollection BlockTexturesValues { get { return BlockTexturesDictionary.Values; } }

        //public List<BlockTextureInfo> BlockTextures { get; set; }
        public ICommand ChangeImageCommand { get; } = new AnotherCommandImplementation(o => ChangeImage((BlockTextureInfo)o));

        public ICommand ExportBlocksTextureCommand { get; } = new AnotherCommandImplementation(o => ExportBlocksTexture());

        public static void ChangeImage(BlockTextureInfo block)
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
                BPV.BlockTexturesDictionary[block.Index].Name = "test";
                BPV.BlockTexturesDictionary[block.Index].Texture = new BitmapImage(new Uri(openFileDialog.FileName));
                BPV.BlockTexturesDictionary[block.Index].BitmapCache = new Bitmap(new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read));
                Debug.WriteLine(block.Index);
            }
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
            return bitmapImage;
        }

        public static void ExportBlocksTexture()
        {
            Bitmap first = BPV.BlockTexturesDictionary[0].BitmapCache;
            int perBlockSize = first.Width;
            PixelFormat pixelFormat = first.PixelFormat;
            Bitmap tempBitmap = new Bitmap(perBlockSize * 16, perBlockSize * 16, pixelFormat);
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    for (int y = 0; y < perBlockSize; y++)
                    {
                        for (int x = 0; x < perBlockSize; x++)
                        {
                            tempBitmap.SetPixel(j * perBlockSize + x, i * perBlockSize + y, BPV.BlockTexturesDictionary[i * 16 + j].BitmapCache.GetPixel(x, y));
                        }
                    }
                }
            }
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Title = "选择保存位置";
            saveFileDialog.Filter = "png文件|*.png|所有文件|*.*";
            saveFileDialog.FileName = "Blocks.png";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.DefaultExt = "png";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tempBitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }
    }
}