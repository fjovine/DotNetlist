//-----------------------------------------------------------------------
// <copyright file="ScanlineComparer.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.11.02</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class ScanlineComparer : IComparer<Scanline>
    {
        public int Compare([AllowNull] Scanline first, [AllowNull] Scanline second)
        {
            if ((first == null) || (second == null))
            {
                throw new ArgumentException("No scanline ca be null");
            }

            return first.Y - second.Y;
        }
    }
}