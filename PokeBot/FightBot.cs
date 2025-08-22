using System;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;

namespace PokeBot
{
    public class FightBot
    {
        private bool _swapDoneForThisCycle = false; // 🔹 new flag
        private readonly InputSimulator _sim;
        private readonly Random _rand;
        private bool _inDittoBattle = false; // ✅ flag to track Ditto battle

        private int dittoCounter = 0; // counter for Ditto encounters
        private const int MaxDittoEncounters = 72;  // exit after this

        public FightBot()
        {
            _sim = new InputSimulator();
            _rand = new Random();
        }

        // Main loop
        public void Start(Func<string> getPokemonName) {
            
            Console.WriteLine("[INFO] FightBot started...");
            DateTime lastDetected = DateTime.Now;

            while (true){
                
            string name = getPokemonName();

             if (string.IsNullOrEmpty(name) || name.Length <= 3) {
                 
            // 🔹 If swap is needed, perform it before resuming movement
            if (dittoCounter > 0 && dittoCounter % 24 == 0 && !_swapDoneForThisCycle)
            {
                Console.WriteLine("[INFO] 24th Ditto reached! Swapping Pokémon before resuming search...");
                PerformSwapSequence();
                _swapDoneForThisCycle = true; // ✅ prevents repeated swaps
            }

            RandomizedWalking();
            continue;
        }
        
        

        lastDetected = DateTime.Now;
        Console.WriteLine("Detected Pokémon: " + name);

        // Wait a few seconds after encounter
        Thread.Sleep(_rand.Next(6000,7000));

        string lowered = name.ToLower();
        bool isDitto = lowered.Contains("ditto") || lowered.Contains("pinc") ||
                       lowered.Contains("itto") || lowered.Contains("dinc")|| lowered.Contains("dicn")|| lowered.Contains("shiny")|| lowered.Contains("httol");

        if (isDitto || _inDittoBattle)
        {
            if (!_inDittoBattle)
            {
                Console.WriteLine("[INFO] Ditto detected! Starting catch sequence...");
                _inDittoBattle = true;
            }
            else
            {
                Console.WriteLine("[INFO] Continuing Ditto catch sequence...");
            }

            // Always enter catch phase after attack phase
            PerformDittoSequence(getPokemonName);

            // Ditto sequence is done
            dittoCounter++;
            Console.WriteLine($"[INFO] Ditto sequence finished. Counter = {dittoCounter}");

            // 🔴 Exit condition for max Dittos
            if (dittoCounter >= MaxDittoEncounters)
            {
                Console.WriteLine($"[INFO] Max Ditto encounters ({MaxDittoEncounters}) reached. Exiting program...");
                Environment.Exit(0); // clean exit
            }


            // 🔹 Reset swap flag when new Ditto count is reached
            _swapDoneForThisCycle = false;
            
            // 🔹 Announce if swap will happen after leaving battle
            if (dittoCounter % 24 == 0)
            {
                Console.WriteLine($"[INFO] Ditto #{dittoCounter} caught. Swap will be performed next before resuming search.");
            }

            // Close popup after catching
            PressKey(VirtualKeyCode.VK_E, 150);
            Console.WriteLine("[INFO] Pressed E after Ditto sequence.");

            _inDittoBattle = false; // unlock after battle really ended
        }
        else
        {
            Console.WriteLine("[INFO] Non-Ditto detected! Running in battle...");
            PerformRunSequence();
        }

        Thread.Sleep(2000);
    }
}

        private void RandomizedWalking()
        {
            // Decide which pattern to use
            bool useFullPattern = _rand.NextDouble() < 0.5; // 50% chance

            if (useFullPattern)
            {
                // Full pattern (original)
                int qHold = _rand.Next(100, 500);
                int dHold = _rand.Next(100, 400);
                int zHold = _rand.Next(100, 350);
                int sHold = _rand.Next(100, 250);

                PressKeyForDuration(VirtualKeyCode.VK_Q, qHold);
                PressKeyForDuration(VirtualKeyCode.VK_D, dHold);
                PressKeyForDuration(VirtualKeyCode.VK_S, sHold);
                PressKeyForDuration(VirtualKeyCode.VK_Z, zHold);
                
            }
            else
            {
                // Short Q/D pattern repeated random 5-10 times
                int repeat = _rand.Next(5, 11);

                for (int i = 0; i < repeat; i++)
                {
                    int qHold = _rand.Next(100, 300);
                    int dHold = _rand.Next(100, 300);

                    PressKeyForDuration(VirtualKeyCode.VK_Q, qHold);
                    PressKeyForDuration(VirtualKeyCode.VK_D, dHold);

                    // Optional tiny pause between repeats
                    Thread.Sleep(_rand.Next(100, 200));
                }
            }

            // Small randomized pause after pattern
            Thread.Sleep(_rand.Next(150, 351));
        }

