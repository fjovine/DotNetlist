using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DotNetlistUnitTest
{
    using DotNetlist;

    [TestClass]
    public class BitmapScannerTests
    {
        [TestMethod]
        public void Scan_GeneratesGood_Scanlines()
        {
            IBitmapAccessor accessor = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "XXXXXXXXX",
                    " XXXXXXX",
                    "  XXXXX",
                    "   XXX",
                    "    X",
                    "",
                });
            var expectedStart = new int[] { 0,1,2,3,4 };
            var expectedEnd = new int[] { 8,7,6,5,4 };

            BitmapScanner dut = new BitmapScanner(accessor);
            dut.Scan();
            int y = 0;
            foreach (var i in dut.Segments) {
                Console.WriteLine(i.ToString());
                Assert.AreEqual(expectedStart[y], i.XMin);
                Assert.AreEqual(expectedEnd[y], i.XMax);
                y ++;
            }

            Assert.AreEqual(y, expectedEnd.Length);
        }

        [TestMethod]
        public void Scan_GeneratesGood_CompositeScanlnes_WorksWell()
        {
            IBitmapAccessor accessor = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "  XXXXX",
                    " XX    X",
                    " XX XXX X",
                    "",
                    " XX XXX X",
                    "  XX    X",
                    "   XXXX",
                    "",
                });
            Console.WriteLine("Check Segments");
            var expectedy     = new int[] { 0,1,1,2,2,2,4,4,4,5,5,6 };
            var expectedStart = new int[] { 2,1,7,1,4,8,1,4,8,2,8,3 };
            var expectedEnd   = new int[] { 6,2,7,2,6,8,2,6,8,3,8,6 };
            BitmapScanner dut = new BitmapScanner(accessor);
            dut.Scan();
            int i = 0;
            foreach (var s in dut.Segments) 
            {
                Assert.AreEqual(expectedy[i], s.Y);
                Assert.AreEqual(expectedStart[i], s.XMin);
                Assert.AreEqual(expectedEnd[i], s.XMax);
                i ++;
            }

            // Check Scanline Index
            var expectedY = new int[] { 0,1,2,4,5,6 };
            var expectedInitial = new int[] { 0, 1, 3, 6, 9, 11 };
            var expectedLen = new int[] { 1,2,3,3,2, 1};

            i = 0;
            Console.WriteLine("Check ScanLineIndex");
            foreach (var index in dut.ScanlineIndex) 
            {
                Console.WriteLine(index);
                Assert.AreEqual(expectedY[i], index.Y);
                Assert.AreEqual(expectedInitial[i], index.InitialIndex);
                Assert.AreEqual(expectedLen[i], index.Length);
                i++;
            }
            Assert.AreEqual(i, expectedY.Length);
        }

        [TestMethod]
        public void ComputeNetlists_WorksWell_Case1()
        {
            IBitmapAccessor accessor = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "X       X",
                    "X       X",
                    "X       X",
                    "X       X",
                    "XXXXXXXXX",
                    "",
                });
            BitmapScanner dut = new BitmapScanner(accessor);
            Assert.IsTrue(IsSingleNetlist(dut));
        }

        [TestMethod]
        public void ComputeNetlists_WorksWell_Case2()
        {
            IBitmapAccessor accessor = new MockupBitmapAccessor (
                new string[] {
                    "  XXXX       XXXXXXXX            X",
                    " XXXXXX     XXXX   XXXXX       XXX",
                    "XXXXXXXXXXXXXX       XXXXX  XXXX",
                    "                        XXXXX"
                });
            BitmapScanner dut = new BitmapScanner(accessor);
            Assert.IsTrue(IsSingleNetlist(dut));
        }

        [TestMethod]
        public void ComputeNetlists_WorksWell_Case3()
        {
            IBitmapAccessor accessor = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "X       X",
                    " X       X",
                    "X       X",
                    " X       X",
                    "XXXXXXXXX",
                    "",
                });
            BitmapScanner dut = new BitmapScanner(accessor);
            Assert.IsTrue(!IsSingleNetlist(dut));
        }

        private bool IsSingleNetlist(BitmapScanner dut) 
        {
            var result = true;
            dut.Scan();
            dut.ComputeNetlists();
            foreach (var segment in dut.Segments) 
            {
                result &= segment.NetList == 1;
            }

            return result;
        }
    }
}