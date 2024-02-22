using StdNounou.Core;
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

        protected Vector3 ParseVector(string arg)
        {
            string[] res = arg.Trim('{','}').Split(',');

            float x, y, z = 0;

            if (res.Length < 2 || res.Length > 3) 
            {
                this.LogError($"Could not parse {arg} to Vector. Wrong number of arguments.");
                return Vector3.zero;
            }

            if (!float.TryParse(res[0], out x))
                this.LogError($"Could not parse {res[0]} to Float as x.");
            if (!float.TryParse(res[1], out y))
                this.LogError($"Could not parse {res[1]} to Float as y.");
            if (res.Length == 3)
            {
                if (!float.TryParse(res[2], out z))
                    this.LogError($"Could not parse {res[2]} to Float as z.");
            }

            return new Vector3(x, y, z);
        }
    }
}