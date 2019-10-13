using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DotNetlistUnitTest
{
    using DotNetlist;

    [TestClass]
    public class ScanlineTests
    {
        [TestMethod]
        public void Foreach_Works_Well()
        {
            IBitmapAccessor accessor = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "XX XX XXX",
                });
            BitmapScanner scanner = new BitmapScanner(accessor);
            scanner.Scan();
            var dut = scanner.ScanlineIndex[0];
            Console.WriteLine($"Qui {dut}");
            int segmentNo = 0;
            dut.Foreach(
                scanner.Segments, 
                (segment)=>{
                    Console.WriteLine(segment);
                    switch (segmentNo) {
                        case 0:
                            Assert.AreEqual(segment.Y,0);
                            Assert.AreEqual(segment.XMin,0);
                            Assert.AreEqual(segment.XMax,1);
                            break;
                        case 1: 
                            Assert.AreEqual(segment.Y,0);
                            Assert.AreEqual(segment.XMin,3);
                            Assert.AreEqual(segment.XMax,4);
                            break;
                        case 2: 
                            Assert.AreEqual(segment.Y,0);
                            Assert.AreEqual(segment.XMin,6);
                            Assert.AreEqual(segment.XMax,8);
                            break;
                        default:
                            Assert.Fail();
                            break;
                    }

                    segmentNo++;
                });
        }

        [TestMethod]
        public void GetTouchingSegment_Works_Well()
        {
            IBitmapAccessor accessor = new MockupBitmapAccessor (
                new string[] {
                //             111111111122222222223333333333
                //   0123456789012345678901234567890123456789
                    "XX XX XXXXXXX XX XXXX XXXXX XXXXX X XXXX",
                    "        XXXXXXXXXXXXXXXXXXXXXX"
                });
            BitmapScanner scanner = new BitmapScanner(accessor);
            scanner.Scan();
            var lastSegment = scanner.Segments[scanner.Segments.Count-1];
            var lastScanline = scanner.ScanlineIndex[0];
            var touching = lastScanline.GetTouchingSegments(scanner.Segments, lastSegment);

            Assert.AreEqual(5, touching.Count);
            AssertSegmentsAreEqual(0,6,12, touching[0]);
            AssertSegmentsAreEqual(0,14,15, touching[1]);
            AssertSegmentsAreEqual(0,17,20, touching[2]);
            AssertSegmentsAreEqual(0,22,26, touching[3]);
            AssertSegmentsAreEqual(0,28,32, touching[4]);
        }

       [TestMethod]
       public void BackPropagate_Works_Well()
       {
            IBitmapAccessor accessor = new MockupBitmapAccessor (
                new string[] {
                //             111111111122222222223333333333
                //   0123456789012345678901234567890123456789
                    "XX XX XXXXXXX XX XXXX XXXXX XXXXX X XXXX",
                    "        XXXXXXXXXXXXXXXXXXXXXX",
                    "XX XX XXXXXXX XX XXXX XXXXX XXXXX X XXXX",
                });
            BitmapScanner scanner = new BitmapScanner(accessor);
            scanner.Scan();
            for (int i=0;i<scanner.Segments.Count; i++) {
                var segment = scanner.Segments[i];
                segment.NetList = 1;
                scanner.Segments[i] = segment;
            }

            var scanline = scanner.ScanlineIndex[1];
            scanline.BackPropagate(scanner.Segments, 2,1);

            foreach (var segment in scanner.Segments)
            {
                Console.WriteLine($"Segment {segment} netlist {segment.NetList}");
                if (segment.Y <= 1) {
                    Assert.AreEqual(2, segment.NetList);
                }
                else
                {
                    Assert.AreEqual(1, segment.NetList);
                }
            }
       }
        private void AssertSegmentsAreEqual(int y,int x, int X, Segment segment)
        {
            Assert.AreEqual(y, segment.Y);
            Assert.AreEqual(x, segment.XMin);
            Assert.AreEqual(X, segment.XMax);            
        }
    }
}