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

    public class Program
    {
        public static void Main(string[] args)
        {
            if (!File.Exists(args[0])) {
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

            File.WriteAllLines(path+".html", GenerateHtml(scanner, path), Encoding.UTF8);
        }

        private static List<string> GenerateHtml(BitmapScanner scanner, string path)
        {
            List<string> html = new List<string>();
            html.AddRange(new string[]
            {
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml/DTD/xhtml1 transitional.dtd\">",
                "<html xmlns=\"http://www.w3.org/1999/xhtml\">",
                "<head>",
                "<title>DotNetlist</title>",
                "<style type=\"text/css\">",
                "img{width:1648px;height:881px;border:0;}",
                ".show{display:block;}",
                ".hide{display:none;}",
                "</style>",
                "<script type=\"text/javascript\">",
                "var openImage;",
                "function showImg(image)",
                "{",
                "    if (!(openImage === undefined)) {",
                "      var oldImage = document.getElementById(openImage);",
                "      oldImage.className = 'hide';",
                "    }",
                "    var obj=document.getElementById(image);",
                "    obj.className = 'show';",
                "    openImage = image;",
                "}",
                "</script>",
                "</head>",
                "<html>",
                "<body>",
            });

            html.Add("<table>");
            html.Add("<tr><td>");
            GenerateNetLinks(scanner, html);
            html.Add("</td>");
            html.Add("<td>");
            GenerateImagesPlaceholders(scanner, html, path);
            html.Add("</td>");
            html.Add("</tr>");
            html.Add("</table>");
            html.Add("</body>");
            html.Add("</html>");
            return html;
        }

        private static void GenerateNetLinks(BitmapScanner scanner, List<string> html)
        {
            html.Add("<table>");
            int line = 1;
            foreach (int netId in scanner.GetNetIds())
            {
                html.Add($"<tr><td><a onclick=\"showImg('image{line}')\" href=\"#\">Net_{line}</a></td></tr>");
                line ++;
            }

            html.Add("</table>");
        }

        private static void GenerateImagesPlaceholders(BitmapScanner scanner, List<string> html, string path)
        {
            int line = 1;
            foreach (int netId in scanner.GetNetIds())
            {
                html.Add($"<img id=\"image{line}\" src=\"{path}/Net_{line}.png\" class=\"hide\">");
                line++;
            }
        }
    }
}
