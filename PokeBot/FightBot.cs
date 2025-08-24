using System;
using System.Diagnostics;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;

namespace PokeBot
{
    public class FightBot
    {
        private bool _swapDoneForThisCycle = false;
        private readonly InputSimulator _sim;
        private readonly Random _rand;
        private bool _inDittoBattle = false;

        private int dittoCounter = 0;
        private int encounterCounter = 0; // ✅ new: count all encounters
        private const int MaxDittoEncounters = 72;

        public FightBot()
        {
            _sim = new InputSimulator();
            _rand = new Random();
        }

        // Main loop
        public void Start(Func<string> getPokemonName)
        {
            Console.WriteLine("[INFO] FightBot started...");
            DateTime lastDetected = DateTime.Now;

            while (true)
            {
                string name = getPokemonName();

                if (string.IsNullOrEmpty(name) || name.Length <= 3)
                {
                    if (dittoCounter > 0 && dittoCounter % 24 == 0 && !_swapDoneForThisCycle)
                    {
                        Console.WriteLine("[INFO] 24zszs Ditto reached! Swapping Pokémon before resuming search...");
                        PerformSwapSequence();
                        _swapDoneForThisCycle = true;
                    }

                    RandomizedWalking();
                    continue;
                }

                lastDetected = DateTime.Now;
                encounterCounter++;
                Console.WriteLine($"[INFO] Detected Pokémon: {name} | Total encounters: {encounterCounter}");

                Thread.Sleep(_rand.Next(5000, 6300));

                string lowered = name.ToLower();
                bool isDitto = (lowered.Contains("ditto") || lowered.Contains("pinc") ||
                                lowered.Contains("itto") || lowered.Contains("dinc") ||
                                lowered.Contains("dicn") || lowered.Contains("pich") ||
                                lowered.Contains("pino"))
                               && !lowered.Contains("incll"); // ✅ exclude bad matches

                if (isDitto || _inDittoBattle)
                {
                    if (!_inDittoBattle)
                    {
                        Console.WriteLine("[INFO] Ditto detected! Starting catch sequence...");
                        StartScreenRecording("DittoBattle_Debug.mp4"); // 🎥 Start recording battle for debug
                        _inDittoBattle = true;
                    }
                    else
                    {
                        Console.WriteLine("[INFO] Continuing Ditto catch sequence...");
                    }

                    PerformDittoSequence(getPokemonName);

                    dittoCounter++;
                    Console.WriteLine($"[INFO] Ditto sequence finished. Ditto counter = {dittoCounter}");

                    if (dittoCounter >= MaxDittoEncounters)
                    {
                        Console.WriteLine(
                            $"[INFO] Max Ditto encounters ({MaxDittoEncounters}) reached. Exiting program...");
                        Environment.Exit(0);
                    }

                    _swapDoneForThisCycle = false;

                    if (dittoCounter % 24 == 0)
                    {
                        Console.WriteLine(
                            $"[INFO] Ditto #{dittoCounter} caught. Swap will be performed next before resuming search.");
                    }

                    PressKey(VirtualKeyCode.VK_E, 150);
                    Console.WriteLine("[INFO] Pressed E after Ditto sequence.");

                    _inDittoBattle = false;
                }
                else
                {
                    Console.WriteLine("[INFO] Non-Ditto detected! Running in battle...");
                    PerformRunSequence();
                }

                Thread.Sleep(1300);
            }
        }

