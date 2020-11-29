using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SurvivalcraftTextureStudio
{
    public class BlocksPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static BlocksPageViewModel BPVM;
        public string BlocksdataPath = System.Environment.CurrentDirectory + @"\Resources\BlocksdataFrom2.2.tsv";
        public int NowPerBlockSize = 32;
        public PixelFormat NowPixelFormat = PixelFormat.Format32bppArgb;

        public static Dictionary<CultureInfo, string> MoreText = new Dictionary<CultureInfo, string>()
        {
            {CultureInfo.GetCultureInfo("zh-CN"),"等" },
            {CultureInfo.GetCultureInfo("en"),", etc" }
        };

        public static Dictionary<CultureInfo, string> LocationText = new Dictionary<CultureInfo, string>()
        {
            {CultureInfo.GetCultureInfo("zh-CN"),"{0}行 {1}列" },
            {CultureInfo.GetCultureInfo("en"),", Row {0}, Column {1}" }
        };

        public BlocksPageViewModel()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (BlockTexturesDictionary.Count == 0)
            {
                IsOperatingBlocksTexture = true;
                Task.Factory.StartNew(() =>
                {
                    ImportBlocksTexture(File.ReadAllText(BlocksdataPath, Encoding.UTF8), Properties.Resources.OriginalBlocksTextureFrom2_2);
                    IsOperatingBlocksTexture = false;
                });
            }
            BPVM = this;
            InitiateCommands();
        }

        public int _BlockIndexOnFocus;

        public int BlockIndexOnFocus
        {
            get { return _BlockIndexOnFocus; }
            set
            {
                foreach (BlockTextureInfo block in BlockTexturesDictionary.Values)
                {
                    block.IsFocused = false;
                }
                _BlockIndexOnFocus = value;
                if (value > -1) BlockTexturesDictionary[value].IsFocused = true;
            }
        }

        public Dictionary<int, BlockTextureInfo> _BlockTexturesDictionary = new Dictionary<int, BlockTextureInfo>();

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
                ChangeImageBySelectingFile((BlockTextureInfo)o);
            });
            EditImageCommand = new AnotherCommandImplementation(o =>
            {
                EditImage((BlockTextureInfo)o);
            });
            ExportBlocksTextureCommand = new AnotherCommandImplementation(o =>
            {
                ExportBlocksTexture();
            });
        }

        //public List<BlockTextureInfo> BlockTextures { get; set; }
        public ICommand ChangeImageCommand { get; set; }

        public ICommand EditImageCommand { get; set; }

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

        public void ImportBlocksTextureBySelectingFile()
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
                    ImportBlocksTexture(File.ReadAllText(BlocksdataPath, Encoding.UTF8), new Bitmap(new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read)));
                }
                IsOperatingBlocksTexture = false;
            });
            ImportThread.SetApartmentState(ApartmentState.STA);
            ImportThread.Start();
        }

        public void ImportBlocksTexture(string inputString, Bitmap inputBitmap)
        {
            Dictionary<int, BlockTextureInfo> newBlockTexturesDictionary = GetBlockTexturesDictionaryFromStringAndBitmap(inputString, inputBitmap);
            int NewPerBlockSize = inputBitmap.Width / 16;
            PixelFormat NewPixelFormat = inputBitmap.PixelFormat;
            lock (BlockTexturesDictionary)
            {
                NowPerBlockSize = NewPerBlockSize;
                NowPixelFormat = NewPixelFormat;
                BlockTexturesDictionary = newBlockTexturesDictionary;
            }
        }

        public static Dictionary<int, BlockTextureInfo> GetBlockTexturesDictionaryFromStringAndBitmap(string inputString, Bitmap inputBitmap)
        {
            Dictionary<int, BlockTextureInfo> output = new Dictionary<int, BlockTextureInfo>();
            int perBlockSize = inputBitmap.Width / 16;
            int[] baseInformationLocation = new int[3];
            Dictionary<CultureInfo, int[]> otherInformationLocation = new Dictionary<CultureInfo, int[]>();
            bool FirstLineReaded = false;
            foreach (string block in inputString.Split('\n'))
            {
                if (FirstLineReaded)
                {
                    string[] str = block.Split('\t');
                    if (str.Length > 3 && str.Length % 2 == 1)
                    {
                        int index = int.Parse(str[baseInformationLocation[0]]);
                        int row = index / 16 + 1;
                        int colum = index % 16 + 1;
                        bool isNull = str[baseInformationLocation[1]].ToLower() == "true";
                        Dictionary<CultureInfo, string> description = new Dictionary<CultureInfo, string>();
                        if (isNull)
                        {
                            foreach (var a in otherInformationLocation)
                            {
                                description.Add(a.Key, string.Format(LocationText[a.Key], row, colum));
                            }
                            output.Add(index, new BlockTextureInfo(index)
                            {
                                _Description = description,
                                BitmapCache = ImageHelper.GetBlockBitmapFromTexture(inputBitmap, index, perBlockSize)
                            });
                        }
                        else
                        {
                            Dictionary<CultureInfo, string> name = new Dictionary<CultureInfo, string>();
                            foreach (var a in otherInformationLocation)
                            {
                                name.Add(a.Key, str[a.Value[0]]);
                                description.Add(a.Key, string.Format(LocationText[a.Key], row, colum) + "\n" + (str[a.Value[1]].Length == 0 ? str[a.Value[0]] : str[a.Value[1]]) + (str[baseInformationLocation[2]].ToLower() == "true" ? MoreText[a.Key] : ""));
                            }
                            output.Add(index, new BlockTextureInfo(index)
                            {
                                _Name = name,
                                _Description = description,
                                BitmapCache = ImageHelper.GetBlockBitmapFromTexture(inputBitmap, index, perBlockSize)
                            });
                        }
                    }
                }
                else
                {
                    FirstLineReaded = true;
                    int i = 0;
                    foreach (string title in block.Split('\t'))
                    {
                        if (title == "Index")
                        {
                            baseInformationLocation[0] = i;
                        }
                        else if (title == "IsNull")
                        {
                            baseInformationLocation[1] = i;
                        }
                        else if (title == "IsMore")
                        {
                            baseInformationLocation[2] = i;
                        }
                        else
                        {
                            string[] str = title.Split('.');
                            if (str.Length == 2)
                            {
                                try
                                {
                                    CultureInfo culture = CultureInfo.GetCultureInfo(str[1]);
                                    if (!otherInformationLocation.ContainsKey(culture))
                                    {
                                        switch (str[0])
                                        {
                                            case "Name": otherInformationLocation.Add(culture, new int[2] { i, -1 }); break;
                                            case "Description": otherInformationLocation.Add(culture, new int[2] { -1, i }); break;
                                        }
                                    }
                                    else
                                    {
                                        switch (str[0])
                                        {
                                            case "Name": otherInformationLocation[culture][0] = i; break;
                                            case "Description": otherInformationLocation[culture][1] = i; break;
                                        }
                                    }
                                }
                                catch (System.Exception) { }
                            }
                        }
                        i++;
                    }
                    foreach (KeyValuePair<CultureInfo, int[]> locations in otherInformationLocation)
                    {
                        if (locations.Value[0] == -1 || locations.Value[1] == -1)
                        {
                            otherInformationLocation.Remove(locations.Key);
                        }
                    }
                }
            }
            System.GC.Collect();
            return output;
        }

        public void ChangeImageBySelectingFile(BlockTextureInfo block)
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
                        //block.BitmapCache = ImageHelper.ResizeBitmapByImageSharp(new Bitmap(new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read)), NowPerBlockSize, NowPerBlockSize);
                        ChangeImage(new Bitmap(new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read)), block);
                    }
                }
                IsOperatingBlocksTexture = false;
            });
            ChangeImageThread.SetApartmentState(ApartmentState.STA);
            ChangeImageThread.Start();
        }

        public void ChangeImage(Bitmap bitmap, BlockTextureInfo block)
        {
            block.BitmapCache = ImageHelper.ResizeBitmapByImageSharp(bitmap, NowPerBlockSize, NowPerBlockSize);
        }
        public void EditImage(BlockTextureInfo block)
        {
            if (IsOperatingBlocksTexture)
            {
                return;
            }
            IsOperatingBlocksTexture = true;
            Task.Factory.StartNew(() =>
            {
                string directory = System.Environment.CurrentDirectory + @"\Cache\";
                string path;
                lock (block)
                {
                    path = directory + block.Index + ".bmp";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    block.BitmapCache.Save(path, ImageFormat.Bmp);
                }
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.FileName = "mspaint.exe";
                info.Arguments = "\""+path+"\"";
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = info;
                proc.Start();
                IsOperatingBlocksTexture = false;
            });
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