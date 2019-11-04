//-----------------------------------------------------------------------
// <copyright file="NetMapper.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.10.31</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{
    /// <summmary>
    /// Manages the correspondance between the nets based on the drill layer.
    /// Receives the bitmaps already scanned of the top and bottom layers as well
    /// as the bitmap of the holes and determines which nets of the  top layer
    /// are connected to which nets of the bottom layer
    /// </summary>
    public class NetMapper
    {
        public NetMapper(BitmapScanner top, BitmapScanner drill, BitmapScanner bottom)
        {
        }
    }
}