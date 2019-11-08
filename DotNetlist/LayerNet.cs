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
    /// Identifies the net on each layer.
    /// </summary>
    public class LayerNet
    {
        /// <summary>
        /// Identifies the top layer.
        /// </summary>
        public const int TOPLAYER = 0;

        /// <summary>
        /// Identifies the bottom layer.
        /// </summary>
        public const int BOTTOMLAYER = 1;

        /// <summary>
        /// Maximum number of supported layers. It is just a placeholder: so far only top and bottom layers are supported.
        /// </summary>
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

        /// <summary>
        /// Generates a hash code for the object so that equal objects have the same hash code.
        /// </summary>
        /// <returns>The hash code for the object.</returns>
        public override int GetHashCode()
        {
            return this.NetId + (this.NetId * MAXLAYER);
        }

        /// <summary>
        /// Computes if this object equals the other one, i.e. if they have the same layer and net identifiers.
        /// </summary>
        /// <param name="obj">Other object to be compared against.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as LayerNet);
        }

        /// <summary>
        /// Computes if this object equals the other one, i.e. if they have the same layer and net identifiers.
        /// </summary>
        /// <param name="other">Other object to be compared against.</param>
        /// <returns>True if the objects are equal.</returns>
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

        /// <summary>
        /// Generates a textual representation of the <see cref="LayerNet"/> object showing the layer and the net identifier.
        /// </summary>
        /// <returns>Textual representation of the object.</returns>
        public override string ToString()
        {
            return $"{this.LayerId}:{this.NetId}";
        }
    }
}