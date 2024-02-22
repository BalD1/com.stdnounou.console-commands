using StdNounou.Core;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New Transform Controller", menuName = "StdNounou/Scriptables/Console/Transform Controller", order = 400)]
    public class SO_Command_Controller_Transform : SO_ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            if (args.Length == 0)
            {
                this.LogError("Controller Transform needs at least one argument.");
                return false;
            }

            if (!TryGetTargetObject(args[0], out GameObject target, out bool foundByID))
                return false;

            if (TryParseVector(args[foundByID ? 1 : 0], out Vector3 position))
            {
                if (target == null)
                {
                    this.LogError("Controller Transform Command failed. Please specify an object ID or select one.");
                    return false;
                }

                target.transform.position = position;

                if (args.Length == (foundByID ? 3 : 2) && TryParseVector(args[foundByID ? 2 : 1], out Vector3 euleurAngles))
                {
                    Quaternion objRotation = target.transform.localRotation;
                    objRotation.eulerAngles = euleurAngles;
                    target.transform.localRotation = objRotation;
                }

                return true;
            }

            this.LogError("Wrong parameters format for Controller Transform. Vector was expected ({x,y,z}).");
            return false;
        }
    }
}
