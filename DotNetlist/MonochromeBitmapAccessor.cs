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

    /// <summary>
    /// Describes a real class that implements the <see cref="IBitmapAccessor"/> interface using
    /// a <see cref="Bitmap"/> of the dot net framework.
    /// </summary>
    public class MonochromeBitmapAccessor : IBitmapAccessor
    {
        /// <summary>
        /// Local store of the bitmap.
        /// </summary>
        private readonly Bitmap bitmap;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonochromeBitmapAccessor"/> class loading
        /// the bitmap in PNG format from the passed pathname.
        /// </summary>
        /// <param name="path">Pathname of the bitmap to be loaded.</param>
        public MonochromeBitmapAccessor(string path)
        {
            this.bitmap = new Bitmap(path);
        }

        /// <summary>
        /// Gets a reference to the bitmap.
        /// </summary>
        public Bitmap Bitmap
        {
            get
            {
                return this.bitmap;
            }
        }

        /// <summary>
        /// Gets the horizontal size of the bitmap in pixel.
        /// </summary>
        public int Width
        {
            get
            {
                return this.bitmap.Width;
            }
        }

        /// <summary>
        /// Gets the vertical size of the bitmap in pixel.
        /// </summary>
        public int Height
        {
            get
            {
                return this.bitmap.Height;
            }
        }

        /// <summary>
        /// Returns true if the pixel at the passed coordinates is set.
        /// </summary>
        /// <param name="x">Abscissa of the read pixel.</param>
        /// <param name="y">Ordinate of the read pixel.</param>
        /// <returns>True of the pixel is set.</returns>
        public bool PixelAt(int x, int y)
        {
            var pixel = this.bitmap.GetPixel(x, y);

            return (pixel.R > 10) || (pixel.G > 10) || (pixel.B > 10);
        }
    }
}