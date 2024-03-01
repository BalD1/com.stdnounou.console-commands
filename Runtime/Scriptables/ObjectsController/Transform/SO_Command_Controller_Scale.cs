using StdNounou.Core;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New Scale Controller", menuName = "StdNounou/Scriptables/Console/Scale Controller", order = 410)]
    public class SO_Command_Controller_Scale : SO_ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            if (args.Length == 0)
            {
                this.LogError("Controller scale needs at least one argument.");
                return false;
            }

            if (!TryGetTargetObject(args[0], out GameObject target, out bool foundByID, out int nextArgIdx))
                return false;

            if (!TryParseArg(args, ref nextArgIdx, out Vector3 scale))
                return false;

            if (!TryParseArg(args, ref nextArgIdx, out float travelTime))
            {
                target.transform.localScale = scale;
                return true;
            }

            TryParseArg(args, ref nextArgIdx, out bool ignoreTimeScale);

            LeanTween.scale(target, scale, travelTime).setIgnoreTimeScale(ignoreTimeScale);
            return true;
        }
    }
}
