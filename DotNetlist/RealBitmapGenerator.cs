//-----------------------------------------------------------------------
// <copyright file="RealBitmapGenerator.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.10.11</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{
    using System;
    using System.Drawing;
    using System.Text;

    public class RealBitmapGenerator : AbstractBitmapGenerator
    {
        private readonly Bitmap bitmap;

        public RealBitmapGenerator(int width, int height)
          : base(width, height)
        {
            this.bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(this.bitmap))
            {
                g.Clear(Color.Black);
            }
        }

        public RealBitmapGenerator(Bitmap backgroundBitmap)
          : base(backgroundBitmap.Width, backgroundBitmap.Height)
        {
            RectangleF cloneRect = new RectangleF(0, 0, this.Width, this.Height);
            this.bitmap = backgroundBitmap.Clone(cloneRect, backgroundBitmap.PixelFormat);
        }

        public override void SetPixel(int x, int y, int intensity)
        {
            this.bitmap.SetPixel(x, y, intensity > 10 ? Color.White : Color.DarkGray);
        }

        public override void SaveTo(string filename)
        {
            this.bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}