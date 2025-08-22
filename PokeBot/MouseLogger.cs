using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace PokeBot
{
    public static class MouseLogger
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        public struct POINT
        {
            public int X;
            public int Y;
        }

        public static void Run()
        {
            Console.WriteLine("Move your mouse to the corners of the Pokémon name box...");
            Console.WriteLine("Press CTRL+C to stop.\n");

            while (true)
            {
                GetCursorPos(out POINT pos);
                Console.WriteLine($"X: {pos.X}, Y: {pos.Y}");
                Thread.Sleep(300); // update every 0.3s
            }
        }
    }
}