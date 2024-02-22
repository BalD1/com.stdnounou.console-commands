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

        protected bool TryParseVector(string arg, out Vector3 result)
        {
            string[] parsedStrings = arg.Trim('{','}').Split(',');

            float x, y, z = 0;

            if (parsedStrings.Length < 2 || parsedStrings.Length > 3) 
            {
                this.LogError($"Could not parse {arg} to Vector. Wrong number of arguments.");
                result = Vector3.zero;
                return false;
            }

            if (!float.TryParse(parsedStrings[0], out x))
                this.LogError($"Could not parse {parsedStrings[0]} to Float as x.");
            if (!float.TryParse(parsedStrings[1], out y))
                this.LogError($"Could not parse {parsedStrings[1]} to Float as y.");
            if (parsedStrings.Length == 3)
            {
                if (!float.TryParse(parsedStrings[2], out z))
                    this.LogError($"Could not parse {parsedStrings[2]} to Float as z.");
            }

            result = new Vector3(x, y, z);
            return true;
        }

        protected bool TryGetTargetObject(string arg, out GameObject obj, out bool foundByID)
        {
            if (int.TryParse(arg, out int id))
            {
                obj = (GameObject)DeveloperConsole.Instance.FindObjectFromInstanceID(id);
                if (obj != null)
                {
                    foundByID = true;
                    return true;
                }
            }

            foundByID = false;

            if (DeveloperConsole.Instance.SelectedObject == null)
            {
                this.LogError("Could not find target object. Please select one or specify an ID.");
                obj = null;
                return false;
            }

            obj = DeveloperConsole.Instance.SelectedObject;
            return true;
        }
    }
}