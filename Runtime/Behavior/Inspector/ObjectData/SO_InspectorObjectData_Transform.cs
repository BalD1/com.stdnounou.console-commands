using StdNounou.Core;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
	[CreateAssetMenu(fileName = "New SO_InspectorObjectData_Transform", menuName = "StdNounou/Scriptables/Console/Hierarchy/InspectorObjectData_Transform", order = 450)]
	public class SO_InspectorObjectData_Transform : SO_InspectorObjectData
    {
        public override string Process(Component component)
        {
            if (!TryCast(component, out  Transform transform))
                return "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<b>World :</b>");
            sb.Append("Position : ");
            sb.AppendLine(transform.position.ToString());
            sb.Append("Rotation : ");
            sb.AppendLine(transform.rotation.eulerAngles.ToString());
            sb.Append("Scale : ");
            sb.AppendLine(transform.localScale.ToString());

            sb.AppendLine("<b>Local :</b>");
            sb.Append("Position : ");
            sb.AppendLine(transform.localPosition.ToString());
            sb.Append("Rotation : ");
            sb.AppendLine(transform.localRotation.eulerAngles.ToString());
            sb.Append("Scale : ");
            sb.AppendLine(transform.localScale.ToString());
            return sb.ToString();
        }
    } 
}