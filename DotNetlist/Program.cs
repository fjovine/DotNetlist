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
    using System.IO;
    using System.Text;

    /// <summary>
    /// Entry point of the command line application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point of the <code>DotNetlist</code> application.
        /// It is a console application accepting the name of the PNG bitmap to be
        /// processed as parameter.
        /// It generates a folder containing all the found networks found and an HTML file
        /// that graphically shows them.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"File {args[0]} not found");
                return;
            }

            MonochromeBitmapAccessor ma = new MonochromeBitmapAccessor(args[0]);

            var scanner = new BitmapScanner(ma);
            scanner.Scan();
            scanner.ComputeNetlists();
            scanner.CompactNets();
            scanner.MapNetlists();

            var path = Path.GetFileNameWithoutExtension(args[0]);
            Directory.CreateDirectory(path);

            foreach (int netId in scanner.GetNetIds())
            {
                AbstractBitmapGenerator outBitmap = new RealBitmapGenerator(ma.Bitmap);
                foreach (var segment in scanner.GetSegmentsOfNet(netId))
                {
                    outBitmap.DrawSegment(segment, 100);
                }

                outBitmap.SaveTo(Path.Combine(path, $"Net_{netId}.png"));
            }

            File.WriteAllLines(path + ".html", GenerateHtml(scanner, path), Encoding.UTF8);
        }

        /// <summary>
        /// Generates the HTML file that permits showing each found net.
        /// </summary>
        /// <param name="scanner">The bitmap scanner to be used.</param>
        /// <param name="path">The pathname of the PNG file to be scanned.</param>
        /// <returns>The list of HTML lines.</returns>
        private static List<string> GenerateHtml(BitmapScanner scanner, string path)
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
            GenerateNetLinks(scanner, path, html);
            html.Add("</select>");
            html.Add("<br/><br/>");
            html.Add($"<img id=\"imageToSwap\" src=\"{path}/Net_1.png\" />");

            html.Add("<script type=\"text/javascript\">");
            html.Add("function swapImage(){");
            html.Add("    var image = document.getElementById(\"imageToSwap\");");
            html.Add("    var dropd = document.getElementById(\"dlist\");");
            html.Add("    image.src = dropd.value;	");
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
        /// <param name="html">List of html lines where the links will be saved.</param>
        private static void GenerateNetLinks(BitmapScanner scanner, string path, List<string> html)
        {
            int line = 1;
            foreach (int netId in scanner.GetNetIds())
            {
                html.Add($"<option value=\"{path}/Net_{line}.png\">Net_{line}</option>");
                line++;
            }

            html.Add("</table>");
        }
    }
}
