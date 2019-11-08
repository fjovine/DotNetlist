//-----------------------------------------------------------------------
// <copyright file="AbstractBitmapGenerator.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.10.11</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{
    /// <summary>
    /// Abstract class able to generate a bitmap.
    /// It must be extended to create a bitmap generator with specific features.
    /// Currently, two subclasses are available, one for compressed PNG bitmaps, the other
    /// for bitmaps rendered as characters, a whitespace for off pixel, a X for on pixels.
    /// </summary>
    public abstract class AbstractBitmapGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBitmapGenerator"/> class.
        /// </summary>
        /// <param name="width">Width of the bitmap in pixel.</param>
        /// <param name="height">Height of the bitmap in pixel.</param>
        public AbstractBitmapGenerator(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets the height in pixel of the bitmap.
        /// </summary>
        public int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the width in pixel of the bitmap.
        /// </summary>
        public int Width
        {
            get;
            private set;
        }

        /// <summary>
        /// Draws the horizontal passed horizontal segment.
        /// </summary>
        /// <param name="segment">Segment to be drawn.</param>
        /// <param name="intensity">The segment is white when intensity is more than 10.</param>
        public void DrawSegment(Segment segment, int intensity)
        {
            for (var x = segment.XMin; x <= segment.XMax; x++)
            {
                this.SetPixel(x, segment.Y, intensity);
            }
        }

        /// <summary>
        /// Saves the drawn bitmap to the passed filename.
        /// </summary>
        /// <param name="filename">Name of the file where the bitmap must be saved.</param>
        public abstract void SaveTo(string filename);

        /// <summary>
        /// Sets a single pixel at the coordinate passed with a white color corresponding to the
        /// intensity value.
        /// </summary>
        /// <param name="x">Abscissa of the pixel to be set.</param>
        /// <param name="y">Ordinate of the pixel to be set.</param>
        /// <param name="intensity">Intensity of the white pixel.</param>
        public abstract void SetPixel(int x, int y, int intensity);
    }
}