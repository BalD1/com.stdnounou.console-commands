using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New SO_InspectorObjectData_Collider", menuName = "StdNounou/Scriptables/Console/Hierarchy/InspectorObjectData_Collider", order = 450)]
    public class SO_InspectorObjectData_Collider : SO_InspectorObjectData
    {
        public override string Process(Component component)
        {
            if (!TryCast(component, out Collider collider))
                return "";

            return "Is Trigger : " + collider.isTrigger;
        }
    }
}