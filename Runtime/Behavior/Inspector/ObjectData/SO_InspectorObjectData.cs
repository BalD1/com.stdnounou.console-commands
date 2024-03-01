using StdNounou.Core;
using System;
using UnityEngine;

namespace StdNounou.ConsoleCommands
{
	public abstract class SO_InspectorObjectData : ScriptableObject
    {
		public abstract string Process(Component component);

		protected bool TryCast<T>(Component component, out T result) where T : Component
		{
            result = component as T;
            if (result != null)
                return true;
            this.LogError($"Could not cast {component} as {typeof(T)}.");
            return false;
        }
	}
}