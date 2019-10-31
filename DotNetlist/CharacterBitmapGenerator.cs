//-----------------------------------------------------------------------
// <copyright file="CharacterBitmapGenerator.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.10.11</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{
    using System;
    using System.Text;

    /// <summary>
    /// In order to implement unit tests and generate textual bitmaps, this class implements the <see cref="AbstractBitmapGenerator"/> class
    /// so that the bitmap is represented with X characters when a pixel is set and space characters when there is no pixel.
    /// For instance an 8 by 4 raster is represented with an array of strings as follows
    /// XX  XX X
    ///  XXXX  X
    ///    XX
    ///    X
    /// </summary>
    public class CharacterBitmapGenerator : AbstractBitmapGenerator
    {
        /// <summary>
        /// Local store for the generated scan lines each of which is a string builder.
        /// </summary>
        private readonly StringBuilder[] scanlines;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterBitmapGenerator"/> class.
        /// The bitmap is void, i.e. no pixel is active.
        /// </summary>
        /// <param name="width">Width of the character bitmap.</param>
        /// <param name="height">Height of the character bitmap.</param>
        public CharacterBitmapGenerator(int width, int height)
          : base(width, height)
        {
            this.scanlines = new StringBuilder[height];
            for (int y = 0; y < height; y++)
            {
                this.scanlines[y] = new StringBuilder();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterBitmapGenerator"/> class.
        /// The bitmap has a bit set for any set bit of the passed bitmap.
        /// </summary>
        /// <param name="bitmapAccessor">Interface to access the passed bitmap. </param>
        public CharacterBitmapGenerator(IBitmapAccessor bitmapAccessor)
          : base(bitmapAccessor.Width, bitmapAccessor.Height)
        {
            this.scanlines = new StringBuilder[this.Height];
            for (int y = 0; y < this.Height; y++)
            {
                this.scanlines[y] = new StringBuilder();
                for (int x = 0; x < this.Width; x++)
                {
                    this.scanlines[y].Append(bitmapAccessor.PixelAt(x, y) ? 'X' : ' ');
                }
            }
        }

        /// <summary>
        /// Sets a pixel which may have an intensity and it will be rendered accordingly.
        /// If the intensity is 0, then the corresponding char will be a space, from 1 to 9 a dot otherwise an X char.
        /// </summary>
        /// <param name="x">Abscissa of the pixel to set.</param>
        /// <param name="y">Ordinate of the pixel to set.</param>
        /// <param name="intensity">Intensity of the pixel.</param>
        public override void SetPixel(int x, int y, int intensity)
        {
            var scanline = this.scanlines[y];
            if (scanline.Length <= x)
            {
                for (int i = scanline.Length; i < x; i++)
                {
                    scanline.Append(' ');
                }

                scanline.Append(GetPixel(intensity));
            }
            else
            {
                try
                {
                    scanline[x] = GetPixel(intensity);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Eccezione : {x},{y} : {scanline.Length}");
                }
            }

            char GetPixel(int intensity)
            {
                if (intensity == 0)
                {
                    return ' ';
                }
                else if (intensity < 10)
                {
                    return '.';
                }
                else
                {
                    return 'X';
                }
            }
        }

        /// <summary>
        /// Saves the generated bitmap to a text file.
        /// </summary>
        /// <param name="filename">Name of the file where the bitmap will be saved.</param>
        public override void SaveTo(string filename)
        {
            string[] lines = new string[this.scanlines.Length];
            for (int i = 0; i < this.scanlines.Length; i++)
            {
                lines[i] = this.scanlines[i].ToString();
            }

            System.IO.File.WriteAllLines(filename, lines);
        }
    }
}