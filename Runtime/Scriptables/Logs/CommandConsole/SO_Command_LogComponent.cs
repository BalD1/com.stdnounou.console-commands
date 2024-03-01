using StdNounou.Core;
using StdNounou.Health;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    [CreateAssetMenu(fileName = "New LogComponent", menuName = "StdNounou/Scriptables/Console/Log Component Command", order = 400)]
    public class SO_Command_LogComponent : SO_ConsoleCommand
    {
        private const string baliseRegexExpression = "%[^%]+%";
        private readonly Regex baliseRegex = new Regex(baliseRegexExpression, RegexOptions.IgnoreCase);

        private readonly Dictionary<string, Func<SO_Command_LogComponent, GameObject, string>> expressionsProcess = new Dictionary<string, Func<SO_Command_LogComponent, GameObject, string>>
        {
            { "%HealthSystem%", (caller, target) =>
                {
                    caller.SearchComponent(target, out HealthSystem targetSystem);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Debug Health System of ");
                    sb.AppendLine(target.name);
                    sb.Append("Health : ");
                    sb.Append(targetSystem.CurrentHealth);
                    sb.Append(" / ");
                    sb.AppendLine(targetSystem.CurrentMaxHealth.ToString());
                    sb.AppendLine("Unique Tick Damages : ");
                    foreach (var item in targetSystem.UniqueTickDamages)
                    {
                        sb = AppendTick(sb, item.Value);
                    }
                    sb.AppendLine(" ");
                    sb.AppendLine("Stackable Tick Damages : ");
                    foreach (var collection in targetSystem.StackableTickDamages)
                    {
                        sb.Append(" > ");
                        sb.AppendLine(collection.Value[0].Data.ID);
                        foreach (var single in collection.Value)
                        {
                            sb.Append(" > ");
                            sb = AppendTick(sb, single);
                        }
                        sb.AppendLine(" ");
                    }
                    return sb.ToString();

                    StringBuilder AppendTick(StringBuilder sb, TickDamages td)
                    {
                        sb.Append(" > ");
                        sb.Append(td.Data.ID);
                        sb.Append(" : ");
                        sb.Append(td.RemainingTicks());
                        sb.Append(" / ");
                        sb.Append(td.Data.TicksLifetime);
                        sb.Append("(");
                        sb.Append(td.RemainingTimeInSeconds());
                        sb.AppendLine(")");
                        return sb;
                    }
                }
            }
        };

        public override bool Process(string[] args)
        {
            if (!TryGetTargetObject(args[0], out GameObject target, out bool foundByID, out int nextArgIdx))
            {
                this.LogError("LogComponent Command failed. Please specify an object ID or select one.");
                return false;
            }

            string logText = string.Join(" ", args, nextArgIdx, foundByID ? args.Length - 1 : args.Length);
            MatchCollection regexMatches = baliseRegex.Matches(logText);
            foreach (Match match in regexMatches)
            {
                if (expressionsProcess.TryGetValue(match.Value, out var process))
                    logText = logText.Replace(match.Value, process.Invoke(this, target));
                else
                    this.LogError($"Could not find expression {match} in dict.");
            }
            DeveloperConsole.Instance.AddTextToConsole(logText);
            return true;
        }
    }
}
