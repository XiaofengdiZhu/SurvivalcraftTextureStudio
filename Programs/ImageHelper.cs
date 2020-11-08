using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
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

        private static ResizedSIzeAndCropRectangleOrTargetRectangle ResizeBitmapPretreatment(Bitmap input, int width, int height, ScaleMode mode)
        {
            double proportion = width / height;
            int inputWidth = input.Width;
            int inputHeight = input.Height;
            double inputProportion = inputWidth / inputHeight;
            double scale = 1;
            if (mode == ScaleMode.Fill || mode == ScaleMode.Fit)
            {
                if (isProportionTheSame(inputWidth, inputHeight, width, height))
                {
                    scale = (double)width / (double)inputWidth;
                }
                else
                {
                    if (mode == ScaleMode.Fill)
                    {
                        if (width > inputWidth)
                        {
                            scale = (double)width / (double)inputWidth;
                            int newHeight = (int)Math.Round(inputHeight * scale);
                            int y1 = (newHeight - height) / 2;
                            return new ResizedSIzeAndCropRectangleOrTargetRectangle() { ResizedSIze = new System.Drawing.Size(width, newHeight), CropRectangle = new System.Drawing.Rectangle(0, y1, width, height) };
                        }
                        else if (height > inputHeight)
                        {
                            scale = (double)height / (double)inputHeight;
                            int newWidth = (int)Math.Round(inputWidth * scale);
                            int x1 = (newWidth - width) / 2;
                            return new ResizedSIzeAndCropRectangleOrTargetRectangle() { ResizedSIze = new System.Drawing.Size(newWidth, height), CropRectangle = new System.Drawing.Rectangle(x1, 0, width, height) };
                        }
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                return new ResizedSIzeAndCropRectangleOrTargetRectangle() { ResizedSIze = new System.Drawing.Size(width, height), TargetRectangle = new System.Drawing.Rectangle(0, 0, width, height) };
            }
            return null;
        }

        private static bool isProportionTheSame(int x1, int y1, int x2, int y2)
        {
            if (x1 == y1 && x2 == y2)
            {
                return true;
            }
            if (x1 < y1)
            {
                if (x2 >= y2)
                {
                    return false;
                }
                int temp = x1;
                x1 = y1;
                y1 = temp;
                temp = x2;
                x2 = y2;
                y2 = temp;
            }
            int quotient = x1 / y1;
            if (quotient != x2 / y2)
            {
                return false;
            }
            int x3 = GetLargestCommonDivisor(x1, x2);
            int y3 = GetLargestCommonDivisor(y1, y2);
            int remainder1 = x1 % y1;
            int remainder2 = x2 % y2;
            int remainder3 = x3 % y3;
            if (quotient != x3 / y3)
            {
                return false;
            }
            if (remainder1 % remainder3 == 0 && remainder2 % remainder3 == 0)
            {
                return true;
            }
            return false;
        }

        private static int GetLargestCommonDivisor(int n1, int n2)
        {
            int max = n1 > n2 ? n1 : n2;
            int min = n1 < n2 ? n1 : n2;
            int remainder;
            while (min != 0)
            {
                remainder = max % min;
                max = min;
                min = remainder;
            }
            return max;
        }

        public static Bitmap ResizeBitmap(Bitmap input, int width, int height, ScaleMode scaleMode=ScaleMode.Stretch, System.Drawing.Drawing2D.InterpolationMode interpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor)
        {
            Bitmap output = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(output))
            {
                g.InterpolationMode = interpolationMode;
                g.DrawImage(input, new System.Drawing.Rectangle(0, 0, width, height), new System.Drawing.Rectangle(0, 0, input.Width, input.Height), GraphicsUnit.Pixel);
            }
            return output;
        }

        public static Bitmap ResizeBitmapByImageSharp(Bitmap input, int width, int height, IResampler resampler = null)
        {
            MemoryStream memory = new MemoryStream();
            input.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(memory, new BmpDecoder());
            image.Mutate(i => { i.Resize(width, height, resampler ?? KnownResamplers.NearestNeighbor); });
            MemoryStream newMemory = new MemoryStream();
            image.SaveAsBmp(newMemory);
            return new Bitmap(newMemory);
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

        public enum ScaleMode
        {
            Fill, Fit, Stretch, CenterNoSoScale, TopLeftNoScale, TopRightNoScale, BottomRightNoScale, BottomLeftNoScale
        }
    }

    public class ResizedSIzeAndCropRectangleOrTargetRectangle
    {
        public System.Drawing.Size ResizedSIze;
        public System.Drawing.Rectangle CropRectangle;
        public System.Drawing.Rectangle TargetRectangle;
    }
}