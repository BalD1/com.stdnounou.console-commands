using StdNounou.Core;
using StdNounou.Health;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New Damager Command", menuName = "StdNounou/Scriptables/Console/Damager Command", order = 400)]
    public class SO_Command_Controller_Damager : SO_ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            if (!TryGetTargetObject(args[0], out GameObject target, out bool foundByID))
                return false;

            if (!target.TryGetComponent(out HealthSystem targetSystem))
            {
                targetSystem = target.GetComponentInChildren<HealthSystem>();
                if (targetSystem == null)
                {
                    targetSystem = target.GetComponentInParent<HealthSystem>();
                    if (targetSystem == null)
                    {
                        this.LogError("Could not find Health System on specified object.");
                        return false;
                    }
                }
            }

            if (!float.TryParse(args[foundByID ? 1 : 0], out float damages))
            {
                this.LogError("Wrong parameters format for Damager Command. float was expected.");
                return false;
            }

            bool isCrit = false;
            if (args.Length > (foundByID ? 2 : 1)) 
                bool.TryParse(args[foundByID ? 2 : 1], out isCrit);

            targetSystem.TryInflictDamages(damages, isCrit);
            return true;
        }
    }
}
