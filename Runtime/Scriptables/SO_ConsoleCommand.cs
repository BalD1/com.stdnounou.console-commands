using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    public abstract class SO_ConsoleCommand : ScriptableObject, IConsoleCommand
    {
        [SerializeField] private string commandKey = string.Empty;
        public string CommandKey => commandKey;

        public abstract bool Process(string[] args);
    }
}