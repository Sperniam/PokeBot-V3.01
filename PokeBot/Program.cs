using System;
using PokeBot;

namespace PokemonBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var capture = new CapturePokemonName();
            var bot = new FightBot();

            // Start the bot with a function that returns the Pokémon name
            bot.Start(() =>
            {
                string name = capture.GetName();
                Console.WriteLine("Detected: " + (string.IsNullOrEmpty(name) ? "[EMPTY]" : name));

                return name; // return the string so FightBot can handle it
            });
        }
    }
}