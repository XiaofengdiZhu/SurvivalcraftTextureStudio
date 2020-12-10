using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            BPVM = this;
            InitiateCommands();
        }

        public void InitiateBlockTexturesDictionary()
        {
            IsOperatingBlocksTexture = true;
            Task.Factory.StartNew(() =>
            {
                MemoryStream memory = new MemoryStream();
                Properties.Resources.OriginalBlocksTextureFrom2_2.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                ImportBlocksTexture(File.ReadAllText(BlocksdataPath, Encoding.UTF8), Image.Load(memory, new PngDecoder()));
                IsOperatingBlocksTexture = false;
            });
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

        public ObservableDictionary<int, BlockTextureInfo> _BlockTexturesDictionary = new ObservableDictionary<int, BlockTextureInfo>();

        public ObservableDictionary<int, BlockTextureInfo> BlockTexturesDictionary
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
            PreviewImageCommand = new AnotherCommandImplementation(o =>
            {
                PreviewingImageIndex = ((BlockTextureInfo)o).Index;
                IsPreviewing = true;
            });
            ClosePreviewImageCommand = new AnotherCommandImplementation(o =>
            {
                BlocksPage.BP.PreviewGridRowDefinition.Height = new System.Windows.GridLength(BlocksPage.BP.PreviewGridRowDefinition.ActualHeight, System.Windows.GridUnitType.Pixel);
                IsPreviewing = false;
            });
        }

        public ICommand ChangeImageCommand { get; set; }

        public ICommand EditImageCommand { get; set; }

        public ICommand ExportBlocksTextureCommand { get; set; }
        public ICommand PreviewImageCommand { get; set; }
        public ICommand ClosePreviewImageCommand { get; set; }
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
                    ImportBlocksTexture(File.ReadAllText(BlocksdataPath, Encoding.UTF8), Image.Load(new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read)));
                }
                IsOperatingBlocksTexture = false;
            });
            ImportThread.SetApartmentState(ApartmentState.STA);
            ImportThread.Start();
        }

        public void ImportBlocksTexture(string inputString, Image inputImage)
        {
            ObservableDictionary<int, BlockTextureInfo> newBlockTexturesDictionary = GetBlockTexturesDictionaryFromStringAndBitmap(inputString, inputImage);
            int NewPerBlockSize = inputImage.Width / 16;
            lock (BlockTexturesDictionary)
            {
                NowPerBlockSize = NewPerBlockSize;
                BlockTexturesDictionary = newBlockTexturesDictionary;
                PropertyChanged(this, new PropertyChangedEventArgs("PreviewingImage"));
            }
        }

        public static ObservableDictionary<int, BlockTextureInfo> GetBlockTexturesDictionaryFromStringAndBitmap(string inputString, Image inputImage)
        {
            ObservableDictionary<int, BlockTextureInfo> output = new ObservableDictionary<int, BlockTextureInfo>();
            int perBlockSize = inputImage.Width / 16;
            int[] baseInformationLocation = new int[3];
            Dictionary<CultureInfo, int[]> otherInformationLocation = new Dictionary<CultureInfo, int[]>();
            string[] lines = inputString.Split('\n');
            string firstLine = lines[0];
            int i = 0;
            foreach (string title in firstLine.Split('\t'))
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
            for (int j = 1; j < lines.Length; j++)
            {
                string block = lines[j];
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
                            ImageCache = ImageHelper.GetBlockImageFromTexture(inputImage, index, perBlockSize)
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
                            ImageCache = ImageHelper.GetBlockImageFromTexture(inputImage, index, perBlockSize)
                        });
                    }
                }
            }
            GC.Collect();
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
                        ChangeImage(Image.Load(new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read)), block);
                    }
                }
                IsOperatingBlocksTexture = false;
            });
            ChangeImageThread.SetApartmentState(ApartmentState.STA);
            ChangeImageThread.Start();
        }

        public void ChangeImage(Image image, BlockTextureInfo block)
        {
            block.ImageCache = ImageHelper.ResizeImage(image, NowPerBlockSize, NowPerBlockSize);
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
                    path = directory + block.Index + ".png";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    block.ImageCache.Save(path, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                }
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.FileName = "mspaint.exe";
                info.Arguments = "\"" + path + "\"";
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
                //Image tempImage = new Image(NowPerBlockSize * 16, NowPerBlockSize * 16);
                Image tempImage = new Image<SixLabors.ImageSharp.PixelFormats.Argb32>(NowPerBlockSize * 16, NowPerBlockSize * 16);
                lock (BlockTexturesDictionary)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            tempImage.Mutate(im =>
                            {
                                im.DrawImage(BlockTexturesDictionary[i * 16 + j].ImageCache, new Point(new Size(j * NowPerBlockSize, i * NowPerBlockSize)), 1);
                            });
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
                    tempImage.Save(saveFileDialog.FileName, new PngEncoder());
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

        public bool _IsPreviewing = false;

        public bool IsPreviewing
        {
            get { return _IsPreviewing; }
            set
            {
                if (_IsPreviewing != value)
                {
                    _IsPreviewing = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsPreviewing"));
                }
            }
        }

        public int _PreviewingImageIndex = -1;

        public int PreviewingImageIndex
        {
            get { return _PreviewingImageIndex; }
            set
            {
                if (_PreviewingImageIndex != value)
                {
                    _PreviewingImageIndex = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("PreviewingImageIndex"));
                    PropertyChanged(this, new PropertyChangedEventArgs("IsPreviewing"));
                    PropertyChanged(this, new PropertyChangedEventArgs("PreviewingBlockTextureInfo"));
                }
            }
        }

        public int _BlockSizeSliderValue = 120;

        public int BlockSizeSliderValue
        {
            get { return _BlockSizeSliderValue; }
            set
            {
                if (_BlockSizeSliderValue != value)
                {
                    _BlockSizeSliderValue = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("BlockSizeSliderValue"));
                }
            }
        }

        public BlockTextureInfo PreviewingBlockTextureInfo
        {
            get
            {
                if (BlockTexturesDictionary is null || BlockTexturesDictionary.Count == 0 || PreviewingImageIndex < 0) { return null; }
                else
                {
                    return BlockTexturesDictionary[PreviewingImageIndex];
                }
            }
        }
    }
}