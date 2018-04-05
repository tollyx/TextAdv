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

            Console.WriteLine("Hello, {0}! Off we go!\n", name);
            GameLoop(new World(name));
        }

        static void GameLoop(World world) {
            while (true) {
                Console.WriteLine("You are here: {0}", world.Player.CurrentPosition.Name);

                string items = "Items:";
                foreach (var item in world.Player.CurrentPosition.Inventory) {
                    items += " " + item.Name;
                }
                Console.WriteLine(items);

                string dirs = "Paths:";
                foreach (var item in world.Player.CurrentPosition.GetDirections()) {
                    dirs += " " + item.ToString();
                }
                Console.WriteLine(dirs);

                ICommand cmd = PromptCommand(world);
                if (cmd.Execute(world)) {
                    world.Tick();
                }
                Console.WriteLine();
            }
        }

        static public string Prompt(string question) {
            Console.WriteLine(question);
            return Prompt();
        }

        static public string Prompt() {
            Console.Write("> ");
            return Console.In.ReadLine();
        }

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
