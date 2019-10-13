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

    public class Scanline
    {
        public int Y
        {
            get;
            set;
        }

        public int InitialIndex
        {
            get;
            set;
        }

        public int Length
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"{this.Y}-({this.InitialIndex},{this.Length})";
        }

        public void Foreach(List<Segment> segments, Action<Segment> iterator)
        {
            for (int i = 0; i < this.Length; i++)
            {
                iterator(segments[i + this.InitialIndex]);
            }
        }

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