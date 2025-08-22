using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace PokemonBot
{
    public static class KeyboardSimulator
    {
        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)] public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        const uint INPUT_KEYBOARD = 1;
        const uint KEYEVENTF_SCANCODE = 0x0008;
        const uint KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        private static readonly Dictionary<char, ushort> scanCodes = new Dictionary<char, ushort>
        {
            {'q', 0x10}, {'w', 0x11}, {'e', 0x12}, {'r', 0x13},
            {'a', 0x1E}, {'s', 0x1F}, {'d', 0x20}, {'z', 0x2C}
        };

        public static void PressKey(char key, int delay)
        {
            if (!scanCodes.ContainsKey(key)) return;

            ushort sc = scanCodes[key];
            INPUT down = new INPUT { type = INPUT_KEYBOARD, u = new InputUnion { ki = new KEYBDINPUT { wScan = sc, dwFlags = KEYEVENTF_SCANCODE, wVk = 0 } } };
            INPUT up = new INPUT { type = INPUT_KEYBOARD, u = new InputUnion { ki = new KEYBDINPUT { wScan = sc, dwFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP, wVk = 0 } } };

            SendInput(1, new INPUT[] { down }, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(delay);
            SendInput(1, new INPUT[] { up }, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void PressSequence(string keys, int delay)
        {
            foreach (char k in keys)
            {
                PressKey(k, delay);
            }
        }
    }
}
