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
    using System.Drawing;

    /// <summary>
    /// Real implementation of the <see cref="AbstractBitmapGenerator"/> for a real bitmap
    /// backed up by a PNG image file.
    /// </summary>
    public class RealBitmapGenerator : AbstractBitmapGenerator
    {
        /// <summary>
        /// Local store of the bitmap.
        /// </summary>
        private readonly Bitmap bitmap;

        /// <summary>
        /// Initializes a new instance of the <see cref="RealBitmapGenerator"/> class.
        /// </summary>
        /// <param name="width">Width of the bitmap in pixels.</param>
        /// <param name="height">Height of the bitmap in pixels.</param>
        public RealBitmapGenerator(int width, int height)
          : base(width, height)
        {
            this.bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(this.bitmap))
            {
                g.Clear(Color.Black);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RealBitmapGenerator"/> class.
        /// The bitmap is a clone of an existing one.
        /// </summary>
        /// <param name="backgroundBitmap">The initial bitmap from which new pixels will be set or reset.</param>
        public RealBitmapGenerator(Bitmap backgroundBitmap)
          : base(backgroundBitmap.Width, backgroundBitmap.Height)
        {
            RectangleF cloneRect = new RectangleF(0, 0, this.Width, this.Height);
            this.bitmap = backgroundBitmap.Clone(cloneRect, backgroundBitmap.PixelFormat);
        }

        /// <summary>
        /// Gets the current bitmap.
        /// </summary>
        /// <returns>The current bitmap.</returns>
        public Bitmap GetBitmap()
        {
            return this.bitmap;
        }

        /// <summary>
        /// Sets a pixel at the specified coordinates with the passed intensity.
        /// </summary>
        /// <param name="x">Abscissa of the pixel to set.</param>
        /// <param name="y">Ordinate of the pixel to set.</param>
        /// <param name="intensity">Intensity of the pixel. If > 10 the pixel will be white</param>
        public override void SetPixel(int x, int y, int intensity)
        {
            this.bitmap.SetPixel(x, y, intensity > 10 ? Color.White : Color.DarkGray);
        }

        /// <summary>
        /// Saves the generated bitmap to a PNG file.
        /// </summary>
        /// <param name="filename">Name of the file where the bitmap will be saved.</param>
        public override void SaveTo(string filename)
        {
            this.bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}