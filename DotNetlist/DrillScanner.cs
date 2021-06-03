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
    /// When a drill bitmap is scanned, it results as nets that extend each hole.
    /// This class scans each net, checks if it is a hole (i.e. if it can be inscribed
    /// in a square) and generates a list of coordinates for each hole.
    /// </summary>
    public class DrillScanner
    {
        /// <summary>
        /// Local scanner of the drill layer.
        /// </summary>
        private readonly BitmapScanner drillBitmap;

        /// <summary>
        /// Private list of the hole coordinates.
        /// </summary>
        private List<PointF> holes = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DrillScanner"/> class.
        /// </summary>
        /// <param name="drillBitmap">BitmapScanner that has already processed the drill bitmap.</param>
        public DrillScanner(BitmapScanner drillBitmap)
        {
            this.drillBitmap = drillBitmap;
        }

        /// <summary>
        /// Computes all the holes returning their coordinates.
        /// A hole is indeed a set of scan lines connected, i.e. a net. There is one test done: the minimum rectangle that
        /// includes the net must be square with sides measuring more than 2 pixels..
        /// </summary>
        /// <returns>The list of points where the holes are.</returns>
        public List<PointF> GetHoles()
        {
            if (this.holes == null)
            {
                this.FindHoles();
            }

            return this.holes;
        }

        /// <summary>
        /// Finds all the holes on the layer.
        /// </summary>
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

        /// <summary>
        /// Checks whether the passed net can represent a hole.
        /// It may be possible that the drill layer contains also other signs, like the outline, that are not real holes.
        /// </summary>
        /// <param name="net">Net it found on the drill layer.</param>
        /// <param name="hole">Point where the hole lies.</param>
        /// <returns>True if the net represents a hole.</returns>
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