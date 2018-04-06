using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdv {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 0 && !PromptYesOrNo("Do you want to go on an adventure?")) {
                return;
            }
            string name = args.Length == 0 ? Prompt("What is your name?") : String.Join(" ", args);
            if (name.Length == 0) {
                name = "Anon";
            }
            Console.WriteLine($"Hello, {name}! Off we go!\n");

            GameLoop(new World(name));
        }

        /// <summary>
        /// Starts and runs the game until the user ends the game.
        /// </summary>
        /// <param name="world">The world to run.</param>
        static void GameLoop(World world) {
            // We want to print the current location when the game starts, 
            // so the player knows where he is.
            LookCommand.Print(world.Player.CurrentPosition);
            while (true) {
                ICommand cmd = PromptCommand(world);
                // The execute method tells us if we should update the world or not,
                // since we don't want to update the world if the command was invalid
                // or it was just inspecting something.
                if (cmd.Execute(world)) {
                    world.Tick();
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Prompts the user for input by asking a question
        /// </summary>
        /// <param name="question">The question to ask</param>
        /// <returns>User input</returns>
        static public string Prompt(string question) {
            Console.WriteLine(question);
            return Prompt();
        }

        /// <summary>
        /// Prompts the user for input
        /// </summary>
        /// <returns>User input string</returns>
        static public string Prompt() {
            Console.Write("> ");
            return Console.ReadLine();
        }

        /// <summary>
        /// Prompts the user for a valid command.
        /// </summary>
        /// <param name="world">The game world the command takes place in</param>
        /// <returns>The parsed command</returns>
        static public ICommand PromptCommand(World world) {
            while (true) {
                string input = Prompt();
                ICommand cmd = Command.Parse(input, world);
                if (cmd != null) {
                    return cmd;
                }
            }
        }

        static readonly string[] YesWords = { "yes", "y", "ja" };
        static readonly string[] NoWords = { "no", "n", "nej" };
        /// <summary>
        /// Prompts the user a question yes/no question and will reprompt the user until it gets a yes or no.
        /// </summary>
        /// <param name="question">The question to ask.</param>
        /// <returns>yes or no</returns>
        static public bool PromptYesOrNo(string question) {
            string response = Prompt(question + " (yes/no)")
                                .ToLower().Trim();
            while (true) {
                if (YesWords.Contains(response)) {
                    return true;
                }
                else if (NoWords.Contains(response)) {
                    return false;
                }
                else {
                    response = Prompt("Yes or No?");
                }
            }
        }
    }
}
