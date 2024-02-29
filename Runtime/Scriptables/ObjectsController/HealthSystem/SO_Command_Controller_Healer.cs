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
            if (!TryGetTargetObject(args[0], out GameObject target, out bool foundByID))
                return false;

            if (!float.TryParse(args[foundByID ? 1 : 0], out float damages))
            {
                this.LogError("Wrong parameters format for Damager Command. float was expected.");
                return false;
            }

            if (!SearchComponent(target, out HealthSystem targetSystem))
                return false;

            bool isCrit = false;
            if (args.Length > (foundByID ? 2 : 1))
                bool.TryParse(args[foundByID ? 2 : 1], out isCrit);

            targetSystem.Heal(damages, isCrit);
            return true;
        }
    }
}
