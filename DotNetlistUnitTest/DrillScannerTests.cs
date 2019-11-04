using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DotNetlistUnitTest
{
    using DotNetlist;

    [TestClass]
    public class DrillScannerTests 
    {
        [TestMethod]
        public void GetHoles_WorksWell_WithProperBitmap()
        {
            IBitmapAccessor accessor = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "XXXXXXXXXXX",
                    "X         X",
                    "X XX  XX  X",
                    "X XX  XX  X",
                    "X         X",
                    "X    XX   X",
                    "X    XX   X",
                    "X         X",
                    "XXXXXXXXXXX",
                });

            BitmapScanner dut = new BitmapScanner(accessor);
            dut.PrepareAll();
            
            DrillScanner scanner = new DrillScanner(dut);

            var holes = scanner.GetHoles();
            Assert.AreEqual(3, holes.Count);
            Assert.AreEqual(2.5, holes[0].X);
            Assert.AreEqual(2.5, holes[0].Y);
            Assert.AreEqual(6.5, holes[1].X);
            Assert.AreEqual(2.5, holes[1].Y);
            Assert.AreEqual(5.5, holes[2].X);
            Assert.AreEqual(5.5, holes[2].Y);
        }
    }
}