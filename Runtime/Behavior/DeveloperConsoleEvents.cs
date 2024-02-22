using System;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
	public static class DeveloperConsoleEvents
	{
		public static event Action<IConsoleCommand, string[]> OnPerformedCommand;
		public static void OnPerformedCommand_Call(this DeveloperConsole console, IConsoleCommand cmd, string[] args)
			=> OnPerformedCommand?.Invoke(cmd, args);

		public static event Action<GameObject> OnSelectedObject;
		public static void OnSelectedObject_Call(this DeveloperConsole console, GameObject obj)
			=> OnSelectedObject?.Invoke(obj);

		public static event Action OnUnSelectedObject;
		public static void OnUnselectedObject_Call(this DeveloperConsole console)
			=> OnUnSelectedObject?.Invoke();
	} 
}