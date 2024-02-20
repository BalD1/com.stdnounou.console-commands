using System.Collections.Generic;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New LogCommand", menuName = "StdNounou/Scriptables/Console/Log Command", order = 400)]
    public class SO_Command_Log : SO_ConsoleCommand
    {
        private Dictionary<string, LogType> logTypesKeys = new Dictionary<string, LogType>
        {
            { "e", LogType.Error },
            { "err", LogType.Error },
            { "error", LogType.Error },
            { "w", LogType.Warning },
            { "warn", LogType.Warning },
            { "warning", LogType.Warning },
        };
        public override bool Process(string[] args)
        {
            string logText = string.Join(" ", args);

            LogType logType = LogType.Log;
            if (args.Length > 1)
            {
                if (logTypesKeys.TryGetValue(args[0], out logType))
                {
                    logText = logText.Remove(0, args[0].Length + 1);
                }
            }

            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(logText);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(logText);
                    break;
                case LogType.Log:
                    Debug.Log(logText);
                    break;
            }
            return true;
        }
    }
}