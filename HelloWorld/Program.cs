using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!PromptYesOrNo("Do you want to go on an adventure?"))
            {
                return;
            }

            string name = Prompt("What is your name?");
            if (name.Length == 0)
            {
                name = "Person";
            }
            Console.WriteLine("Hello, {0}! Off we go!\n", name);
            GameLoop(new World(name));
        }

        static void GameLoop(World world)
        {
            while (true)
            {
                Console.WriteLine("You are here: {0}", world.Player.CurrentPosition.Name);
                string dirs = "Paths:";
                foreach (var item in world.Player.CurrentPosition.GetDirections())
                {
                    dirs += " " + item.ToString();
                }
                Console.WriteLine(dirs);
                ICommand cmd = PromptCommand();
                if (cmd.Execute(world))
                {
                    world.Tick();
                }
                Console.WriteLine();
            }
        }

        static public string Prompt(string question)
        {
            Console.WriteLine(question);
            return Prompt();
        }

        static public string Prompt()
        {
            Console.Write("> ");
            return Console.In.ReadLine();
        }

        static public ICommand PromptCommand()
        {
            while (true)
            {
                string input = Prompt();
                ICommand cmd = Command.Parse(input);
                if (cmd is NoneCommand)
                {
                    Console.WriteLine("Sorry, I didn't understand that.");
                }
                else
                {
                    return cmd;
                }
            }
        }

        static readonly string[] YesWords = { "yes", "y" };
        static readonly string[] NoWords = { "no", "n" };
        static public bool PromptYesOrNo(string question)
        {
            string response = Prompt(question + " (yes/no)")
                                .ToLower().Trim();
            while (true)
            {
                if (YesWords.Contains(response))
                {
                    return true;
                }
                else if (NoWords.Contains(response))
                {
                    return false;
                }
                else
                {
                    response = Prompt("Yes or No?");
                }
            }
        }
    }
}
