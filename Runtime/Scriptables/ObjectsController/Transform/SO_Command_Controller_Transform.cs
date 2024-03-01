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

            int nextArgIdx = foundByID ? 1 : 0;

            if (TryParseArg(args, nextArgIdx, out bool asLocal))
                nextArgIdx++;

            if (args.Length == nextArgIdx + 1 && TryParseVector(args[nextArgIdx], out Vector3 position))
            {
                if (target == null)
                {
                    this.LogError("Controller Transform Command failed. Please specify an object ID or select one.");
                    return false;
                }

                if (asLocal)
                    target.transform.localPosition = position;
                else
                    target.transform.position = position;
                nextArgIdx++;

                if (args.Length == nextArgIdx + 1 && TryParseVector(args[nextArgIdx], out Vector3 euleurAngles))
                {
                    if (asLocal)
                    {
                        Quaternion objRotation = target.transform.localRotation;
                        objRotation.eulerAngles = euleurAngles;
                        target.transform.localRotation = objRotation;
                    }
                    else
                    {
                        Quaternion objRotation = target.transform.rotation;
                        objRotation.eulerAngles = euleurAngles;
                        target.transform.rotation = objRotation;
                    }
                    nextArgIdx++;
                }

                return true;
            }

            this.LogError("Wrong parameters format for Controller Transform. Vector was expected ({x,y,z}).");
            return false;
        }
    }
}
