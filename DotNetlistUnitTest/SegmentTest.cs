using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DotNetlistUnitTest
{
    using DotNetlist;

    [TestClass]
    public class SegmentTest
    {
        [TestMethod]
        public void Touches_WorksWell_InAllCases()
        {
            Segment segment = new Segment() 
            {
                Y=1,
                XMin = 10,
                XMax = 20
            };

            Check(5, 5, false);
            Check(5, 10, true);
            Check(5, 11, true);
            Check(5, 20, true);
            Check(5, 21, true);

            Check(10,15, true);
            Check(11,15, true);

            Check(15, 20, true);
            Check(15, 21, true);

            Check(20, 21, true);
            Check(21, 30, false);

            void Check(int Min, int Max, bool touches) {
                Segment other = new Segment()
                {
                    Y=0,
                    XMin=Min,
                    XMax = Max
                };
                Assert.AreEqual(touches, segment.Touches(other));
            }
        }
    }
}
