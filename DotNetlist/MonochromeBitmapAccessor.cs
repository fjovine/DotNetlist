//-----------------------------------------------------------------------
// <copyright file="MonochromeBitmapAccessor.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.10.11</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{
    using System.Drawing;

    public class MonochromeBitmapAccessor : IBitmapAccessor
    {
        private readonly Bitmap bitmap;

        public MonochromeBitmapAccessor(string path)
        {
            this.bitmap = new Bitmap(path);
        }

        public Bitmap Bitmap
        {
            get
            {
                return this.bitmap;
            }
        }

        public int Width
        {
            get
            {
                return this.bitmap.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.bitmap.Height;
            }
        }

        public bool PixelAt(int x, int y)
        {
            var pixel = this.bitmap.GetPixel(x, y);

            return (pixel.R > 10) || (pixel.G > 10) || (pixel.B > 10);
        }
    }
}