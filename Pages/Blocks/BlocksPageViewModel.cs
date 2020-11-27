﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace SurvivalcraftTextureStudio
{
    public class BlocksPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public static BlocksPageViewModel BPVM;
        public int NowPerBlockSize = 32;
        public PixelFormat NowPixelFormat = PixelFormat.Format32bppArgb;

        public BlocksPageViewModel()
        {
            if (BlockTexturesDictionary == null)
            {
                BlockTexturesDictionary = new Dictionary<int, BlockTextureInfo>();
                Bitmap OrigianlBlocksTexture = Properties.Resources.OriginalBlocksTextureFrom2_2;
                NowPerBlockSize = OrigianlBlocksTexture.Width / 16;
                NowPixelFormat = OrigianlBlocksTexture.PixelFormat;
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        Bitmap tempBitmap = ImageHelper.GetBlockBitmapFromTexture(OrigianlBlocksTexture, i * 16 + j, NowPerBlockSize);
                        BlockTexturesDictionary.Add(i * 16 + j, new BlockTextureInfo(i * 16 + j) {BitmapCache = tempBitmap });
                    }
                }
                System.Diagnostics.Debug.WriteLine("材质加载完成");
            }
            BPVM = this;
            InitiateCommands();
            System.Diagnostics.Debug.WriteLine("BlocksPageViewModel加载完成");
        }
        public int _BlockIndexOnFocus;
        public int BlockIndexOnFocus
        {
            get { return _BlockIndexOnFocus; }
            set
            {
                foreach(BlockTextureInfo block in BlockTexturesDictionary.Values)
                {
                    block.IsFocused = false;
                }
                _BlockIndexOnFocus = value;
                if(value>-1)BlockTexturesDictionary[value].IsFocused = true;
            }
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

        public bool _IsExportCacceled = false;

        public bool IsExportCanceled
        {
            get { return _IsExportCacceled; }
            set
            {
                if (_IsExportCacceled != value)
                {
                    _IsExportCacceled = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExportCanceled"));
                }
            }
        }
        public bool _ExportButtonRecover = false;

        public bool ExportButtonRecover
        {
            get { return _ExportButtonRecover; }
            set
            {
                if (_ExportButtonRecover != value)
                {
                    _ExportButtonRecover = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ExportButtonRecover"));
                }
            }
        }
        public void ImportBlocksTexture()
        {
            if (IsOperatingBlocksTexture)
            {
                return;
            }
            IsOperatingBlocksTexture = true;
            Thread ImportThread = new Thread(() =>
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Title = "选择图片";
                openFileDialog.Filter = "png文件|*.png|所有文件|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;
                openFileDialog.DefaultExt = "png";
                if (openFileDialog.ShowDialog().Value)
                {
                    Dictionary<int, BlockTextureInfo> newBlockTexturesDictionary = new Dictionary<int, BlockTextureInfo>();
                    Bitmap bitmap = new Bitmap(new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read));
                    int NewPerBlockSize = bitmap.Width / 16;
                    PixelFormat NewPixelFormat = bitmap.PixelFormat;
                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            Bitmap tempBitmap = ImageHelper.GetBlockBitmapFromTexture(bitmap, i * 16 + j, NewPerBlockSize);
                            newBlockTexturesDictionary.Add(i * 16 + j, new BlockTextureInfo(i * 16 + j) { BitmapCache = tempBitmap });
                        }
                    }
                    lock (BlockTexturesDictionary)
                    {
                        NowPerBlockSize = NewPerBlockSize;
                        NowPixelFormat = NewPixelFormat;
                        BlockTexturesDictionary = newBlockTexturesDictionary;
                    }
                }
                IsOperatingBlocksTexture = false;
            });
            ImportThread.SetApartmentState(ApartmentState.STA);
            ImportThread.Start();
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
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Title = "选择图片";
                openFileDialog.Filter = "png文件|*.png|所有文件|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;
                openFileDialog.DefaultExt = "png";
                if (openFileDialog.ShowDialog().Value)
                {
                    lock (block)
                    {
                        block.BitmapCache = ImageHelper.ResizeBitmapByImageSharp(new Bitmap(new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read)), NowPerBlockSize, NowPerBlockSize);
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
            IsExportCanceled = false;
            ExportButtonRecover = false;
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
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog()
                {
                    Title = "选择保存位置",
                    Filter = "png文件|*.png|所有文件|*.*",
                    FileName = "Blocks.png",
                    FilterIndex = 1,
                    RestoreDirectory = true,
                    DefaultExt = "png"
                };
                if (saveFileDialog.ShowDialog().Value)
                {
                    tempBitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                    IsExportComplete = true;
                }
                else
                {
                    IsExportCanceled = true;
                }
                IsOperatingBlocksTexture = false;
                Thread.Sleep(3000);
                if (!IsOperatingBlocksTexture)
                {
                    IsExportComplete = false;
                    IsExportCanceled = false;
                    ExportButtonRecover = true;
                }
            });
            ExportThread.SetApartmentState(ApartmentState.STA);
            ExportThread.Start();
        }
    }
}