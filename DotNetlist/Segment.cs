//-----------------------------------------------------------------------
// <copyright file="Segment.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.10.11</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{
    public class Segment
    {
        public int Y
        {
            get;
            set;
        }

        public int XMin
        {
            get;
            set;
        }

        public int XMax
        {
            get;
            set;
        }

        public int NetList
        {
            get;
            set;
        }

        public bool Touches(Segment other)
        {
            if (this.Y != other.Y + 1)
            {
                return false;
            }

            if (other.XMax < this.XMin)
            {
                return false;
            }

            if (other.XMin > this.XMax)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"{this.Y},{this.XMin}-{this.XMax},{this.NetList}";
        }
    }
}