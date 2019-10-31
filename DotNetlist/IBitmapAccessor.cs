//-----------------------------------------------------------------------
// <copyright file="IBitmapAccessor.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.10.11</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{
    /// <summary>
    /// Describes a simple access to a virtual bitmap.
    /// </summary>
    public interface IBitmapAccessor
    {
        /// <summary>
        /// Gets the horizontal size of the bitmap in pixel.
        /// </summary>
        int Width
        {
            get;
        }

        /// <summary>
        /// Gets the vertical size of the bitmap in pixel.
        /// </summary>
        int Height
        {
            get;
        }

        /// <summary>
        /// Bitmaps are monochromatic, i.e. either a bit is set or not.
        /// This function determines the presence of a set bit.
        /// </summary>
        /// <param name="x">Abscissa of the pixel.</param>
        /// <param name="y">Ordinate of the pixel.</param>
        /// <returns>True if the bit is set.</returns>
        bool PixelAt(int x, int y);
    }
}