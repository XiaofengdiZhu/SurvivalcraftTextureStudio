using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using SixLabors.ImageSharp.Processing;

namespace SurvivalcraftTextureStudio
{
    public class ImportBlocksTextureDialogViewModel : INotifyPropertyChanged
    {
        public ImportBlocksTextureDialogViewModel()
        {
            int a = 64;
            while (a <= 8192)
            {
                ResolutionList.Add(new Resolution(a));
                a *= 2;
            }
            PropertyInfo[] properties = typeof(KnownResamplers).GetProperties();
            foreach(var property in properties)
            {
                ResamplerList.Add(new Resampler() {Name=property.Name, iResampler = (IResampler)property.GetValue(typeof(KnownResamplers), null)});
                Debug.WriteLine(ResamplerList.Last().ToString());
            }
            Debug.WriteLine("输出完成");
        }

        public List<Resolution> _resolutionList = new List<Resolution>();

        public List<Resolution> ResolutionList
        {
            get { return _resolutionList; }
            set
            {
                if (_resolutionList != value)
                {
                    _resolutionList = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ResolutionList"));
                }
            }
        }
        public List<Resampler> _resamplerList = new List<Resampler>();

        public List<Resampler> ResamplerList
        {
            get { return _resamplerList; }
            set
            {
                if (_resamplerList != value)
                {
                    _resamplerList = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ResamplerList"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public class Resolution
        {
            public Resolution(int n)
            {
                Width = n;
                Height = n;
            }

            public Resolution(int width, int height)
            {
                Width = width;
                Height = height;
            }
            public int Width { get; set; }
            public int Height { get; set; }

            public override string ToString()
            {
                return Width + "×" + Height;
            }
        }
        public class Resampler
        {
            public string Name { get; set; }
            public IResampler iResampler { get; set; }
            public string Description { get; set; }
            public override string ToString()
            {
                return Name;
            }
        }
    }
}