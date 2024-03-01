using StdNounou.Core;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New Transform Controller", menuName = "StdNounou/Scriptables/Console/Transform Controller", order = 410)]
    public class SO_Command_Controller_Transform : SO_ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            if (args.Length == 0)
            {
                this.LogError("Controller Transform needs at least one argument.");
                return false;
            }

            if (!TryGetTargetObject(args[0], out GameObject target, out bool foundByID, out int nextArgIdx))
                return false;

            TryParseArg(args, ref nextArgIdx, out bool asLocal);

            if (TryParseArg(args, ref nextArgIdx, out Vector3 position))
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

                if (TryParseArg(args, ref nextArgIdx, out Vector3 eulerAngles))
                {
                    if (asLocal)
                    {
                        Quaternion objRotation = target.transform.localRotation;
                        objRotation.eulerAngles = eulerAngles;
                        target.transform.localRotation = objRotation;
                    }
                    else
                    {
                        Quaternion objRotation = target.transform.rotation;
                        objRotation.eulerAngles = eulerAngles;
                        target.transform.rotation = objRotation;
                    }
                }

                if (TryParseArg(args, ref nextArgIdx, out Vector3 scale))
                    target.transform.localScale = scale;

                return true;
            }

            this.LogError("Wrong parameters format for Controller Transform. Vector was expected ({x,y,z}).");
            return false;
        }
    }
}
