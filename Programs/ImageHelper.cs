using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SurvivalcraftTextureStudio
{
    public static class ImageHelper
    {
        public static Image CropImage(Image input,int x,int y,int width,int height)
        {
            return input.Clone(i =>
            {
                i.Crop(new Rectangle(x, y, width, height));
            });
        }
        public static Image GetBlockImageFromTexture(Image texture, int index, int perBlockSize)
        {
            if (index < 0 || index > 255)
            {
                return null;
            }
            else
            {
                return CropImage(texture, index % 16 * perBlockSize, index / 16 * perBlockSize, perBlockSize, perBlockSize);
            }
        }
        public static Image ResizeImage(Image input, int width, int height, ResizeMode mode = ResizeMode.Stretch, IResampler resampler = null)
        {
            return input.Clone(i =>
            {
                i.Resize(new ResizeOptions()
                {
                    Size = new Size(width, height),
                    Mode = mode,
                    Sampler = resampler ?? KnownResamplers.NearestNeighbor
                });
            });
        }
        public static System.Windows.Media.Imaging.BitmapImage Bitmap2BitmapImage(Image image)
        {
            if (image == null) return null;
            lock (image)
            {
                MemoryStream memory = new MemoryStream();
                image.Save(memory, new PngEncoder());
                memory.Position = 0;
                System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
    }
}
