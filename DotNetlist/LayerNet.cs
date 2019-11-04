//-----------------------------------------------------------------------
// <copyright file="LayerNet.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.10.11</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{        
    /// <summary>
    /// Identifies the net on eachh layer.
    /// </summary>
    public class LayerNet
    {
        public const int TOPLAYER = 0;
        public const int BOTTOMLAYER = 1;

        public const int MAXLAYER = 32;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerNet"/> class.
        /// </summary>
        /// <param name="layer">Layer where the local net is.</param>
        /// <param name="net">Net identifier of the local net.</param>
        public LayerNet(int layer, int net)
        {
            this.LayerId = layer;
            this.NetId = net;
        }

        /// <summary>
        /// Gets the layer identifier.
        /// 0 is the top layer
        /// -1 is the bottom layer
        /// higher numbers are internal layers.
        /// </summary>
        public int LayerId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the local net identifier.
        /// </summary>
        /// <value></value>
        public int NetId
        {
            get;
            private set;
        }

        public override int GetHashCode()
        {
            return this.NetId + (this.NetId * MAXLAYER);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as LayerNet);
        }

        public bool Equals(LayerNet other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return (this.LayerId == other.LayerId) && (this.NetId == other.NetId);
        }

        public override string ToString()
        {
            return $"{this.LayerId}:{this.NetId}";
        }
    }
}