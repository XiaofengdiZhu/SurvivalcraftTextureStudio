using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace SurvivalcraftTextureStudio
{
    public static class ImageHelper
    {
        public static Bitmap CropBitmap(Bitmap input, int x, int y, int width, int height)
        {
            System.Drawing.Rectangle cropRect = new System.Drawing.Rectangle(x, y, width, height);
            Bitmap output = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(output))
            {
                g.DrawImage(input, new System.Drawing.Rectangle(0, 0, width, height), cropRect, GraphicsUnit.Pixel);
            }
            return output;
        }

        public static Bitmap GetBlockBitmapFromTexture(Bitmap texture, int index, int perBlockSize)
        {
            if (index < 0 || index > 255)
            {
                return null;
            }
            else
            {
                return CropBitmap(texture, index % 16 * perBlockSize, index / 16 * perBlockSize, perBlockSize, perBlockSize);
            }
        }

        public static Bitmap ResizeBitmap(Bitmap input, int width, int height, System.Drawing.Drawing2D.InterpolationMode interpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor)
        {
            Bitmap output = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(output))
            {
                g.InterpolationMode = interpolationMode;
                g.DrawImage(input, new System.Drawing.Rectangle(0, 0, width, height), new System.Drawing.Rectangle(0, 0, input.Width, input.Height), GraphicsUnit.Pixel);
            }
            return output;
        }

        public static Bitmap ResizeBitmapByImageSharp(Bitmap input, int width, int height, ResizeMode mode = ResizeMode.Stretch, IResampler resampler = null)
        {
            MemoryStream memory = new MemoryStream();
            input.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(memory, new BmpDecoder());
            memory.Dispose();
            image.Mutate(i =>
            {
                i.Resize(new ResizeOptions()
                {
                    Size = new SixLabors.ImageSharp.Size(width, height),
                    Mode = mode,
                    Sampler = resampler ?? KnownResamplers.NearestNeighbor
                });
            });
            MemoryStream newMemory = new MemoryStream();
            image.SaveAsBmp(newMemory);
            image.Dispose();
            return new Bitmap(newMemory);
        }

        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            if (bitmap == null) return null;
            lock (bitmap)
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
        }
    }

    public class ResizedSIzeAndCropRectangleOrTargetRectangle
    {
        public System.Drawing.Size ResizedSIze;
        public System.Drawing.Rectangle CropRectangle;
        public System.Drawing.Rectangle TargetRectangle;
    }
}