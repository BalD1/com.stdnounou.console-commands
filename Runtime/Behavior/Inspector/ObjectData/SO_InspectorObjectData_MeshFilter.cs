using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New SO_InspectorObjectData_MeshFilter", menuName = "StdNounou/Scriptables/Console/Hierarchy/InspectorObjectData_MeshFilter", order = 450)]
    public class SO_InspectorObjectData_MeshFilter : SO_InspectorObjectData
    {
        public override string Process(Component component)
        {
            if (!TryCast(component, out MeshFilter meshFilter))
                return "";

            return "Mesh : " + meshFilter.mesh.ToString();
        }
    }
}
