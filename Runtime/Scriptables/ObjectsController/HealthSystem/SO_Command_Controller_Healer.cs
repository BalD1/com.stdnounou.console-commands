using StdNounou.Core;
using StdNounou.Health;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New Healer Command", menuName = "StdNounou/Scriptables/Console/Healer Command", order = 400)]
    public class SO_Command_Controller_Healer : SO_ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            if (!TryGetTargetObject(args[0], out GameObject target, out bool foundByID, out int nextArgIdx))
                return false;

            if (!TryParseArg(args, ref nextArgIdx, out float damages))
            {
                this.LogError("Wrong parameters format for Damager Command. float was expected.");
                return false;
            }

            if (!SearchComponent(target, out HealthSystem targetSystem))
                return false;

            TryParseArg(args, ref nextArgIdx, out bool isCrit);
            targetSystem.Heal(damages, isCrit);
            return true;
        }
    }
}
