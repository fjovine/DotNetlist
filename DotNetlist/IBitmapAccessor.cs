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
    public interface IBitmapAccessor
    {
        int Width
        {
            get;
        }

        int Height
        {
            get;
        }

        bool PixelAt(int x, int y);
    }
}