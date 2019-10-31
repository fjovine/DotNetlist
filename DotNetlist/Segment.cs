//-----------------------------------------------------------------------
// <copyright file="Segment.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.10.11</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{
    /// <summary>
    /// Represents a horizontal segment of continuous copper on the
    /// PCB.
    /// </summary>
    public class Segment
    {
        /// <summary>
        /// Gets or sets the ordinate of the segment.
        /// As the segment is horizontal, this ordinate is common to all the points
        /// composing the segment.
        /// </summary>
        public int Y
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum abscissa of the horizontal segment.
        /// </summary>
        public int XMin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum abscissa of the horizontal segment.
        /// </summary>
        public int XMax
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the net index the segment belongs to.
        /// </summary>
        public int NetList
        {
            get;
            set;
        }

        /// <summary>
        ///  Computes whether the current segment touches another one.
        ///  A segment touches another segment if and only if
        ///  * the ordinate of the segments differ of 1 pixel, i.e. they are contiguous vertically
        ///  * there is at least one pixel having the same abscissa in common between the segments.
        /// </summary>
        /// <param name="other">The segment that is verified.</param>
        /// <returns>True if this segment touches the other one.</returns>
        public bool Touches(Segment other)
        {
            if (this.Y != other.Y + 1)
            {
                return false;
            }

            if (other.XMax < this.XMin)
            {
                return false;
            }

            if (other.XMin > this.XMax)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Computes a textual representation of the segment.
        /// </summary>
        /// <returns>A string containing the relevant properties of the segment.</returns>
        public override string ToString()
        {
            return $"{this.Y},{this.XMin}-{this.XMax},{this.NetList}";
        }
    }
}