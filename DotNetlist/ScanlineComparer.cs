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

    /// <summary>
    /// Service class that implement the <see cref="IComparer"/> interface for <see cref="Scanline"/>.
    /// </summary>
    public class ScanlineComparer : IComparer<Scanline>
    {
        /// <summary>
        /// Computes an integer number which is negative, positive or zero if
        /// the first scan line is before, following or identical to the second
        /// scan line following the ordering criteria for <see cref="Scanline"/>.
        /// i.e. the ordinate of its points.
        /// </summary>
        /// <param name="first">First scan line to be compared.</param>
        /// <param name="second">Second scan line to be compared.</param>
        /// <returns>An integer for ordering the scan lines.</returns>
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