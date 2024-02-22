using StdNounou.Core;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New Create From Resources", menuName = "StdNounou/Scriptables/Console/CreateFromResources", order = 400)]
    public class SO_Command_CreateFromResources : SO_ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            if (args.Length < 1 || args.Length > 3)
            {
                this.LogError("Invalid Arguments for command CreateFromResources.");
                return false;
            }

            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;

            if (args.Length >= 2)
                pos = ParseVector(args[1]);
            if (args.Length == 3)
                rot.eulerAngles = ParseVector(args[2]);

            return CreateObject(pos, rot, args[0]);
        }

        private bool CreateObject(Vector3 pos, Quaternion rot, string path)
        {
            Object loadResult = Resources.Load(path);
            if (loadResult == null)
            {
                this.LogError($"Create Command could not find object at {path}.");
                return false;
            }
            Instantiate(loadResult, pos, rot);
            return true;
        }
    }
}
