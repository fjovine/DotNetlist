//-----------------------------------------------------------------------
// <copyright file="Scanline.cs" company="Francesco Iovine">
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

    /// <summary>
    /// Describes the segments composing a scan line.
    /// While scanning a scan line, a number of segments are generated that are sequentially stored in
    /// a segment list.
    /// So the scan line holds the index of the first.
    /// </summary>
    public class Scanline
    {
        /// <summary>
        /// Gets or sets the ordinate of the scan line. All the segments composing the scan line share this same value.
        /// </summary>
        public int Y
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index of the first segment in the corresponding segment list that belongs to this scan line.
        /// </summary>
        public int InitialIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of subsequent segments that belong to this scan line.
        /// </summary>
        public int Length
        {
            get;
            set;
        }

        /// <summary>
        /// Builds a textual representation of the object.
        /// </summary>
        /// <returns>The textual representation of the object.</returns>
        public override string ToString()
        {
            return $"{this.Y}-({this.InitialIndex},{this.Length})";
        }

        /// <summary>
        /// Iterates in the list of segments belonging to the scan line in the passed segment list.
        /// </summary>
        /// <param name="segments">The list of segments composing the bitmap.</param>
        /// <param name="iterator">Callback action that is applied to each segment.</param>
        public void Foreach(List<Segment> segments, Action<Segment> iterator)
        {
            for (int i = 0; i < this.Length; i++)
            {
                iterator(segments[i + this.InitialIndex]);
            }
        }

        /// <summary>
        /// Returns the list of segment touched by the passed segment.
        /// This is the list of segments having one less ordinate, that share at least one pixel having the same abscissa with the passed segment.
        /// </summary>
        /// <param name="segments">List of segments composing the scanned bitmap.</param>
        /// <param name="segment">The segment that must be analyzed.</param>
        /// <returns>The list of segments touched by the passed one.</returns>
        public List<Segment> GetTouchingSegments(List<Segment> segments, Segment segment)
        {
            List<Segment> result = new List<Segment>();
            this.Foreach(
                segments,
                (s) =>
                {
                    if (segment.Touches(s))
                    {
                        result.Add(s);
                    }
                });
            return result;
        }

        /// <summary>
        /// Propagates back the new net id.
        /// This method is called whenever a new segment makes a short circuit between segments that have already a net identifier.
        /// It iterates back in the segment list and changes the old net into the new one. At the end of the process all the
        /// connected segment have the new net identifier.
        /// </summary>
        /// <param name="segments">List of the scanned segments.</param>
        /// <param name="newNetlist">Old net identifier.</param>
        /// <param name="oldNetlist">New net identifier.</param>
        public void BackPropagate(List<Segment> segments, int newNetlist, int oldNetlist)
        {
            if (newNetlist == oldNetlist)
            {
                return;
            }

            for (int segmentIndex = this.InitialIndex + this.Length - 1; segmentIndex >= 0; segmentIndex--)
            {
                var currentSegment = segments[segmentIndex];
                if (currentSegment.NetList == oldNetlist)
                {
                    currentSegment.NetList = newNetlist;
                    segments[segmentIndex] = currentSegment;
                }
            }
        }
    }
}