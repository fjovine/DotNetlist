//-----------------------------------------------------------------------
// <copyright file="BitmapScanner.cs" company="Francesco Iovine">
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
    using System.Diagnostics;

    public class BitmapScanner
    {
        private readonly IBitmapAccessor bitmap;

        private readonly List<Segment> segments = new List<Segment>();

        private readonly List<Scanline> scanlines = new List<Scanline>();

        private readonly Dictionary<int, List<Segment>> netlists = new Dictionary<int, List<Segment>>();

        public BitmapScanner(IBitmapAccessor bitmap)
        {
            this.bitmap = bitmap;
        }

        public List<Scanline> ScanlineIndex
        {
            get
            {
                return this.scanlines;
            }
        }

        public List<Segment> Segments
        {
            get
            {
                return this.segments;
            }
        }

        public List<Segment> GetSegmentsOfNet(int netId)
        {
            return this.netlists[netId];
        }

        public void Scan()
        {
            Segment currentSegment = new Segment();
            bool isSegmentOpen = false;

            for (int y = 0; y < this.bitmap.Height; y++)
            {
                // Console.WriteLine($"Y={y}");
                var currentScanline = new Scanline()
                {
                    Y = y,
                    InitialIndex = this.segments.Count,
                };

                bool previousPixel = false;
                for (int x = 0; x <= this.bitmap.Width; x++)
                {
                    bool currentPixel = x < this.bitmap.Width ? this.bitmap.PixelAt(x, y) : false;
                    if (!previousPixel && currentPixel)
                    {
                        // Console.WriteLine($"Start at {x}");
                        // Starts a contiguous segment
                        currentSegment = new Segment()
                        {
                            Y = y,
                            XMin = x,
                        };
                        isSegmentOpen = true;
                    }

                    if (previousPixel && !currentPixel)
                    {
                        // Console.WriteLine($"End at {x-1}");
                        // Ends a contiguous segment
                        Debug.Assert(isSegmentOpen, "The segment must be open in this case.");
                        currentSegment.XMax = x - 1;
                        currentScanline.Length++;
                        this.segments.Add(currentSegment);
                        isSegmentOpen = false;
                    }

                    previousPixel = currentPixel;
                }

                if (currentScanline.Length > 0)
                {
                    this.scanlines.Add(currentScanline);
                }
            }
        }

        public void ComputeNetlists()
        {
            int netlist = 1;
            Scanline previousScanline = null;
            foreach (var scanline in this.scanlines)
            {
                scanline.Foreach(
                    this.segments,
                    (segment) =>
                    {
                        segment.NetList = netlist++;
                        if ((previousScanline == null) || (previousScanline.Y < scanline.Y - 1))
                        {
                            return;
                        }

                        var touchingSegments = previousScanline.GetTouchingSegments(this.segments, segment);
                        bool isFirst = true;
                        foreach (var touchingSegment in touchingSegments)
                        {
                            if (isFirst)
                            {
                                segment.NetList = touchingSegment.NetList;
                                isFirst = false;
                            }
                            else
                            {
                                scanline.BackPropagate(this.segments, segment.NetList, touchingSegment.NetList);
                            }
                        }
                    });
                previousScanline = scanline;
            }
        }

        public void CompactNets()
        {
            SortedDictionary<int, int> nets = new SortedDictionary<int, int>();
            int sequential = 1;
            foreach (var segment in this.segments)
            {
                if (!nets.ContainsKey(segment.NetList))
                {
                    nets.Add(segment.NetList, sequential++);
                }
            }

            foreach (var segment in this.segments)
            {
                segment.NetList = nets[segment.NetList];
            }
        }

        public void MapNetlists()
        {
            foreach (var segment in this.segments)
            {
                var netId = segment.NetList;
                if (!this.netlists.ContainsKey(netId))
                {
                    this.netlists.Add(netId, new List<Segment>());
                }

                this.netlists[netId].Add(segment);
            }
        }

        public IEnumerable<int> GetNetIds()
        {
            foreach (var netId in this.netlists.Keys)
            {
                yield return netId;
            }
        }
    }
}