using Godot;
using GodotInk;
using System.Threading.Tasks;
using Godot.Collections;

public partial class ScriptManager : GodotObject
{
	public Array<AbstractScriptAction> ScriptActionQueue { get; set; } = new Array<AbstractScriptAction>();
	public Array<ScriptObjectController> ScriptObjects { get; set; } = new Array<ScriptObjectController>();
	public Dictionary<string, Variant> ScriptVariables { get; set; } = new Dictionary<string, Variant>();

	public virtual async Task RunActionQueue()
	{
		Logger.Log($"Running {ScriptActionQueue.Count} actions in queue", Logger.LogTypeEnum.Script);
		foreach (var action in ScriptActionQueue)
		{
			Logger.Log($"Running action: {action.GetType()}", Logger.LogTypeEnum.Script);
			await action.Execute();
		}
		ScriptActionQueue.Clear();
	}

	public InkStory InkStory { get; set; }

	Dictionary<string, Callable> ExternalFunctions = new();

	public ScriptManager(InkStory story)
	{
		InkStory = story;

		// Bind default functions
		var dict = new Dictionary<string, string>
		{
			// { "wait", MethodName.Wait },
			{ "get_game_var", MethodName.GetGameVar },
			{ "set_game_var", MethodName.SetGameVar },
			{ "print_error", MethodName.PrintError }
		};

		foreach (var item in dict)
			BindExternalFunction(item.Key, new Callable(this, item.Value));
	}

	public void BindExternalFunction(string functionName, Callable function)
	{
		if (ExternalFunctions.ContainsKey(functionName))
		{
			InkStory.UnbindExternalFunction(functionName);
			Logger.Log($"Warning: Overwriting existing external function binding: {functionName}", Logger.LogTypeEnum.Warning);
		}
		else
			Logger.Log($"Binding external function: {functionName}", Logger.LogTypeEnum.Script);
		ExternalFunctions[functionName] = function;
		InkStory.BindExternalFunction(functionName, function);
	}

	public void Cleanup()
	{
		ScriptActionQueue.Clear();
		ScriptVariables.Clear();
		ScriptObjects.Clear();

		foreach (var externalFunction in ExternalFunctions)
		{
			Logger.Log($"Unbinding external function: {externalFunction.Key}", Logger.LogTypeEnum.Script);
			InkStory.UnbindExternalFunction(externalFunction.Key);
		}
	}

	public void RegisterScriptObject(ScriptObjectController scriptObjectController)
	{
		ScriptObjects.Add(scriptObjectController);

		Logger.Log($"Registered script object with parent node: {scriptObjectController.Parent.Name}", Logger.LogTypeEnum.Script);
	}

    public void QueueAction(AbstractScriptAction scriptAction) => ScriptActionQueue.Add(scriptAction);

    public Variant GetStoryVariable(string variableName) => InkStory.FetchVariable(variableName);

    public void SetStoryVariable(string variableName, Variant value) => InkStory.StoreVariable(variableName, value);

    public Variant GetGameVar(string varName)
	{
		if (ScriptVariables.ContainsKey(varName))
			return ScriptVariables[varName];
		return new Variant();
	}

	public void SetGameVar(string varName, Variant value)
	{
		if (ScriptVariables.ContainsKey(varName))
			ScriptVariables[varName] = value;
		else
			ScriptVariables.Add(varName, value);
	}

    public void Wait(float seconds) => QueueAction(new ScriptActionWait(seconds));

    public void PrintError(string message) => GD.PrintErr(message);
}