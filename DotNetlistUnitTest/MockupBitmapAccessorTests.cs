using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetlistUnitTest
{
    using DotNetlist;

    [TestClass]
    public class MockupBitmapAccessorTests
    {
        private void AssertAllSet(int width, int height, IBitmapAccessor accessor)
        {
            Assert.AreEqual(width, accessor.Width);
            Assert.AreEqual(height, accessor.Height);
            for (int x = 0; x < width; x++) 
            {
                for (int y=0; y<height; y++)
                {
                    Assert.IsTrue(accessor.PixelAt(x,y));
                }
            }
        }

        [TestMethod]
        public void MockupBitmapAccessor_WorksWell_WithAllSet2x2bitmap()
        {
            IBitmapAccessor accessor = new MockupBitmapAccessor(
                new string[]
                {
                    "XX",
                    "XX"
                });
            AssertAllSet(2,2, accessor);
        }

        [TestMethod]
        public void MockupBitmapAccessor_WorksWell_WithOneBitSet1x1bitmap()
        {
            IBitmapAccessor accessor = new MockupBitmapAccessor(
                new string[]
                {
                    "X",
                });
            AssertAllSet(1,1, accessor);
        }

        [TestMethod]
        public void MockupBitmapAccessor_WorksWell_AlternatingFullAndVoidLines()
        {
            IBitmapAccessor accessor = new MockupBitmapAccessor(
                new string[]
                {
                    "XXXXXX",
                    "",
                    "XXXXXX",
                    "",
                });

            Assert.AreEqual(6, accessor.Width);
            Assert.AreEqual(4, accessor.Height);
            for (int x = 0; x < accessor.Width; x++) 
            {
                for (int y=0; y<accessor.Height; y++)
                {
                    if (y%2 ==1)
                    {
                        Assert.IsFalse(accessor.PixelAt(x,y));
                    }
                    else
                    {
                        Assert.IsTrue(accessor.PixelAt(x,y));
                    }
                }
            }
        }
    }
}
