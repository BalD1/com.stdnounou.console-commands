using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
	[CreateAssetMenu(fileName = "New SO_InspectorObjectDataContainer", menuName = "StdNounou/Scriptables/Console/Hierarchy/InspectorObjectDataContainer", order = 450)]
	public class SO_InspectorObjectDataContainer : ScriptableObject
	{
		[field: SerializeField] public SerializedDictionary<string, SO_InspectorObjectData> Data { get; private set; }
	} 
}