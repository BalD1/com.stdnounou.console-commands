using StdNounou.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New ExportToText Command", menuName = "StdNounou/Scriptables/Console/ExportToText", order = 400)]
    public class SO_Command_ExportToTxt : SO_ConsoleCommand
    {
        [SerializeField] private string filePath;

        public override bool Process(string[] args)
        {
            if (args.Length != 1)
            {
                this.LogError("Command ExportToTxt requires exactly 1 argument.");
                return false;
            }

            string targetPath = args[0];
            targetPath = targetPath.Replace("%Path%", filePath);
            targetPath = targetPath.Replace("%AppPath%", Application.dataPath);

            string output = DateTime.UtcNow.ToString() + "\n";
            output += DeveloperConsole.Instance.GetConsoleText();

            Debug.Log("Exported console logs to " + targetPath);

            if (!File.Exists(targetPath))
            {
                File.WriteAllText(targetPath, output);
                return true;
            }

            output = output.Insert(0, "\n ______________________ \n");
            File.AppendAllText(targetPath, output);

            return true;
        }
    }
}
