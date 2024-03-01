using StdNounou.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
    public abstract class SO_ConsoleCommand : ScriptableObject, IConsoleCommand
    {
        [SerializeField] private string commandKey = string.Empty;
        [SerializeField] private string commandArguments = string.Empty;

        public string CommandKey => commandKey;
        public string CommandArguments => commandArguments;

        public abstract bool Process(string[] args);

        private static readonly Dictionary<Type, Func<string, (bool, object)>> parsers = new()
        {
            { typeof(bool), (input) => { if (bool.TryParse(input, out bool bRes)) return (true, bRes); else return (false, null); } },
            { typeof(int), (input) => { if (int.TryParse(input, out int iRes)) return (true, iRes); else return (false, null); } },
            { typeof(float), (input) => { if (float.TryParse(input, out float fRes)) return (true, fRes); else return (false, null); } },
            { typeof(Vector2), (input) => { if (TryParseVector(input, out Vector3 v2Res)) return (true, v2Res); else return (false, null); } },
            { typeof(Vector3), (input) => { if (TryParseVector(input, out Vector3 v3Res)) return (true, v3Res); else return (false, null); } },
        };

        protected bool TryParseArg<T>(string[] args, ref int targetIdx, out T result)
        {
            if (targetIdx >= args.Length)
            {
                result = default(T);
                return false;
            }
            if (!parsers.ContainsKey(typeof(T)))
            {
                this.LogError("Could not find defined behavior for type " + typeof(T));
                result = default(T);
                return false;
            }

            var (success, value) = parsers[typeof(T)](args[targetIdx]);
            if (success)
            {
                targetIdx++;
                result = (T)value;
                return true;
            }
            result = default(T);
            return false;
        }

        protected static bool TryParseVector(string arg, out Vector3 result)
        {
            string[] parsedStrings = arg.Trim('{','}').Split(',');

            float x, y, z = 0;

            if (parsedStrings.Length < 2 || parsedStrings.Length > 3) 
            {
                CustomLogger.LogError(typeof(SO_ConsoleCommand), $"Could not parse {arg} to Vector. Wrong number of arguments.");
                result = Vector3.zero;
                return false;
            }

            if (!float.TryParse(parsedStrings[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x))
                CustomLogger.LogError(typeof(SO_ConsoleCommand), $"Could not parse {parsedStrings[0]} to Float as x.");
            if (!float.TryParse(parsedStrings[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                CustomLogger.LogError(typeof(SO_ConsoleCommand), $"Could not parse {parsedStrings[1]} to Float as y.");
            if (parsedStrings.Length == 3)
            {
                if (!float.TryParse(parsedStrings[2], NumberStyles.Float, CultureInfo.InvariantCulture, out z))
                    CustomLogger.LogError(typeof(SO_ConsoleCommand), $"Could not parse {parsedStrings[2]} to Float as z.");
            }

            result = new Vector3(x, y, z);
            return true;
        }

        protected bool TryGetTargetObject(string arg, out GameObject obj, out bool foundByID, out int nextArgIdx)
        {
            obj = null;
            foundByID = false;

            if (DeveloperConsole.Instance.SelectedObject != null)
            {
                obj = DeveloperConsole.Instance.SelectedObject;
                foundByID = false;
                nextArgIdx = 0;
                return true;
            }

            if (int.TryParse(arg, out int id))
            {
                object foundObj = GameObjectExtensions.FindObjectFromInstanceID(id);
                if (foundObj is GameObject)
                {
                    obj = (GameObject)GameObjectExtensions.FindObjectFromInstanceID(id);
                    if (obj != null)
                    {
                        foundByID = true;
                        nextArgIdx = 1;
                        return true;
                    }
                }
                else
                    this.LogError($"Object with ID \"{arg}\" was not a gameobject.");
            }

            this.LogError("Could not find target object. Please select one or specify an ID.");
            nextArgIdx = 0;
            return false;
        }

        protected bool SearchComponent<T>(GameObject target, out T result) where T : Component
        {
            result = null;
            if (!target.TryGetComponent(out result))
            {
                result = target.GetComponentInChildren<T>();
                if (result == null)
                {
                    result = target.GetComponentInParent<T>();
                    if (result == null)
                    {
                        this.LogError("Could not find Health System on specified object.");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}