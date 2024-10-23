using Godot;
using System;
using GodotInk;
using Godot.Collections;

public partial class CustomScriptManager : ScriptManager
{
	public CustomScriptManager(InkStory inkStory) : base(inkStory)
	{
		var dict = new Dictionary<string, string>
		{
			{ "print", MethodName.PrintFunction },
			{ "move_object_by", MethodName.MoveObjectByFunction },
			{ "move_object_to", MethodName.MoveObjectToFunction },
			{ "destroy_object", MethodName.DestroyObjectFunction }
		};

		foreach (var item in dict)
			BindExternalFunction(item.Key, new Callable(this, item.Value));
	}

	public void PrintFunction(string objectControllerID, string message) => QueueAction(new ScriptActionPrint(ScriptObjects, objectControllerID, message));
	public void MoveObjectByFunction(string objectControllerID, float posX, float posY) => QueueAction(new ScriptActionMoveObjectBy(ScriptObjects, objectControllerID, new Vector2(posX, posY)));
	public void MoveObjectToFunction(string objectControllerID, float posX, float posY) => QueueAction(new ScriptActionMoveObjectTo(ScriptObjects, objectControllerID, new Vector2(posX, posY)));
	public void DestroyObjectFunction(string objectControllerID) => QueueAction(new ScriptActionDestroyObject(ScriptObjects, objectControllerID));
}

public partial class Main : Node2D
{
	[Export] public InkStory InkStory { get; set; }

	public ScriptManager ScriptManager { get; set; }

	Button Button => GetNode<Button>("%Button");
	Label Output => GetNode<Label>("%Output");

	public override void _Ready()
	{
		ScriptManager = new CustomScriptManager(InkStory);

		Button.Pressed += OnButtonPressed;

		// Find and register all nodes of type ScriptObjectController
		foreach (Node node in GetTree().GetNodesInGroup("controller"))
			if (node is ScriptObjectController scriptObjectController)
				ScriptManager.RegisterScriptObject(scriptObjectController);

		ScriptManager.ScriptVariables["test_var1"] = "okay";
		ScriptManager.ScriptVariables["test_var2"] = "bleh";
	}

	public override void _ExitTree() => ScriptManager.Cleanup();

	private async void OnButtonPressed()
	{
		ScriptManager.InkStory.ResetState();
		while (ScriptManager.InkStory.CanContinue)
		{
			string storyText = ScriptManager.InkStory.Continue();
		}

		await ScriptManager.RunActionQueue();

		GD.Print(ScriptManager.GetStoryVariable("test_string"));
		GD.Print(ScriptManager.GetStoryVariable("test_number"));
	}
}