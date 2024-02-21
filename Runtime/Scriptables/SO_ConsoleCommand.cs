using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    public abstract class SO_ConsoleCommand : ScriptableObject, IConsoleCommand
    {
        [SerializeField] private string commandKey = string.Empty;
        [SerializeField] private string commandArguments = string.Empty;

        public string CommandKey => commandKey;
        public string CommandArguments => commandArguments;

        public abstract bool Process(string[] args);
    }
}