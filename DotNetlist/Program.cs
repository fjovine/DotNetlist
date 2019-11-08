//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Francesco Iovine">
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
    using System.Drawing;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Entry point of the command line application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point of the <code>DotNetlist</code> application.
        /// It is a console application accepting the nameS of the PNG bitmap to be
        /// processed as parameters.
        /// The first PNG is the top layer, the second one is the drill layer while the third is the bottom layer.
        /// It generates a folder containing all the found networks found and an HTML file
        /// that graphically shows them.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                ProcessSingleLayer(args[0]);
            }
            else if (args.Length == 3)
            {
                ProcessDoubleLayerWithDrill(args[0], args[1], args[2], true, true);
            }
            else
            {
                Console.WriteLine("Syntax DotNetlist <topLayer> [<drillLayer> <bottomLayer>] ");
            }
        }

        /// <summary>
        /// Processes a single PNG file representing a copper layer.
        /// The nets, i.e. the areas in electrical contact are found, a bitmap with each net highlighted is generated as well
        /// as a HTML file to show them properly.
        /// </summary>
        /// <param name="filename">Filename of the file containing the PNG image of the copper layer.</param>
        public static void ProcessSingleLayer(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File {filename} not found");
                return;
            }

            MonochromeBitmapAccessor topLayer = new MonochromeBitmapAccessor(filename);

            var scannerToplayer = LoadAndScan(filename);

            var path = Path.GetFileNameWithoutExtension(filename);
            Directory.CreateDirectory(path);

            foreach (int netId in scannerToplayer.GetNetIds())
            {
                AbstractBitmapGenerator outBitmap = new RealBitmapGenerator(topLayer.Bitmap);
                foreach (var segment in scannerToplayer.GetSegmentsOfNet(netId))
                {
                    outBitmap.DrawSegment(segment, 100);
                }

                outBitmap.SaveTo(Path.Combine(path, $"Net_{netId}.png"));
            }

            File.WriteAllLines(path + ".html", GenerateHtml(path, GenerateNetLinks(scannerToplayer, path)), Encoding.UTF8);
        }

        /// <summary>
        /// Loads and processes the PNG bitmaps containing the bitmaps representing the top, drill and bottom layers.
        /// It generates bitmaps containing the copper layers with each available net highlighted and an HTML file to easily
        /// show all the found nets.
        /// </summary>
        /// <param name="topLayerFilename">Filename of the PNG file containing the top copper layer.</param>
        /// <param name="drillLayerFilename">Filename of the PNG file containing the drill layer.</param>
        /// <param name="bottomLayerFilename">Filename of the PNG file containing the bottom copper layer.</param>
        /// <param name="doHoles">True if holes should be drawn on copper layers.</param>
        /// <param name="doMirror">True if the bottom layer should be mirrored in the output file produced.</param>
        public static void ProcessDoubleLayerWithDrill(string topLayerFilename, string drillLayerFilename, string bottomLayerFilename, bool doHoles, bool doMirror)
        {
            if (!File.Exists(topLayerFilename))
            {
                Console.WriteLine($"The top layer file {topLayerFilename} was not found");
                return;
            }

            if (!File.Exists(drillLayerFilename))
            {
                Console.WriteLine($"The drill layer file {topLayerFilename} was not found");
                return;
            }

            if (!File.Exists(bottomLayerFilename))
            {
                Console.WriteLine($"The bottom layer file {topLayerFilename} was not found");
                return;
            }

            var top = LoadAndScan(topLayerFilename);
            var drill = LoadAndScan(drillLayerFilename);
            var bottom = LoadAndScan(bottomLayerFilename);

            MonochromeBitmapAccessor topLayer = new MonochromeBitmapAccessor(topLayerFilename);
            MonochromeBitmapAccessor bottomLayer = new MonochromeBitmapAccessor(bottomLayerFilename);
            MonochromeBitmapAccessor drillLayer = new MonochromeBitmapAccessor(drillLayerFilename);
            DrillScanner drillScanner = new DrillScanner(drill);

            DrillConnector connector = new DrillConnector(top, bottom, drillScanner);
            connector.ComputeGlobalNet();

            var path = Path.GetFileNameWithoutExtension(topLayerFilename);
            Directory.CreateDirectory(path);

            if (doHoles)
            {
                MonochromeBitmapAccessor drillPlane = new MonochromeBitmapAccessor(drillLayerFilename);
                Bitmap bitmap = drillPlane.Bitmap;
                for (int x = 0; x < drillPlane.Width; x++)
                {
                    for (int y = 0; y < drillPlane.Height; y++)
                    {
                        if (drillPlane.PixelAt(x, y))
                        {
                            topLayer.Bitmap.SetPixel(x, y, System.Drawing.Color.Black);
                            bottomLayer.Bitmap.SetPixel(x, y, System.Drawing.Color.Black);
                        }
                    }
                }
            }

            // Generates the bitmaps with the highlighted segments.
            // Bottom layer is mirrored
            foreach (int key in connector.GetNets())
            {
                RealBitmapGenerator topBitmap = new RealBitmapGenerator(topLayer.Bitmap);
                RealBitmapGenerator bottomBitmap = new RealBitmapGenerator(bottomLayer.Bitmap);
                foreach (var layerNet in connector.GetLayerNetsOfNet(key))
                {
                    if (layerNet.LayerId == LayerNet.TOPLAYER)
                    {
                        foreach (var segment in top.GetSegmentsOfNet(layerNet.NetId))
                        {
                            topBitmap.DrawSegment(segment, 100);
                        }
                    }
                    else
                    {
                        foreach (var segment in bottom.GetSegmentsOfNet(layerNet.NetId))
                        {
                            bottomBitmap.DrawSegment(segment, 100);
                        }
                    }
                }

                int margin = 20;
                Bitmap full = new Bitmap(
                    topBitmap.Width + bottomBitmap.Width + margin,
                    topBitmap.Height);
                Graphics gr = Graphics.FromImage(full);
                gr.DrawImage(
                    topBitmap.GetBitmap(),
                    0,
                    0,
                    topBitmap.Width,
                    topBitmap.Height);
                var bottomImage = (Bitmap)bottomBitmap.GetBitmap().Clone();
                if (doMirror)
                {
                    bottomImage.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipX);
                }

                gr.DrawImage(
                    bottomImage,
                    topBitmap.Width + margin,
                    0);
                full.Save(Path.Combine(path, $"Net_{key}.png"));
            }

            var options = new List<string>();
            int line = 1;
            foreach (int netId in connector.GetNets())
            {
                options.Add($"<option value=\"{path}/Net_{line}.png\">Net_{line}</option>");
                line++;
            }

            File.WriteAllLines(path + ".html", GenerateHtml(path, options), Encoding.UTF8);
        }

        /// <summary>
        /// Loads the PNG bitmap contained by the file filename and returns it inside a <see cref="BitmapScanner"/> class.
        /// </summary>
        /// <param name="filename">Filename of the PNG file.</param>
        /// <returns>A <see cref="BitmapScanner"/> object containing the loaded bitmap.</returns>
        private static BitmapScanner LoadAndScan(string filename)
        {
            MonochromeBitmapAccessor layer = new MonochromeBitmapAccessor(filename);
            Console.WriteLine($"Loading {filename}");

            var result = new BitmapScanner(layer);
            result.PrepareAll();
            return result;
        }

        /// <summary>
        /// Generates the HTML file that permits showing each found net.
        /// </summary>
        /// <param name="path">The pathname of the PNG file to be scanned.</param>
        /// <param name="optionValues">The options of the net list box.</param>
        /// <returns>The list of HTML lines.</returns>
        private static List<string> GenerateHtml(string path, List<string> optionValues)
        {
            List<string> html = new List<string>();
            html.AddRange(new string[]
            {
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml/DTD/xhtml1 transitional.dtd\">",
                "<html xmlns=\"http://www.w3.org/1999/xhtml\">",
                "<head>",
                "<title>DotNetlist</title>",
                "</head>",
                "<html>",
                "<body>",
            });

            html.Add("<select id=\"dlist\" onChange=\"swapImage()\">");
            html.AddRange(optionValues);
            html.Add("</select>");
            html.Add("<br/><br/>");
            html.Add($"<img id=\"imageToSwap\" src=\"{path}/Net_1.png\" />");

            html.Add("<script type=\"text/javascript\">");
            html.Add("function swapImage(){");
            html.Add("    var image = document.getElementById(\"imageToSwap\");");
            html.Add("    var dropd = document.getElementById(\"dlist\");");
            html.Add("    image.src = dropd.value;  ");
            html.Add("};");
            html.Add("</script>");
            html.Add("</body>");
            html.Add("</html>");
            return html;
        }

        /// <summary>
        /// Generates the links to the external bitmaps in the HTML file.
        /// </summary>
        /// <param name="scanner">The bitmap scanner to be used.</param>
        /// <param name="path">The pathname of the PNG file to be scanned.</param>
        /// <returns>A list of strings containing the HTML lines with the net lists.</returns>
        private static List<string> GenerateNetLinks(BitmapScanner scanner, string path)
        {
            var result = new List<string>();
            int line = 1;
            foreach (int netId in scanner.GetNetIds())
            {
                result.Add($"<option value=\"{path}/Net_{line}.png\">Net_{line}</option>");
                line++;
            }

            return result;
        }
    }
}
