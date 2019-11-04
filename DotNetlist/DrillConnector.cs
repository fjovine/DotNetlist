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
        private BitmapScanner top;

        /// <summary>
        /// Scanner of the bottom layer.
        /// </summary>
        private BitmapScanner bottom;

        /// <summary>
        /// Scanner of the drill layer.
        /// </summary>
        private DrillScanner drill;

        /// <summary>
        /// Maps the global net to the local nets of each layer.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, List<LayerNet>> net2connectedLayerNets = new Dictionary<int, List<LayerNet>>();

        private Dictionary<LayerNet, int> layerNet2net = new Dictionary<LayerNet, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DrillConnector"/> class.
        /// </summary>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="drill"></param>
        public DrillConnector(BitmapScanner top, BitmapScanner bottom, DrillScanner drill)
        {
            this.top = top;
            this.bottom = bottom;
            this.drill = drill;
        }

        public IEnumerable<int> GetNets()
        {
            return this.net2connectedLayerNets.Keys;
        }

        public IEnumerable<LayerNet> GetLayerNetsOfNet(int net)
        {
            return this.net2connectedLayerNets[net];
        }
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

                Console.WriteLine($"TopNet {topNet} BottomNet {bottomNet}");
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

                    Console.WriteLine($"Connessione 1 net {globalNet}");
                    globalNet++;
                    continue;
                }

                if (topNetAlreadyConnected && !bottomNetAlreadyConnected)
                {
                    int unify = this.layerNet2net[topNet];
                    this.layerNet2net.Add(bottomNet, unify);
                    this.net2connectedLayerNets[unify].Add(bottomNet);
                    Console.WriteLine($"Connessione 2 - unify {unify}");
                    continue;
                }

                if (bottomNetAlreadyConnected && !topNetAlreadyConnected)
                {
                    int unify = this.layerNet2net[bottomNet];
                    this.layerNet2net.Add(topNet, unify);
                    this.net2connectedLayerNets[unify].Add(topNet);
                    Console.WriteLine($"Connessione 3 - unify {unify}");
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
                    this.layerNet2net[bottomNet] = unify;
                    Console.WriteLine($"Connessione 4 {localBottomNet}->{unify}");
                }
            }
        }
    }
}