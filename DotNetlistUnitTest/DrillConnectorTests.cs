using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DotNetlistUnitTest
{
    using DotNetlist;
    using System.Text;

    [TestClass]
    public class DrillConnectorTests
    {
        private IBitmapAccessor top;
        private IBitmapAccessor bottom;
        private IBitmapAccessor drill;

        [TestMethod]
        public void ComputeGlobalNet_WorksWell_Case1()
        {
            this.top = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "",
                    "  XX  XX",
                    "  XX  XX",
                    "  XX  XX",
                    "  XX  XX",
                    "  XX  XX",
                    "  XX  XX",
                    ""
                });

            this.bottom = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "",
                    "  XXXXXX",
                    "  XXXXXX",
                    "",
                    "",
                    "  XXXXXX",
                    "  XXXXXX",
                    ""
                });

            this.drill = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "",
                    "  XX  XX",
                    "  XX  XX",
                    "",
                    "",
                    "  XX  XX",
                    "  XX  XX",
                    ""
                });
            
            CheckConnection(new string[] {"0:1,1:1,0:2,1:2"});
        }

        [TestMethod]
        public void ComputeGlobalNet_WorksWell_Case2()
        {
            this.top = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "",
                    "  XX  XX",
                    "  XX  XX",
                    "  XX  XX",
                    "  XX  XX",
                    "  XX  XX",
                    "  XX  XX",
                    ""
                });

            this.bottom = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "",
                    "  XXXXXX",
                    "  XXXXXX",
                    "",
                    "",
                    "  XXXXXX",
                    "  XXXXXX",
                    ""
                });

            this.drill = new MockupBitmapAccessor (
                new string[] {
                //   012345678
                    "",
                    "  XX",
                    "  XX",
                    "",
                    "",
                    "      XX",
                    "      XX",
                    ""
                });
            
            CheckConnection(new string[] {"0:1,1:1", "0:2,1:2"});
            Assert.IsTrue(false);
        }

        private void CheckConnection(string[] expected)
        {
            BitmapScanner topLayer = new BitmapScanner(this.top);
            topLayer.PrepareAll();

            BitmapScanner bottomLayer = new BitmapScanner(this.bottom);
            bottomLayer.PrepareAll();

            BitmapScanner drillLayer = new BitmapScanner(this.drill);
            drillLayer.PrepareAll();

            DrillScanner drillScanner = new DrillScanner(drillLayer);

            DrillConnector drillConnector = new DrillConnector(topLayer, bottomLayer, drillScanner);
            drillConnector.ComputeGlobalNet();

            var nets = drillConnector.GetNets();
            int netCount = 0;
            foreach (int key in nets) 
            {
                Console.WriteLine($"KEY: {key}");
                StringBuilder sb = new StringBuilder();
                foreach (var layerNet in drillConnector.GetLayerNetsOfNet(key))
                {
                    if (sb.Length > 0) {
                        sb.Append(',');
                    }
                    sb.Append(layerNet.ToString());
                }
                Console.WriteLine($"  {sb}");
                Assert.AreEqual(expected[netCount], sb.ToString());
                netCount ++;
            }
            Assert.AreEqual(expected.Length, netCount);
        }
    } 
}
