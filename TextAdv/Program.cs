using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdv {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 0 && !AskYesOrNo("Do you want to go on an adventure?")) {
                return;
            }
            string name = args.Length == 0 ? Ask("What is your name?") : String.Join(" ", args);
            if (name.Length == 0) {
                name = "Steve";
            }
            Say($"Hello, {name}! Off we go!\n");

            GameLoop(new World(name));
        }

        /// <summary>
        /// Starts and runs the game until the user ends the game.
        /// </summary>
        /// <param name="world">The world to run.</param>
        static void GameLoop(World world) {
            // We want to print the current location when the game starts, 
            // so the player knows where he is.
            LookCommand.Print(world.Player.Location);
            while (true) {
                // The execute method tells us if we should update the world or not,
                // since we don't want to update the world if the command was invalid
                // or it was just inspecting something.
                if (PromptCommand(world).Execute(world)) {
                    world.Tick();
                }
                Spacer();
            }
        }

        static public void Say(string words, bool linebreak = true) {
            if (linebreak) {
                Console.WriteLine(words);
            }
            else {
                Console.Write(words);
            }
        }

        static public void Error(string what, string why) {
            Say($"Error: {what}: {why}");
        }

        static public void Spacer() {
            Say("");
        }

        /// <summary>
        /// Asks the user a question.
        /// </summary>
        /// <param name="question">The question to ask</param>
        /// <returns>The users answer</returns>
        static public string Ask(string question) {
            Say($"{question}\n$ ", false);
            return Console.ReadLine();
        }

        /// <summary>
        /// Prompts the user for input and retries until a valid command was entered
        /// </summary>
        /// <param name="world">The game world the command takes place in</param>
        /// <returns>The parsed command. The command is guaranteed to not be null</returns>
        static public ICommand PromptCommand(World world) {
            while (true) {
                Say("> ", false);
                ICommand cmd = Command.Parse(Console.ReadLine(), world);
                if (cmd != null) {
                    return cmd;
                }
            }
        }

        static readonly string[] YesWords = { "yes", "y", "ja", "j", "true" };
        static readonly string[] NoWords = { "no", "n", "nej", "false" };
        /// <summary>
        /// Asks the user a yes/no question and will keep asking the user until we get a y/n answer.
        /// </summary>
        /// <param name="question">The question to ask.</param>
        /// <returns>yes or no</returns>
        static public bool AskYesOrNo(string question) {
            string response = Ask(question + " (yes/no)")
                                .ToLower().Trim();
            while (true) {
                if (YesWords.Contains(response)) {
                    return true;
                }
                else if (NoWords.Contains(response)) {
                    return false;
                }
                else {
                    response = Ask("Yes or No?");
                }
            }
        }

        static public int AskInt(string question) {
            string response = Ask(question + " (number)")
                                .ToLower().Trim();
            while (true) {
                if (Int32.TryParse(response, out int result)) {
                    return result;
                }
                else {
                    response = Ask("Enter a number please.");
                }
            }
        }

        public static void Clear() {
            Console.Clear();
        }
    }
}
