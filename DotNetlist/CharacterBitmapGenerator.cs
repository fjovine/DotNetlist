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

    public class CharacterBitmapGenerator : AbstractBitmapGenerator
    {
        private StringBuilder[] scanlines;

        public CharacterBitmapGenerator(int width, int height)
          : base(width, height)
        {
            this.scanlines = new StringBuilder[height];
            for (int y = 0; y < height; y++)
            {
                this.scanlines[y] = new StringBuilder();
            }
        }

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