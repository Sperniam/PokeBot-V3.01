using System;
using System.Drawing;
using System.IO;
using Tesseract;

namespace PokeBot
{
    public class CapturePokemonName
    {
        private readonly Rectangle _region = new Rectangle(360, 212, 150, 42); // LAPTOP SCREEN COORDINATES 1920X1080 - 1920x1200
        // private readonly Rectangle _region = new Rectangle(480, 254, 200, 50); DESKTOP SCREEN COORDINATES 2480x....
        
        public string GetName()
        {
            using (Bitmap screenshot = new Bitmap(_region.Width, _region.Height))
            {
                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    g.CopyFromScreen(_region.X, _region.Y, 0, 0, _region.Size);
                }

                string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
                string imgFolder = Path.Combine(projectRoot, "img");
                Directory.CreateDirectory(imgFolder);
                string filePath = Path.Combine(imgFolder, "debug.png");

                using (Bitmap processed = new Bitmap(screenshot.Width * 2, screenshot.Height * 2))
                {
                    using (Graphics g2 = Graphics.FromImage(processed))
                    {
                        g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                        g2.DrawImage(screenshot, 0, 0, processed.Width, processed.Height);
                    }

                    for (int y = 0; y < processed.Height; y++)
                    {
                        for (int x = 0; x < processed.Width; x++)
                        {
                            Color c = processed.GetPixel(x, y);
                            int gray = (int)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B);
                            processed.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                        }
                    }

                    processed.Save(filePath);

                    string tessdataFolder = Path.Combine(projectRoot, "tessdata");
                    using (var engine = new TesseractEngine(tessdataFolder, "eng", EngineMode.Default))
                    {
                        engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
                        using (var img = PixConverter.ToPix(processed))
                        using (var page = engine.Process(img, PageSegMode.SingleWord))
                        {
                            return page.GetText().Trim();
                        }
                    }
                }
            }
        }
    }
}
