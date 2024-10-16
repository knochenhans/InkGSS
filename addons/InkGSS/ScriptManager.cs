using Godot;
using GodotInk;
using System.Threading.Tasks;
using Godot.Collections;
using System;

public partial class ScriptManager : GodotObject
{
	public Array<AbstractScriptAction> ActionQueue { get; set; } = new Array<AbstractScriptAction>();

	public async Task RunActionQueue()
	{
		GD.Print($"Running {ActionQueue.Count} actions in queue");
		foreach (var action in ActionQueue)
		{
			GD.Print($"Running action: {action.GetType()}");
			await action.Execute();
		}
		ActionQueue.Clear();
	}

	// External Ink functions

	// Func<string, Variant> InkGetVariable;
	// Action<string, bool> InkSetVariable;
	// Func<string, Variant> InkGetScriptVisits;

	public InkStory InkStory { get; set; }

	Dictionary<string, Callable> ExternalFunctions = new();

	public ScriptManager(InkStory story)
	{
		InkStory = story;
	}

	public void BindExternalFunction(string functionName, Callable function)
	{
		GD.Print($"Binding external function: {functionName}");
		ExternalFunctions[functionName] = function;
		InkStory.BindExternalFunction(functionName, function);
	}

	public void UnbindExternalFunctions()
	{
		foreach (var externalFunction in ExternalFunctions)
		{
			GD.Print($"Unbinding external function: {externalFunction.Key}");
			InkStory.UnbindExternalFunction(externalFunction.Key);
		}
	}

	public void AddAction(AbstractScriptAction scriptAction)
	{
		ActionQueue.Add(scriptAction);
	}
}