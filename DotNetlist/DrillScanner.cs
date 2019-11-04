//-----------------------------------------------------------------------
// <copyright file="DrillScanner.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.10.11</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// This class recognizes the holes from a <see cref="BitmapScanner"/> object passed.
    /// When a drill bitmap is scanned, it results in as nets that extend to each hole.
    /// This class scans each net, checks if it is a hole (i.e.static if it can be inscribed
    /// in a square) and generates a list of coordinates for each hole.
    /// </summary>
    public class DrillScanner
    {
        private BitmapScanner drillBitmap;

        private List<PointF> holes = null;

        public DrillScanner(BitmapScanner drillBitmap)
        {
            this.drillBitmap = drillBitmap;
        }

        public List<PointF> GetHoles()
        {
            if (this.holes == null) 
            {
                this.FindHoles();
            }

            return this.holes;
        }

        private void FindHoles()
        {
            this.holes = new List<PointF>();
            foreach (var net in this.drillBitmap.GetNetIds())
            {
                if (this.TryGetHole(net, out PointF hole))
                {
                    this.holes.Add(hole);
                }
            }
        }

        private bool TryGetHole(int net, out PointF hole)
        {
            hole = new PointF(0, 0);

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var segment in this.drillBitmap.GetSegmentsOfNet(net))
            {
                minX = Math.Min(segment.XMin, minX);
                maxX = Math.Max(segment.XMax, maxX);
                minY = Math.Min(segment.Y, minY);
                maxY = Math.Max(segment.Y, maxY);
            }

            var xSide = 1 + maxX - minX;
            var ySide = 1 + maxY - minY;
            if ((xSide < 2) || (ySide < 2))
            {
                return false;
            }

            if (Math.Abs(xSide - ySide) / (xSide + ySide) < 0.05)
            {
                hole.X = (maxX + minX) / 2.0f;
                hole.Y = (maxY + minY) / 2.0f;
                return true;
            }

            return false;
        }
    }
}