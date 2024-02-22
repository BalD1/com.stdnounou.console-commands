using StdNounou.Core;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New TimeScale Command", menuName = "StdNounou/Scriptables/Console/Set TimeScale", order = 400)]
    public class SO_Command_TimeScale : SO_ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            if (args.Length != 1)
            {
                this.LogError("Command TimeScale requires exactly one argument.");
                return false;
            }
            if (!float.TryParse(args[0], out float result))
            {
                this.LogError($"Could not parse {result} to float.");
                return false;
            }
            Time.timeScale = result;
            return true;
        }
    }
}
