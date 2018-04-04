using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    class Program
    {
        MapNode currentpos;

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
            Console.WriteLine("Hello, %s! Off we go!\r\n", name);

            
        }

        static public string Prompt(string question)
        {
            Console.WriteLine(question);
            Console.Write("> ");
            return Console.In.ReadLine();
        }

        static readonly string[] YesWords = { "yes", "y", "true" };
        static readonly string[] NoWords = { "no", "n", "false" };

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
