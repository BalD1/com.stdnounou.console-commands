using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New Console Commands Holder", menuName = "StdNounou/Scriptables/Console/Commands Holder", order = 400)]
    public class SO_ConsoleCommandsHolder : ScriptableObject
    {
        [field: SerializeField] public SO_ConsoleCommand[] Commands { get; private set; }
    }
}
