using System;
using System.Collections.Generic;
using System.Linq;

namespace StdNounou.ConsoleCommands
{
    public class DeveloperConsole
    {
        private readonly string prefix;
        public readonly IEnumerable<IConsoleCommand> commands;

        public event Action<IConsoleCommand, string[]> OnActivatedCommand;

        public DeveloperConsole(string prefix, IEnumerable<IConsoleCommand> commands)
        {
            this.prefix = prefix;
            this.commands = commands;
        }

        public void ProcessCommand(string inputValue)
        {
            inputValue = inputValue.Remove(0, prefix.Length);

            string[] inputSplit = inputValue.Split(' ');

            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

            ProcessCommand(commandInput, args);
        }
        public void ProcessCommand(string commandInput, string[] args)
        {
            foreach (var item in commands)
            {
                if (!commandInput.Equals(item.CommandKey, System.StringComparison.OrdinalIgnoreCase))
                    continue;

                if (item.Process(args))
                {
                    OnActivatedCommand?.Invoke(item, args);
                    return;
                }
            }
        }
    }
}