        private void RandomizedWalking()
        {
            bool useFullPattern = _rand.NextDouble() < 0.4;
            bool useZSZSZZ = _rand.NextDouble() < 0.2;

            if (useZSZSZZ)
            {
                Console.WriteLine("[WALK] Using ZSZSZZ pattern...");
                string sequence = "ZSZSZZ";
                foreach (char c in sequence)
                {
                    VirtualKeyCode key = c switch
                    {
                        'Z' => VirtualKeyCode.VK_Z,
                        'S' => VirtualKeyCode.VK_S,
                        _ => VirtualKeyCode.VK_Z
                    };
                    PressKeyForDuration(key, _rand.Next(150, 280));
                }
            }
            else if (useFullPattern)
            {
                Console.WriteLine("[WALK] Using full QDSZ pattern...");
                int qHold = _rand.Next(180, 400);
                int dHold = _rand.Next(180, 420);
                int zHold = _rand.Next(100, 350);
                int sHold = _rand.Next(100, 230);

                PressKeyForDuration(VirtualKeyCode.VK_Q, qHold);
                PressKeyForDuration(VirtualKeyCode.VK_D, dHold);
                PressKeyForDuration(VirtualKeyCode.VK_S, sHold);
                PressKeyForDuration(VirtualKeyCode.VK_Z, zHold);
            }
            else
            {
                Console.WriteLine("[WALK] Using short QD repeat pattern...");
                int repeat = _rand.Next(5, 11);
                for (int i = 0; i < repeat; i++)
                {
                    int qHold = _rand.Next(150, 300);
                    int dHold = _rand.Next(150, 300);
                    int zHold = _rand.Next(150, 300);

                    PressKeyForDuration(VirtualKeyCode.VK_D, dHold);
                    if (i > 2)
                        PressKeyForDuration(VirtualKeyCode.VK_Z, zHold);
                    PressKeyForDuration(VirtualKeyCode.VK_Q, qHold);

                    Thread.Sleep(_rand.Next(50, 150));
                }
            }

            Thread.Sleep(_rand.Next(120, 251));
        }

        private void PerformSwapSequence()
        {
            VirtualKeyCode[] keys = new[]
            {
                VirtualKeyCode.VK_X, VirtualKeyCode.VK_A, VirtualKeyCode.VK_A, VirtualKeyCode.VK_A,
                VirtualKeyCode.VK_S, VirtualKeyCode.VK_S, VirtualKeyCode.VK_S, VirtualKeyCode.VK_A,
                VirtualKeyCode.VK_A, VirtualKeyCode.VK_S, VirtualKeyCode.VK_S, VirtualKeyCode.VK_S,
                VirtualKeyCode.VK_S, VirtualKeyCode.VK_A, VirtualKeyCode.VK_Z, VirtualKeyCode.VK_Z,
                VirtualKeyCode.VK_E,
            };

            foreach (var key in keys)
            {
                Thread.Sleep(_rand.Next(1300, 1500));
                PressKey(key, 200);
            }

            Console.WriteLine("[INFO] Swap sequence complete.");
        }

        private void PerformDittoSequence(Func<string> getPokemonName)
        {
            Console.WriteLine("[DITTO] Step 1: Q Z A A...");
            PressKey(VirtualKeyCode.VK_Q, RandPressTime());
            PressKey(VirtualKeyCode.VK_Z, RandPressTime());
            PressKey(VirtualKeyCode.VK_A, RandPressTime());
            Thread.Sleep(_rand.Next(100, 150));
            PressKey(VirtualKeyCode.VK_A, RandPressTime());

            Console.WriteLine("[DITTO] Waiting 10–12 sec...");
            Thread.Sleep(_rand.Next(8000, 8005));

            Console.WriteLine("[DITTO] Step 2: A S A...");
            Thread.Sleep(_rand.Next(250, 350));
            PressKey(VirtualKeyCode.VK_A, RandPressTime());
            Thread.Sleep(_rand.Next(200, 298));
            PressKey(VirtualKeyCode.VK_S, RandPressTime());
            Thread.Sleep(_rand.Next(200, 309));
            PressKey(VirtualKeyCode.VK_A, RandPressTime());

            Console.WriteLine("[DITTO] Waiting 14.5–17 sec...");
            Thread.Sleep(_rand.Next(14500, 17000));

            Console.WriteLine("[DITTO] Step 3: Catch loop until Ditto gone...");
            while (true)
            {
                string name = getPokemonName();
                string lowered = name?.ToLower() ?? "";

                bool isDitto = (lowered.Contains("ditto") || lowered.Contains("pinc") ||
                                lowered.Contains("itto") || lowered.Contains("dinc") ||
                                lowered.Contains("dicn") || lowered.Contains("pich") ||
                                lowered.Contains("pino"))
                               && !lowered.Contains("incll");

                if (string.IsNullOrEmpty(name) || !isDitto)
                {
                    Console.WriteLine("[DITTO] Ditto caught or gone!");
                    StopScreenRecording(); // 🎥 Stop recording when battle ends
                    break;
                }

                PressKey(VirtualKeyCode.VK_D, RandPressTime());
                Thread.Sleep(_rand.Next(150, 203));
                PressKey(VirtualKeyCode.VK_A, RandPressTime());
                Thread.Sleep(_rand.Next(280, 506));
                PressKey(VirtualKeyCode.VK_A, RandPressTime());

                Console.WriteLine("[DITTO] Waiting 14–15.5 sec before next attempt...");
                Thread.Sleep(_rand.Next(14000, 15500));
            }

            PressKey(VirtualKeyCode.VK_E, 250);
            Console.WriteLine("[DITTO] Closed popup with E key.");
        }

