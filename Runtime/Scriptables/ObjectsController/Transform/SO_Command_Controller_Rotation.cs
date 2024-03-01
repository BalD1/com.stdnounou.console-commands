using StdNounou.Core;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New Rotation Controller", menuName = "StdNounou/Scriptables/Console/Rotation Controller", order = 410)]
    public class SO_Command_Controller_Rotation : SO_ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            if (args.Length == 0)
            {
                this.LogError("Controller position needs at least one argument.");
                return false;
            }

            if (!TryGetTargetObject(args[0], out GameObject target, out bool foundByID, out int nextArgIdx))
                return false;

            TryParseArg(args, ref nextArgIdx, out bool asLocal);
            if (!TryParseArg(args, ref nextArgIdx, out Vector3 eulerAngles))
                return false;

            if (!TryParseArg(args, ref nextArgIdx, out float rotationTime))
            {
                if (asLocal)
                {
                    Quaternion quat = target.transform.localRotation;
                    quat.eulerAngles = eulerAngles;
                    target.transform.localRotation = quat;
                }
                else
                {
                    Quaternion quat = target.transform.rotation;
                    quat.eulerAngles = eulerAngles;
                    target.transform.rotation = quat;
                }
            }

            TryParseArg(args, ref nextArgIdx, out bool ignoreTimeScale);

            if (asLocal)
                LeanTween.rotateLocal(target, eulerAngles, rotationTime).setIgnoreTimeScale(ignoreTimeScale);
            else
                LeanTween.rotate(target, eulerAngles, rotationTime).setIgnoreTimeScale(ignoreTimeScale);

            return true;
        }
    }
}
