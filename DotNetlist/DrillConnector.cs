//-----------------------------------------------------------------------
// <copyright file="DrillConnector.cs" company="Francesco Iovine">
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public V2.0
// </copyright>
// <author>Francesco Iovine</author>
// <created>2019.10.11</created>
//-----------------------------------------------------------------------
namespace DotNetlist
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Starting from the top and bottom layer, using the passed drill layer
    /// computes the connection between the nets of the top and the bottom layer.
    /// </summary>
    public class DrillConnector
    {
        /// <summary>
        /// Scanner of the top layer.
        /// </summary>
        private readonly BitmapScanner top;

        /// <summary>
        /// Scanner of the bottom layer.
        /// </summary>
        private readonly BitmapScanner bottom;

        /// <summary>
        /// Scanner of the drill layer.
        /// </summary>
        private readonly DrillScanner drill;

        /// <summary>
        /// Maps the identifier of nets on each layer to the global net identifier.
        /// </summary>
        private readonly Dictionary<LayerNet, int> layerNet2net = new Dictionary<LayerNet, int>();

        /// <summary>
        /// Maps the global net to the local nets of each layer.
        /// </summary>
        private Dictionary<int, List<LayerNet>> net2connectedLayerNets = new Dictionary<int, List<LayerNet>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DrillConnector"/> class.
        /// This class builds the connection between nets on the top and bottom layers of the PCB by means
        /// of the drills layer.
        /// </summary>
        /// <param name="top">BitmapScanner of the top layer.</param>
        /// <param name="bottom">BitmapScanner of the bottom layer layer.</param>
        /// <param name="drill">DrillScanner of the drill layer.</param>
        public DrillConnector(BitmapScanner top, BitmapScanner bottom, DrillScanner drill)
        {
            this.top = top;
            this.bottom = bottom;
            this.drill = drill;
        }

        /// <summary>
        /// Computes the enumeration of defined local nets.
        /// </summary>
        /// <returns>The enumeration of defined local nets.</returns>
        public IEnumerable<int> GetNets()
        {
            return this.net2connectedLayerNets.Keys;
        }

        /// <summary>
        /// Computes the list of local nets (i.e. nets on top and bottom layers) corresponding to the passed
        /// global net.
        /// </summary>
        /// <param name="net">Global net identifier.</param>
        /// <returns>The enumeration of <see cref="LayerNet"/> that reference each net on both layers that are connected.</returns>
        public IEnumerable<LayerNet> GetLayerNetsOfNet(int net)
        {
            return this.net2connectedLayerNets[net];
        }

        /// <summary>
        /// This method computes the global nets of the PCB considering that nets of the top and bottom layers are
        /// indeed connected, i.e. just one, when they touch a hole.
        /// The correspondence between the nets on top and bottom layers is stored in the <see cref="layerNet2net"/> and <see cref="net2connectedLayerNets"/> dictionaries.
        /// </summary>
        public void ComputeGlobalNet()
        {
            int globalNet = 1;
            foreach (var hole in this.drill.GetHoles())
            {
                LayerNet topNet = null;
                LayerNet bottomNet = null;
                if (this.top.TryGetNetAt(hole.X, hole.Y, out int top))
                {
                    topNet = new LayerNet(LayerNet.TOPLAYER, top);
                }

                if (this.bottom.TryGetNetAt(hole.X, hole.Y, out int bottom))
                {
                    bottomNet = new LayerNet(LayerNet.BOTTOMLAYER, bottom);
                }

                bool topNetAlreadyConnected = (topNet != null) && this.layerNet2net.ContainsKey(topNet);
                bool bottomNetAlreadyConnected = (bottomNet != null) && this.layerNet2net.ContainsKey(bottomNet);

                if (!topNetAlreadyConnected && !bottomNetAlreadyConnected)
                {
                    // The nets are not connected. A new global net is created.
                    this.net2connectedLayerNets.Add(globalNet, new List<LayerNet>());
                    if (topNet != null)
                    {
                        this.net2connectedLayerNets[globalNet].Add(topNet);
                        this.layerNet2net.Add(topNet, globalNet);
                    }

                    if (bottomNet != null)
                    {
                        this.net2connectedLayerNets[globalNet].Add(bottomNet);
                        this.layerNet2net.Add(bottomNet, globalNet);
                    }

                    globalNet++;
                    continue;
                }

                if (topNetAlreadyConnected && !bottomNetAlreadyConnected)
                {
                    int unify = this.layerNet2net[topNet];
                    this.layerNet2net.Add(bottomNet, unify);
                    this.net2connectedLayerNets[unify].Add(bottomNet);
                    continue;
                }

                if (bottomNetAlreadyConnected && !topNetAlreadyConnected)
                {
                    int unify = this.layerNet2net[bottomNet];
                    this.layerNet2net.Add(topNet, unify);
                    this.net2connectedLayerNets[unify].Add(topNet);
                    continue;
                }

                if (topNetAlreadyConnected && bottomNetAlreadyConnected)
                {
                    // If both are connected, we reduce to the top layer.
                    int unify = this.layerNet2net[topNet];
                    int localBottomNet = this.layerNet2net[bottomNet];
                    if (unify == localBottomNet)
                    {
                        continue;
                    }

                    var bottomList = this.net2connectedLayerNets[localBottomNet];
                    this.net2connectedLayerNets[unify].AddRange(bottomList);
                    this.net2connectedLayerNets.Remove(localBottomNet);
                    foreach (var localnet in bottomList)
                    {
                        this.layerNet2net[localnet] = unify;
                    }
                }
            }

            // Adds the top layers that have no viases nor through pins
            AddNetsOnlyOnOneLayer(this.top, LayerNet.TOPLAYER);
            AddNetsOnlyOnOneLayer(this.bottom, LayerNet.BOTTOMLAYER);
            CompactNets();

            void AddNetsOnlyOnOneLayer(BitmapScanner layer, int layerId)
            {
                foreach (var netId in layer.GetNetIds())
                {
                    var net = new LayerNet(layerId, netId);
                    var net2GlobalNet = new Dictionary<int, int>();
                    if (!this.layerNet2net.ContainsKey(net))
                    {
                        if (!net2GlobalNet.ContainsKey(netId))
                        {
                            net2GlobalNet.Add(netId, globalNet++);
                        }

                        var thisNet = net2GlobalNet[netId];

                        if (!this.net2connectedLayerNets.ContainsKey(thisNet))
                        {
                            this.net2connectedLayerNets.Add(thisNet, new List<LayerNet>());
                        }

                        this.net2connectedLayerNets[thisNet].Add(net);
                    }
                }
            }

            void CompactNets()
            {
                Dictionary<int, List<LayerNet>> compacted = new Dictionary<int, List<LayerNet>>();
                int i = 1;
                foreach (var key in this.net2connectedLayerNets.Keys)
                {
                    var list = this.net2connectedLayerNets[key];
                    compacted.Add(i++, list);
                }

                this.net2connectedLayerNets = compacted;
            }
        }
    }
}