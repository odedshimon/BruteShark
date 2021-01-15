using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteSharkCli
{
    class CliShell
    {
        public string Seperator { get; set; }
        private List<CliShellCommand> _commands;
        private bool _exit;

        public CliShell(string seperator = ">")
        {
            this.Seperator = seperator;
            this._commands = new List<CliShellCommand>();

            // Add the help command
            this.AddCommand(new CliShellCommand(
                "help",
                 param => this.PrintCommandsWithDescription(), 
                 "Print help menu"));

            // Add the exit command
            this.AddCommand(new CliShellCommand(
                "exit",
                 param => this._exit = true,
                 "Exit CLI"));
        }

        public void AddCommand(CliShellCommand cliShellCommand)
        {
            this._commands.Add(cliShellCommand);
        }

        public void PrintCommandsWithDescription()
        {
            this._commands.ToDataTable().Print();
        }

        internal void Start()
        {
            _exit = false;

            do
            {
                HandleUserInput();
            }
            while (!_exit);
        }

     

        private void HandleUserInput()
        {
            bool legalInput;

            do
            {
                Console.Write(this.Seperator);
                string userInput = Console.ReadLine();
                legalInput = RunCommand(userInput);
            }
            while (!legalInput);
        }

        public bool RunCommand(string userInput)
        {
            bool legalInput = true;
            string[] inputParts = userInput.Split();

            CliShellCommand wanted_command = this._commands.Where(c => c.Keyword == inputParts[0]).FirstOrDefault();

            if (wanted_command != null)
            {
                // Run command.
                string commandData = userInput.Substring(userInput.IndexOf(" ") + 1);
                wanted_command.Action(commandData);
            }
            else
            {
                Console.WriteLine("Illegal Input.");
                legalInput = false;
            }

            return legalInput;
        }


    }
}
