using StdNounou.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New LogCommand", menuName = "StdNounou/Scriptables/Console/Log Console Command", order = 400)]
    public class SO_Command_LogConsole : SO_ConsoleCommand
    {
        [SerializeField] private LogType logType;

        private const string baliseRegexExpression = "%[^%]+%";
        private readonly Regex baliseRegex = new Regex(baliseRegexExpression);

        private readonly Dictionary<string, Func<string>> expressionsProcess = new Dictionary<string, Func<string>>
        {
            { "%TimeScale%", () =>
                {
                    return Time.timeScale.ToString();
                }
            },
            { "%Time%", () =>
                {
                    return DateTime.Now.ToString("HH:mm:ss:ff");
                }
            }
        };

        public override bool Process(string[] args)
        {
            string logText = string.Join(" ", args);

            MatchCollection regexMatches = baliseRegex.Matches(logText);
            foreach (Match match in regexMatches)
            {
                if (expressionsProcess.TryGetValue(match.Value, out Func<string> process))
                    logText = logText.Replace(match.Value, process.Invoke());
                else
                    this.LogError($"Could not find expression {match} in dict.");
            }

            switch (logType)
            {
                case LogType.Error:
                    StringBuilder sb = new StringBuilder("<color=#FF0000>");
                    sb.Append(logText);
                    sb.Append("</color>");
                    DeveloperConsoleBehaviour.Instance.AddTextToConsole(sb.ToString());
                    break;
                case LogType.Warning:
                    sb = new StringBuilder("<color=#FFFF00>");
                    sb.Append(logText);
                    sb.Append("</color>");
                    DeveloperConsoleBehaviour.Instance.AddTextToConsole(sb.ToString());
                    break;
                case LogType.Log:
                    DeveloperConsoleBehaviour.Instance.AddTextToConsole(logText);
                    break;
            }
            return true;
        }
    }
}