        public void PerformRunSequence()
        {
            Thread.Sleep(_rand.Next(600, 800));
            PressKey(VirtualKeyCode.VK_D, RandPressTime());
            Thread.Sleep(_rand.Next(200, 400));
            PressKey(VirtualKeyCode.VK_S, RandPressTime());
            Thread.Sleep(_rand.Next(250, 800));
            PressKey(VirtualKeyCode.VK_A, RandPressTime());
        }

        private void PressKeyForDuration(VirtualKeyCode key, int durationMilliseconds)
        {
            _sim.Keyboard.KeyDown(key);
            Thread.Sleep(durationMilliseconds);
            _sim.Keyboard.KeyUp(key);
        }

        private void PressKey(VirtualKeyCode key, int holdMilliseconds)
        {
            _sim.Keyboard.KeyDown(key);
            Thread.Sleep(holdMilliseconds);
            _sim.Keyboard.KeyUp(key);
        }

        private int RandPressTime()
        {
            return _rand.Next(120, 180); // ✅ variable press time for anti-detection
        }



        private Process _recordingProcess;

        private Process _ffmpegProcess;

        /// <summary>
        /// Starts recording the screen with ffmpeg.
        /// </summary>
        private void StartScreenRecording(string outputFile)
        {
            // Stop any existing recording just in case
            StopScreenRecording();

            string ffmpegPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "bin", "ffmpeg.exe");
            string args = $"-f gdigrab -framerate 30 -i desktop -pix_fmt yuv420p -t 00:05:00 -y \"{outputFile}\"";

            _ffmpegProcess = new Process();
            _ffmpegProcess.StartInfo.FileName = ffmpegPath;
            _ffmpegProcess.StartInfo.Arguments = args;
            _ffmpegProcess.StartInfo.UseShellExecute = false;
            _ffmpegProcess.StartInfo.RedirectStandardInput = true;  // ✅ allow sending 'q'
            _ffmpegProcess.StartInfo.CreateNoWindow = true;
            _ffmpegProcess.Start();

            Console.WriteLine("[DEBUG] Started Ditto battle recording.");
        }


        /// <summary>
        /// Stops the recording process if it's running.
        /// </summary>
        private void StopScreenRecording()
        {
            if (_ffmpegProcess != null && !_ffmpegProcess.HasExited)
            {
                try
                {
                    _ffmpegProcess.StandardInput.WriteLine("q"); // ✅ gracefully stop FFmpeg
                    if (!_ffmpegProcess.WaitForExit(5000))       // wait max 5 sec
                    {
                        _ffmpegProcess.Kill();                  // fallback
                    }
                }
                catch { /* ignore exceptions */ }

                _ffmpegProcess.Dispose();
                _ffmpegProcess = null;
                Console.WriteLine("[DEBUG] Ditto battle recording stopped.");
            }
        }



    }


}
