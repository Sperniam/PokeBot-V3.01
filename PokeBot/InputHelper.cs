using System.Runtime.InteropServices;

namespace PokeBot
{
    public static class InputHelper
    {
        [DllImport("user32.dll")] private static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")] private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;

        public static void Click(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }
    }
}