namespace DotNetlistUnitTest
{
    using DotNetlist;
    using System;

    /// <summmary>
    /// Implements a mockup bitmap that uses strings to define bitmaps
    /// For instance we may instantiate an object like
    /// new MockupBitmapAccessor( new String[] {
    ///    "XXXXXXXX",
    ///    "   XX"
    ///    "XX XX"
    /// });
    /// to create a bitmap having as width the maximum width of the passed string array, height like the number of strings and exposing an 

    public class MockupBitmapAccessor : IBitmapAccessor
    {
        public int Width 
        {
            get;
            set;
        }

        public int Height 
        {
            get;
            set;
        }

        public bool PixelAt(int x, int y)
        {
            if ((x>=this.Width) || (y>=this.Height)) 
            {
                throw new ArgumentException("Invalid bit coordinates");
            }

            var scanline = this.bitmap[y];
            if (x>=scanline.Length) 
            {
                return false;
            }
            
            return scanline[x]=='X';
        }

        private string[] bitmap;
        public MockupBitmapAccessor (string[] bitmap)  
        {
            this.bitmap = bitmap;
            int maxStringLen = 0;
            foreach (var line in bitmap) 
            {
                maxStringLen = Math.Max(maxStringLen, line.Length);
            }

            this.Width = maxStringLen;
            this.Height = bitmap.Length;
        }
    }
}