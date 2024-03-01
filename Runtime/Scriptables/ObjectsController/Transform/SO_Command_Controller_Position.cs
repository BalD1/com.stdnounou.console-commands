using StdNounou.Core;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New Position Controller", menuName = "StdNounou/Scriptables/Console/Position Controller", order = 410)]
    public class SO_Command_Controller_Position : SO_ConsoleCommand
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
            if (!TryParseArg(args, ref nextArgIdx, out Vector3 pos))
                return false;

            if (!TryParseArg(args, ref nextArgIdx, out float travelTime))
            {
                if (asLocal)
                    target.transform.localPosition = pos;
                else
                    target.transform.position = pos;
                return true;
            }

            TryParseArg(args, ref nextArgIdx, out bool ignoreTimeScale);

            if (asLocal)
                LeanTween.moveLocal(target, pos, travelTime).setIgnoreTimeScale(ignoreTimeScale);
            else
                LeanTween.move(target, pos, travelTime).setIgnoreTimeScale(ignoreTimeScale);

            return true;
        }
    }
}