        // Swap Pokémon sequence after 24th Ditto
        private void PerformSwapSequence()
        {
            VirtualKeyCode[] keys = new[]
            {
                VirtualKeyCode.VK_X,
                VirtualKeyCode.VK_A,
                VirtualKeyCode.VK_A,
                VirtualKeyCode.VK_A,
                VirtualKeyCode.VK_S,
                VirtualKeyCode.VK_S,
                VirtualKeyCode.VK_S,
                VirtualKeyCode.VK_A,
                VirtualKeyCode.VK_A,
                VirtualKeyCode.VK_S,
                VirtualKeyCode.VK_S,
                VirtualKeyCode.VK_S,
                VirtualKeyCode.VK_S,
                VirtualKeyCode.VK_A,
                VirtualKeyCode.VK_Z,
                VirtualKeyCode.VK_Z,
                VirtualKeyCode.VK_E,
            };

            foreach (var key in keys)
            {
                Thread.Sleep(1500);  // wait 0.5 seconds between presses
                PressKey(key, 200);
            }

            Console.WriteLine("[INFO] Swap sequence complete.");
        }

        // Ditto-specific multi-step sequence
        private void PerformDittoSequence(Func<string> getPokemonName)
        {
            Console.WriteLine("[INFO] Starting Ditto catch sequence...");

            // Step 1: Q Z A A
            PressKey(VirtualKeyCode.VK_Q, 100);
            PressKey(VirtualKeyCode.VK_Z, 100);
            PressKey(VirtualKeyCode.VK_A, 100);
            Thread.Sleep(_rand.Next(100,150));
            PressKey(VirtualKeyCode.VK_A, 100);

            Console.WriteLine("[INFO] Waiting 10 seconds...");
            Thread.Sleep(_rand.Next(11000,12000));

            // Step 2: A S A
            Thread.Sleep(_rand.Next(250,350));
            PressKey(VirtualKeyCode.VK_A, 100);
            Thread.Sleep(_rand.Next(100,150));
            PressKey(VirtualKeyCode.VK_S, 100);
            Thread.Sleep(_rand.Next(100,150));
            PressKey(VirtualKeyCode.VK_A, 100);

            Console.WriteLine("[INFO] Waiting 15 seconds...");
            Thread.Sleep(_rand.Next(14500,17000));

            // Step 3: Keep pressing D A A until name is no longer a Ditto variation
            while (true)
            {
                string name = getPokemonName();
                string lowered = name?.ToLower() ?? "";

                bool isDitto = lowered.Contains("ditto") || lowered.Contains("pinc") ||
                               lowered.Contains("itto") || lowered.Contains("dinc")|| lowered.Contains("pich")|| lowered.Contains("pino")|| lowered.Contains("dicn");

                if (string.IsNullOrEmpty(name) || !isDitto)
                {
                    Console.WriteLine("[INFO] Ditto caught or gone!");
                    break;
                }

                PressKey(VirtualKeyCode.VK_D, 100);
                Thread.Sleep(_rand.Next(100,150));
                PressKey(VirtualKeyCode.VK_A, 100);
                Thread.Sleep(_rand.Next(100,150));
                PressKey(VirtualKeyCode.VK_A, 100);

                Console.WriteLine("[INFO] Waiting 15 seconds before next attempt...");
                Thread.Sleep(_rand.Next(14000,15500));
            }

            // ✅ Press E to close popup after catching/escaping
            PressKey(VirtualKeyCode.VK_E, 250);
            Console.WriteLine("[INFO] Closed popup with E key.");
        }


        public void PerformAttackSequence()
        {
            PressKey(VirtualKeyCode.VK_Q, 100);
            PressKey(VirtualKeyCode.VK_W, 100);
            PressKey(VirtualKeyCode.VK_E, 100);
        }

        public void PerformRunSequence()
        {
            Thread.Sleep(_rand.Next(1000, 1300));
            PressKey(VirtualKeyCode.VK_D, 100);
            Thread.Sleep(_rand.Next(200, 400));
            PressKey(VirtualKeyCode.VK_S, 100);
            Thread.Sleep(_rand.Next(250, 1000));
            PressKey(VirtualKeyCode.VK_A, 100);
        }

        public void PressKeyForDuration(VirtualKeyCode key, int durationMilliseconds)
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
    }
}
