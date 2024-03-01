using System.Text;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New SO_InspectorObjectData_MeshRenderer", menuName = "StdNounou/Scriptables/Console/Hierarchy/InspectorObjectData_MeshRenderer", order = 450)]
    public class SO_InspectorObjectData_MeshRenderer : SO_InspectorObjectData
    {
        public override string Process(Component component)
        {
            if (!TryCast(component, out MeshRenderer meshRenderer))
                return "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<b>Materials :</b>");
            foreach (var item in meshRenderer.materials)
                sb.AppendLine(item.ToString());

            sb.AppendLine("<b>Lighting :</b>");
            sb.AppendLine(meshRenderer.shadowCastingMode.ToString());
            sb.Append("Receives shadows : ");
            sb.AppendLine(meshRenderer.receiveShadows.ToString());

            return sb.ToString();
        }
    }
}
