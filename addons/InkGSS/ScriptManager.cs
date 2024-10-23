using Godot;
using GodotInk;
using System.Threading.Tasks;
using Godot.Collections;

public partial class ScriptManager : GodotObject
{
	public Array<AbstractScriptAction> ActionQueue { get; set; } = new Array<AbstractScriptAction>();
	public Array<ScriptObjectController> ScriptObjects { get; set; } = new Array<ScriptObjectController>();
	public Dictionary<string, Variant> ScriptVariables { get; set; } = new Dictionary<string, Variant>();

	public async Task RunActionQueue()
	{
		Logger.Log($"Running {ActionQueue.Count} actions in queue", Logger.LogTypeEnum.Script);
		foreach (var action in ActionQueue)
		{
			Logger.Log($"Running action: {action.GetType()}", Logger.LogTypeEnum.Script);
			await action.Execute();
		}
		ActionQueue.Clear();
	}

	public InkStory InkStory { get; set; }

	Dictionary<string, Callable> ExternalFunctions = new();

	public ScriptManager(InkStory story)
	{
		InkStory = story;
	}

	public void BindExternalFunction(string functionName, Callable function)
	{
		Logger.Log($"Binding external function: {functionName}", Logger.LogTypeEnum.Script);
		ExternalFunctions[functionName] = function;
		InkStory.BindExternalFunction(functionName, function);
	}

	public void Cleanup()
	{
		ActionQueue.Clear();
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

	public void QueueAction(AbstractScriptAction scriptAction)
	{
		ActionQueue.Add(scriptAction);
	}

	public Variant GetStoryVariable(string variableName)
	{
		return InkStory.FetchVariable(variableName);
	}

	public void SetStoryVariable(string variableName, Variant value)
	{
		InkStory.StoreVariable(variableName, value);
	}

	public Variant GetGameVarFunction(string varName)
	{
		return ScriptVariables[varName];
	}

	public void SetGameVarFunction(string varName, Variant value)
	{
		ScriptVariables[varName] = value;
	}
}