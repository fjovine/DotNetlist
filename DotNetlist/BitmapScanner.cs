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

    /// <summary>
    /// This class implements the algorithm of scanning the passed bitmap.
    /// It starts from the topmost <see cref="Scanline"/> and builds all the segments, connecting them to each net as soon as a relation of being touched
    /// by other segments is found.
    /// </summary>
    public class BitmapScanner
    {
        /// <summary>
        /// Local repository of the bitmap being scanned.
        /// </summary>
        private readonly IBitmapAccessor bitmap;

        /// <summary>
        /// List of segments found.
        /// </summary>
        private readonly List<Segment> segments = new List<Segment>();

        /// <summary>
        /// List of horizontal scan lines composing the bitmap.
        /// </summary>
        private readonly List<Scanline> scanlines = new List<Scanline>();

        /// <summary>
        /// Maps a net identifier to the list of segments belonging to that net.
        /// This means that all the segments in each list is electrically connected with all the others.
        /// </summary>
        private readonly Dictionary<int, List<Segment>> netlists = new Dictionary<int, List<Segment>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapScanner"/> class, ready to scan the passed bitmap.
        /// </summary>
        /// <param name="bitmap">Bitmap to be scanned.</param>
        public BitmapScanner(IBitmapAccessor bitmap)
        {
            this.bitmap = bitmap;
        }

        /// <summary>
        /// Gets the list of scan lines.
        /// </summary>
        public List<Scanline> ScanlineIndex
        {
            get
            {
                return this.scanlines;
            }
        }

        /// <summary>
        /// Gets the list of segments contained in this bitmap.
        /// </summary>
        public List<Segment> Segments
        {
            get
            {
                return this.segments;
            }
        }

        /// <summary>
        /// Gets the list of segments belonging to the passed bitmap.
        /// </summary>
        /// <param name="netId">Identifier of the required net.</param>
        /// <returns>The list of segments belonging to the passed net.</returns>
        public List<Segment> GetSegmentsOfNet(int netId)
        {
            return this.netlists[netId];
        }

        /// <summary>
        /// Prepares the net list, i.e. computes them, compacts the, i.e. reduces the net identifiers to consecutive
        /// numbers.
        /// </summary>
        public void PrepareAll()
        {
            this.Scan();
            this.ComputeNetlists();
            this.CompactNets();
            this.MapNetlists();
        }

        /// <summary>
        /// Scans the bitmap, i.e. builds the list of scanned segments.
        /// </summary>
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

        /// <summary>
        /// Checks which segments are connected together and sets an identical net identifier to all of them.
        /// </summary>
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

        /// <summary>
        /// Computes the number of nets found.
        /// </summary>
        /// <returns>Returns the number of nets found.</returns>
        public int GetNetCount()
        {
            return this.netlists.Keys.Count;
        }

        /// <summary>
        /// After having checked which segments are electrically connected, the net identifiers are non sequential integers.
        /// This method compacts them so that all the net identifiers are sequential integers starting from 1.
        /// </summary>
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

        /// <summary>
        /// After the net identifiers have been computed, fills the dictionary <see cref="netlists"/> that map every net identifier to the
        /// list of segments composing the net.
        /// </summary>
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

        /// <summary>
        /// Returns true if the passed point is inside a net.
        /// If this is the case, then the net id is passed
        /// to the out parameter.
        /// </summary>
        /// <param name="x">Abscissa of the point to check.</param>
        /// <param name="y">Ordinate of the point to check.</param>
        /// <param name="netId">Net Id of the net touched by the point, if any.</param>
        /// <returns>True if the passed point lies inside a net, i.e. copper.</returns>
        public bool TryGetNetAt(float x, float y, out int netId)
        {
            var reference = new Scanline()
            {
                Y = (int)y,
            };
            var index = this.scanlines.BinarySearch(reference, new ScanlineComparer());
            netId = 1;
            if (index < 0)
            {
                // No scanline at the passed orindate
                return false;
            }

            // A scanline exists at the passed ordinate
            Scanline found = this.scanlines[index];
            for (int i = 0; i <= found.Length; i++)
            {
                Segment segment = this.segments[found.InitialIndex + i];
                if (segment.ContainsAbscissa(x))
                {
                    netId = segment.NetList;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the list of available net identifiers.
        /// </summary>
        /// <returns>The list of available net identifiers.</returns>
        public IEnumerable<int> GetNetIds()
        {
            foreach (var netId in this.netlists.Keys)
            {
                yield return netId;
            }
        }
    }
}